using System.Collections.Generic;
using UnityEngine;

namespace RMC.Playground2D.Shared
{
	/// <summary>
	/// Manage player spawn points allowing for repeated use
	/// of the same spawn points
	/// </summary>
	public class SpawnPoints : MonoBehaviour
	{
		//  Fields ----------------------------------------
		[SerializeField]
		private List<GameObject> _crateSpawnPoints;

		
		[SerializeField]
		private List<GameObject> _playerSpawnPoints;

		//  Methods ---------------------------------------
		public GameObject GetCrateSpawnPoint()
		{
			return _crateSpawnPoints[0];
		}
		
		
		public GameObject GetPlayerSpawnPointByIndex(int index)
		{
			//Get player spot
			int newIndex = index % _playerSpawnPoints.Count;
			
			return _playerSpawnPoints[newIndex];
		}
	}
}
