using System;
using RMC.Core.Architectures.Mini.Context;
using RMC.Core.Architectures.Mini.Controller;
using Unity.Netcode;

namespace RMC.Playground2D.MVCS.Mini.Controller
{
    //  Namespace Properties ------------------------------

    //  Class Attributes ----------------------------------

    /// <summary>
    /// The Controller coordinates everything between
    /// the <see cref="IConcern"/>s and contains the core app logic 
    /// </summary>
    public class BaseNetworkBehaviourController<TModel,TView,TService>: NetworkBehaviour, IController
    {
        //  Events ----------------------------------------


        //  Properties ------------------------------------
        public bool IsInitialized { get { return _isInitialized;} }
        public IContext Context { get { return _context;} }
        
        //  Fields ----------------------------------------
        private bool _isInitialized = false;
        protected TModel _model;
        protected TView _view;
        protected TService _service;
        private IContext _context;

        public void Initialize(TModel model, TView view, TService service) 
        {
            _model = model;
            _view = view;
            _service = service;
        }

        //  Initialization  -------------------------------
        public virtual void Initialize(IContext context)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _context = context;
            }
        }

        public void RequireIsInitialized()
        {
            if (!_isInitialized)
            {
                throw new Exception("MustBeInitialized");
            }
        }
        
        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------
    }
}