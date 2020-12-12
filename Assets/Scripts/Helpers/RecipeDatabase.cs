using Miner.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
		private static Dictionary<int, Recipe> dict = new Dictionary<int, Recipe>();
		public static bool initialized;
		private static bool initializing;
		public static Recipe GetRecipe(int recipeId)
		{
			if (!initialized && !initializing)
			{
				return null;
			}
			return dict[recipeId];
		}

		public static IEnumerator Initialize()
		{
			initializing = true;
			return Communication.DataApi.GetRecipes((items, err) =>
			{
				dict = items;
				initializing = false;
				initialized = true;
			});
		}
	}
}