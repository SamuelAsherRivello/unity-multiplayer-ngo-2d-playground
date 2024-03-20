using RMC.Playground2D.MVCS.Mini;
using RMC.Playground2D.MVCS.Mini.View;
using RMC.Playground2D.Shared;
using UnityEngine;

namespace RMC.Playground2D.MVCS
{
    /// <summary>
    /// The main entry point of the scene.
    ///
    /// OVERVIEW: TOPICS OF INTEREST
    ///     1.  XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    /// 
    /// </summary>
    public class GameSceneMVCS : MonoBehaviour
    {
        //  Fields ----------------------------------------
        [SerializeField]
        protected WorldView _worldView;

        [SerializeField]
        private Crate _crateNetworkPrefab;

        private PlaygroundMini _playgroundMini;
        
        //  Unity Methods ---------------------------------
        protected void Start()
        {
            // Set manageable size to help if/when
            // spawning multiple builds for testing
            Screen.SetResolution(960, 540, FullScreenMode.Windowed);
            
            _worldView.CrateNetworkPrefab = _crateNetworkPrefab;
            _playgroundMini = new PlaygroundMini(_worldView);
            _playgroundMini.Initialize();
        }
        
        protected void OnDestroy()
        {
            _playgroundMini.Dispose();
        }
    }
}