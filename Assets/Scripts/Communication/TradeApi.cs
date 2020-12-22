// Example of usage Api.cs

using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Miner.Models;
using static Miner.Communication.MultiActionRequest;
using Assets.Scripts;
using Assets.Scripts.Models.Trade;

namespace Miner.Communication
{

	public class TradeApi : API
	{
		public static IEnumerator GetSellRequest(
			string orderId,
			Action<SellRequest, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_TRADE_PATH + "/GetSellRequest"), new
				{ orderId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<SellRequest>(request), null);
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
		public static IEnumerator GetBuyRequest(
			string orderId,
			Action<BuyRequest, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_TRADE_PATH + "/GetBuyRequest"), new
				{ orderId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<BuyRequest>(request), null);
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
		public static IEnumerator GetUserSellRequests(
			string userId,
			Action<List<SellRequest>, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_TRADE_PATH + "/GetUserSellRequests"), new
				{ userId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<List<SellRequest>>(request), null);
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
		public static IEnumerator GetUserBuyRequests(
			string userId,
			Action<List<BuyRequest>, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);

			try
			{
				return Get(Path(GET_TRADE_PATH + "/GetUserBuyRequests"), new
				{ userId },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<List<BuyRequest>>(request), null);
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
		public static IEnumerator Buy(
			string userId, string itemId, int quantity, int price, Vector3 location,
			Action<string, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_TRADE_PATH + "/Buy"), new
				{ userId, itemId, quantity, price },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<string>(request), null);
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
		public static IEnumerator Sell(
			string userId, string itemId, int quantity, int price, Vector3 location,
			Action<string, string> doneCallback = null)
		{
			var done = WrapCallback(doneCallback);
			try
			{
				return Post(Path(GET_TRADE_PATH + "/Sell"), new
				{ userId, itemId, quantity, price },
					(request) =>
					{
						if (request.isNetworkError || request.responseCode != 200)
							done(null, RequestError(request));
						else
							done(RequestResponse<string>(request), null);
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