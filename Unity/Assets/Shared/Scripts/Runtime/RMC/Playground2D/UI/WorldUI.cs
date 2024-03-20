using UnityEngine;
using TMPro;

namespace RMC.Playground2D.Shared.UI
{
	/// <summary>
	/// Hold references to the user interface elements
	/// </summary>
	public class WorldUI : MonoBehaviour
	{
		//  Properties ------------------------------------
		public TMP_Text ScoreText
		{
			get
			{
				return _scoreText;
			}
		}
		
		public TMP_Text InstructionsText
		{
			get
			{
				return _instructionsText;
			}
		}
		
		public TMP_Text SceneDescriptionText
		{
			get
			{
				return _sceneDescriptionText;
			}
		}

		//  Fields ----------------------------------------
		[SerializeField]
		private TMP_Text _scoreText;
		
		[SerializeField]
		private TMP_Text _instructionsText;
		
		[SerializeField]
		private TMP_Text _sceneDescriptionText;
	}
}
