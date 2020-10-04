using Miner.Communication;
using Miner.Helpers;
using Miner.Models;
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
		public IEnumerator DoWork(GameManager gameManager)
		{
			var playerData = gameManager.playerData;
			var player = GameObject.FindGameObjectWithTag("Player");
			var character = player.GetComponent<Character>();

			var result = ResourceActionDatabase.GetResourceResult(actionId);
			bool success = false;
			while (!success)
			{
				//Task.Delay(TimeSpan.FromSeconds(interval));
				//await Task.Yield();
				success = UnityEngine.Random.Range(0.0f, 1.0f) > result.Probability;
				yield return new WaitForSeconds((float)result.Interval);
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
					playerData.Inventory.Store(inventoryItem);
					var s = playerData.Progress.Skills.FirstOrDefault(x => x.SkillType == result.Skill);

					if (s != null)
					{
						s.AddExperience(result.Experience);
						SendMessageUpwards("ExperienceEarned", result.Skill);
					}
					gameManager.inventory.Reload();
				}
			}));
		}

		public IEnumerator Recharge()
		{
			yield return new WaitForSeconds(5);
			Depleted = false;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			var character = player.GetComponent<Character>();

			var gc = GameObject.FindGameObjectWithTag("GameController");
			var gameManager = gc.GetComponent<GameManager>();
			var closest = this.GetComponent<Collider>().ClosestPoint(character.transform.position);
			NavMeshHit myNavHit;
			if (NavMesh.SamplePosition(closest, out myNavHit, 100, -1))
			{
				character.GetComponent<NavMeshAgent>().ResetPath();
				character.GetComponent<NavMeshAgent>().SetDestination(myNavHit.position);

				var bounds = this.GetComponent<Collider>().bounds;
				if (!Depleted)
				{
					character.DoAction(myNavHit.position, StartTrigger, DoWork(gameManager), Math.Max(bounds.size.x / 2.0f, bounds.size.z / 2.0f) + Vector3.Distance(myNavHit.position, this.transform.position));
				}
			}
		}
	}
}