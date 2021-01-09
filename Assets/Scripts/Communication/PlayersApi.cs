// Example of usage Api.cs

using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Miner.Models;
using static Miner.Communication.MultiActionRequest;
using Assets.Scripts;
using Assets.Scripts.Models.Quests;

namespace Miner.Communication
{

	public class PlayersApi : API
	{
		public static IEnumerator Get(
			string userName,
			Action<Player, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_PLAYER_PATH + "/byName" + "?name=" + userName),
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<Player>(request), null);
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
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/UpdatePlayerLocation"), new
				{ playerName, X = location.x, Y = location.y, Z = location.z },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/DoResourceAction"), new
				{ playerName, actionId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
		public static IEnumerator DoCombatAction(
			string playerName, int actionId, int amount,
			Action<bool, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/DoCombatAction"), new
				{ playerName, actionId, amount },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/DoRecipe"), new
				{ playerName, recipeId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
		public static IEnumerator SetEquippedItem(
			string playerName, EquipmentSpot equipmentSpot, Item item,
			Action<bool, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/SetEquippedItem"), new
				{ playerName, equipmentSpot, item },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
			var done = WrapCallback(doneCallback);

			try
			{
				//return Post(path(GET_PLAYER_PATH + "/addToInventory" + "?playerId=" + userId + "&itemId=" + itemId +"&quantity=" +quantity), null,
				return Post(Path(GET_PLAYER_PATH + "/RemoveFromInventory"), new
				{ playerName, itemId, quantity },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
			string playerName, Dictionary<int, int> items,
			Action<bool, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				//return Post(path(GET_PLAYER_PATH + "/addToInventory" + "?playerId=" + userId + "&itemId=" + itemId +"&quantity=" +quantity), null,
				return Post(Path(GET_PLAYER_PATH + "/RemoveMultipleFromInventory"), new
				{ playerName, items },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
			var done = WrapCallback(doneCallback);

			try
			{

				//return Post(path(GET_PLAYER_PATH + "/addToInventory" + "?playerId=" + userId + "&itemId=" + itemId +"&quantity=" +quantity), null,
				return Post(Path(GET_PLAYER_PATH + "/MoveToBank"), new
				{ playerName, itemId, quantity },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
			var done = WrapCallback(doneCallback);

			try
			{

				return Post(Path(GET_PLAYER_PATH + "/MoveAllToBank"), new
				{ playerName },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
			var done = WrapCallback(doneCallback);

			try
			{
				return Post(Path(GET_PLAYER_PATH + "/MoveToInventory"), new
				{ playerName, itemId, quantity },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
		public static IEnumerator AddToInventory(
			string playerName, int itemId, int quantity,
			Action<bool, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Post(Path(GET_PLAYER_PATH + "/AddToInventory"), new
				{ playerName, itemId, quantity },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
		public static IEnumerator SetHealth(
			string playerName, int amount,
			Action<bool, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/SetHealth"), new
				{ playerName, amount },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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

		public static IEnumerator DoMultipleActions(
			string playerName, List<SimpleActionRequest> requests,
			Action<bool, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Post(Path(GET_PLAYER_PATH + "/MultiAction"), new
				{ playerName, requests },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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
		public static IEnumerator UpdateQuestProgress(
				string playerName, QuestProgress questProgress,
				Action<bool, UpdateQuestResponse, string> doneCallback = null)
		{

			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/UpdateQuestProgress"), new
				{ playerName, questProgress },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, null, RequestError(request));
						else
							done(true, RequestResponse<UpdateQuestResponse>(request), null);
					});
			}
			catch (Exception ex)
			{
				// catch here all the exceptions ensure never die
				Debug.Log(ex.Message);
				done(false, null, ex.Message);
			}
			return null;
		}
		public class UpdateQuestResponse
		{
			public int QuestId { get; set; }
			public bool NowComplete { get; set; }
			public List<SkillExperience> ExpRewards { get; set; }
		}
		public static IEnumerator CollectQuestItems(
				string playerName, int questId,
				Action<bool, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_PLAYER_PATH + "/CollectQuestItems"), new
				{ playerName, questId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(false, RequestError(request));
						else
							done(true, null);
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

	[Serializable]
	public class MultiActionRequest
	{
		public enum ActionType
		{
			SetHealth,
			AddAttackExperience,
			AddDefenseExperience
		}

		public List<SimpleActionRequest> Requests { get; set; }
		public string PlayerName { get; set; }

		public static SimpleActionRequest GetSimpleRequest(ActionType type, int amount)
		{
			return new SimpleActionRequest() { ActionType = type, Amount = amount };
		}
	}

	[Serializable]
	public class SimpleActionRequest
	{
		public ActionType ActionType { get; set; }
		public int Amount { get; set; }
	}
}