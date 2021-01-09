using Assets.Scripts.Models.Quests;
using Miner.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Miner.Helpers
{
	public static class QuestDatabase
	{

		public static List<Quest> Quests
		{
			get
			{
				if (!initialized)
				{
					Initialize();
				}
				return quests;
			}
		}
		private static List<Quest> quests = new List<Quest>();
		public static bool initialized;

		public static Quest GetQuest(int id)
		{
			if (!initialized)
			{
				Initialize();
			}
			return quests.Find(item => item.Id == id);
		}

		public static IEnumerator Initialize(Action fail = null)
		{
			return Communication.DataApi.GetQuests((quests, err) =>
			{
				if (err == null)
				{
					QuestDatabase.quests = quests;
					initialized = true;
				}
				else
				{
					Debug.LogError(err);
					fail?.Invoke();
				}
			});
		}
	}
}