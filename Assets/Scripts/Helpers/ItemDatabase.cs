using Miner.Models;
using Newtonsoft.Json;
using System.Collections;
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
				if (!initialized && !initializing)
				{
					BuildItemDatabase();
				}
                return items;
            }
        }
        private static List<Item> items = new List<Item>();
        public static bool initialized;
        private static bool initializing;

        public static Item GetItem(int id)
        {
			if (!initialized && !initializing)
			{
				BuildItemDatabase();
			}
			return items.Find(item => item.id == id);
        }

		public static string GetModelPath(int id)
		{
			return $"items/" + id;
		}

		public static IEnumerator BuildItemDatabase()
		{
			initializing = true;
			return Communication.DataApi.GetItems((items, err) =>
			{
				ItemDatabase.items = items;
				initializing = false;
				initialized = true;
			});
		}
	}
}