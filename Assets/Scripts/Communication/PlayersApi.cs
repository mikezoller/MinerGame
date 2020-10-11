// Example of usage Api.cs

using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Miner.Models;

namespace Miner.Communication
{

	public class PlayersApi : API
	{
		public static IEnumerator Get(
			string userName,
			Action<Player, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);

			try
			{
				return Get(path(GET_PLAYER_PATH + "/byName" + "?name=" + userName),
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, requestError(request));
						else
							done(requestResponse<Player>(request), null);
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
		public static IEnumerator UpdateLocation(
			string playerName, Vector3 location,
			Action<bool, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);
			try
			{
				return Post(path(GET_PLAYER_PATH + "/UpdatePlayerLocation"), new
				{ playerName, X = location.x, Y = location.y, Z =location.z },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, requestError(request));
						else
							done(requestResponse<bool>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, ex.Message);
			}
			return null;
		}
		public static IEnumerator DoResourceAction(
			string playerName, int actionId,
			Action<bool, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);
			try
			{
				return Post(path(GET_PLAYER_PATH + "/DoResourceAction"), new
				{ playerName, actionId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, requestError(request));
						else
							done(requestResponse<bool>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, ex.Message);
			}
			return null;
		}
		public static IEnumerator DoRecipe(
			string playerName, int recipeId,
			Action<bool, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);
			try
			{
				return Post(path(GET_PLAYER_PATH + "/DoRecipe"), new
				{ playerName, recipeId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, requestError(request));
						else
							done(requestResponse<bool>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, ex.Message);
			}
			return null;
		}
		public static IEnumerator RemoveFromInventory(
			string playerName, int itemId, int quantity,
			Action<bool, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);

			try
			{
				//return Post(path(GET_PLAYER_PATH + "/addToInventory" + "?playerId=" + userId + "&itemId=" + itemId +"&quantity=" +quantity), null,
				return Post(path(GET_PLAYER_PATH + "/RemoveFromInventory"), new
				{ playerName, itemId, quantity },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, requestError(request));
						else
							done(requestResponse<bool>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, ex.Message);
			}
			return null;
		}

		public static IEnumerator TransferToBank(
			string playerName, int itemId, int quantity,
			Action<bool, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);

			try
			{

				//return Post(path(GET_PLAYER_PATH + "/addToInventory" + "?playerId=" + userId + "&itemId=" + itemId +"&quantity=" +quantity), null,
				return Post(path(GET_PLAYER_PATH + "/MoveToBank"), new
				{ playerName, itemId, quantity },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, requestError(request));
						else
							done(requestResponse<bool>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, ex.Message);
			}
			return null;
		}

		public static IEnumerator TransferAllToBank(
			string playerName,
			Action<bool, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);

			try
			{

				return Post(path(GET_PLAYER_PATH + "/MoveAllToBank"), new
				{ playerName },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, requestError(request));
						else
							done(requestResponse<bool>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, ex.Message);
			}
			return null;
		}

		public static IEnumerator TransferToInventory(
			string playerName, int itemId, int quantity,
			Action<bool, string> doneCallback = null)
		{
			var done = wrapCallback(doneCallback);

			try
			{
				return Post(path(GET_PLAYER_PATH + "/MoveToInventory"), new
				{ playerName, itemId, quantity },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, requestError(request));
						else
							done(requestResponse<bool>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, ex.Message);
			}
			return null;
		}
	}
}