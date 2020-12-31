using Miner.GameObjects;
using Miner.Helpers;
using Miner.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GameObjects
{
	public class CraftingObject : InteractionObject
	{
		public List<int> RecipeIds;
		private List<Recipe> recipes;

		public CraftingObject()
		{
			
		}

		public void Start()
		{
			recipes = new List<Recipe>();
			foreach (int id in RecipeIds)
			{
				recipes.Add(RecipeDatabase.GetRecipe(id));
			}
		}

		public void Clicked()
		{
			var script = uiToShow.GetComponent<PanelCraft>();
			script.Activate(recipes);
		}

	}

	

}
