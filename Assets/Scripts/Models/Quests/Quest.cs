using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Models.Quests
{

	public class Quest
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<QuestItem> Rewards { get; set; } = new List<QuestItem>();
		public List<SkillExperience> ExpRewards { get; set; } = new List<SkillExperience>();
		public List<QuestCheckpoint> Checkpoints { get; set; } = new List<QuestCheckpoint>();
	}
}
