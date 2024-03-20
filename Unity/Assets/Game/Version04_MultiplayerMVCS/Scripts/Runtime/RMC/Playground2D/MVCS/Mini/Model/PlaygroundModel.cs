using RMC.Core.Architectures.Mini.Context;
using RMC.Core.Architectures.Mini.Model;

namespace RMC.Playground2D.MVCS.Mini.Model
{
    //  Namespace Properties ------------------------------

    //  Class Attributes ----------------------------------

    /// <summary>
    /// The Model stores runtime data 
    /// </summary>
    public class PlaygroundModel: BaseModel // Extending 'base' is optional
    {
        //  Events ----------------------------------------

        //  Properties ------------------------------------
        
        //  Fields ----------------------------------------
        
        //  Initialization  -------------------------------
        public override void Initialize(IContext context) 
        {
            if (!IsInitialized)
            {
                base.Initialize(context);
            }
        }
        
        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------

    }
}