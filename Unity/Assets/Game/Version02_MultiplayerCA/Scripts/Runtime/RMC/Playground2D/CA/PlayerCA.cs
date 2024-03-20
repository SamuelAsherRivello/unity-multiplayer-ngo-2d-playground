using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.Events;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace RMC.Playground2D.CA
{
	/// <summary>
	/// Handle player movement and scoring
	/// </summary>
	public class PlayerCA : NetworkBehaviour
	{
		//  Events ----------------------------------------
		public readonly IntUnityEvent OnScoreChanged = new IntUnityEvent();

		
		//  Properties ------------------------------------
		private int Score
		{
			set
			{
				_score = value;
				OnScoreChanged.Invoke(_score);
			}
			get
			{
				return _score;
			}
		}

		
		//  Constants -------------------------------------
		private const float DeltaPositionIsRunningThreshold = 0.01f;
		
		
		//  Fields ----------------------------------------
		[SerializeField]
		private Rigidbody2D _rigidBody2D;

		[SerializeField]
		private NetworkTransform _networkTransform;

		[SerializeField]
		private Animator _animator;

		[Tooltip("Movement multiplayer by frame")]
		[SerializeField]
		private float _moveSpeed = 15;

		private Vector2 _deltaPosition;
		
		private int _score = 0;

		
		//  Unity Methods ---------------------------------
		public override void OnNetworkSpawn()
		{
			if (!IsOwner)
			{
				enabled = false;
			}
			Score = 0;
		}
		
		
		protected void Update()
		{
			// Get input axes
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");

			// Calculate change in position
			_deltaPosition = new Vector2(moveHorizontal, moveVertical) 
			                 * _moveSpeed;
			
			// Animate	
			bool isRunning = Mathf.Abs(_deltaPosition.x) > DeltaPositionIsRunningThreshold;
			_animator.SetBool("IsRunning", isRunning);
		}
		
		protected void FixedUpdate()
		{
			// Apply movement through RigidBody2D
			// To preserve physics fidelity
			_rigidBody2D.MovePosition(_rigidBody2D.position + _deltaPosition);
		}

		
		//  Methods ---------------------------------------
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
		public void OnCollisionEnter2D ( Collision2D collision2D)
		{
			Crate crate = collision2D.gameObject.GetComponent<Crate>();
			if (crate != null)
			{
				Score++;
				
				// When touching the crate ensure the crate is owned by the player
				// So the crate can be pushed by the player
				NetworkObject networkObject = crate.GetComponent<NetworkObject>();
				if (networkObject != null && NetworkManager.Singleton.IsServer)
				{
					networkObject.ChangeOwnership(OwnerClientId);
				}
			}
		}
	}
}
