using Miner.Models;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Models.Trade
{
	public class TradeResult
	{
		private List<InventoryItem> RefundItems { get; set; } = new List<InventoryItem>();

		public List<InventoryItem> GetRefund()
		{
			return RefundItems;
		}
	}
}
