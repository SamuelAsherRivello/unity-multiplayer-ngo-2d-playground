using RMC.Core.Architectures.Mini.Context;
using RMC.Core.Architectures.Mini;
using RMC.Playground2D.MVCS.Mini.Controller;
using RMC.Playground2D.MVCS.Mini.Model;
using RMC.Playground2D.MVCS.Mini.Service;
using RMC.Playground2D.MVCS.Mini.View;

namespace RMC.Playground2D.MVCS.Mini
{
    //  Namespace Properties ------------------------------

    //  Class Attributes ----------------------------------

    /// <summary>
    /// The MiniMvcs is the parent object containing
    /// all <see cref="IConcern"/>s as children. It
    /// defines one instance of the Mvcs architectural
    /// framework within an application.
    /// </summary>
    public class PlaygroundMini: MiniMvcs
            <Context, 
            PlaygroundModel, 
            PlaygroundView, 
            PlaygroundController,
            PlaygroundService>
    {
        //  Events ----------------------------------------

        //  Properties ------------------------------------
        
        //  Fields ----------------------------------------

        //  Initialization  -------------------------------
        public PlaygroundMini(PlaygroundView view)
        {
            _view = view;
        
        }
        
        public override void Initialize()
        {
            if (!IsInitialized)
            {
                _isInitialized = true;
                
                //
                _context = new Context();
                _model = new PlaygroundModel();
                _service = new PlaygroundService();
                _controller = new PlaygroundController(_model, _view, _service);

                //
                _model.Initialize(_context);
                _view.Initialize(_context);
                _service.Initialize(_context);
                _controller.Initialize(_context);
            }
        }

        //  Methods ---------------------------------------

        //  Event Handlers --------------------------------
    }
}