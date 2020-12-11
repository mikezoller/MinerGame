using Miner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GameObjects
{
	public class Droppable : MonoBehaviour, IPointerClickHandler
	{
		public InventoryItem Item { get; set; }
		public int Probability { get; set; }
		//Detect if a click occurs
		public void OnPointerClick(PointerEventData pointerEventData)
		{
			//Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
			Debug.Log(name + " Game Object Clicked!");
		}
	}
}
