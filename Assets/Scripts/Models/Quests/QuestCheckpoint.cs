using System.Collections.Generic;

namespace Assets.Scripts.Models.Quests
{

	public enum CheckpointType
	{
		Basic,
		FilledItems,
		HasItem,
	}

	public class QuestCheckpoint
	{
		public int Number { get; set; }
		public string Description { get; set; }
		public CheckpointType CheckpointType { get; set; }
		public List<QuestItem> Items { get; set; }

		public override string ToString()
		{
			return $"({Number}) - {Description}";
		}
	}
}
