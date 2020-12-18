using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Miner.Helpers
{
	public static class SpriteManager
	{
		private static Dictionary<int, Sprite> _itemSprites;
		private static Dictionary<int, Sprite> ItemSprites
		{
			get
			{
				if (_itemSprites == null)
				{
					_itemSprites = new Dictionary<int, Sprite>();

					for (int i = 0; i < 61; i++)
					{
						var x = Resources.LoadAll<Sprite>("spritesheets/items" + i);
						int count = 0;
						foreach (var y in x)
						{
							int column = (int)y.rect.x / 64;
							int row = 9 - ((int)y.rect.y / 64);
							ItemSprites.Add(i * 100 + (row * 10 + column ), y);
							count++;
						}
					}

				}
				return _itemSprites;
			}
		}

		public static Sprite GetItemSprite(int id)
		{
			Sprite sprite = null;
			if (ItemSprites.ContainsKey(id))
			{
				sprite = ItemSprites[id];
			}

			return sprite;
		}
	}
}