using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Quests
{
	public class QuestProgress
	{
		public int QuestId { get; set; }
		public bool Complete { get; set; }
		public List<CheckpointProgress> Checkpoints { get; set; }
		public DateTime CompletedDate { get; set; }
		public bool ItemsCollected { get; set; }
	}
}
