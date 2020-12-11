using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Character;

namespace Miner.Models
{
	public enum ItemTypes
	{
		None = 0,
		Food = 1,
		Weapon = 2,
		Armor = 3
	}

	[Serializable]
    public class Item
    {
        public int id;
        public string title;
        public string description;
        public bool stackable = true;

		public ItemTypes ItemType { get; set; }
		public string ItemData { get; set; }
		public Item() { }
        public Item(int id, string title, string description, bool stackable)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.stackable = stackable;
        }

        public Item(Item item)
        {
            this.id = item.id;
            this.title = item.title;
            this.description = item.description;
        }
    }

	[Serializable]
	public class InventoryItem
	{
		public Item item { get; set; }
		public int quantity { get; set; }

		public InventoryItem Copy(int qty)
		{
			return new InventoryItem()
			{
				item = this.item,
				quantity = qty
			};
		}
	}

	[Serializable]
	public class FoodData
	{
		private ItemTypes _itemType;
		public ItemTypes ItemType { get => ItemTypes.Food; set { _itemType = value; } }
		public int HealAmount { get; set; }
	}

	[Serializable]
	public class WeaponData
	{
		private ItemTypes _itemType;
		public ItemTypes ItemType { get => ItemTypes.Weapon; set { _itemType = value; } }
		public int AttackBoost { get; set; }
		public int AccuracyBoost { get; set; }
		public int StrengthBoost { get; set; }
		public int Speed { get; set; }
	}
	

	[Serializable]
	public class ArmorData
	{
		public BodyPart BodyPart { get; set; }
		public ArmorType ArmorType { get; set; }
		private static Dictionary<BodyPart, int> DefenseLookup = new Dictionary<BodyPart, int>()
		{
			{ BodyPart.Arms, 5 },
			{ BodyPart.Chest, 10 },
			{ BodyPart.Feet, 2 },
			{ BodyPart.Hands, 2 },
			{ BodyPart.Head, 5 },
			{ BodyPart.Legs, 5 },
		};
		public int GetDefense()
		{
			int defense = DefenseLookup[BodyPart];
			switch (ArmorType)
			{
				case ArmorType.Leather:
					defense += 1;
					break;
				case ArmorType.Bronze:
					defense += 2;
					break;
				case ArmorType.Iron:
					defense += 4;
					break;
				case ArmorType.Steel:
					defense += 6;
					break;
			}
			return defense;
		}
	}

	public enum ArmorType
	{
		None,
		Leather = 10,
		Bronze = 20,
		Iron = 30,
		Steel = 40,
	}

}