using RMC.Playground2D.SA;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace RMC.Playground2D.MVCS
{
	//  Class Attributes ----------------------------------
	public class ShootRequestUnityEvent : UnityEvent<ulong, Vector2, Vector2> {}
	
	/// <summary>
	/// Borrow directly from <see cref="PlayerSA"/>
	/// </summary>
	public class PlayerMVCS : PlayerSA
	{
		//  Events ----------------------------------------
		public ShootRequestUnityEvent OnShootRequested = new ShootRequestUnityEvent();

		//  Fields ----------------------------------------
		[SerializeField]
		protected SpriteRenderer _spriteRenderer;

		[SerializeField]
		protected NetworkObject _networkObject;

		[SerializeField] 
		protected GameObject _spawnPoint;

		
		//  Unity Methods ---------------------------------
		/// <summary>
		/// Structure is for development only.
		///
		/// In production, this Update and the base Update would be combined
		/// </summary>
		protected override void Update()
		{
			base.Update();

			if (Input.GetKeyDown(KeyCode.Space))
			{
				Vector2 direction = Vector2.left;
				if (_spriteRenderer.flipX)
				{
					direction = Vector2.right;
				}
				OnShootRequested.Invoke(_networkObject.OwnerClientId, _spawnPoint.transform.position, direction);;
			}
		}

		//  Methods ---------------------------------------
		public void TakeDamage()
		{
			if (PlayerScore.Value > 0)
			{
				SetScoreValueServerRpc(--PlayerScore.Value);
			}
		}

		[ServerRpc]
		private void SetScoreValueServerRpc(int newScore)
		{
			PlayerScore.Value = newScore;
		}
	}
}
