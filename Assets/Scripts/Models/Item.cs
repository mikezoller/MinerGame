using Assets.Scripts;
using Newtonsoft.Json;
using System;
using System.Drawing;
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
		public int Id;
		public string Title;
		public string Description;
		public bool Stackable = true;

		public ItemTypes ItemType { get; set; }
		public string ItemData { get; set; }
		public Item() { }
		public Item(int id, string title, string description, bool stackable)
		{
			this.Id = id;
			this.Title = title;
			this.Description = description;
			this.Stackable = stackable;
		}

		public Item(Item item)
		{
			this.Id = item.Id;
			this.Title = item.Title;
			this.Description = item.Description;
		}
	}

	[Serializable]
	public class InventoryItem
	{
		public Item Item { get; set; }
		public int Quantity { get; set; }

		public InventoryItem Copy(int qty)
		{
			return new InventoryItem()
			{
				Item = this.Item,
				Quantity = qty
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
		public MaterialType MaterialType { get; set; }
		public MaterialType HandleMaterialType { get; set; }
		public float BaseDamage { get; set; }
		public bool CanChop { get; set; }
		public bool CanMine { get; set; }
		public float ProbabilityBuff { get; set; }
		public float AttackBuff { get; set; }
		public float StrengthBuff { get; set; }
		public float AccuracyBuff { get; set; }
		public string ModelName { get; set; }
		public string OrnamentColor { get; set; }
	}
	public enum MaterialType
	{
		Wood = 0,
		Stone = 10,
		Leather = 20,
		Bronze = 30,
		Iron = 40,
		Steel = 50,
	}

	[Serializable]
	public class ArmorData
	{
		private ItemTypes _itemType;
		public ItemTypes ItemType { get => ItemTypes.Armor; set { _itemType = value; } }
		public BodyPart BodyPart { get; set; }
		public MaterialType MaterialType { get; set; }
		public float AttackBuff { get; set; }
		public float StrengthBuff { get; set; }
		public float AccuracyBuff { get; set; }
		public float DefenseBuff { get; set; }
		public string ModelName { get; set; }
		public Color OrnamentColor { get; set; }
		[JsonIgnore]
		public EquipmentSpot EquipmentSpot
		{
			get
			{
				EquipmentSpot spot;
				switch (BodyPart)
				{
					case BodyPart.Head:
						spot = EquipmentSpot.Head;
						break;
					case BodyPart.Chest:
						spot = EquipmentSpot.Chest;
						break;
					case BodyPart.Hands:
						spot = EquipmentSpot.Hands;
						break;
					case BodyPart.Legs:
						spot = EquipmentSpot.Legs;
						break;
					case BodyPart.Arms:
						spot = EquipmentSpot.Arms;
						break;
					case BodyPart.Feet:
					default:
						spot = EquipmentSpot.Feet;
						break;
				}
				return spot;
			}
		}
	}
}