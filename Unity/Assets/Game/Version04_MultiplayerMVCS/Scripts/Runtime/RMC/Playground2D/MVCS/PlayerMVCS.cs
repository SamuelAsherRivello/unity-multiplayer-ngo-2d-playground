using RMC.Playground2D.SA;
using RMC.Playground2D.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace RMC.Playground2D.MVCS
{
	//  Class Attributes ----------------------------------
	public class ShootRequestUnityEvent : UnityEvent<ulong, Vector2, Vector2> {}
	
	/// <summary>
	/// Borrow directly from <see cref="PlayerSA"/>.
	///
	/// Ideally the MVCS demo does not borrow any code from the SA demo, but
	/// this is a quick way to get the MVCS demo up and running.
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

		[SerializeField]
		public BulletMVCS BulletNetworkPrefab;
		
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
				//OnShootRequested.Invoke(_networkObject.OwnerClientId, _spawnPoint.transform.position, direction);

				Debug.Log("1: " + OwnerClientId);
				BlahServerRpc(OwnerClientId);
			}
		}

		[ServerRpc]
		public void BlahServerRpc(ulong shooterClientId)
		{
			Debug.Log("2: " + NetworkManager.Singleton.IsServer);
			
			Vector2 direction = Vector2.left;
			if (_spriteRenderer.flipX)
			{
				direction = Vector2.right;
			}
			BulletMVCS bullet = Object.Instantiate(BulletNetworkPrefab, _spawnPoint.transform.position, Quaternion.identity);
			bullet.CustomSpawn(shooterClientId, direction * 1);
	
		}
		
		
		[ClientRpc]
		public void BlahClientRpc()
		{
			Debug.Log("3: " + NetworkManager.Singleton.IsServer);

			if (NetworkManager.Singleton.IsServer)
			{
				return;
			}
			
	
		}

		//  Methods ---------------------------------------
		public void TakeDamage()
		{
			if (PlayerScore.Value > 0)
			{
				SetScoreValueServerRpc(PlayerScore.Value - 1 );
			}
		}

		[ServerRpc (RequireOwnership = false)]
		private void SetScoreValueServerRpc(int newScore)
		{
			PlayerScore.Value = newScore;
		}
	}
}
