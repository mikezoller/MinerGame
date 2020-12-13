using Miner.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Miner.Helpers
{
	public class ResourceResult
	{
		public int ActionId { get; set; }
		public string Name { get; set; }
		public int RequiredLevel { get; set; }
		public int ItemId { get; set; }
		public int Quantity { get; set; }
		public SkillType Skill { get; set; }
		public int Experience { get; set; }
		public double Probability { get; set; }
		public double Interval { get; set; }
	}
	public static class ResourceActionDatabase
	{
		private static Dictionary<int, ResourceResult> dict = new Dictionary<int, ResourceResult>();
		public static bool initialized;
		private static bool initializing;
		public static ResourceResult GetResourceResult(int actionId)
		{
			if (!initialized && !initializing)
			{
				Initialize();
			}
			return dict[actionId];
		}

		public static IEnumerator Initialize(Action fail = null)
		{
			initializing = true;
			return Communication.DataApi.GetResourceActions((items, err) =>
			{
				if (err == null)
				{
					dict = items;
					initializing = false;
					initialized = true;
				}
				else
				{
					fail?.Invoke();
				}
			});
		}
	}
}