using Miner.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace Miner.GameObjects
{
	public class SkillsPanel : MonoBehaviour
	{
		List<Skill> Skills;

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		public void SetPlayerSkills(List<Skill> skills)
		{
			Skills = skills;
			Reload();
		}

		public void Reload()
		{
			var allSkills = Enum.GetValues(typeof(SkillType));
			foreach(SkillType skill in allSkills)
			{
				ReloadSkill(skill);
			}
		}

		public void ReloadSkill(SkillType skillType)
		{
			var skill = Skills.FirstOrDefault(x => x.SkillType == skillType);
			if (skill == null)
			{
				skill = new Skill()
				{
					Name = skillType.ToString(),
					Description = skillType.ToString(),
					SkillType = skillType,
					Experience = 0
				};
				Skills.Add(skill);
			}
			GameObject vallSkill = GameObject.Find("val" + skillType.ToString());
			if (vallSkill != null)
			{
				Text text = vallSkill.GetComponent<Text>();
				text.text = skill.Level.ToString();
			}
		}
	}
}