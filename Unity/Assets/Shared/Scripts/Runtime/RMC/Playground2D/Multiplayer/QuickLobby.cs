using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RMC.Playground2D.Shared.Multiplayer
{
    /// <summary>
    /// Here the local player joins the network
    /// 
    /// Here are a few iterations of this type of functionality
    /// listed in order of increasing maturity.
    ///
    /// 1. Via Unity NetworkManager (Using input to join)
    ///     -   Use keyboard input to join as host or client.
    ///         Good for development playing on one total computer.
    ///         Less hassle, less power
    ///         **THIS IS IMPLEMENTED BELOW**
    ///
    /// 2. Via Unity NetworkManager (Using automated join - 3rd Party)
    ///     -   Use https://github.com/VeriorPies/ParrelSync to determine
    ///         "Am I in the editor, then join as host, else join as client"
    ///         (or other such alternatives)
    ///         Good for development playing on one total computer.
    ///         Less hassle, less power
    ///         (When using Unity 2022 LTS, and 3rd party tools are allowed, this is recommended)
    ///         (When using Unity 2023, Unity has its own 1st-party solution for this instead)
    ///
    /// 2. Via Unity NetworkManager (Using automated join - 3rd Party)
    ///     -   Use https://docs-multiplayer.unity3d.com/netcode/current/tutorials/command-line-helper/
    ///         This is similar to the pros/cons of #2.
    /// 
    /// 4. Via Unity NetworkManager (Using UI to join)
    ///     -   Offer UI buttons to "join as host" or "join as client".
    ///         Good for development playing on one total computer.
    ///         I prefer #1 to #3 here. Speedier UX for me as the developer.
    ///         More hassle, more power
    ///
    /// 5. Via Unity Gaming Services (Using UI to join)
    ///         [Relay, Matchmaking, Lobby, etc...]
    ///     -   Offer Lobby with UI buttons to "join as host" or "join as client".
    ///         Good for production playing on one or more total computers.
    ///         Most hassle, most power
    /// 
    /// </summary>
    public class QuickLobby : MonoBehaviour
    {
        //  Fields ----------------------------------------
        [SerializeField]
        protected World _world;

        [SerializeField]
        private bool _isLogging = true;
        
        private bool _hasReachedClientConnected = false;

        //  Unity Methods ---------------------------------
        protected async void Start()
        {
            SetInstructions(MultiplayerConstants.Instructions01_Join);
            
            // KLUGE: This delay-a-frame helps NetworkManager.Singleton stability,
            // especially after DisconnectLocalClient() is called
            await Task.Delay(100);
            
            NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;
        }
        
        protected void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnConnectionEvent -= NetworkManager_OnConnectionEvent;
            }
        }
        
        
        protected void Update()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    SetInstructions(MultiplayerConstants.Instructions02_WaitingForHost);
                    NetworkManager.Singleton.StartHost();
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    SetInstructions(MultiplayerConstants.Instructions02_WaitingForHost);
                    NetworkManager.Singleton.StartClient();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (_hasReachedClientConnected)
                    {
                        SetInstructions(MultiplayerConstants.Instructions01_Join);
                    }
                    else
                    {
                        //KLUGE
                        //When a client joins a host-less room then disconnects
                        //the app is unstable. Hacky fix is to restart the app.
                        SetInstructions(MultiplayerConstants.Instructions04_MustRestartApp);
                    }
                    
                    _hasReachedClientConnected = false;

                    DisconnectLocalClient();
     
                }
            }
        }

        //  Methods ---------------------------------------
        private void SetInstructions(string message)
        {
            _world.WorldUI.InstructionsText.text = message;
        }
        
        private void DisconnectLocalClient()
        {
            NetworkManager.Singleton.Shutdown(false);
                    
            // If the host has quit, reload the scene (i.e, to reset the crate)
            if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }

        //  Event Handlers --------------------------------
        private void NetworkManager_OnConnectionEvent(  NetworkManager networkManager, 
                                                        ConnectionEventData connectionEventData)
        {
            if (_isLogging)
            {
                Debug.Log($"{this.GetType().Name}. NetworkManager_OnConnectionEvent = {connectionEventData.EventType}"); 
            }
    
            if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
            {
                _hasReachedClientConnected = true;

                string localRole = MultiplayerConstants.GetRoleNameByIsHost(networkManager.IsHost);
                SetInstructions(string.Format(MultiplayerConstants.Instructions03_Playing, localRole));
      
            }
            else if (connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
            {
                SetInstructions(MultiplayerConstants.Instructions01_Join);
            }
        }
    }
}