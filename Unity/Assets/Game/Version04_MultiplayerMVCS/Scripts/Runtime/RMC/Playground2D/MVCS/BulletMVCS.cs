using RMC.Playground2D.MVCS;
using Unity.Netcode;
using UnityEngine;

namespace RMC.Playground2D.Shared
{
    /// <summary>
    /// Reset position when offscreen to add
    /// an emergent gameplay element of fun.
    /// </summary>
    public class BulletMVCS : NetworkBehaviour
    {
        //  Fields ----------------------------------------
        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        [SerializeField]
        private NetworkObject _networkObject;

        [SerializeField]
        private float _speed = 1;

        private ulong _shooterClientId;
        private float _lastTimeOnScreen = 0;
        
        [SerializeField]
        private Camera _camera;
        
        //  Properties ------------------------------------
        public Rigidbody2D Rigidbody2D { get { return _rigidbody2D;}}
        public NetworkObject NetworkObject { get { return _networkObject;}}
        public ulong ShooterClientId { get { return _shooterClientId;} set { _shooterClientId = value;}}
        
        public float Speed { get { return _speed;}}
        
        //  Unity Methods ---------------------------------
        public override void OnNetworkSpawn()
        {
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
                    _networkObject.Despawn(true);
                }
            }
            else
            {
                _lastTimeOnScreen = Time.time;
            }
        }
        
        //  Methods ---------------------------------------
        [ServerRpc (RequireOwnership = false)]
        private void DespawnServerRpc()
        {
            _networkObject.Despawn(true);
        }
        		
        //  Event Handlers --------------------------------
        protected void OnTriggerEnter2D ( Collider2D collider2D)
        {
            PlayerMVCS player = collider2D.gameObject.GetComponent<PlayerMVCS>();
            
            Debug.Log(player.NetworkObject.OwnerClientId + " and " + ShooterClientId);
            if (player != null && player.NetworkObject.OwnerClientId != ShooterClientId)
            {
                player.TakeDamage();
                DespawnServerRpc();
            }
        }
    }
}