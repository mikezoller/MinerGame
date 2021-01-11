using Miner.GameObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Miner.Scripts
{
	public partial class ButtonListCreator : MonoBehaviour
	{

		[SerializeField]
		private Transform SpawnPoint = null;

		[SerializeField]
		private GameObject item = null;

		[SerializeField]
		private RectTransform content = null;

		public List<ButtonItemDetails> items = new List<ButtonItemDetails>();
		public List<Button> active= new List<Button>();

		// Use this for initialization
		void Start()
		{
		}

		public void SetItems(IEnumerable<ButtonItemDetails> newItems)
		{
			foreach(var i in active)
			{
				Destroy(i.gameObject);
			}
			active.Clear();
			items = newItems.ToList();
			Refresh();
		}

		public void Refresh()
		{
			var itemHeight = item.GetComponent<RectTransform>().rect.height;
			//setContent Holder Height;
			content.sizeDelta = new Vector2(0, items.Count * itemHeight + 15);

			for (int i = 0; i < items.Count; i++)
			{
				// 60 width of item
				float spawnY =15 + (-i * itemHeight);
				//newSpawn Position
				Vector3 pos = new Vector3(0, spawnY, SpawnPoint.position.z);
				//instantiate item
				GameObject SpawnedItem = Instantiate(item, pos, SpawnPoint.rotation);
				//setParent
				SpawnedItem.transform.SetParent(SpawnPoint, false);
				//get ItemDetails Component
				Button itemButton = SpawnedItem.GetComponent<Button>();
				itemButton.onClick.AddListener(items[i].OnClick);
				Text itemText = SpawnedItem.GetComponentInChildren<Text>(true);
				itemText.text = items[i].text;

				active.Add(itemButton);
			}
		}
	}
}