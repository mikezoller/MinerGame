using Miner.Communication;
using Miner.Helpers;
using Miner.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Miner.GameObjects
{

	public class PanelCraft : MonoBehaviour
	{
		public ItemGrid itemGrid;
		public Player playerData;
		private List<Recipe> recipes;
		public GameManager gameManager;

		public void Activate(List<Recipe> recipes)
		{
			this.recipes = recipes;
			List<InventoryItem> invItems = new List<InventoryItem>();
			foreach(var recipe in recipes)
			{
				var recipeOutput = recipe.Output;
				var outItem = recipe.Output.First();
				invItems.Add(new InventoryItem()
				{
					item = ItemDatabase.GetItem(outItem.ItemId),
					quantity = outItem.Quantitiy,
				});
			}
			itemGrid.items = invItems;

			this.gameObject.SetActive(true);
			itemGrid.Reload();
		}

		public void Deactivate()
		{
			this.gameObject.SetActive(false);
			this.recipes.Clear();
			this.itemGrid.items.Clear();
		}

		private void Awake()
		{
		}

		private void OnEnable()
		{
		}


		public void Reload()
		{
			itemGrid.Reload();
		}


		public void ItemClicked(ItemCell cell)
		{
			var item = cell.item;
			var recipe = recipes.FirstOrDefault(x => x.Output[0].ItemId == item.item.id);

			if (recipe != null)
			{
				if (PlayerCanMake(recipe))
				{
					StartCoroutine(PlayersApi.DoRecipe("mwnzoller", recipe.Id, (user, err) =>
					{
						if (err != null)
						{
							Debug.LogError("error syncing with server!");
						}
						else
						{

							// Do animation
							// After timeout, destroy input items and give output items
							foreach (var req in recipe.Input)
							{
								playerData.Inventory.Remove(new InventoryItem()
								{
									item = ItemDatabase.GetItem(req.ItemId),
									quantity = req.Quantitiy,
								});
							}
							foreach (var res in recipe.Output)
							{
								playerData.Inventory.Store(new InventoryItem()
								{
									item = ItemDatabase.GetItem(res.ItemId),
									quantity = res.Quantitiy,
								});
							}
							gameManager.ReloadInventory();
						}
					}));
				}
			}
		}

		private bool PlayerCanMake(Recipe recipe)
		{
			bool canMake = true;
			foreach (var req in recipe.Input)
			{
				if (!playerData.Inventory.HasAtLeast(req.ItemId, req.Quantitiy))
				{
					canMake = false;
					break;
				}
			}
			return canMake;
		}
	}
}