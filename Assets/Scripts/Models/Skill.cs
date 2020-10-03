using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Miner.Models
{
	public enum SkillType
    {
        Mining = 0,
        Smithing = 1,
        Woodcutting = 2,
        Fishing = 3,
        Cooking = 4,
    }
	[Serializable]
	public class Skill
	{
		public static Dictionary<int, int> ExpLookup { get; private set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Experience { get; set; }
		public int Level
		{
			get
			{
				var nextLevel = ExpLookup.FirstOrDefault(x => x.Value > Experience);
				return nextLevel.Key - 1;
			}
		}
		public Sprite Image { get; set; }
		public SkillType SkillType { get; set; }

		public Skill()
		{
			if (ExpLookup == null)
			{
				ExpLookup = new Dictionary<int, int>();
				for (int i = 1; i < 200; i++)
				{
					var val = (((Math.Pow(i, (5.0 / 2.0)) + i) / 2.0 * 50.0) - (i * 50.0));
					var roundedTo25 = (int)Math.Ceiling(val / 25) * 25;
					ExpLookup.Add(i, roundedTo25);
				}
			}
		}

		public virtual void AddExperience(int experience)
		{
			this.Experience += experience;
		}
	}
}