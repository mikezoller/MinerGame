using Miner.Models;
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
			GameObject valMining = GameObject.Find("valMining");
			var text = valMining.GetComponent<Text>();
			var mining = Skills.FirstOrDefault(x => x.SkillType == SkillType.Mining);

			if (mining != null)
			{
				text.text = mining.Experience.ToString() + " : " + mining.Level.ToString();
			}

			var wc = Skills.FirstOrDefault(x => x.SkillType == SkillType.Woodcutting);
			GameObject valWoodcutting = GameObject.Find("valWoodcutting");
			text = valWoodcutting.GetComponent<Text>();
			if (wc != null)
			{
				text.text = wc.Experience.ToString() + " : " + wc.Level.ToString();
			}
		}
    }
}