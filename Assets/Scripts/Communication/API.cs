using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.Text;

namespace Miner.Communication
{
	public abstract class API
	{
		// TODO: add https certificate verification
#if UNITY_WEBGL
        // WebGL version is loaded by browser under https protocol.
        public const string HOST = "https://localhost:44332/"";
#else
		public const string HOST = "https://localhost:44332/";
#endif

		public static readonly string TOKEN_QUERY_PARAM = "token=";
		public static readonly string GET_PLAYER_PATH = "api/players";
		public static readonly string GET_TRADE_PATH = "api/trade";
		public static readonly string GET_DATA_PATH = "api/data";

		#region internal methods for http client
		internal static string Path(string path)
		{
			return HOST + path;
		}

		internal static string RequestError(UnityWebRequest request)
		{
			string responseBody = string.Empty;
			if (request.downloadHandler != null)
			{
				responseBody = request.downloadHandler.text;
			}

			return string.Format(
				"[api#error] request status code: {0}, data: ======= response: {1}, error: {2} =======",
				request.responseCode, responseBody, request.error);
		}

		internal static T RequestResponse<T>(UnityWebRequest request)
		{
			try
			{
				var responseData = request.downloadHandler.text;
				var obj = JsonConvert.DeserializeObject<T>(responseData, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.All });
				return obj;
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return default;
			}
		}

		public const string CONTENT_TYPE_JSON = "application/json";

		/// <summary>
		/// Create the instance of authenticated http client.
		/// </summary>
		/// <returns>The client.</returns>
		internal static IEnumerator Request(string url, string method, string data = null, Action<UnityWebRequest> done = null)
		{
			UnityWebRequest request;
			Debug.Log(url);
			switch (method)
			{
				case UnityWebRequest.kHttpVerbGET:
					request = UnityWebRequest.Get(url);
					yield return request.SendWebRequest();
					done?.Invoke(request);
					break;
				case UnityWebRequest.kHttpVerbPOST:
					request = UnityWebRequest.Post(url, data);
					request.method = UnityWebRequest.kHttpVerbPOST;
					request.downloadHandler = new DownloadHandlerBuffer();
					request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
					request.SetRequestHeader("Content-Type", CONTENT_TYPE_JSON);
					request.SetRequestHeader("Accept", CONTENT_TYPE_JSON);
					yield return request.SendWebRequest();
					done?.Invoke(request);
					break;
			}
		}

		internal static IEnumerator Post(string url, object o, Action<UnityWebRequest> done = null) =>
			Request(url, UnityWebRequest.kHttpVerbPOST, JsonConvert.SerializeObject(o), done);

		internal static IEnumerator Get(string url, Action<UnityWebRequest> done = null) =>
			Request(url, UnityWebRequest.kHttpVerbGET, null, done);

		internal static IEnumerator Get(string url, object o, Action<UnityWebRequest> done = null) =>
			Request(url, UnityWebRequest.kHttpVerbGET, JsonConvert.SerializeObject(o), done);
		internal static Action<T1, T2> WrapCallback<T1, T2>(Action<T1, T2> doneCallback)
		{
			// in case of having missing done callback use empty function to skip checks
			// on null or not callback instance.
			return doneCallback ?? ((_arg1, _arg2) => { });
		}
	}
	#endregion
}