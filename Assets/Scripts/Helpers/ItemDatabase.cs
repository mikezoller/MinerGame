using Miner.Models;
using Newtonsoft.Json;
using System;
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
				if (!initialized)
				{
					Initialize();
				}
                return items;
            }
        }
        private static List<Item> items = new List<Item>();
        public static bool initialized;

        public static Item GetItem(int id)
        {
			if (!initialized)
			{
				Initialize();
			}
			return items.Find(item => item.Id == id);
        }

		public static string GetModelPath(int id)
		{
			return $"items/" + id;
		}

		public static IEnumerator Initialize(Action fail = null)
		{
			return Communication.DataApi.GetItems((items, err) =>
			{
				if (err == null)
				{
					ItemDatabase.items = items;
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