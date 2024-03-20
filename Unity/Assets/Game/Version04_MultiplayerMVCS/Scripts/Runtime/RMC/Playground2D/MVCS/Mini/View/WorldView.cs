using System;
using RMC.Core.Architectures.Mini.Context;
using RMC.Playground2D.MVCS.Mini.Controller.Commands;
using RMC.Playground2D.MVCS.Mini.Model;
using RMC.Core.Architectures.Mini.View;
using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RMC.Playground2D.MVCS.Mini.View
{
    //  Namespace Properties ------------------------------

    //  Class Attributes ----------------------------------

    /// <summary>
    /// The View handles user interface and user input
    /// </summary>
    public class WorldView: MonoBehaviour, IView
    {
        //  Events ----------------------------------------

        //  Properties ------------------------------------
        public bool IsInitialized { get { return _isInitialized;} }
        public IContext Context { get { return _context;} }
        public World World { get { return _world;} }
        public Crate CrateNetworkPrefab { get; set; }
        public BulletMVCS BulletNetworkPrefab { get; set; }

        //  Fields ----------------------------------------
        private bool _isInitialized = false;
        private IContext _context;

        [SerializeField] 
        private World _world;

        
        //  Initialization  -------------------------------
        public void Initialize(IContext context)
        {
            if (!IsInitialized)
            {
                _isInitialized = true;
                _context = context;
                
                //
                Context.CommandManager.AddCommandListener<CounterChangedCommand>(
                    OnCounterValueChangedCommand);

                // Demo - Optional: View may READ model DIRECTLY...
                var x = Context.ModelLocator.GetItem<PlaygroundModel>();
            }
        }

        public void RequireIsInitialized()
        {
            if (!IsInitialized)
            {
                throw new Exception("MustBeInitialized");
            }
        }
        
        
        //  Unity Methods ---------------------------------
        protected void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RequireIsInitialized();
                
            }
        }
        
        protected void OnDestroy()
        {
            Context.CommandManager.RemoveCommandListener<CounterChangedCommand>(
                OnCounterValueChangedCommand);
        }

        //  Methods ---------------------------------------
        
        
        //  Event Handlers --------------------------------
        private void OnCounterValueChangedCommand(CounterChangedCommand counterChangedCommand)
        {
            RequireIsInitialized();
        }
    }
}