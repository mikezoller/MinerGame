using Miner.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Miner.GameObjects
{
	public class Inventory : MonoBehaviour
	{
		public ItemGrid itemGrid;
		Miner.Models.Inventory playerInventory;
		public GameManager gameManager;

		public bool Editing = false;

		private void Awake()
		{
			itemGrid.Reload();

			var gc = GameObject.FindGameObjectWithTag("GameController");
			gameManager = gc.GetComponent<GameManager>();
		}

		private void OnEnable()
		{
			itemGrid.Reload();
		}

		private ItemCell selectedCell;
		public InventoryItem GetSelectedItem()
		{
			InventoryItem item = null;
			if (selectedCell != null)
			{
				item = selectedCell.item;
			}
			return item;
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
						var item = GetSelectedItem();
						gameManager.character.TransferToBank(new InventoryItem() { Item = item.Item, Quantity = 1 }, () =>
						{
							itemGrid.Reload();
						});
						gameManager.HideItemOptionsPanel();
					})
				});
				buttons.Add(new ButtonItemDetails()
				{
					text = "Transfer All",
					OnClick = new UnityAction(() =>
					{
						var item = GetSelectedItem();
						gameManager.character.TransferToBank(new InventoryItem() { Item = item.Item, Quantity = playerInventory.GetCount(item.Item.Id) }, () =>
						{
							itemGrid.Reload();
						});
						itemGrid.Reload();
						gameManager.HideItemOptionsPanel();
					})
				});
			}
			else
			{
				buttons.Add(new ButtonItemDetails()
				{
					text = "Drop 1",
					OnClick = new UnityAction(() =>
					{
						var item = GetSelectedItem();
						gameManager.character.DropFromInventory(new InventoryItem() { Item = item.Item, Quantity = 1 });
						itemGrid.Reload();
						gameManager.HideItemOptionsPanel();
					})
				});
				buttons.Add(new ButtonItemDetails()
				{
					text = "Drop All",
					OnClick = new UnityAction(() =>
					{
						var item = GetSelectedItem();
						gameManager.character.DropFromInventory(new InventoryItem() { Item = item.Item, Quantity = playerInventory.GetCount(item.Item.Id) });
						itemGrid.Reload();
						gameManager.HideItemOptionsPanel();
					})
				});

				if (cell.item.Item.Id >= 200 && cell.item.Item.Id < 250)
				{
					buttons.Add(new ButtonItemDetails()
					{
						text = "Start Fire",
						OnClick = new UnityAction(() =>
						{
							var item = GetSelectedItem();
							gameManager.character.StartFire(cell.item);
							gameManager.HideItemOptionsPanel();
						})
					});
				}
			}
			gameManager.ShowItemOptionsPanel(buttons);
		}

		public void ItemClicked(ItemCell cell)
		{
			if (Editing)
			{
				if (cell.item.Quantity == 1)
				{
					playerInventory.InventoryItems.Remove(cell.item);
					Destroy(cell);
				}
				else
				{
					cell.item.Quantity--;
				}
				Reload();
			}
			else
			{
				if (selectedCell != null)
				{
					var img = selectedCell.GetComponent<Image>();
					var temp = img.color;
					temp.a = 0f;
					img.color = temp;
				}

				if (selectedCell != cell)
				{

					selectedCell = cell;
					var img2 = selectedCell.GetComponent<Image>();
					var temp2 = img2.color;
					temp2.a = 1f;
					img2.color = temp2;
				}
				else
				{
					selectedCell = null;
				}

			}
			SendMessageUpwards("InventoryItemClicked", cell.item);
		}

		private void SetImageColor(Image img, int r, int g, int b)
		{
			var temp2 = img.color;
			temp2.r = r;
			temp2.g = g;
			temp2.b = b;
			img.color = temp2;
		}
		public void SetPlayerInventory(Miner.Models.Inventory inv)
		{
			playerInventory = inv;
			itemGrid.items = inv.InventoryItems;
			itemGrid.Reload();
		}

		public void Reload()
		{
			itemGrid.Reload();
		}
	}
}