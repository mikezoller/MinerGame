using Assets.Scripts.Helpers;
using Miner.Communication;
using Miner.GameObjects;
using Miner.Helpers;
using Miner.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Miner
{
	public class GameManager : MonoBehaviour
	{
		// UI 
		public StartMenu startMenu;
		public SkillsPanel skillsPanel;
		public GameObjects.Inventory inventory;
		public Map map;
		public HUD hud;
		public MessagePanel messagePanel;
		public GameObjects.Bank panelBank;
		public GameObjects.PanelRequirements panelRequirements;
		public Character character;

		// Services
		private InventoryService inventoryService;

		// DATA
		public Player playerData;



		// Start is called before the first frame update
		public void Start()
		{

			LoadGame();
			messagePanel.HideMessage();
			panelBank.gameObject.SetActive(false);
			panelRequirements.gameObject.SetActive(false);


		}

		public void PauseCamera(bool pause)
		{
			//var d = Camera.main.GetComponent<DragCamera2D>();
			//d.enabled = !pause;
		}
		public void TogglePauseCamera()
		{
			//var d = Camera.main.GetComponent<DragCamera2D>();
			//d.enabled = !d.enabled;
		}
		private void OnApplicationQuit()
		{
			SaveGame();
		}
		public void SetAnimBool(string trigger)
		{
			character.SetAnimBool(trigger);
		}
		public void ClearAnimBool(string trigger)
		{
			character.ClearAnimBool(trigger);
		}
		public void StartCharacterAnim(string trigger)
		{
			character.StartAnimation(trigger);
		}
		private void ShowOptions(bool show)
		{

			startMenu.gameObject.SetActive(show);
		}

		private void ShowHUD(bool show)
		{

			hud.gameObject.SetActive(show);
		}

		private void ShowInventory(bool show)
		{
			inventory.gameObject.SetActive(show);
			//PauseCamera(show);
		}
		// Update is called once per frame
		private void Update()
		{
			//if (Input.GetButton("Horizontal"))
			//{
			//	var dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
			//	transform.RotateAround(character.transform.position, Vector3.up, dir * Time.deltaTime * 100);
			//}
			if (Input.GetMouseButtonDown(0))
			{

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;

				if (!hud.DidHit(ray))
				{
					// hit something not in the player layer

					if (Physics.Raycast(ray, out hitInfo) && GUIUtility.hotControl == 0)
					{
						if (character.currentAction != null)
						{
							StopCoroutine(character.currentAction);
							character.currentAction = null;
							if (!string.IsNullOrEmpty(character.activeAnimation))
							{
								character.ClearAnimBool(character.activeAnimation);
							}
							character.StartAnimation("Idle");
						}
						hud.HideExtra();
						character.GetComponent<NavMeshAgent>().ResetPath();
						character.GetComponent<NavMeshAgent>().SetDestination(hitInfo.point);
					}
				}
			}
		}


		public void ExperienceEarned(SkillType skill)
		{
				skillsPanel.Reload();
		}

		private void LoadMap()
		{
			//Instantiate(map, new Vector3(0, 0, 0), Quaternion.identity);

		}

		public void SaveGame()
		{
			// 2
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
			bf.Serialize(file, playerData);
			file.Close();

			Debug.Log("Game Saved");
		}

		public void LoadGame()
		{
			if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
			{
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
				//playerData = (PlayerData)bf.Deserialize(file);
				file.Close();

				StartCoroutine(PlayersApi.Get("mwnzoller", (user, err) =>
				{
					if (err != null)
					{
						Debug.LogError(err);
					}
					else
					{
						playerData.Inventory = user.Inventory;
						playerData.Bank = user.Bank;
						playerData.Progress = user.Progress;

						var playerPos = new Vector3((float)user.LastLocation.X, (float)user.LastLocation.Y, (float)user.LastLocation.Z);

						NavMeshHit myNavHit;
						if (NavMesh.SamplePosition(playerPos, out myNavHit, 100, -1))
						{
							playerPos = myNavHit.position;
						}
							character.GetComponent<NavMeshAgent>().Warp(playerPos);

						inventoryService = new InventoryService(playerData.Bank, playerData.Inventory, panelBank, inventory);

						inventory.SetPlayerInventory(playerData.Inventory);
						panelBank.SetPlayerBank(playerData.Bank);
						skillsPanel.SetPlayerSkills(playerData.Progress.Skills);
					}
					LoadMap();
					ShowHUD(true);
				}));

			}
			else
			{
				Debug.Log("No game saved!");
			}
		}

		public void ReloadInventory()
		{
			this.inventory.Reload();
		}
		public void InventoryItemClicked(InventoryItem invItem)
		{

			// BANK
			if (panelBank.gameObject.activeSelf)
			{
				var i = invItem.Copy(1);
				if (playerData.Bank.CanAdd(i))
				{
					StartCoroutine(PlayersApi.TransferToBank("mwnzoller", i.item.id, i.quantity, (user, err) =>
					{
						if (err != null)
						{
							Debug.LogError(err);
						}
						else
						{
							playerData.Inventory.Remove(i);
							playerData.Bank.Store(i);

							panelBank.Reload();
							inventory.Reload();
						}
					}));
				}
			}
			// Requirements
			if (panelRequirements.gameObject.activeSelf)
			{
				var i = invItem.Copy(1);
				if (playerData.Bank.CanAdd(i))
				{
					//StartCoroutine(PlayersApi.TransferToBank("mwnzoller", i.item.id, i.quantity, (user, err) =>
					//{
					//	if (err != null)
					//	{
					//		Debug.LogError(err);
					//	}
					//	else
					//	{
					//		playerData.inventory.Remove(i);
					//		playerData.bank.Store(i);

					//		panelBank.Reload();
					//		inventory.Reload();
					//	}
					//}));
					var returnItem = panelRequirements.AddItem(i);

					if (returnItem == null)
					{
						playerData.Inventory.Remove(i);
						inventory.Reload();
					}

					panelRequirements.Reload();
				}
			}
		}

		public void TransferAllToBank()
		{
			if (panelBank.gameObject.activeSelf)
			{
				StartCoroutine(PlayersApi.TransferAllToBank("mwnzoller", (user, err) =>
				{
					if (err != null)
					{
						Debug.LogError(err);
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

						panelBank.Reload();
						inventory.Reload();
					}
				}));
			}
			}

		public void BankItemClicked(InventoryItem invItem)
		{
			if (panelBank.gameObject.activeSelf)
			{
				var i = invItem.Copy(1);
				if (playerData.Inventory.CanAdd(i))
				{
					StartCoroutine(PlayersApi.TransferToInventory("mwnzoller", i.item.id, i.quantity, (user, err) =>
					{
						if (err != null)
						{
							Debug.LogError(err);
						}
						else
						{

							playerData.Inventory.Store(i);
							playerData.Bank.Remove(i);

							panelBank.Reload();
							inventory.Reload();
						}
					}));
				}
			}
		}
	}
}