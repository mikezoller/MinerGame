using Miner.Models;
using Newtonsoft.Json;
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
		private static Dictionary<int, ResourceResult> dict;
		private static bool initialized;
		public static ResourceResult GetResourceResult(int actionId)
		{
			if (!initialized)
			{
				Initialize();
			}
			return dict[actionId];
		}

		private static void Initialize()
		{
			using (StreamReader r = new StreamReader(@"D:\Projects\Miner\resourceActions.json"))
			{
				string json = r.ReadToEnd();
				dict = new Dictionary<int, ResourceResult>();
				var l = JsonConvert.DeserializeObject<List<ResourceResult>>(json);
				foreach (var f in l)
				{
					dict.Add(f.ActionId, f);
				}
				initialized = true;
			}
			initialized = true;
		}
	}
}