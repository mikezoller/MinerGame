using Miner.GameObjects;

namespace Assets.Scripts.GameObjects.Resources
{
	public enum TreeType
	{
		None = 0,
		Ash = 10,
	}
	public class WoodcuttingResource : ResourceObject
	{
		public TreeType type;
		private TreeType _type;

		public TreeType Type
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

		public void Start()
		{
			SetType(Type);
		}

		public WoodcuttingResource() : this(TreeType.None)
		{

		}
		public WoodcuttingResource(TreeType type)
		{
			this.StartTrigger = "chopping";
			this.StopTrigger = "Idle";
		}

		public void SetType(TreeType type)
		{
			switch (type)
			{
				case TreeType.Ash:
					actionId = 200;
					break; ;
				case TreeType.None:
				default:
					actionId = -1;
					resourceMaterialName = "None";
					break;
			}
		}
	}
}
