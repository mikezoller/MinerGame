using Miner.GameObjects;
using UnityEngine;

namespace Assets.Scripts.GameObjects.Resources
{
	public enum RockType
	{
		None = 0,
		Stone = 10,
		Tin = 20,
		Copper = 30,
		Iron = 40,
		Coal = 50,
		Gold = 60,
	}
	public class MiningResource : ResourceObject
	{
		public RockType type;
		private RockType _type;

		public RockType Type
		{
			get => _type; set
			{
				_type = value;
				SetType(value);
				SetMaterial();
			}
		}

		private void OnValidate()
		{
			this.Type = type;
		}

		public void SetType(RockType type)
		{
			switch (type)
			{
				case RockType.Stone:
					actionId = 100;
					resourceMaterialName = "Stone";
					break;
				case RockType.Tin:
					actionId = 101;
					resourceMaterialName = "Tin";
					break;
				case RockType.Copper:
					actionId = 102;
					resourceMaterialName = "Copper";
					break;
				case RockType.Iron:
					actionId = 103;
					resourceMaterialName = "Iron";
					break;
				case RockType.Coal:
					actionId = 104;
					resourceMaterialName = "Coal";
					break;
				case RockType.Gold:
					actionId = 105;
					resourceMaterialName = "Gold";
					break;
				case RockType.None:
				default:
					actionId = -1;
					resourceMaterialName = "None";
					break;
			}
		}

		public void Start()
		{
			SetType(Type);
			SetMaterial();
		}

		private void SetMaterial()
		{
			resourceMaterial = UnityEngine.Resources.Load<Material>("Materials\\" + resourceMaterialName);
			GameObject go = null;
			foreach (Transform child in transform)
			{
				if (child.tag == "MaterialTarget")
					go = child.gameObject;
			}
			if (go != null)
			{
				MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
				// Get the current material applied on the GameObject
				Material oldMaterial = meshRenderer.sharedMaterial;
				// Set the new material on the GameObject
				meshRenderer.sharedMaterial = resourceMaterial;
			}
		}

		public MiningResource() : this(RockType.None)
		{

		}
		public MiningResource(RockType type)
		{
			this.StartTrigger = "mining";
			this.StopTrigger = "Idle";
		}
	}
}
