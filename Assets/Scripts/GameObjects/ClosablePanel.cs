using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameObjects
{
	public class ClosablePanel : MonoBehaviour
	{
		public delegate void OnClosed();
		public OnClosed OnClosedDelegate;


		public void Close()
		{
			gameObject.SetActive(false);
			if (OnClosedDelegate != null)
			{
				OnClosedDelegate();
			}
		}
	}
}
