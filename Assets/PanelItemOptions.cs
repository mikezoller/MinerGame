using Assets.Scripts.GameObjects;
using Miner.GameObjects;
using Miner.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelItemOptions : ClosablePanel
{
	public ButtonListCreator listCreator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void SetButtons(List<ButtonItemDetails> options)
	{
		listCreator.SetItems(options);
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
