using RMC.Core.Architectures.Mini.Context;
using RMC.Core.Architectures.Mini.Controller;
using RMC.Playground2D.MVCS.Mini.Model;
using RMC.Playground2D.MVCS.Mini.Service;
using RMC.Playground2D.MVCS.Mini.View;
using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace RMC.Playground2D.MVCS.Mini.Controller
{
    /// <summary>
    /// The Controller coordinates everything between
    /// the Concerns and contains the core app logic 
    /// </summary>
    public class PlaygroundController: BaseController // Extending 'base' is optional
            <PlaygroundModel, 
            PlaygroundView, 
            PlaygroundService> 
    {
        //  Events ----------------------------------------

        
        //  Properties ------------------------------------
        
        
        //  Fields ----------------------------------------
        private Crate _crateInstance;
        private PlayerMVCS _localPlayer;
        
        
        //  Initialization  -------------------------------
        public PlaygroundController(PlaygroundModel model, PlaygroundView view, PlaygroundService service) 
            : base(model, view, service)
        {
        }
  
        
        public override void Initialize(IContext context)
        {
            if (!IsInitialized)
            {
                base.Initialize(context);
                
                // Mimic event to refresh UI
                PlayerScore_OnValueChanged(0, 0);
                
                _service.OnConnectionUnityEvent.AddListener(Service_OnConnectionUnityEvent);
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
            PlayerMVCS player = networkObject.GetComponent<PlayerMVCS>();
            player.PlayerName.Value = playerName;
        }
        
        
        private void SetPlayerPosition(NetworkObject networkObject, int localPlayerIndex)
        {
            GameObject spawnPoint = _view.World.SpawnPoints.GetPlayerSpawnPointByIndex(localPlayerIndex);
            PlayerMVCS eventPlayer = networkObject.GetComponent<PlayerMVCS>();
            eventPlayer.Teleport(spawnPoint.transform.position);
        }

        
        [ServerRpc (RequireOwnership = false)]
        private void ShootServerRPC(ulong shooterClientId, Vector2 position, Vector2 direction)
        {

        }
        
        
        //  Event Handlers --------------------------------
        private void PlayerScore_OnValueChanged(int oldValue, int newValue)
        {
            _view.World.WorldUI.ScoreText.text = $"{nameof(GameSceneMVCS)}\nScore : {newValue:000}";
        }

        
        private void Player_OnShootRequested(ulong shooterClientId, Vector2 position, Vector2 direction)
        {
            ShootServerRPC( shooterClientId, position, direction);
        }
        
        
        private void Service_OnConnectionUnityEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            RequireIsInitialized();

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
                    _localPlayer = eventNetworkObject.GetComponent<PlayerMVCS>();
                    _localPlayer.PlayerScore.OnValueChanged += PlayerScore_OnValueChanged;
                    _localPlayer.OnShootRequested.AddListener(Player_OnShootRequested);
                }

                //Create crate once per session.
                if (isConnectionClientMeAsServer)
                {
                    GameObject crateSpawnPoint = _view.World.SpawnPoints.GetCrateSpawnPoint();
                    GameObject crateGameObject = Object.Instantiate<GameObject>(_view.CrateNetworkPrefab.gameObject,
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