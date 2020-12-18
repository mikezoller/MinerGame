using Assets.Scripts;
using Miner;
using Miner.GameObjects;
using Miner.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{

	public EquippedItems equippedItems;
	public Character character;
	public GameManager gameManager;
	// Start is called before the first frame update
	void Start()
    {
        
    }

	public void SetEquippedItem(EquipmentSpot spot, Item item)
	{
		Transform t = this.gameObject.transform.Find(spot.ToString());
		ItemCell cell = t.gameObject.GetComponent<ItemCell>();

		if (item == null)
		{
			cell.SetItem(null);
		}
		else
		{
			cell.SetItem(new InventoryItem()
			{
				item = item,
				quantity = 1,
			});
		}
	}
	public void ItemClicked(ItemCell cell)
	{
		string name = cell.gameObject.name;
		EquipmentSpot spot = (EquipmentSpot)Enum.Parse(typeof(EquipmentSpot), name);
		character.EquipItemServer(null, spot, () =>
		{
			SetEquippedItem(spot, null);
			character.RemoveEquipmentGameObject(spot);
			gameManager.ReloadInventory();
		});
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
