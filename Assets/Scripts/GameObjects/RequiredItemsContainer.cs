using Miner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameObjects
{
	[Serializable]
	public class RequiredItemsContainer : ItemContainer
	{
		public bool RequirementsFullfilled { get { return CheckRequirements(); } }
		public List<RequiredItem> Requirements;
		public override int Size => Requirements == null ? 0 : Requirements.Count;

		public RequiredItemsContainer()
		{
			allowStackingAll = true;
		}
		public override bool Store(InventoryItem invItem, int index = -1)
		{
			bool success = base.Store(invItem, index);

			return success;
		}

		public bool CheckRequirements()
		{
			bool hasAll = true;

			foreach (var item in Requirements)
			{
				var invItem = InventoryItems.FirstOrDefault(i => i.Item.Id == item.ItemId);
				if (invItem != null)
				{
					if (invItem.Quantity < item.Quantity)
					{
						hasAll = false;
						break;
					}
				}
				else
				{
					hasAll = false;
					break;
				}
			}

			return hasAll;
		}
	}
}
