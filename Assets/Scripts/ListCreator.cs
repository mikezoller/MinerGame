using Miner.GameObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Miner.Scripts
{
	public partial class ListCreator : MonoBehaviour
	{

		[SerializeField]
		private Transform SpawnPoint = null;

		[SerializeField]
		private GameObject item = null;

		[SerializeField]
		private RectTransform content = null;

		public List<ListItemInfo> items = new List<ListItemInfo>();

		private List<ItemDetails> active = new List<ItemDetails>();
		// Use this for initialization
		void Start()
		{
			Refresh();			
		}

		public void SetItems(IEnumerable<ListItemInfo> newItems)
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
			content.sizeDelta = new Vector2(0, items.Count * itemHeight);

			for (int i = 0; i < items.Count; i++)
			{
				// 60 width of item
				float spawnY = (i + 1) * itemHeight;
				//newSpawn Position
				Vector3 pos = new Vector3(SpawnPoint.position.x, -spawnY, SpawnPoint.position.z);
				//instantiate item
				GameObject SpawnedItem = Instantiate(item, pos, SpawnPoint.rotation);
				//setParent
				SpawnedItem.transform.SetParent(SpawnPoint, false);
				//get ItemDetails Component
				ItemDetails itemDetails = SpawnedItem.GetComponent<ItemDetails>();
				//set name
				itemDetails.text.text = items[i].Text;
				itemDetails.text.color = items[i].FontColor;
				active.Add(itemDetails);
			}
		}
	}
}