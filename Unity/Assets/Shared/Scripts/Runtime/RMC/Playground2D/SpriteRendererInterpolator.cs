using Unity.Netcode;
using UnityEngine;

namespace RMC.Playground2D.Shared
{
	/// <summary>
	/// Handle facing-direction of <see cref="SpriteRenderer"/> changes
	/// locally, so no network synchronization is needed.
	/// </summary>
	public class SpriteRendererInterpolator : MonoBehaviour
	{
		//  Fields ----------------------------------------
		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		private float _lastTransformPositionX;
		
		private float _currentVelocityX;

		
		//  Unity Methods ---------------------------------
		protected void Start()
		{
			_lastTransformPositionX = transform.position.x;
			_currentVelocityX = 0;
		}

		
		protected void FixedUpdate()
		{
			// Since movement is done on FixedUpdate, 
			// Calculate velocity on FixedUpdate too
			_currentVelocityX = transform.position.x - _lastTransformPositionX;
			
			// Face direction of movement
			if (_currentVelocityX != 0)
			{
				bool isFlipX = _currentVelocityX > 0;
				_spriteRenderer.flipX = isFlipX;
			}
			
			_lastTransformPositionX = transform.position.x;
		}
	}
}
