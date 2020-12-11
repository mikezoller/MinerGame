using Assets.Scripts;
using Assets.Scripts.GameObjects;
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
		public PanelCraft craftPanel;
		public GameObjects.Bank panelBank;
		public GameObjects.PanelRequirements panelRequirements;
		public Character character;

		// Services
		private InventoryService inventoryService;


		// Start is called before the first frame update
		public void Start()
		{

			LoadGame();
			messagePanel.HideMessage();
			panelBank.gameObject.SetActive(false);
			panelRequirements.gameObject.SetActive(false);
			var cpgo = GameObject.FindWithTag("CraftPanel");
			cpgo.SetActive(false);
			craftPanel = cpgo.GetComponent<PanelCraft>();


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
		private void FixedUpdate()
		{
			if (Input.GetMouseButton(0))
			{

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;

				if (!hud.DidHit(ray))
				{
					// hit something not in the player layer

					if (Physics.Raycast(ray, out hitInfo) && GUIUtility.hotControl == 0)
					{
						hud.HideExtra();
						NavMeshHit myNavHit;
						if (NavMesh.SamplePosition(hitInfo.point, out myNavHit, 1, NavMesh.AllAreas))
						{
							character.WalkTo(myNavHit.position);
						}
					}
				}

				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(ray.origin, ray.direction, out hit))
				{
					this.character.HandleRayCastHit(hit, () =>
					{
						inventory.Reload();
					});
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
			bf.Serialize(file, character.playerData);
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
						character.playerData.Inventory = user.Inventory;
						character.playerData.Bank = user.Bank;
						character.playerData.Progress = user.Progress;

						var playerPos = new Vector3((float)user.LastLocation.X, (float)user.LastLocation.Y, (float)user.LastLocation.Z);

						NavMeshHit myNavHit;
						if (NavMesh.SamplePosition(playerPos, out myNavHit, 100, -1))
						{
							playerPos = myNavHit.position;
						}
						character.GetComponent<NavMeshAgent>().Warp(playerPos);

						inventoryService = new InventoryService(character.playerData.Bank, character.playerData.Inventory, panelBank, inventory);

						inventory.SetPlayerInventory(character.playerData.Inventory);
						panelBank.SetPlayerBank(character.playerData.Bank);
						skillsPanel.SetPlayerSkills(character.playerData.Progress.Skills);
						craftPanel.playerData = character.playerData;
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
				character.TransferToBank(i, () =>
				{
					panelBank.Reload();
					inventory.Reload();
				});
			}

			// Requirements
			if (panelRequirements.gameObject.activeSelf)
			{
				var i = invItem.Copy(1);
				if (character.playerData.Bank.CanAdd(i))
				{
					var returnItem = panelRequirements.AddItem(i);

					if (returnItem == null)
					{
						character.playerData.Inventory.Remove(i);
						inventory.Reload();
					}

					panelRequirements.Reload();
				}
			}

			player.InventoryItemClicked(invItem);
			
		}

		public void TransferAllToBank()
		{
			if (panelBank.gameObject.activeSelf)
			{
				character.TransferAllToBank(() =>
				{
					panelBank.Reload();
					inventory.Reload();
				});
			}
		}

		public void BankItemClicked(InventoryItem invItem)
		{
			if (panelBank.gameObject.activeSelf)
			{
				var i = invItem.Copy(1);
				character.TransferToInventory(i, () =>
				{
					panelBank.Reload();
					inventory.Reload();
				});
			}
		}
	}
}