using Miner.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	public enum EquipmentSpot
	{
		Head,
		Arms,
		Hands,
		Chest,
		Legs,
		Feet,
		Weapon,
		Shield
	}
	[Serializable]
	public class EquippedItems
	{
		public Item Head { get; set; }
		public Item Arms { get; set; }
		public Item Hands { get; set; }
		public Item Chest { get; set; }
		public Item Legs { get; set; }
		public Item Feet { get; set; }
		public Item Weapon { get; set; }
		public Item Shield { get; set; }

		public int GetTotalDefense()
		{
			int defense = 0;
			foreach (Item item in new[] { Head, Arms, Hands, Chest, Legs, Feet })
			{
				if (item != null)
				{
					var armorData = JsonConvert.DeserializeObject<ArmorData>(item.ItemData);
					if (armorData != null)
					{
						defense += armorData.GetDefense();
					}
				}
			}
			return defense;
		}
	}
}
