using Assets.Scripts;
using Miner.Communication;
using Miner.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Miner.GameObjects
{

	public class Bank : MonoBehaviour, IClickable
	{
		public ItemGrid itemGrid;

		public Button editButton;

		public bool Editing = false;
		public GameObject ui => this.gameObject;

		private Player _playerData;
		public Player playerData
		{
			get
			{
				return _playerData;
			}
			set
			{
				_playerData = value;
				SetPlayerBank(_playerData.Bank);
			}
		}
		private void Awake()
		{
		}

		private void OnEnable()
		{
		}

		public void ToggleEdit()
		{
			Editing = !Editing;
			var img = editButton.GetComponent<Image>();
			if (Editing)
			{
				SetImageColor(img, 255, 0, 0);
			}
			else
			{
				SetImageColor(img, 255, 255, 255);
			}
		}

		private ItemCell selectedCell;


		public void ItemClicked(ItemCell cell)
		{
			SendMessageUpwards("BankItemClicked", cell.item);
		}

		public void TransferAllClicked()
		{
			SendMessageUpwards("TransferAllToBank");
		}

		private void SetImageColor(Image img, int r, int g, int b)
		{
			var temp2 = img.color;
			temp2.r = r;
			temp2.g = g;
			temp2.b = b;
			img.color = temp2;
		}
		public void SetPlayerBank(Miner.Models.Bank bank)
		{
			itemGrid.items = bank.InventoryItems;
			itemGrid.Reload();
		}

		public void Reload()
		{
			itemGrid.Reload();
		}

		public void Clicked()
		{
			// Do nothing
		}
		
	}
}