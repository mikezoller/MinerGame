using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Miner.Models;
using Miner.Helpers;

namespace Miner.GameObjects
{
    public class ItemCell : MonoBehaviour
    {
        public InventoryItem item;
        public Text text;
        public Button button;
		public int RequiredAmount;
        public bool Selected;
        // Start is called before the first frame update
        void Start()
        {
			if (item != null && item.item != null)
			{
				var s = button.GetComponent<Image>();
				s.sprite = SpriteManager.GetItemSprite(item.item.id);
			}
        }

        public void FireItemClicked()
        {
            SendMessageUpwards("ItemClicked", this);
        }
		// Update is called once per frame
		void FixedUpdate()
		{
			Refresh();
		}

		public void Refresh()
		{
			if (item != null && text != null)
			{
				text.text = item.quantity > 1 ? item.quantity.ToString() : "";
			}
		}

		public void SetItem(InventoryItem item){
				var s = button.GetComponent<Image>();
			if (item != null)
			{
				s.sprite = SpriteManager.GetItemSprite(item.item.id);
				s.color = Color.white;
				text.text = item.quantity > 1 ? item.quantity.ToString() : "";
			}
			else
			{
				s.sprite = null;
				s.color = Color.clear;
				text.text = "";
			}
			this.item = item;
		}
    }
}