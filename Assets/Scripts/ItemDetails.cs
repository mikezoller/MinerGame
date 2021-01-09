using UnityEngine;
using UnityEngine.UI;

namespace Miner.GameObjects
{
	public class ItemDetails : MonoBehaviour
	{
		//Item Name
		public Text text = null;
	}

	public class ListItemInfo
	{
		public string Text { get; set; }
		public Color FontColor { get; set; }
	}
}