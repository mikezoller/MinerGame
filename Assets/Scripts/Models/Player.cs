using Assets.Scripts;
using Miner.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miner.Models
{
	[Serializable]
	public class PlayerLocation
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
		public DateTime TimeStampUTC { get; set; }
	}

	[Serializable]
	public abstract class ItemContainer
	{
		public abstract int Size { get; }
		public List<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
		protected bool allowStackingAll = false;

		public ItemContainer()
		{
		}

		public virtual bool HasAtLeast(int itemId, int quantity)
		{
			var reqItem = ItemDatabase.GetItem(itemId);
			bool hasAtLeast = true;
			if (reqItem.stackable || allowStackingAll)
			{
				var invItem = InventoryItems.FirstOrDefault(x => x.item.id == itemId);
				if (invItem == null || invItem.quantity < quantity)
				{
					hasAtLeast = false;
				}
			}
			else
			{
				if (InventoryItems.Count(x => x.item.id == itemId) < quantity)
				{
					hasAtLeast = false;
				}
			}

			return hasAtLeast;
		}

		public virtual bool CanAdd(InventoryItem invItem)
		{
			bool canAdd = true;
			if (invItem.item.stackable || allowStackingAll)
			{
				if (InventoryItems.FirstOrDefault(x => x.item.id == invItem.item.id) == null && InventoryItems.Count + 1 > Size)
				{
					canAdd = false;
				}
			}
			else
			{
				if (InventoryItems.Count + invItem.quantity > Size)
				{
					canAdd = false;
				}
			}

			return canAdd;
		}

		public virtual void Remove(InventoryItem invItem)
		{
			if (allowStackingAll || invItem.item.stackable)
			{
				var item = this.InventoryItems.FirstOrDefault(i => i.item.id == invItem.item.id);
				if (item != null)
				{
					item.quantity -= invItem.quantity;
				}
				if (item.quantity <= 0)
				{
					this.InventoryItems.Remove(item);
				}
			}
			else
			{
				for (int i = 0; i < invItem.quantity; i++)
				{
					var e = this.InventoryItems.FirstOrDefault(x => x.item.id == invItem.item.id);
					if (e != null)
					{
						this.InventoryItems.Remove(e);
					}
					else
					{
						break;
					}
				}
			}
		}

		public virtual bool Store(InventoryItem invItem, int index = -1)
		{
			bool success = false;

			if ((allowStackingAll || invItem.item.stackable) && InventoryItems.Count(c => c.item.id == invItem.item.id) > 0)
			{
				for (int i = 0; i < Size; i++)
				{
					if (InventoryItems[i].item.id == invItem.item.id)
					{
						InventoryItems[i].quantity += invItem.quantity;
						success = true;
						break;
					}
				}
			}
			else
			{
				if (InventoryItems.Count + 1 <= Size)
				{
					if (index >= 0 && index < Size)
					{
						if (invItem.item.stackable || allowStackingAll)
						{
							InventoryItems.Insert(index, invItem);
						}
						else
						{
							for (var i = 0; i < invItem.quantity; i++)
							{
								InventoryItems.Insert(index, new InventoryItem()
								{
									item = invItem.item,
									quantity = 1
								});
							}
						}
					}
					else
					{
						if (invItem.item.stackable || allowStackingAll)
						{
							InventoryItems.Add(invItem);
						}
						else
						{
							for (var i = 0; i < invItem.quantity; i++)
							{
								InventoryItems.Add(new InventoryItem()
								{
									item = invItem.item,
									quantity = 1
								});
							}
						}
					}
					success = true;
				}
			}

			return success;
		}
	}

	[Serializable]
	public class Inventory : ItemContainer
	{
		public override int Size => 24;

		public Inventory() : base()
		{

		}
	}
	[Serializable]
	public class Bank : ItemContainer
	{
		public override int Size => 100;

		public Bank() : base()
		{
			allowStackingAll = true;
		}
	}
	[Serializable]
	public class Progress
	{
		public List<Skill> Skills { get; set; } = new List<Skill>();

		public Progress()
		{
		}

		public int GetFullHeath()
		{
			var hp = GetSkill(SkillType.Hitpoints);
			return 10 + hp.Level * 2;
		}
		public int GetAttack()
		{
			var attack = GetSkill(SkillType.Attack);
			return attack.Level;
		}
		public int GetStrength()
		{
			var strength = GetSkill(SkillType.Strength);
			return strength.Level;
		}
		public int GetDefense()
		{
			var defense = GetSkill(SkillType.Defense);
			return defense.Level;
		}
		public int GetAccuracy()
		{
			var acc = GetSkill(SkillType.Accuracy);
			return 50 + acc.Level / 2;
		}
		public int GetSkillLevel(SkillType skillType)
		{
			var skill = GetSkill(skillType);
			return skill.Level;
		}
		public Skill GetSkill(SkillType skillType)
		{
			Skill skill = Skills.FirstOrDefault(x => x.SkillType == skillType);
			if (skill == null)
			{
				skill = new Skill()
				{
					SkillType = skillType,
					Name = skillType.ToString(),
					Experience = 0
				};
			}
			return skill;
		}
	}

	[Serializable]
	public class Player
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public Inventory Inventory { get; set; } = new Inventory();
		public Bank Bank { get; set; } = new Bank();
		public Progress Progress { get; set; } = new Progress();
		public PlayerLocation LastLocation { get; set; }
		public CurrentStats CurrentStats { get; set; }
		public EquippedItems EquippedItems { get; set; }
		public Player(string id, string name)
		{
			this.Id = id;
			this.Name = name;
		}
	}
}