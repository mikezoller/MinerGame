// Example of usage Api.cs

using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Miner.Models;
using Assets.Scripts;
using Miner.Helpers;
using Assets.Scripts.Models;

namespace Miner.Communication
{

	public class DataApi : API
	{
		public static IEnumerator GetItems(
			Action<List<Item>, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_DATA_PATH + "/getitems"),
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<List<Item>>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(null, ex.Message);
			}

			return null;
		}
		public static IEnumerator GetEnemies(
			Action<Dictionary<int, EnemyData>, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_DATA_PATH + "/getEnemies"),
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<Dictionary<int, EnemyData>>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(null, ex.Message);
			}

			return null;
		}
		public static IEnumerator GetRecipes(
			Action<Dictionary<int, Recipe>, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_DATA_PATH + "/getRecipes"),
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<Dictionary<int, Recipe>>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(null, ex.Message);
			}

			return null;
		}
		public static IEnumerator GetResourceActions(
			Action<Dictionary<int, ResourceResult>, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_DATA_PATH + "/getResourceActions"),
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse< Dictionary < int, ResourceResult>>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(null, ex.Message);
			}

			return null;
		}
	}
}