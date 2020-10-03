using Assets.Scripts;
using Assets.Scripts.GameObjects;
using Miner.Communication;
using Miner.Helpers;
using Miner.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Miner.GameObjects
{

	public class PanelRequirements : MonoBehaviour
	{
		public ItemGrid itemGrid;

		private RepairableObstacle obstacle;

		public void SetObstacle(RepairableObstacle obs)
		{
			obstacle = obs;
			itemGrid.items = obs.Container.InventoryItems;
		}
		private void Awake()
		{
		}

		private void OnEnable()
		{
		}


		public InventoryItem AddItem(InventoryItem item)
		{
			InventoryItem returnItem = obstacle.AddItem(item);
			Reload();
			Debug.Log("Unlocked " + this.obstacle.Container.RequirementsFullfilled);
			return returnItem;
		}
		public void Reload()
		{
			itemGrid.Reload();
		}
		
	}
}