using Assets.Scripts.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
	public class EnemyData : Component
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public int CurrentHP { get; set; }
		public int BaseHP { get; set; }
		public int BaseStrength { get; set; }
		public int BaseAccuracy { get; set; }
		public int BaseDefense { get; set; }
		public float LevelHPScale { get; set; }
		public float LevelStrengthScale { get; set; }
		public float LevelAccuracyScale { get; set; }
		public float LevelDefenseScale { get; set; }
		public List<Droppable> Drops { get; set; }

		public int HP { get { return (int) (BaseHP * LevelHPScale); } }
		public int Strength { get { return (int)(BaseStrength * LevelStrengthScale); } }
		public int Accuracy { get { return (int)(BaseAccuracy * LevelAccuracyScale); } }
		public int Defense { get { return (int)(BaseDefense * LevelDefenseScale); } }

		public EnemyData()
		{
			CurrentHP = HP;
		}
	}
}
