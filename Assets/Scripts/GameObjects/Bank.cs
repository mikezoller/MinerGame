using Assets.Scripts;
using Miner.Communication;
using Miner.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Miner.GameObjects
{

	public class Bank : MonoBehaviour, IClickable
	{
		public ItemGrid itemGrid;

		public Button editButton;

		public bool Editing = false;
		public GameObject ui => this.gameObject;

		public GameManager gameManager;
		
		private void Awake()
		{
			var gc = GameObject.FindGameObjectWithTag("GameController");
			gameManager = gc.GetComponent<GameManager>();

		}

		private void OnEnable()
		{
		}

		public void ToggleEdit()
		{
			Editing = !Editing;
			var img = editButton.GetComponent<Image>();
			if (Editing)
			{
				SetImageColor(img, 255, 0, 0);
			}
			else
			{
				SetImageColor(img, 255, 255, 255);
			}
		}

		private ItemCell selectedCell;


		public void ItemClicked(ItemCell cell)
		{
			SendMessageUpwards("BankItemClicked", cell.item);
		}

		public void TransferAllClicked()
		{
			SendMessageUpwards("TransferAllToBank");
		}
		public void ItemLongClicked(ItemCell cell)
		{
			List<ButtonItemDetails> buttons = new List<ButtonItemDetails>();
			selectedCell = cell;
			if (gameManager.panelBank.isActiveAndEnabled)
			{
				buttons.Add(new ButtonItemDetails()
				{
					text = "Transfer 1",
					OnClick = new UnityAction(() =>
					{
						var item = cell.item;
						gameManager.character.TransferToInventory(new InventoryItem() { Item = item.Item, Quantity = 1 }, () =>
						{
							itemGrid.Reload();
						});
						itemGrid.Reload();
						gameManager.HideItemOptionsPanel();
					})
				});
				buttons.Add(new ButtonItemDetails()
				{
					text = "Transfer 5",
					OnClick = new UnityAction(() =>
					{
						var item = cell.item;
						gameManager.character.TransferToInventory(new InventoryItem() { Item = item.Item, Quantity = 5 }, () =>
						{
							itemGrid.Reload();
						});
						itemGrid.Reload();
						gameManager.HideItemOptionsPanel();
					})
				});
				buttons.Add(new ButtonItemDetails()
				{
					text = "Transfer All",
					OnClick = new UnityAction(() =>
					{
						var item = cell.item;
						gameManager.character.TransferToInventory(new InventoryItem() { Item = item.Item, Quantity = gameManager.character.playerData.Bank.GetCount(item.Item.Id) }, () =>
						{
							itemGrid.Reload();
						});
						itemGrid.Reload();
						gameManager.HideItemOptionsPanel();
					})
				});
			}
			gameManager.ShowItemOptionsPanel(buttons);
		}
		private void SetImageColor(Image img, int r, int g, int b)
		{
			var temp2 = img.color;
			temp2.r = r;
			temp2.g = g;
			temp2.b = b;
			img.color = temp2;
		}
		public void SetPlayerBank(Miner.Models.Bank bank)
		{
			itemGrid.items = bank.InventoryItems;
			itemGrid.Reload();
		}

		public void Reload()
		{
			itemGrid.Reload();
		}

		public void Clicked()
		{
			// Do nothing
		}
		
	}
}