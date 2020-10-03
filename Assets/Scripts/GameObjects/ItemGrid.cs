using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Miner.Models;

namespace Miner.GameObjects
{
	public class ItemGrid : MonoBehaviour
	{

		public int rows = 4;
		public int columns = 4;
		public ItemCell itemCellPrefab;
		public List<InventoryItem> items = new List<InventoryItem>();
		private List<ItemGridItem> cells = new List<ItemGridItem>();
		// Start is called before the first frame update
		void Start()
		{
			var rect = GetComponent<RectTransform>().rect;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					var pos = new Vector3(20 + j * 32 - rect.width / 2, -(i * 32) - 20 + rect.height / 2, 0);
					var cell = Instantiate(itemCellPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
					cell.transform.localPosition = pos;
					cell.transform.localRotation = Quaternion.identity;
					cells.Add(new ItemGridItem(cell));
				}
			}
			Reload();
		}

		public void Reload()
		{
			for (int i = 0; i < cells.Count; i++)
			{
				if (i < items.Count)
				{
					var item = items[i];
					cells[i].cell.SetItem(item);
				}
				else
				{
					cells[i].cell.SetItem(null);
				}
			}
		}

		private class ItemGridItem
		{
			public ItemCell cell;
			private InventoryItem _item;
			public InventoryItem item { get { return _item; } set { _item = value; cell.SetItem(value); } }

			public ItemGridItem(ItemCell cell)
			{
				this.cell = cell;
			}
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
