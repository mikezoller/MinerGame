using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Miner.GameObjects
{
    public class Inventory : MonoBehaviour
    {
        public ItemGrid itemGrid;
        Miner.Models.Inventory playerInventory;

        public Button editButton;

        public bool Editing = false;

        private void Awake()
        {
            itemGrid.Reload();
        }

        private void OnEnable()
        {
            itemGrid.Reload();
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
            if (Editing)
            {
                if (cell.item.quantity == 1)
                {
                    playerInventory.InventoryItems.Remove(cell.item);
                    Destroy(cell);
                }
                else
                {
                    cell.item.quantity--;
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