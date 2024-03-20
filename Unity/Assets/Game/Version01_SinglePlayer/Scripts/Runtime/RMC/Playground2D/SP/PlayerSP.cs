using RMC.Playground2D.Shared;
using RMC.Playground2D.Shared.Events;
using UnityEngine;

namespace RMC.Playground2D.SP
{
	/// <summary>
	/// Handle player movement and scoring
	/// </summary>
	public class PlayerSP : MonoBehaviour
	{
		//  Events ----------------------------------------
		public IntUnityEvent OnScoreChanged = new IntUnityEvent();

		//  Properties ------------------------------------
		private int Score
		{
			set
			{
				_score = value;
				OnScoreChanged.Invoke(_score);
			}
			get
			{
				return _score;
			}
		}
		
		//  Constants -------------------------------------
		private const float DeltaPositionIsRunningThreshold = 0.01f;

		//  Fields ----------------------------------------
		[SerializeField]
		private Rigidbody2D _rigidBody2D;

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[Tooltip("Movement multiplayer by frame")]
		[SerializeField]
		private float _moveSpeed = 2.0f;

		private Vector2 _deltaPosition;
		
		private int _score = 0;

		//  Unity Methods ---------------------------------
		protected void Start()
		{
			Score = 0;
		}


		protected void Update()
		{
			// Get input axes
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");

			// Calculate change in position
			_deltaPosition = new Vector2(moveHorizontal, moveVertical) 
			                 * (_moveSpeed);

			// Animate	
			bool isRunning = Mathf.Abs(_deltaPosition.x) > DeltaPositionIsRunningThreshold;
			_animator.SetBool("IsRunning", isRunning);

			// Face toward movement
			if (_deltaPosition.x != 0)
			{
				bool isFlipX = _deltaPosition.x > 0;
				_spriteRenderer.flipX = isFlipX;
			}
		}
		
		
		protected void FixedUpdate()
		{
			// Apply movement through RigidBody2D
			// To preserve physics fidelity
			_rigidBody2D.MovePosition(_rigidBody2D.position + (_deltaPosition * Time.deltaTime));
		}


		//  Methods ---------------------------------------

		//  Event Handlers --------------------------------
		public void OnCollisionEnter2D ( Collision2D collision2D)
		{
			Crate crate = collision2D.gameObject.GetComponent<Crate>();
			if (crate != null)
			{
				Score++;
			}
		}
	}
}
