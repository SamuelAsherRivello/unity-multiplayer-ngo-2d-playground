using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.UI;
using Unity.Netcode;
using UnityEngine;

namespace RMC.Playground2D.CA
{
    /// <summary>
    /// The main entry point of the scene.
    ///
    /// OVERVIEW: TOPICS OF INTEREST
    ///     1.  This is a multiplayer scene. This uses a CLIENT authoritative approach. (Easier development, but less secure)
    ///     2.  The <see cref="PlayerCA"/> rewards points for collision with <see cref="Crate"/>.
    ///         It uses "NetworkObject.ChangeOwnership" to give control to the client.
    ///     3.  The <see cref="GameSceneCA"/> updates the <see cref="WorldUI"/>.
    /// 
    /// </summary>
    public class GameSceneCA : MonoBehaviour
    {
        //  Fields ----------------------------------------
        private PlayerCA _localPlayer;

        //  Fields ----------------------------------------
        [SerializeField]
        protected World _world;

        [SerializeField]
        private Crate _crateNetworkPrefab;

        private Crate _crateInstance;

        //  Unity Methods ---------------------------------
        protected void Start()
        {
            // Set manageable size to help if/when
            // spawning multiple builds for testing
            Screen.SetResolution(960, 540, FullScreenMode.Windowed);
            
            // Mimic event to refresh UI
            Player_OnScoreChanged(0);
            
            NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;
        }
        
        
        protected void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnConnectionEvent -= NetworkManager_OnConnectionEvent;
            }
        }

        //  Methods ---------------------------------------
        private void DestroyCrate()
        {
            NetworkObject crateNetworkObject = _crateInstance.GetComponent<NetworkObject>();
            crateNetworkObject.Despawn(true);
            _crateInstance = null;
        }
        

        //  Event Handlers --------------------------------
        private void Player_OnScoreChanged(int value)
        {
            _world.WorldUI.ScoreText.text = $"{this.GetType().Name}\nScore : {value:000}";
        }
        
        
        private void NetworkManager_OnConnectionEvent(  NetworkManager networkManager, 
            ConnectionEventData connectionEventData)
        {
            
            // Is the code running on the server and is the connection relates to the server's local client
            // This is useful to know if the server is the one that connected or disconnected
            bool isConnectionClientMeAsServer = NetworkManager.Singleton.IsServer &&
                                          NetworkManager.Singleton.LocalClientId == connectionEventData.ClientId;
                
            
            if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
            {
                // Get custom script for the player from who connected
                NetworkObject networkObject = networkManager.LocalClient.PlayerObject;
                _localPlayer = networkObject.GetComponent<PlayerCA>();
                
                // Set player position
                int localPlayerIndex = (int)networkManager.LocalClientId;
                GameObject playerSpawnPoint = _world.SpawnPoints.GetPlayerSpawnPointByIndex(localPlayerIndex);
                _localPlayer.Teleport(playerSpawnPoint.transform.position);
            
                // Subscribe to player events
                _localPlayer.OnScoreChanged.AddListener(Player_OnScoreChanged);

                //Create crate once per session.
                if (isConnectionClientMeAsServer)
                {
                    GameObject crateSpawnPoint = _world.SpawnPoints.GetCrateSpawnPoint();
                    GameObject crateGameObject = Instantiate(_crateNetworkPrefab.gameObject,
                        crateSpawnPoint.transform.position, Quaternion.identity);
                    
                    if(_crateInstance != null)
                    {
                        DestroyCrate();
                    }
                    
                    _crateInstance = crateGameObject.GetComponent<Crate>();
                    NetworkObject crateNetworkObject = _crateInstance.GetComponent<NetworkObject>();
                    crateNetworkObject.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId, true);
                    crateNetworkObject.DontDestroyWithOwner = true;
                }
      
            }
            else if (connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
            {
                if (isConnectionClientMeAsServer && _crateInstance != null)
                {
                    DestroyCrate();
                }
                
                if (_localPlayer != null)
                {
                    // Unsubscribe to player events
                    _localPlayer.OnScoreChanged.RemoveListener(Player_OnScoreChanged);
                }
            }
        }
    }
}