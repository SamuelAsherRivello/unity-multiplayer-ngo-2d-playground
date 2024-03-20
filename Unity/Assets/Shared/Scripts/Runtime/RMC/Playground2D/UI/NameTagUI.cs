using UnityEngine;
using TMPro;

namespace RMC.Playground2D.Shared.UI
{
	/// <summary>
	/// Hold references to the user interface elements
	/// </summary>
	public class NameTagUI : MonoBehaviour
	{
		//  Properties ------------------------------------
		public TMP_Text NameTagText
		{
			get
			{
				return _nameTagText;
			}
		}
		
		//  Fields ----------------------------------------
		[SerializeField]
		private TMP_Text _nameTagText;
	}
}
