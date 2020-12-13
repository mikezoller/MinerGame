using Assets.Scripts;
using Assets.Scripts.GameObjects;
using Miner;
using Miner.Communication;
using Miner.Helpers;
using Miner.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
	// The target (cylinder) position.
	private Animator animator;


	public bool isInCombat;
	public float strikeRate = 2f;
	public bool isDead;
	public float respawnRate = 10f; // seconds
	public int enemyId;
	private Vector3 respawnLocation;
	public Enemy opponent;
	public float interactionRadius = 5f;
	NavMeshAgent agent;
	private StatDisplay statDisplay;
	public EquippedItems equippedItems;
	public Vector3 initialPosition;
	public Vector3 Destination { get { return agent.destination; } }

	public Player playerData;

	GameManager gameManager;

	public Coroutine currentAction;


	// Start is called before the first frame update
	void Start()
	{
		animator = this.GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		// Don’t update position automatically
		//agent.updatePosition = false;

		var gc = GameObject.FindGameObjectWithTag("GameController");
		gameManager = gc.GetComponent<GameManager>();
		SetClothing();

		if (statDisplay == null)
		{
			statDisplay = this.gameObject.AddComponent<StatDisplay>();
		}
		var bounds = this.GetComponent<BoxCollider>().bounds;
		statDisplay.Init(new Vector3(0, bounds.size.y, 0));
		UpdateHealthBar();
	}
	public void WalkTo(Vector3 pos)
	{
		agent.stoppingDistance = 0;
		CancelAllActions();
		isInCombat = false;
		agent.ResetPath();
		agent.SetDestination(pos);
	}
	public void CancelAllActions()
	{
		if (currentAction != null)
		{
			StopCoroutine(currentAction);
			currentAction = null;
			if (!string.IsNullOrEmpty(activeAnimation))
			{
				ClearAnimBool(activeAnimation);
			}
			StartAnimation("Idle");
		}
		if (isInCombat)
		{
			StopFight();
		}
	}

	public void HandleRayCastHit(RaycastHit hit, Action callback = null)
	{
		var fightable = hit.transform.gameObject.GetComponent<Enemy>();
		if (fightable != null)
		{
			InitiateCombat(fightable);
		}
		var droppable = hit.transform.gameObject.GetComponent<DroppableComponent>();
		if (droppable != null && !droppable.Claimed)
		{
			droppable.Claimed = true;
			var i = droppable.Item.Copy(1);
			AddToInventory(i, () =>
			{
				Destroy(hit.transform.gameObject);
			},
			() =>
			{
				// TODO: Show fail message 
			}
			);
		}
		var campfire = hit.transform.gameObject.GetComponent<CookingFire>();
		if (campfire != null)
		{
			var item = gameManager.inventory.GetSelectedItem();
			if (item != null)
			{
				if (campfire.isLit)
				{
					InventoryItem result = campfire.Cook(item);
					// TODO: remove this after implementing Cook Server method
					// Add cookable, eatable flags to Item
					RemoveFromInventory(item);
					AddToInventory(result);
				}
			}
		}

		callback?.Invoke();
	}

	public void AddToInventory(InventoryItem item, Action success = null, Action fail = null)
	{
		if (playerData.Inventory.CanAdd(item))
		{
			StartCoroutine(PlayersApi.AddToInventory("mwnzoller", item.item.id, item.quantity, (user, err) =>
			{
				if (err != null)
				{
					Debug.LogError(err);
					fail?.Invoke();
				}
				else
				{
					success?.Invoke();
					playerData.Inventory.Store(item);
					gameManager.inventory.Reload();
				}
			}));
		}
	}

	public void SetAnimBool(string trigger)
	{
		animator.SetBool(trigger, true);
	}
	public void ClearAnimBool(string trigger)
	{
		animator.SetBool(trigger, false);
	}
	public void StartAnimation(string trigger)
	{
		animator.SetTrigger(trigger);
	}

	public string activeAnimation;
	public void DoAction(Vector3 position, string animation, IEnumerator toDo, float targetRadius = 10)
	{
		if (currentAction != null)
		{
			StopCoroutine(currentAction);
			currentAction = null;
		}
		if (!string.IsNullOrEmpty(activeAnimation))
		{
			animator.SetBool(activeAnimation, false);
			animator.SetTrigger("Idle");
		}
		activeAnimation = animation;
		currentAction = StartCoroutine(DoIt(position, animation, toDo, targetRadius));
	}

	private IEnumerator DoIt(Vector3 position, string animation, IEnumerator toDo, float targetRadius = 10)
	{
		while (true)
		{
			var distance = Vector3.Distance(position, transform.position);
			if (distance < targetRadius)
			{
				this.transform.LookAt(position, Vector3.up);
				break;
			}
			yield return new WaitForSeconds(1);
		}
		if (!string.IsNullOrEmpty(animation))
		{
			animator.SetBool(animation, true);
		}
		yield return toDo;
		if (!string.IsNullOrEmpty(animation))
		{
			animator.SetBool(animation, false);
			animator.SetTrigger("Idle");
		}
		activeAnimation = "";
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		bool shouldMove = agent.remainingDistance > agent.stoppingDistance;

		// Update animation parameters
		if (shouldMove)
		{
			animator.SetBool("walking", true);

		}
		else if (animator.GetBool("walking"))
		{
			StartCoroutine(PlayersApi.UpdateLocation("mwnzoller", this.transform.position));
			animator.SetBool("walking", false);
		}

		if (isInCombat)
		{
			this.transform.LookAt(this.opponent.transform);
			if (Vector3.Distance(this.transform.position, this.opponent.transform.position) > interactionRadius)
			{
				agent.stoppingDistance = interactionRadius;
				agent.SetDestination(opponent.transform.position);
			}
		}
	}

	public enum BodyPart
	{
		Head,
		Arms,
		Hands,
		Chest,
		Legs,
		Feet,
	}

	#region Clothing
	public enum ClothingType
	{
		Body,
		Clothing1,
		Armor1,
		All
	}

	private readonly Dictionary<BodyPart, string> bodyPartDict = new Dictionary<BodyPart, string>
	{
		{ BodyPart.Head, "Head" },
		{ BodyPart.Arms, "Arms" },
		{ BodyPart.Hands, "Hands" },
		{ BodyPart.Chest, "Chest" },
		{ BodyPart.Legs, "Legs" },
		{ BodyPart.Feet, "Feet" },
	};


	public void RemoveAllClothing()
	{
		foreach (var t in bodyPartDict.Keys)
		{
			RemoveBodyPartClothing(t);
		}
	}

	public void RemoveBodyPartClothing(BodyPart bodyPart)
	{
		SetBodyPartClothing(bodyPart, ClothingType.All, false);
		SetBodyPartClothing(bodyPart, ClothingType.Body, true);
	}
	public void SetClothing()
	{
		RemoveAllClothing();
		SetBodyPartClothing(BodyPart.Legs, ClothingType.Armor1, true);
		SetSkinColor(Color.red);
	}

	public void SetSkinColor(Color color)
	{
		var body = this.transform.Find("Body");

		var bodyParts = Enum.GetValues(typeof(BodyPart));
		foreach (BodyPart bodyPart in bodyParts)
		{
			var bodyPartRoot = body.Find(bodyPart.ToString());
			string bpString = bodyPart.ToString() + "Body";
			var bp = bodyPartRoot.Find(bpString);
			var renderer = bp.GetComponent<SkinnedMeshRenderer>();
			renderer.material.color = color;
		}
	}
	private void SetBodyPartClothing(BodyPart bodyPart, ClothingType clothingType, bool active)
	{
		var body = this.transform.Find("Body");

		var bp = body.Find(bodyPart.ToString());
		if (clothingType == ClothingType.All)
		{
			for (var i = 0; i < bp.childCount; i++)
			{
				var child = bp.GetChild(i);
				child.gameObject.SetActive(active);
			}
		}
		else
		{
			// Hide the rest of this body part clothing
			if (active)
			{
				SetBodyPartClothing(bodyPart, ClothingType.All, false);
			}
			var item = bp.Find(bodyPart.ToString() + clothingType.ToString());
			item.gameObject.SetActive(active);
		}
	}
	#endregion Clothing

	#region Combat
	private bool InitiateCombat(Enemy opponent)
	{
		if (CanFight(opponent) && opponent.CanFight(this))
		{
			statDisplay.ShowHealthBar(true);
			agent.stoppingDistance = interactionRadius;
			agent.SetDestination(opponent.Destination);
			DoAction(opponent.Destination, null, StartFight(opponent), interactionRadius);
		}

		return isInCombat;
	}

	private IEnumerator StartFight(Enemy opponent)
	{
		isInCombat = true;
		this.opponent = opponent;
		CancelInvoke("Strike");
		InvokeRepeating("Strike", 1, strikeRate);
		yield return null;
	}

	public void StopFight()
	{
		this.isInCombat = false;
		this.opponent = null;
		Invoke("HideHealthBar", 3f);
		CancelInvoke("Strike");
	}

	public void HideHealthBar()
	{
		statDisplay.ShowHealthBar(false);

	}

	public void ReceiveDamage(Enemy enemy, int amount)
	{
		if (enemy != null && CanFight(enemy) && this.opponent != enemy)
		{
			DoAction(opponent.transform.position, null, StartFight(enemy), interactionRadius);
		}
		List<SimpleActionRequest> requests = new List<SimpleActionRequest>()
		{
			MultiActionRequest.GetSimpleRequest(MultiActionRequest.ActionType.SetHealth, this.playerData.CurrentStats.Health - amount),
			MultiActionRequest.GetSimpleRequest(MultiActionRequest.ActionType.AddDefenseExperience,  amount),
		};

		StartCoroutine(PlayersApi.DoMultipleActions("mwnzoller", requests, (success, err) =>
		{
			if (err != null)
			{
				Debug.LogError("error syncing with server!");
			}
			else
			{
				var s = this.playerData.Progress.Skills.FirstOrDefault(x => x.SkillType == SkillType.Hitpoints);

				if (s != null)
				{
					s.AddExperience(amount);
					SendMessageUpwards("ExperienceEarned", SkillType.Hitpoints);
				}

				statDisplay.AddHit(amount);
				this.playerData.CurrentStats.Health -= amount;
				UpdateHealthBar();
				isDead = this.playerData.CurrentStats.Health <= 0;

				if (isDead)
				{
					StopFight();

					// Start respawn timer
					Invoke("Respawn", 5f);
				}
			}
		}));
	}

	public void DoDamage(int amount)
	{
		var rand = UnityEngine.Random.Range(0, 100);
		if (rand <= this.opponent.enemyData.Defense)
		{
			amount = 0;
			this.opponent.ReceiveDamage(this, amount);
		}
		else
		{
			StartCoroutine(PlayersApi.DoCombatAction("mwnzoller", 400, amount, (user, err) =>
			{
				if (err != null)
				{
					Debug.LogError("error syncing with server!");
				}
				else
				{
					var s = this.playerData.Progress.Skills.FirstOrDefault(x => x.SkillType == SkillType.Attack);

					if (s != null)
					{
						s.AddExperience(amount);
						SendMessageUpwards("ExperienceEarned", SkillType.Attack);
					}
					this.opponent.ReceiveDamage(this, amount);
				}
			}));
		}
	}
	#endregion Combat

	public void Respawn()
	{
		int fullHealth = this.playerData.Progress.GetFullHeath();
		StartCoroutine(PlayersApi.SetHealth("mwnzoller", fullHealth, (user, err) =>
		{
			if (err != null)
			{
				Debug.LogError(err);
			}
			else
			{
				this.playerData.CurrentStats.Health = fullHealth;

				this.gameObject.transform.position = respawnLocation;
				this.agent.SetDestination(respawnLocation);

				UpdateHealthBar();
				isDead = false;
				this.gameObject.SetActive(true);
			}
		}));
	}

	private void UpdateHealthBar()
	{
		this.statDisplay.SetHealth(this.playerData.Progress.GetFullHeath(), this.playerData.CurrentStats.Health);
	}

	public void Strike()
	{
		if (this.opponent != null && !this.opponent.isDead)
		{
			if (!this.opponent.isInCombat)
			{
				InitiateCombat(this.opponent);
			}
			StartAnimation("striking");
			var acc = playerData.Progress.GetAccuracy();
			bool hit = UnityEngine.Random.Range(0, 100) < acc;
			int amount = 0;
			if (hit)
			{
				var strength = playerData.Progress.GetStrength();
				amount = (int)Math.Ceiling(UnityEngine.Random.Range(0.0f, 1.0f) * strength);
			}
			this.DoDamage(amount);
		}
		else
		{
			StopFight();
		}
	}

	private void RemoveFromInventory(InventoryItem invItem, Action success = null, Action fail = null)
	{
		if (playerData.Inventory.HasAtLeast(invItem.item.id, invItem.quantity))
		{
			StartCoroutine(PlayersApi.RemoveFromInventory("mwnzoller", invItem.item.id, invItem.quantity, (user, err) =>
			{
				if (err != null)
				{
					Debug.LogError(err);
					fail?.Invoke();
				}
				else
				{
					success?.Invoke();
					playerData.Inventory.Remove(invItem);
					gameManager.inventory.Reload();
				}
			}));
		}
	}

	public void StartFire(InventoryItem item)
	{
		RemoveFromInventory(item, () =>
		{
			var o = (GameObject)Instantiate(Resources.Load("Prefabs\\campfire"), this.transform.position, Quaternion.identity);
			var cookingFire = o.AddComponent<CookingFire>();
			DoAction(transform.position, null, cookingFire.Light(playerData));
		});
	}

	public bool CanFight(Enemy o)
	{
		return !isInCombat || (this.opponent == o);
	}

	public void TransferToInventory(InventoryItem item, Action success = null, Action fail = null)
	{
		if (playerData.Inventory.CanAdd(item))
		{
			StartCoroutine(PlayersApi.TransferToInventory("mwnzoller", item.item.id, item.quantity, (user, err) =>
			{
				if (err != null)
				{
					Debug.LogError(err);
					fail?.Invoke();
				}
				else
				{
					playerData.Inventory.Store(item);
					playerData.Bank.Remove(item);
					success?.Invoke();
				}
			}));
		}
	}
	public void TransferToBank(InventoryItem item, Action success = null, Action fail = null)
	{
		if (playerData.Bank.CanAdd(item))
		{
			StartCoroutine(PlayersApi.TransferToBank("mwnzoller", item.item.id, item.quantity, (user, err) =>
			{
				if (err != null)
				{
					Debug.LogError(err);
					fail?.Invoke();
				}
				else
				{
					playerData.Inventory.Remove(item);
					playerData.Bank.Store(item);

					success?.Invoke();
				}
			}));
		}
	}
	public void TransferAllToBank(Action success = null, Action fail = null)
	{
		StartCoroutine(PlayersApi.TransferAllToBank("mwnzoller", (user, err) =>
		{
			if (err != null)
			{
				Debug.LogError(err);
				fail?.Invoke();
			}
			else
			{

				var items = playerData.Inventory.InventoryItems;
				bool canAddAll = true;
				foreach (var item in items)
				{
					if (!playerData.Bank.CanAdd(item))
					{
						canAddAll = false;
						break;
					}
				}

				if (canAddAll)
				{
					foreach (var item in items)
					{
						playerData.Bank.Store(item);
					}
					playerData.Inventory.InventoryItems.Clear();
				}
				success?.Invoke();

			}
		}));
	}

	private void SetHealth(int amount, Action success = null, Action fail = null)
	{
		// Send to server
		StartCoroutine(PlayersApi.SetHealth("mwnzoller", amount, (user, err) =>
		{
			if (err != null)
			{
				Debug.LogError(err);
				fail?.Invoke();
			}
			else
			{
				success?.Invoke();
			}
		}));
	}

	public void InventoryItemClicked(InventoryItem invItem)
	{
		ItemTypes itemType = invItem.item.ItemType;

		if (itemType == ItemTypes.Food)
		{
			FoodData data = JsonConvert.DeserializeObject<FoodData>(invItem.item.ItemData);
			playerData.CurrentStats.AddHealth(data.HealAmount);
			SetHealth(playerData.CurrentStats.Health, () =>
			{
				RemoveFromInventory(invItem);
			});
		}
		if (invItem.item.id >= 200 && invItem.item.id < 250)
		{
			StartFire(invItem);
		}
	}
}
