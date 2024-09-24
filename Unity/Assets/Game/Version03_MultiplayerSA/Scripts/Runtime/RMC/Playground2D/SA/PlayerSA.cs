using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.UI;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode.Components;

namespace RMC.Playground2D.SA
{
	/// <summary>
	/// Handle player movement and scoring
	/// </summary>
	public class PlayerSA : NetworkBehaviour
	{
		//  Properties ------------------------------------

		
		//  Constants -------------------------------------
		private const float DeltaPositionIsRunningThreshold = 0.03f;
		
		
		//  Fields ----------------------------------------
		[SerializeField]
		private Rigidbody2D _rigidBody2D;

		[SerializeField]
		private NetworkTransform _networkTransform;

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private NameTagUI _nameTagUI;

		[Tooltip("Movement multiplayer by frame")]
		[SerializeField]
		private float _moveSpeed = .1f;

		[Tooltip("True: SmoothDamp changes, False: Direct changes")]
		[SerializeField]
		private bool _isInterpolated = true;
		
		[Tooltip("True: Owner moves immediately. False: Owner moves later.")]
		[SerializeField]
		private bool _isOwnerPrioritized = true;

		public NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>(
			readPerm:NetworkVariableReadPermission.Everyone, 
			writePerm:NetworkVariableWritePermission.Owner);
		
		public NetworkVariable<int> PlayerScore = new NetworkVariable<int>(
			readPerm:NetworkVariableReadPermission.Everyone, 
			writePerm:NetworkVariableWritePermission.Owner);
		
		private Vector2 _interpolatedCurrentVelocity;
		private readonly float _interpolatedSmoothTime = .001f;

		
		//  Unity Methods ---------------------------------
		public override void OnNetworkSpawn()
		{
			if (!IsOwner)
			{
				enabled = false;
			}

			// Observe and synchronize
			PlayerName.OnValueChanged += PlayerName_OnValueChanged;
			PlayerName_OnValueChanged("", PlayerName.Value);
			
			// Initialize
			if (IsOwner)
			{
				PlayerScore.Value = 0;
			}
		}
		
		
		protected void Update()
		{
			// Get input axes
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");

			// Calculate change in position
			Vector2 deltaPosition = new Vector2(moveHorizontal, moveVertical)
			                        * _moveSpeed;
			
			// Request change in position
			SendMoveRequest(deltaPosition);
		}

		
		//  Methods ---------------------------------------
		private void SendMoveRequest(Vector2 deltaPosition)
		{
			// Send move request to server for all
			MoveServerRpc(deltaPosition);
			
			if (IsOwner && _isOwnerPrioritized)
			{
				// To reduce perceived latency...
				// Immediately apply to self
				// But then don't apply below to self
				// NOTE: Maybe, this helps cosmetics, but hurts accuracy
				RespondToMoveRequest(deltaPosition);
			}
		}
		
		
		[ServerRpc]
		private void MoveServerRpc(Vector2 deltaPosition)
		{
			MoveClientRpc(deltaPosition);
		}
		
		
		[ClientRpc]
		private void MoveClientRpc(Vector2 deltaPosition)
		{
			if (!(IsOwner && _isOwnerPrioritized))
			{
				RespondToMoveRequest(deltaPosition);
			}
		}

		
		private void RespondToMoveRequest(Vector2 deltaPosition)
		{
			Vector2 newPosition;
			
			if (_isInterpolated)
			{
				newPosition = Vector2.SmoothDamp (
					_rigidBody2D.position, 
					_rigidBody2D.position + deltaPosition,
					ref _interpolatedCurrentVelocity, 
					_interpolatedSmoothTime);
			}
			else
			{
				newPosition = _rigidBody2D.position + deltaPosition;
			}
			
			// Apply movement through RigidBody2D
			// To preserve physics fidelity
			_rigidBody2D.MovePosition(newPosition);
			
			// Animate
			bool isRunning = Mathf.Abs(deltaPosition.x) > DeltaPositionIsRunningThreshold;
			_animator.SetBool("IsRunning", isRunning);
		}

		/// <summary>
		/// Move without interpolate.
		/// Used to place player at spawn point.
		/// </summary>
		/// <param name="newPosition"></param>
		public void Teleport(Vector3 newPosition)
		{
			_networkTransform.Teleport(newPosition, 
				_networkTransform.transform.rotation, 
				_networkTransform.transform.localScale);
		}
		
		//  Event Handlers --------------------------------
		private void PlayerName_OnValueChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
		{
			_nameTagUI.NameTagText.text = newValue.ToString();
		}
		
		
		public void OnCollisionEnter2D ( Collision2D collision2D)
		{
			Crate crate = collision2D.gameObject.GetComponent<Crate>();
			if (crate != null)
			{
				PlayerScore.Value++;
			}
		}
	}
}
