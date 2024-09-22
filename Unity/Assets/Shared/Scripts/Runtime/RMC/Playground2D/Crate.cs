using UnityEngine;

namespace RMC.Playground2D.Shared
{
    /// <summary>
    /// Reset position when offscreen to add
    /// an emergent gameplay element of fun.
    /// </summary>
    public class Crate : MonoBehaviour
    {
        //  Fields ----------------------------------------
        [SerializeField]
        private Rigidbody2D _rigidBody2D;

        private float _lastTimeOnScreen = 0;
		
        private Camera _camera;
        
        //  Unity Methods ---------------------------------
        protected void Start()
        {
            _camera = Camera.main;
        }

        protected void Update()
        {
            Vector3 myScreenPosition = _camera.WorldToScreenPoint(transform.position);
            bool isMyCenterOffscreen = !Screen.safeArea.Contains(myScreenPosition);
			
            if (isMyCenterOffscreen)
            {
                //When offscreen for X seconds, reset to a screen-centered position
                if (Time.time - _lastTimeOnScreen > 1)
                {
                    transform.position = new Vector3(0, 0, 0);
                    _rigidBody2D.linearVelocity = new Vector2();
                }
            }
            else
            {
                _lastTimeOnScreen = Time.time;
            }
        }
    }
}