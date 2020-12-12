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
	public class DroppableComponent : MonoBehaviour, IPointerClickHandler
	{
		public InventoryItem Item { get; set; }
		public bool Claimed { get; set; }

		public void OnPointerClick(PointerEventData eventData)
		{
			
		}
	}
}
