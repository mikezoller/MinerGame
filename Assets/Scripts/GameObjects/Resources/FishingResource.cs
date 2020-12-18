using Miner.GameObjects;
using UnityEngine;

namespace Assets.Scripts.GameObjects.Resources
{
	public enum FishType
	{
		None = 0,
		Bluegill = 1,
	}
	public class FishingResource : ResourceObject
	{
		public FishType type;
		private FishType _type;

		public FishType Type
		{
			get => _type; set
			{
				_type = value;
				SetType(value);
			}
		}

		private void OnValidate()
		{
			this.Type = type;
		}

		public void SetType(FishType type)
		{
			switch (type)
			{
				case FishType.Bluegill:
					actionId = 300;
					break;
				case FishType.None:
				default:
					actionId = 300;
					resourceMaterialName = "None";
					break;
			}

		}

		public override void OnDepleted()
		{
			base.OnDepleted();
			this.GetComponent<MeshRenderer>().enabled = false;
		}

		public override void OnRefilled()
		{
			base.OnRefilled();
			this.GetComponent<MeshRenderer>().enabled = true;
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

		public FishingResource() : this(FishType.None)
		{

		}
		public FishingResource(FishType type)
		{
			resourceType = ResourceType.Fishing;
			SetType(type);
			this.StartTrigger = "fishing";
			this.StopTrigger = "Idle";
		}
	}
}
