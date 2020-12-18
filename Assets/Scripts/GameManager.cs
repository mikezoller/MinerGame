using Assets.Scripts.Helpers;
using Miner.GameObjects;
using Miner.Models;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.AI;
using static MessagePanel;

namespace Miner
{
	public class GameManager : MonoBehaviour
	{
		// UI 
		public StartMenu startMenu;
		public HUD hud;
		public SkillsPanel skillsPanel;
		public GameObjects.Inventory inventory;
		public MessagePanel panelMessage;
		public PanelCraft craftPanel;
		public GameObjects.Bank panelBank;
		public GameObjects.PanelRequirements panelRequirements;
		public GameObject panelLoading;
		public EquipmentPanel panelEquipment;

		public Character character;
		// Start is called before the first frame update
		public void Start()
		{
			NavMeshHit myNavHit;
			if (NavMesh.SamplePosition(character.initialPosition, out myNavHit, 100, -1))
			{
				character.initialPosition = myNavHit.position;
			}
			character.GetComponent<NavMeshAgent>().Warp(character.initialPosition);


			inventory.SetPlayerInventory(character.playerData.Inventory);
			panelBank.SetPlayerBank(character.playerData.Bank);
			skillsPanel.SetPlayerSkills(character.playerData.Progress.Skills);
			craftPanel.playerData = character.playerData;

			panelMessage.HideMessage();
			panelBank.gameObject.SetActive(false);
			panelRequirements.gameObject.SetActive(false);
			var cpgo = GameObject.FindWithTag("CraftPanel");
			cpgo.SetActive(false);
			craftPanel = cpgo.GetComponent<PanelCraft>();

			ShowHUD(true);

		}

		public void ShowMessage(string message, MessageType type, Action accept = null, Action cancel = null)
		{
			panelMessage.ShowMessage(message, type, accept, cancel);
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


		public void SaveGame()
		{
			// 2
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
			bf.Serialize(file, character.playerData);
			file.Close();

			Debug.Log("Game Saved");
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

			character.InventoryItemClicked(invItem);

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