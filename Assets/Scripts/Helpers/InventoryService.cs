using Miner.Communication;
using Miner.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
	public class InventoryService
	{
		private Miner.GameObjects.Bank goBank;
		private Miner.GameObjects.Inventory goInventory;

		private Bank playerBank;
		private Inventory playerInventory;

		public InventoryService(Bank playerBank, Inventory playerInventory, Miner.GameObjects.Bank goBank, Miner.GameObjects.Inventory goInventory)
		{
			this.playerBank = playerBank;
			this.playerInventory = playerInventory;

			this.goBank = goBank;
			this.goInventory = goInventory;
		}
		public void TransferFromInventoryToBank(Item invItem)
		{
			//StartCoroutine(PlayersApi.TransferToBank("mwnzoller", invItem.item.id, invItem.quantity, (user, err) =>
			//{
			//	if (err != null)
			//	{
			//		Debug.LogError(err);
			//	}
			//	else
			//	{
			//		playerInventory.Remove(invItem);
			//		playerBank.Store(invItem);

			//		goBank.Reload();
			//		goInventory.Reload();
			//	}
			//}));
		}

		public IEnumerator TransferFromBankToInventory(InventoryItem invItem)
		{
			PlayersApi.TransferToInventory("mwnzoller", invItem.item.id, invItem.quantity, (user, err) =>
			{
				if (err != null)
				{
					Debug.LogError(err);
				}
				else
				{
					playerInventory.Store(invItem);
					playerBank.Remove(invItem);

					goBank.Reload();
					goInventory.Reload();
				}
			});
			yield return null;
		}
	}
}
