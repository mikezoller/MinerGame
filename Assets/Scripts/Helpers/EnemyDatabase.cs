using Assets.Scripts.Models;
using Miner.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Miner.Helpers
{
	public static class EnemyDatabase
	{
		private static Dictionary<int, EnemyData> dict = new Dictionary<int, EnemyData>();
		public static bool initialized;
		private static bool initializing;
		public static EnemyData GetEnemy(int enemyId)
		{
			if (initialized)
			{
				var data = dict[enemyId];
				data.CurrentHP = data.HP;
				return data;
			}
			return null;
		}
		public static string GetModelPath(int enemyId)
		{
			return $"~/Assets/Models/Enemies/{enemyId}.fbx";
		}

		public static IEnumerator Initialize()
		{
			initializing = true;
			return Communication.DataApi.GetEnemies((items, err) =>
			{
				dict = items;
				initializing = false;
				initialized = true;
			});
		}
	}
}