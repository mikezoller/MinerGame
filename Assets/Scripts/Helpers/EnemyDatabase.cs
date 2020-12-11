using Assets.Scripts.Models;
using Miner.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Miner.Helpers
{
	public static class EnemyDatabase
	{
		private static Dictionary<int, EnemyData> dict;
		private static bool initialized;
		public static EnemyData GetEnemy(int enemyId)
		{
			if (!initialized)
			{
				Initialize();
			}
			var data = dict[enemyId];
			data.CurrentHP = data.HP;
			return data;
		}
		public static string GetModelPath(int enemyId)
		{
			return $"~/Assets/Models/Enemies/{enemyId}.fbx";
		}

		private static void Initialize()
		{
			using (StreamReader r = new StreamReader(@"D:\Projects\Miner\enemies.json"))
			{
				string json = r.ReadToEnd();
				dict = new Dictionary<int, EnemyData>();
				var l = JsonConvert.DeserializeObject<List<EnemyData>>(json);
				foreach (var f in l)
				{
					dict.Add(f.Id, f);
				}
				initialized = true;
			}
			initialized = true;
		}
	}
}