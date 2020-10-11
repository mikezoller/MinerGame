using Miner.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Miner.Helpers
{
	public class Recipe
	{
		public int Id { get; set; }
		public List<BasicInventoryItem> Input { get; set; }
		public List<BasicInventoryItem> Output { get; set; }
		public SkillType Skill { get; set; }
		public int Experience { get; set; }
		public int RequiredLevel { get; set; }
	}
	public class BasicInventoryItem
	{
		public int ItemId { get; set; }
		public int Quantitiy { get; set; }
	}
	public static class RecipeDatabase
	{
		private static Dictionary<int, Recipe> dict;
		private static bool initialized;
		public static Recipe GetRecipe(int recipeId)
		{
			if (!initialized)
			{
				Initialize();
			}
			return dict[recipeId];
		}

		private static void Initialize()
		{
			using (StreamReader r = new StreamReader(@"D:\Projects\Miner\recipes.json"))
			{
				string json = r.ReadToEnd();
				dict = new Dictionary<int, Recipe>();
				var l = JsonConvert.DeserializeObject<List<Recipe>>(json);
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