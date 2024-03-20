using RMC.Core.Architectures.Mini.Context;
using RMC.Core.Architectures.Mini.Service;
using Unity.Netcode;
using UnityEngine.Events;

namespace RMC.Playground2D.MVCS.Mini.Service
{
    //  Namespace Properties ------------------------------
    public class ConnectionUnityEvent : UnityEvent<NetworkManager, ConnectionEventData> {}

    //  Class Attributes ----------------------------------

    /// <summary>
    /// The Service handles external data 
    /// </summary>
    public class PlaygroundService : BaseService  // Extending 'base' is optional
    {
        //  Events ----------------------------------------
        public readonly ConnectionUnityEvent OnConnectionUnityEvent = new ConnectionUnityEvent();

        //  Properties ------------------------------------
        
        //  Fields ----------------------------------------
        private int _counterInitialValue;

        //  Initialization  -------------------------------
        public override void Initialize(IContext context)
        {
            if (!IsInitialized)
            {
                base.Initialize(context);
                NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;
            }
        }
        
        //  Methods ---------------------------------------
        
        
        //  Event Handlers --------------------------------
        private void NetworkManager_OnConnectionEvent(  NetworkManager networkManager, 
                                                        ConnectionEventData connectionEventData)
        {
            OnConnectionUnityEvent.Invoke(networkManager, connectionEventData);
        }
    }
}