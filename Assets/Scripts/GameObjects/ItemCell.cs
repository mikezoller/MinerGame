using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Miner.Models;
using Miner.Helpers;
using UnityEngine.Events;

namespace Miner.GameObjects
{
    public class ItemCell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public InventoryItem item;
        public Text text;
        public Image image;
		public int RequiredAmount;
        public bool Selected;

		// Long Press
		[Tooltip("How long must pointer be down on this object to trigger a long press")]
		public float durationThreshold = 1.0f;

		public UnityEvent onLongPress = new UnityEvent();

		private bool isPointerDown = false;
		private bool longPressTriggered = false;
		private float timePressStarted;

		// Start is called before the first frame update
		void Start()
        {
			if (item != null && item.Item != null)
			{
				image.sprite = SpriteManager.GetItemSprite(item.Item.Id);
			}
        }

        public void FireItemClicked()
        {
            SendMessageUpwards("ItemClicked", this);
        }
		public void FireItemLongClicked()
		{
			SendMessageUpwards("ItemLongClicked", this);
		}
		// Update is called once per frame
		void FixedUpdate()
		{
			Refresh();
		}
		private void Update()
		{
			if (isPointerDown && !longPressTriggered)
			{
				if (Time.time - timePressStarted > durationThreshold)
				{
					longPressTriggered = true;
					onLongPress.Invoke();
				}
			}
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			timePressStarted = Time.time;
			isPointerDown = true;
			longPressTriggered = false;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			isPointerDown = false;
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			isPointerDown = false;
		}
		public void Refresh()
		{
			if (item != null && text != null)
			{
				text.text = item.Quantity > 1 ? item.Quantity.ToString() : "";
			}
		}

		public void SetItem(InventoryItem item){
			if (item != null)
			{
				image.sprite = SpriteManager.GetItemSprite(item.Item.Id);
				image.color = Color.white;
				text.text = item.Quantity > 1 ? item.Quantity.ToString() : "";
			}
			else
			{
				image.sprite = null;
				image.color = Color.clear;
				text.text = "";
			}
			this.item = item;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!longPressTriggered)
			{
				FireItemClicked();
			}
		}
	}
}