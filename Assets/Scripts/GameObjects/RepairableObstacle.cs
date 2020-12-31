using Miner;
using Miner.GameObjects;
using Miner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GameObjects
{
	[Serializable]
	public class RepairableObstacle : InteractionObject
	{
		public string Name;
		public bool IsRepaired { get { return Container.RequirementsFullfilled; } }
		public GameObject Obstacle;
		public GameObject FixedModel;
		public GameObject BrokenModel;

		public RequiredItemsContainer Container;
		public List<RequiredItem> RequiredItems;
		public PanelRequirements panelRequirements;
		public void Awake()
		{
			Container = new RequiredItemsContainer();
			Container.Requirements = RequiredItems;

			if (Container.RequirementsFullfilled)
			{
				FixedModel.SetActive(true);
				BrokenModel.SetActive(false);
			}
			else
			{

				FixedModel.SetActive(false);
				BrokenModel.SetActive(true);
			}
		}
		public void FixedUpdate()
		{

		}

		public void Clicked()
		{
			if (!Container.RequirementsFullfilled)
			{
				panelRequirements.SetObstacle(this);
			}
			else
			{
				var player = GameObject.FindGameObjectWithTag("Player");
				var character = player.GetComponent<Character>();

				var gc = GameObject.FindGameObjectWithTag("GameController");
				var gameManager = gc.GetComponent<GameManager>();
				character.GetComponent<NavMeshAgent>().ResetPath();
				var bounds = this.GetComponent<Collider>().bounds;
				character.GetComponent<NavMeshAgent>().SetDestination(bounds.center);
				character.DoAction(this.transform.position, null, null, Math.Max(bounds.size.x / 2.0f, bounds.size.z / 2.0f) + 5);
			}
		}

		public InventoryItem AddItem(InventoryItem item)
		{
			InventoryItem returnItem = null;
			if (Container.CanAdd(item) && !Container.RequirementsFullfilled)
			{
				Container.Store(item);
			}
			else
			{
				returnItem = item;
			}

			if (Container.RequirementsFullfilled)
			{
				var obstacles = GetComponentsInChildren<NavMeshObstacle>();
				foreach(var obstacle in obstacles)
				{
					obstacle.gameObject.SetActive(false);
				}

				BrokenModel.SetActive(false);
				FixedModel.SetActive(true);
			}

			return returnItem;
		}

	}

	[Serializable]
	public class RequiredItem{
		public int ItemId;
		public int Quantity;
		}
}
