using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.UI;
using UnityEngine;

namespace RMC.Playground2D.SP
{
    /// <summary>
    /// The main entry point of the scene.
    ///
    /// OVERVIEW: TOPICS OF INTEREST
    ///     1.  This is a singleplayer scene. Its the baseline used for core gameplay development.
    ///     2.  The <see cref="PlayerSP"/> rewards points for collision with <see cref="Crate"/>
    ///     3.  The <see cref="GameSceneSP"/> updates the <see cref="WorldUI"/>.
    /// 
    /// </summary>
    public class GameSceneSP : MonoBehaviour
    {
        //  Fields ----------------------------------------
        [SerializeField]
        private World _world;

        [SerializeField]
        private PlayerSP _playerSP;

        //  Unity Methods ---------------------------------
        protected void Start()
        {
            // Set manageable size to help if/when
            // spawning multiple builds for testing
            Screen.SetResolution(960, 540, FullScreenMode.Windowed);
            
            // Subscribe to player events
            _playerSP.OnScoreChanged.AddListener(Player_OnScoreChanged);
            
            // Mimic event to refresh th
            Player_OnScoreChanged(0);
        }

        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------
        private void Player_OnScoreChanged(int value)
        {
            _world.WorldUI.ScoreText.text = $"{this.GetType().Name}\nScore : {value:000}";
        }
    }
}