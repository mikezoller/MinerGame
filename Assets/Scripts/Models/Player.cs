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
			if (reqItem.Stackable || allowStackingAll)
			{
				var invItem = InventoryItems.FirstOrDefault(x => x.Item.Id == itemId);
				if (invItem == null || invItem.Quantity < quantity)
				{
					hasAtLeast = false;
				}
			}
			else
			{
				if (InventoryItems.Count(x => x.Item.Id == itemId) < quantity)
				{
					hasAtLeast = false;
				}
			}

			return hasAtLeast;
		}

		public virtual bool CanAdd(Item item, int quantity = 1)
		{
			return CanAdd(new InventoryItem()
			{
				Item = item,
				Quantity = quantity
			});
		}
		public virtual bool CanAdd(InventoryItem invItem)
		{
			bool canAdd = true;
			if (invItem.Item.Stackable || allowStackingAll)
			{
				if (InventoryItems.FirstOrDefault(x => x.Item.Id == invItem.Item.Id) == null && InventoryItems.Count + 1 > Size)
				{
					canAdd = false;
				}
			}
			else
			{
				if (InventoryItems.Count + invItem.Quantity > Size)
				{
					canAdd = false;
				}
			}

			return canAdd;
		}

		public virtual void Remove(Item item, int quantity = 1)
		{
			Remove(new InventoryItem()
			{
				Item = item,
				Quantity = quantity
			});
		}
		public virtual void Remove(InventoryItem invItem)
		{
			if (allowStackingAll || invItem.Item.Stackable)
			{
				var item = this.InventoryItems.FirstOrDefault(i => i.Item.Id == invItem.Item.Id);
				if (item != null)
				{
					item.Quantity -= invItem.Quantity;
				}
				if (item.Quantity <= 0)
				{
					this.InventoryItems.Remove(item);
				}
			}
			else
			{
				for (int i = 0; i < invItem.Quantity; i++)
				{
					var e = this.InventoryItems.FirstOrDefault(x => x.Item.Id == invItem.Item.Id);
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

		public virtual bool Store(Item item, int quantity)
		{
			return Store(new InventoryItem()
			{
				Item = item,
				Quantity = quantity
			});
		}
		public virtual bool Store(InventoryItem invItem, int index = -1)
		{
			bool success = false;

			if ((allowStackingAll || invItem.Item.Stackable) && InventoryItems.Count(c => c.Item.Id == invItem.Item.Id) > 0)
			{
				for (int i = 0; i < Size; i++)
				{
					if (InventoryItems[i].Item.Id == invItem.Item.Id)
					{
						InventoryItems[i].Quantity += invItem.Quantity;
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
						if (invItem.Item.Stackable || allowStackingAll)
						{
							InventoryItems.Insert(index, invItem);
						}
						else
						{
							for (var i = 0; i < invItem.Quantity; i++)
							{
								InventoryItems.Insert(index, new InventoryItem()
								{
									Item = invItem.Item,
									Quantity = 1
								});
							}
						}
					}
					else
					{
						if (invItem.Item.Stackable || allowStackingAll)
						{
							InventoryItems.Add(invItem);
						}
						else
						{
							for (var i = 0; i < invItem.Quantity; i++)
							{
								InventoryItems.Add(new InventoryItem()
								{
									Item = invItem.Item,
									Quantity = 1
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