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

	public GameManager gameManager;

	public Coroutine currentAction;

	// Equipment
	public GameObject weaponBone;

	// Start is called before the first frame update
	void Start()
	{
		animator = this.GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

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
			StartCoroutine(PlayersApi.AddToInventory("mwnzoller", item.Item.Id, item.Quantity, (user, err) =>
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
		if (playerData.Inventory.HasAtLeast(invItem.Item.Id, invItem.Quantity))
		{
			StartCoroutine(PlayersApi.RemoveFromInventory("mwnzoller", invItem.Item.Id, invItem.Quantity, (user, err) =>
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
			StartCoroutine(PlayersApi.TransferToInventory("mwnzoller", item.Item.Id, item.Quantity, (user, err) =>
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
			StartCoroutine(PlayersApi.TransferToBank("mwnzoller", item.Item.Id, item.Quantity, (user, err) =>
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
		var baseItem = ItemDatabase.GetItem(invItem.Item.Id);

		// Don't do anything if we are working with the panels
		if (!PanelsActive())
		{
			ItemTypes itemType = baseItem.ItemType;
			if (itemType == ItemTypes.Food)
			{
				FoodData data = JsonConvert.DeserializeObject<FoodData>(invItem.Item.ItemData);
				SetHealth(playerData.CurrentStats.Health, () =>
				{
					RemoveFromInventory(invItem);
					playerData.CurrentStats.AddHealth(data.HealAmount);
					UpdateHealthBar();
				});
			}
			if (invItem.Item.Id >= 200 && invItem.Item.Id < 250)
			{
				StartFire(invItem);
			}
			if (itemType == ItemTypes.Armor)
			{
				ArmorData armorData = JsonConvert.DeserializeObject<ArmorData>(baseItem.ItemData);
				EquipmentSpot spot = armorData.EquipmentSpot;
				// Equip armor
				SetEquipment(spot, baseItem);
			}
			if (itemType == ItemTypes.Weapon)
			{
				SetEquipment(EquipmentSpot.Weapon, baseItem);
			}
		}
	}

	private bool PanelsActive()
	{
		return gameManager.panelBank.gameObject.activeSelf
			|| gameManager.panelRequirements.gameObject.activeSelf;
	}

	// MAIN Equipment method. Calls both game obejct and server methods if necessary
	public void SetEquipment(EquipmentSpot spot, Item item)
	{
		EquipItemServer(null, EquipmentSpot.Weapon, () =>
		{
			RemoveEquipmentGameObject(EquipmentSpot.Weapon);

			if (item != null && item.ItemType == ItemTypes.Weapon)
			{
				EquipItemServer(item, EquipmentSpot.Weapon, () =>
				{
					SetEquipmentItemGameObject(item);
				});
			}
		});
	}

	public void RemoveEquipmentGameObject(EquipmentSpot spot)
	{
		switch (spot)
		{
			case EquipmentSpot.Weapon:
				if (weaponBone.transform.childCount > 2)
				{
					var existing = weaponBone.transform.GetChild(2);
					Destroy(existing.gameObject);
				}
				break;
		}
	}
	public void SetEquipmentItemsGameObjects(EquippedItems eq)
	{
		foreach (EquipmentSpot spot in Enum.GetValues(typeof(EquipmentSpot)))
		{
			if (eq[spot] != null)
			{
				SetEquipmentItemGameObject(eq[spot]);
			}
		}
	}
	public void SetEquipmentItemGameObject(Item item)
	{
		EquipmentSpot spot = EquipmentSpot.Feet;
		if (item.ItemType == ItemTypes.Weapon)
		{
			var weaponData = JsonConvert.DeserializeObject<WeaponData>(item.ItemData);
			var o = (GameObject)Instantiate(Resources.Load("Prefabs\\" + weaponData.ModelName), this.transform.position, Quaternion.identity);

			o.transform.parent = weaponBone.transform;
			o.transform.localRotation = Quaternion.identity;
			o.transform.Rotate(0, -120, 180);
			o.transform.localPosition = new Vector3(0, 0, 0);
			string path = "Materials\\" + weaponData.HandleMaterialType.ToString();
			o.GetComponent<MeshRenderer>().material = UnityEngine.Resources.Load<Material>(path);
			var metal = o.transform.GetChild(0);
			path = "Materials\\" + weaponData.MaterialType.ToString();
			metal.GetComponent<MeshRenderer>().material = UnityEngine.Resources.Load<Material>(path);
			spot = EquipmentSpot.Weapon;
		}

		if (item.ItemType == ItemTypes.Armor)
		{
			var armorData = JsonConvert.DeserializeObject<ArmorData>(item.ItemData);
			var o = (GameObject)Instantiate(Resources.Load("Prefabs\\" + armorData.ModelName), this.transform.position, Quaternion.identity);

			o.transform.parent = weaponBone.transform;
			o.transform.localRotation = Quaternion.identity;
			o.transform.Rotate(0, -120, 180);
			o.transform.localPosition = new Vector3(0, 0, 0);
			o.GetComponent<MeshRenderer>().material.SetColor("_Color", armorData.OrnamentColor);
			var metal = o.transform.GetChild(0);
			metal.GetComponent<MeshRenderer>().material = UnityEngine.Resources.Load<Material>("Materials\\" + armorData.MaterialType.ToString());
			spot = armorData.EquipmentSpot;
		}
		gameManager.panelEquipment.SetEquippedItem(spot, item);
		equippedItems.SetItem(spot, item);
	}

	public void EquipItemServer(Item item, EquipmentSpot spot, Action done = null)
	{
		switch (spot)
		{
			case EquipmentSpot.Weapon:
				var existing = equippedItems.Weapon;
				if (item != null)
				{
					if (existing == null || playerData.Inventory.CanAdd(item))
					{
						StartCoroutine(PlayersApi.SetEquippedItem("mwnzoller", spot, item, (success, err) =>
						{
							if (existing != null)
							{
								playerData.Inventory.Store(existing, 1);
							}
							equippedItems.Weapon = item;
							playerData.Inventory.Remove(item);
							gameManager.ReloadInventory();

							done?.Invoke();
						}));
					}
				}
				else
				{
					StartCoroutine(PlayersApi.SetEquippedItem("mwnzoller", spot, null, (success, err) =>
					{
						equippedItems.Weapon = null;
						if (existing != null)
						{
							playerData.Inventory.Store(existing, 1);
						}
						gameManager.ReloadInventory();
						done?.Invoke();

					}));

				}
				break;
		}
	}

	public Item GetBestTool(out WeaponData weaponData, Predicate<WeaponData> pred)
	{
		var weapons = playerData.Inventory.InventoryItems.Where(x => x.Item.ItemType == ItemTypes.Weapon);
		weaponData = null;
		Item item = null;
		foreach (var weapon in weapons)
		{
			var wd = JsonConvert.DeserializeObject<WeaponData>(weapon.Item.ItemData);
			if (pred(wd))
			{
				if (item == null)
				{
					item = weapon.Item;
					weaponData = wd;
				}
				else if (wd.ProbabilityBuff > weaponData.ProbabilityBuff)
				{
					item = weapon.Item;
					weaponData = wd;
				}
			}
		}
		return item;
	}
}
