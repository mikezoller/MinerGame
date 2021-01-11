using Assets.Scripts.GameObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Miner.GameObjects
{
	public class HUD : MonoBehaviour
    {
        public Inventory inventory;
		public GameObject bankPanel;
		public GameObject panelRequirements;
		public GameObject panelItemOptions;
		private GraphicRaycaster m_Raycaster;
		private PointerEventData m_PointerEventData;
		private EventSystem m_EventSystem;

		private void Start()
		{
			//Fetch the Raycaster from the GameObject (the Canvas)
			m_Raycaster = GetComponent<GraphicRaycaster>();
			//Fetch the Event System from the Scene
			m_EventSystem = GetComponent<EventSystem>();
		}


		public void HideExtra()
		{
			if (bankPanel.activeInHierarchy)
			{
				bankPanel.SetActive(false);
			}
			if (panelRequirements.activeInHierarchy)
			{
				panelRequirements.GetComponent<ClosablePanel>().Close();
			}
			if (panelItemOptions.activeInHierarchy)
			{
				panelItemOptions.GetComponent<ClosablePanel>().Close();
			}
		}
		public bool DidHit(Ray ray)
		{
			bool didHit = false;
			//Set up the new Pointer Event
			m_PointerEventData = new PointerEventData(m_EventSystem);
			//Set the Pointer Event Position to that of the mouse position
			m_PointerEventData.position = Input.mousePosition;

			//Create a list of Raycast Results
			List<RaycastResult> results = new List<RaycastResult>();

			//Raycast using the Graphics Raycaster and mouse click position
			m_Raycaster.Raycast(m_PointerEventData, results);

			//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
			foreach (RaycastResult result in results)
			{
				didHit = true;
			}
			return didHit;
		}
        public void ToggleInventory()
        {
            inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
        }
    }
}