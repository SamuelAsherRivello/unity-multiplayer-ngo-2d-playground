using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.Multiplayer;
using RMC.Playground2D.Shared.UI;
using Unity.Netcode;
using UnityEngine;

namespace RMC.Playground2D.SA
{
    /// <summary>
    /// The main entry point of the scene.
    ///
    /// OVERVIEW: TOPICS OF INTEREST
    ///     1.  This is a multiplayer scene. This uses a SERVER authoritative approach. (Easier development, but less secure)
    ///     2.  The <see cref="PlayerSA"/> rewards points for collision with <see cref="Crate"/>.
    ///         It uses "if (!IsOwner)" to remove direct control from the client.
    ///         It uses "ServerRPC" and "ClientRPC" to handle movement.
    ///     3.  The <see cref="GameSceneSA"/> updates the <see cref="WorldUI"/>.
    /// 
    /// </summary>
    public class GameSceneSA : MonoBehaviour
    {
        //  Fields ----------------------------------------
        private PlayerSA _localPlayer;

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
            PlayerScore_OnValueChanged(0, 0);
            
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
        
        private void SetPlayerName(NetworkObject networkObject, int localPlayerIndex)
        {
            bool isHost = networkObject.IsOwnedByServer;
            string localRole = MultiplayerConstants.GetRoleNameByIsHost(isHost);
            string playerName = $"Player {localPlayerIndex}\n<size=0.08>({localRole})</size>";
            PlayerSA player = networkObject.GetComponent<PlayerSA>();
            player.PlayerName.Value = playerName;
        }
        
        private void SetPlayerPosition(NetworkObject networkObject, int localPlayerIndex)
        {
            GameObject spawnPoint = _world.SpawnPoints.GetPlayerSpawnPointByIndex(localPlayerIndex);
            PlayerSA eventPlayer = networkObject.GetComponent<PlayerSA>();
            eventPlayer.Teleport(spawnPoint.transform.position);
        }
        

        //  Event Handlers --------------------------------
        private void PlayerScore_OnValueChanged(int oldValue, int newValue)
        {
            _world.WorldUI.ScoreText.text = $"{this.GetType().Name}\nScore : {newValue:000}";
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
                NetworkObject eventNetworkObject;
                
                // Host sets position and name
                if (NetworkManager.Singleton.IsServer)
                {
                    int eventPlayerIndex = (int)connectionEventData.ClientId;
                    eventNetworkObject = networkManager.ConnectedClients[connectionEventData.ClientId].PlayerObject;
                    SetPlayerPosition(eventNetworkObject, eventPlayerIndex);
                    SetPlayerName(eventNetworkObject, eventPlayerIndex);
                }
                else
                {
                    eventNetworkObject = networkManager.LocalClient.PlayerObject;
                }

                // Local player observes score only for local player
                if (eventNetworkObject.IsLocalPlayer)
                {
                    _localPlayer = eventNetworkObject.GetComponent<PlayerSA>();
                    _localPlayer.PlayerScore.OnValueChanged += PlayerScore_OnValueChanged;
                }

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
                    _localPlayer.PlayerScore.OnValueChanged -= PlayerScore_OnValueChanged;
                }
            }
        }
    }
}