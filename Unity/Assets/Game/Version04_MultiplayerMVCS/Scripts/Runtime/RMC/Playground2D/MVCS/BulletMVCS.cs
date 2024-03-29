using RMC.Playground2D.MVCS;
using Unity.Netcode;
using UnityEngine;

namespace RMC.Playground2D.Shared
{
    /// <summary>
    /// Reset position when offscreen to add
    /// an emergent gameplay element of fun.
    ///
    /// OVERVIEW: TOPICS OF INTEREST
    ///     1.  XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    /// 
    /// </summary>
    public class BulletMVCS : NetworkBehaviour
    {
        //  Fields ----------------------------------------
        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        [SerializeField]
        private float _speed = 1;

        private ulong _shooterClientId;
        private float _lastTimeOnScreen = 0;
        
        [SerializeField]
        private Camera _camera;
        
        //  Properties ------------------------------------
        public ulong ShooterClientId { get { return _shooterClientId;} }
        
        //  Unity Methods ---------------------------------
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {   
                enabled = false;
            }
            
            _camera = Camera.main;
        }

        
        protected void Update()
        {
            if (_camera == null)
            {
                return;
            }
            
            Vector3 myScreenPosition = _camera.WorldToScreenPoint(transform.position);
            bool isMyCenterOffscreen = !Screen.safeArea.Contains(myScreenPosition);
			
            if (isMyCenterOffscreen)
            {
                //When offscreen for X seconds, destroy
                if (Time.time - _lastTimeOnScreen > 1)
                {
                    NetworkObject.Despawn(true);
                }
            }
            else
            {
                _lastTimeOnScreen = Time.time;
            }
        }
        
        
        //  Methods ---------------------------------------
        public void CustomSpawn(ulong shooterClientId, Vector2 direction)
        {
            _shooterClientId = shooterClientId;
            _rigidbody2D.velocity = direction * _speed;
            
            Debug.Log("About to spawn: " + NetworkObject.IsOwnedByServer);
            NetworkObject.Spawn();

        }
        
        
        [ServerRpc (RequireOwnership = false)]
        private void CustomDespawnServerRpc()
        {
            NetworkObject.Despawn(true);
        }
        
        
        //  Event Handlers --------------------------------
        protected void OnTriggerEnter2D(Collider2D collider2D)
        {
            // OnTriggerEnter2D may be called before OnNetworkSpawn, 
            // so check again
            if (!IsServer)
            {
                return;
            }
            
            PlayerMVCS player = collider2D.gameObject.GetComponent<PlayerMVCS>();
            if (player != null)
            {
                if (player.NetworkObject.OwnerClientId != ShooterClientId)
                {
                    player.TakeDamage();
                    CustomDespawnServerRpc();
                }
            }
        }
    }
}