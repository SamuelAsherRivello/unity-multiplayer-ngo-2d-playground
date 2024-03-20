using RMC.Playground2D.Shared.UI;
using UnityEngine;

namespace RMC.Playground2D.Shared
{
	/// <summary>
	/// Hold references to Scene elements
	/// </summary>
	public class World : MonoBehaviour
	{
		//  Properties ------------------------------------
		public WorldUI WorldUI
		{
			get
			{
				return worldUI;
			}
		}
		
		public SpawnPoints SpawnPoints
		{
			get
			{
				return _spawnPoints;
			}
		}

		//  Fields ----------------------------------------
		[SerializeField]
		private WorldUI worldUI;
		
		[SerializeField]
		private SpawnPoints _spawnPoints;
		
	}
}
