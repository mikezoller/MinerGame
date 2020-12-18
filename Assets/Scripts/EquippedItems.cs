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
		public Item this[EquipmentSpot spot]
		{
			get
			{
				Item item = null;
				switch (spot)
				{
					case EquipmentSpot.Arms:
						item = Arms;
						break;
					case EquipmentSpot.Chest:
						item = Chest;
						break;
					case EquipmentSpot.Feet:
						item = Feet;
						break;
					case EquipmentSpot.Hands:
						item = Hands;
						break;
					case EquipmentSpot.Legs:
						item = Legs;
						break;
					case EquipmentSpot.Shield:
						item = Shield;
						break;
					case EquipmentSpot.Weapon:
						item = Weapon;
						break;
				}
				return item;
			}
		}

		public Item Head { get; set; }
		public Item Arms { get; set; }
		public Item Hands { get; set; }
		public Item Chest { get; set; }
		public Item Legs { get; set; }
		public Item Feet { get; set; }
		public Item Weapon { get; set; }
		public Item Shield { get; set; }
		public Item ToolTempSwap { get; set; }

		public void SetItem(EquipmentSpot spot, Item item)
		{
			switch (spot)
			{
				case EquipmentSpot.Arms:
					Arms = item;
					break;
				case EquipmentSpot.Chest:
					Chest = item;
					break;
				case EquipmentSpot.Feet:
					Feet = item;
					break;
				case EquipmentSpot.Hands:
					Hands = item;
					break;
				case EquipmentSpot.Legs:
					Legs = item;
					break;
				case EquipmentSpot.Shield:
					Shield = item;
					break;
				case EquipmentSpot.Weapon:
					Weapon = item;
					break;
			}
		}

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
						defense += (int)armorData.DefenseBuff;
					}
				}
			}
			return defense;
		}
	}
}
