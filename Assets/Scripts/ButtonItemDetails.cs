using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace Miner.GameObjects
{
	public class ButtonItemDetails
	{
		//Item Name
		public string text = null;
		public UnityAction OnClick = null;
	}
}