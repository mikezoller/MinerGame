using Assets.Scripts.GameObjects.Resources;
using Miner.Helpers;
using Miner.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class CookingFire : MonoBehaviour
	{
		public float duration = 30f;
		public TreeType logType;
		private GameObject fire;
		public bool isLit;
		public void Start()
		{
			gameObject.AddComponent<BoxCollider>();
		}

		public IEnumerator Light(Player playerData)
		{
			// Calculate based on firemaking skill
			while (true)
			{
				bool hit = UnityEngine.Random.Range(0, 100) < 50;

				if (hit)
				{
					isLit = true;
					fire = (GameObject)Instantiate(Resources.Load("Prefabs\\Fire"), gameObject.transform);
					break;
				}
				yield return new WaitForSeconds(4);
			}
			Invoke("Extinguish", duration);
			yield return null;
		}

		public InventoryItem Cook(InventoryItem input)
		{
			InventoryItem result = null;
			// Based on cooking skill, return cooked item or ash

			result = new InventoryItem()
			{
				Item = ItemDatabase.GetItem(input.Item.Id + 1000),
				Quantity = 1
			};
			// Add cooking skill

			return result;
		}

		public void Extinguish()
		{
			isLit = false;
			Destroy(fire);
			Destroy(gameObject);
			// destroy logs, drop ashes
		}

	}
}
