using Assets.Scripts.Helpers;
using Assets.Scripts.Models.Quests;
using Miner;
using Miner.GameObjects;
using Miner.Helpers;
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
		public bool IsRepaired { get { return AddedItemsContainer.RequirementsFullfilled; } }
		public GameObject Obstacle;
		public GameObject FixedModel;
		public GameObject BrokenModel;

		public RequiredItemsContainer AddedItemsContainer;
		public List<RequiredItem> RequiredItems;
		public PanelRequirements panelRequirements;
		public Character character;

		public int questId;
		public int checkpointNumber;
		public void Awake()
		{
			AddedItemsContainer = new RequiredItemsContainer();
			AddedItemsContainer.Requirements = RequiredItems;
			CheckRequirements();

			var player = GameObject.FindGameObjectWithTag("Player");
			character = player.GetComponent<Character>();
		}
		public void FixedUpdate()
		{

		}

		public void Refresh()
		{
			panelRequirements.Reload();
		}

		public void CheckRequirements()
		{
			if (AddedItemsContainer.RequirementsFullfilled)
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

		public override void Clicked()
		{
			if (!AddedItemsContainer.RequirementsFullfilled)
			{
				panelRequirements.SetObstacle(this);
				panelRequirements.Reload();
			}
			else
			{
				// If complete, just walk to the clicked position
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				// hit something not in the player layer
				if (Physics.Raycast(ray, out hitInfo) && GUIUtility.hotControl == 0)
				{
					character.WalkTo(hitInfo.point);
				}
			}
		}
		public void AddInitialItem(InventoryItem item)
		{
			if (AddedItemsContainer.CanAdd(item) && !AddedItemsContainer.RequirementsFullfilled)
			{
				AddedItemsContainer.Store(item);
			}
		}
		public InventoryItem AddItem(InventoryItem item)
		{
			InventoryItem returnItem = null;
			if (AddedItemsContainer.CanAdd(item) && !AddedItemsContainer.RequirementsFullfilled)
			{
				AddedItemsContainer.Store(item);
				UpdateQuest(item);
			}
			else
			{
				returnItem = item;
			}

			if (AddedItemsContainer.RequirementsFullfilled)
			{
				CompleteQuest();
				var obstacles = GetComponentsInChildren<NavMeshObstacle>();
				foreach (var obstacle in obstacles)
				{
					obstacle.gameObject.SetActive(false);
				}

				BrokenModel.SetActive(false);
				FixedModel.SetActive(true);
			}

			return returnItem;
		}

		private void UpdateQuest(InventoryItem item)
		{
			var qp = QuestManager.GetQuestProgress(character, questId);

			if (checkpointNumber != 0 && qp.Checkpoints.Count >= checkpointNumber)
			{
				var existing = qp.Checkpoints[checkpointNumber - 1].Items.FirstOrDefault(x => x.ItemId == item.Item.Id);
				if (existing != null)
				{
					existing.Quantity += item.Quantity;
				}
				else
				{
					qp.Checkpoints[checkpointNumber - 1].Items.Add(new QuestItem() { ItemId = item.Item.Id, Quantity = item.Quantity });
				}
				qp.Checkpoints[checkpointNumber - 1].CompletedDate = DateTime.UtcNow;
			}

			StartCoroutine(QuestManager.UpdateQuest(questId, qp, (success, response, err) => { if (success) { SendMessageUpwards("QuestUpdated", response); } }));
		}

		public void CompleteQuest()
		{
			var qp = QuestManager.GetQuestProgress(character, questId);

			if (checkpointNumber != 0 && qp.Checkpoints.Count >= checkpointNumber)
			{
				qp.Checkpoints[checkpointNumber - 1].Complete = true;
				qp.Checkpoints[checkpointNumber - 1].CompletedDate = DateTime.UtcNow;
			}

			StartCoroutine(QuestManager.UpdateQuest(questId, qp, (success, response, err) =>
			{
				if (success)
				{
					SendMessageUpwards("QuestUpdated", response);
				}
			}
			));
		}
	}

	[Serializable]
	public class RequiredItem
	{
		public int ItemId;
		public int Quantity;
	}
}
