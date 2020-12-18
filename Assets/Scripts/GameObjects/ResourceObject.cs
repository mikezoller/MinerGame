using Miner.Communication;
using Miner.Helpers;
using Miner.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
#endif

namespace Miner.GameObjects
{
	public class ResourceObject : MonoBehaviour, IPointerClickHandler
	{
		protected int actionId;
		protected string StartTrigger;
		protected string StopTrigger = "Idle";
		protected string resourceMaterialName;
		protected Material resourceMaterial;
		private bool depleted;
		public ResourceType resourceType;
		public GameManager gameManager;
		public enum ResourceType
		{
			None,
			Chopping,
			Mining,
			Fishing,
		}
		public bool Depleted
		{
			get { return depleted; }
			set
			{
				depleted = value;
				if (depleted)
				{
					OnDepleted();
				}
				else
				{
					OnRefilled();
				}
			}
		}
		public void Awake()
		{
			var gc = GameObject.FindGameObjectWithTag("GameController");
			 gameManager = gc.GetComponent<GameManager>();
		}
		public virtual void OnDepleted()
		{

		}
		public virtual void OnRefilled()
		{

		}
		// Refresh yourself
		public InventoryItem GetItem()
		{
			var result = ResourceActionDatabase.GetResourceResult(actionId);
			return new InventoryItem() { item = ItemDatabase.GetItem(result.ItemId), quantity = result.Quantity };
		}
		public IEnumerator DoWork(GameManager gameManager, WeaponData toolData, Action done = null)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			var character = player.GetComponent<Character>();

			var result = ResourceActionDatabase.GetResourceResult(actionId);
			bool success = false;
			while (!success)
			{
				success = UnityEngine.Random.Range(0.0f, 1.0f) < result.Probability + (toolData != null ? toolData.ProbabilityBuff : 0);
				yield return new WaitForSeconds(1f);
			}
			Depleted = true;
			StartCoroutine(Recharge());

			var inventoryItem = GetItem();
			StartCoroutine(PlayersApi.DoResourceAction("mwnzoller", actionId, (user, err) =>
			{
				if (err != null)
				{
					Debug.LogError("error syncing with server!");
				}
				else
				{
					character.playerData.Inventory.Store(inventoryItem);
					var s = character.playerData.Progress.Skills.FirstOrDefault(x => x.SkillType == result.Skill);

					if (s != null)
					{
						s.AddExperience(result.Experience);
						SendMessageUpwards("ExperienceEarned", result.Skill);
					}

					gameManager.inventory.Reload();

					done?.Invoke();
				}
			}));
		}

		public IEnumerator Recharge()
		{
			var result = ResourceActionDatabase.GetResourceResult(actionId);
			yield return new WaitForSeconds((float)result.Interval);
			Depleted = false;
		}

		public bool CanAccessResource(out Item tool, out WeaponData wd)
		{
			bool canAccess = false;

			canAccess = HasTool(out tool, out wd);

			return canAccess;
		}

		public bool HasTool(out Item tool, out WeaponData wd)
		{
			// Make sure character has tool/weapon
			bool hasTool = false;
			wd = null;
			tool = null;
			if (resourceType == ResourceType.Chopping)
			{
				tool = gameManager.character.GetBestTool(out wd, x => x.CanChop);
				hasTool = true;
			}
			else if (resourceType == ResourceType.Mining)
			{
				tool = gameManager.character.GetBestTool(out wd, x => x.CanMine);
				hasTool = true;
			}
			else { hasTool = true; }
			return hasTool;
		}
		public bool NeedsTool()
		{
			// Make sure character has tool/weapon
			bool needsTool = false;
			if (resourceType == ResourceType.Chopping)
			{
				needsTool = true;
				if (gameManager.character.equippedItems.Weapon != null)
				{
					needsTool = !JsonConvert.DeserializeObject<WeaponData>(gameManager.character.equippedItems.Weapon.ItemData).CanChop;
				}
			}
			else if (resourceType == ResourceType.Mining)
			{
				needsTool = true;
				if (gameManager.character.equippedItems.Weapon != null)
				{
					needsTool = !JsonConvert.DeserializeObject<WeaponData>(gameManager.character.equippedItems.Weapon.ItemData).CanMine;
				}
			}
			else { needsTool = false; }
			return needsTool;
		}

		private void HarvestResource()
		{

		}

		public void OnPointerClick(PointerEventData eventData)
		{

			var closest = this.GetComponent<Collider>().ClosestPoint(gameManager.character.transform.position);
			NavMeshHit myNavHit;
			if (NavMesh.SamplePosition(closest, out myNavHit, 100, -1))
			{
				gameManager.character.GetComponent<NavMeshAgent>().ResetPath();
				gameManager.character.GetComponent<NavMeshAgent>().SetDestination(myNavHit.position);

				Item tool = null;
				WeaponData toolData = null;
				bool needsTool = NeedsTool();
				bool hasTool = false;

				if (needsTool) hasTool = HasTool(out tool, out toolData);

				if (Depleted)
				{
					gameManager.ShowMessage("Resource depleted. Wait for recharge", MessagePanel.MessageType.OK);
				}
				else if (needsTool && !hasTool)
				{
					gameManager.ShowMessage("You don't have the required tool.", MessagePanel.MessageType.OK);
				}
				else
				{
					if (needsTool && hasTool && gameManager.character.equippedItems.Weapon != tool)
					{
						gameManager.character.equippedItems.ToolTempSwap = gameManager.character.equippedItems.Weapon;
						gameManager.character.SetEquipment(Assets.Scripts.EquipmentSpot.Weapon, tool);
					}
					var bounds = this.GetComponent<Collider>().bounds;
					var maxDim = Math.Max(bounds.size.x / 2.0f, bounds.size.z / 2.0f);
					float stoppingDistance = maxDim;// + +Vector3.Distance(myNavHit.position, this.transform.position);

					gameManager.character.DoAction(myNavHit.position, StartTrigger, DoWork(gameManager, toolData, () =>
					{
						if (tool != null && gameManager.character.equippedItems.ToolTempSwap != null)
						{
							gameManager.character.SetEquipment(Assets.Scripts.EquipmentSpot.Weapon, gameManager.character.equippedItems.ToolTempSwap);
						}
					}), stoppingDistance);
				}
			}
		}
	}
}