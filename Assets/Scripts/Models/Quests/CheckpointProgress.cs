using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Quests
{
	public class CheckpointProgress
	{
		public int QuestId { get; set; }
		public int Number { get; set; }
		public bool Complete { get; set; }
		public List<QuestItem> Items { get; set; } = new List<QuestItem>();
		public DateTime CompletedDate { get; set; }
	}
}
