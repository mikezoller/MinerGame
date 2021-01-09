using Assets.Scripts.Helpers;
using Assets.Scripts.Models.Quests;
using Miner;
using Miner.Communication;
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

			panelRequirements.OnClosedDelegate += OnUiClosed;
		}
		void OnUiClosed()
		{
			UpdateQuest();
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
			bool done = AddedItemsContainer.RequirementsFullfilled;

			FixedModel.SetActive(done);
			BrokenModel.SetActive(!done);

			var obstacles = GetComponentsInChildren<NavMeshObstacle>(true);
			foreach (var obstacle in obstacles)
			{
				obstacle.gameObject.SetActive(!done);
			}
		}

		public override void Clicked()
		{
			if (!AddedItemsContainer.RequirementsFullfilled)
			{
				panelRequirements.SetObstacle(this);
				panelRequirements.Reload();
				base.Clicked();
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
				//UpdateQuest(item);
			}
			else
			{
				returnItem = item;
			}

			if (AddedItemsContainer.RequirementsFullfilled)
			{
				//CompleteQuest();
				CheckRequirements();
			}

			return returnItem;
		}
		private void UpdateQuest()
		{
			var qp = QuestManager.GetQuestProgress(character, questId);
			Dictionary<int, int> itemsAdded = new Dictionary<int, int>();

			if (checkpointNumber != 0 && qp.Checkpoints.Count >= checkpointNumber)
			{
				bool isComplete = true;
				var baseQuest = QuestDatabase.GetQuest(questId);
				var baseCheckpoint = baseQuest.Checkpoints[checkpointNumber - 1];

				var checkpoint = qp.Checkpoints[checkpointNumber - 1];
				foreach (var item in AddedItemsContainer.InventoryItems)
				{
					int numAdded = 0;

					// Update quest progress
					var existing = checkpoint.Items.FirstOrDefault(x => x.ItemId == item.Item.Id);
					if (existing != null)
					{
						numAdded = item.Quantity - existing.Quantity;
						existing.Quantity = item.Quantity;
					}
					else
					{
						checkpoint.Items.Add(new QuestItem() { ItemId = item.Item.Id, Quantity = item.Quantity });
						numAdded = item.Quantity;
					}

					// Add to dictionary of items to remove
					if (itemsAdded.ContainsKey(item.Item.Id))
					{
						itemsAdded[item.Item.Id] += numAdded;
					}
					else
					{
						itemsAdded.Add(item.Item.Id, numAdded);
					}

					// Check if we have fulfilled this checkpoints requirements for this item
					if (item.Quantity != baseCheckpoint.Items.Find(x => x.ItemId == item.Item.Id).Quantity)
					{
						isComplete = false;
					}
				}

				// Mark complete if all required items have been filled
				// Also make sure the number of added inventory items match, otherwise wouldn't have had to set isComplete above
				if (AddedItemsContainer.InventoryItems.Count == baseCheckpoint.Items.Count && isComplete)
				{
					checkpoint.Complete = true;
					checkpoint.CompletedDate = DateTime.UtcNow;
				}
			}


			if (itemsAdded.Count > 0)
			{
				StartCoroutine(QuestManager.UpdateQuest(questId, qp, (success, response, err) =>
				{
					if (success)
					{
						SendMessageUpwards("QuestUpdated", response);
						// Remove items from players server inventory
						StartCoroutine(PlayersApi.RemoveFromInventory(character.playerData.Name, itemsAdded));
					}
				}));
			}
		}
		//private void UpdateQuest(InventoryItem item)
		//{
		//	var qp = QuestManager.GetQuestProgress(character, questId);

		//	if (checkpointNumber != 0 && qp.Checkpoints.Count >= checkpointNumber)
		//	{
		//		var existing = qp.Checkpoints[checkpointNumber - 1].Items.FirstOrDefault(x => x.ItemId == item.Item.Id);
		//		if (existing != null)
		//		{
		//			existing.Quantity += item.Quantity;
		//		}
		//		else
		//		{
		//			qp.Checkpoints[checkpointNumber - 1].Items.Add(new QuestItem() { ItemId = item.Item.Id, Quantity = item.Quantity });
		//		}
		//		qp.Checkpoints[checkpointNumber - 1].CompletedDate = DateTime.UtcNow;
		//	}

		//	StartCoroutine(QuestManager.UpdateQuest(questId, qp, (success, response, err) => { if (success) { SendMessageUpwards("QuestUpdated", response); } }));
		//}

		//public void CompleteQuest()
		//{
		//	var qp = QuestManager.GetQuestProgress(character, questId);

		//	if (checkpointNumber != 0 && qp.Checkpoints.Count >= checkpointNumber)
		//	{
		//		qp.Checkpoints[checkpointNumber - 1].Complete = true;
		//		qp.Checkpoints[checkpointNumber - 1].CompletedDate = DateTime.UtcNow;
		//	}

		//	StartCoroutine(QuestManager.UpdateQuest(questId, qp, (success, response, err) =>
		//	{
		//		if (success)
		//		{
		//			SendMessageUpwards("QuestUpdated", response);
		//		}
		//	}
		//	));
		//}
	}

	[Serializable]
	public class RequiredItem
	{
		public int ItemId;
		public int Quantity;
	}
}
