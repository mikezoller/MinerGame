using Miner.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Miner.Helpers
{
	public static class ItemDatabase
    {

        public static List<Item> Items
        {
            get
            {
                BuildItemDatabase();
                return items;
            }
        }
        private static List<Item> items = new List<Item>();
        private static bool initialized;

        public static Item GetItem(int id)
        {
            BuildItemDatabase();
            return items.Find(item => item.id == id);
        }

		public static string GetModelPath(int id)
		{
			return $"items/" + id;
		}

		private static void BuildItemDatabase()
		{
			if (!initialized)
			{
				using (StreamReader r = new StreamReader(@"D:\Projects\Miner\items.json"))
				{
					string json = r.ReadToEnd();
					items = new List<Item>();
					items.AddRange(JsonConvert.DeserializeObject<List<Item>>(json));
					initialized = true;
				}
			}

		}
	}
}