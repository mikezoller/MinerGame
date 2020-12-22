﻿using Miner.Models;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Trade
{
	public class SellRequest
	{
		public string Id { get; set; }
		public bool Complete { get; set; } // Marked complete after all items sold or bought
		public bool Done { get; set; } // Marked done when complete or cancelled and all results collected
		public bool Canceled { get; set; }
		public Guid SellerId { get; set; }
		public long ItemId { get { return InventoryItem == null || InventoryItem.Item == null ? -1 : InventoryItem.Item.Id; } set { } }
		public InventoryItem InventoryItem { get; set; }
		public int Price { get; set; }
		public List<BuyHistory> Buyers { get; set; }
		public int QuantityFilled { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public TradeResult TradeResult { get; set; }
	}

	public class BuyHistory
	{
		public Guid PlayerId { get; set; }
		public string SellRequestId { get; set; }
		public int Quantity { get; set; }
		public int Cost { get; set; }
	}
}
