using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Supabase.Core;
using Supabase.Core.Extensions;
using Supabase.Postgrest.Exceptions;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Responses;
using UnityEngine;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("PostgrestTests")]

namespace Supabase.Postgrest
{

	internal static class Helpers
	{
		//private static readonly HttpClient Client = new HttpClient();

		private static readonly Guid AppSession = Guid.NewGuid();

		/// <summary>
		/// Helper to make a request using the defined parameters to an API Endpoint and coerce into a model. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOptions"></param>
		/// <param name="method"></param>
		/// <param name="url"></param>
		/// <param name="data"></param>
		/// <param name="headers"></param>
		/// <param name="serializerSettings"></param>
		/// <param name="getHeaders"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<ModeledResponse<T>> MakeRequest<T>(ClientOptions clientOptions, string method, string url, JsonSerializerSettings serializerSettings, object? data = null,
			Dictionary<string, string>? headers = null, Func<Dictionary<string, string>>? getHeaders = null, CancellationToken cancellationToken = default) where T : BaseModel, new()
		{
			var baseResponse = await MakeRequest(clientOptions, method, url, serializerSettings, data, headers, cancellationToken);
			return new ModeledResponse<T>(baseResponse, serializerSettings, getHeaders);
		}

		/// <summary>
		/// Helper to make a request using the defined parameters to an API Endpoint.
		/// </summary>
		/// <param name="clientOptions"></param>
		/// <param name="method"></param>
		/// <param name="url"></param>
		/// <param name="data"></param>
		/// <param name="headers"></param>
		/// <param name="serializerSettings"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<BaseResponse> MakeRequest(ClientOptions clientOptions, string method, string url, JsonSerializerSettings serializerSettings, object? data = null,
			Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
		{
			var builder = new UriBuilder(url);
			var query = HttpUtility.ParseQueryString(builder.Query);

			if (data != null && method == UnityWebRequest.kHttpVerbGET)
			{
				// Case if it's a Get request the data object is a dictionary<string,string>
				if (data is Dictionary<string, string> reqParams)
				{
					foreach (var param in reqParams)
						query[param.Key] = param.Value;
				}
			}

			builder.Query = query.ToString();

			using var requestMessage = new UnityWebRequest(builder.Uri, method);

			if (method != UnityWebRequest.kHttpVerbDELETE)
			{
				requestMessage.downloadHandler = new DownloadHandlerBuffer();
			}

			if (data != null && method != UnityWebRequest.kHttpVerbGET)
			{
				var stringContent = JsonConvert.SerializeObject(data, serializerSettings);

				if (!string.IsNullOrWhiteSpace(stringContent) && JToken.Parse(stringContent).HasValues)
				{
					requestMessage.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(stringContent));
					requestMessage.uploadHandler.contentType = "application/json";
				}
			}

			if (headers != null)
			{
				foreach (var kvp in headers)
				{
					requestMessage.SetRequestHeader(kvp.Key, kvp.Value);
				}
			}

			var response = requestMessage.SendWebRequest();
			while (!response.isDone)
			{
				await Task.Yield();
			}

			string content = "";
			if (method != UnityWebRequest.kHttpVerbDELETE)
			{
				content = response.webRequest.downloadHandler.text;
			}

            if (response.webRequest.result is UnityWebRequest.Result.Success)
				return new BaseResponse(clientOptions, response.webRequest.error, (int)response.webRequest.responseCode, content, response.webRequest.uri, response.webRequest.GetResponseHeaders());

			var exception = new PostgrestException(content)
			{
				Content = content,
				Response = response.webRequest.error,
				StatusCode = (int)response.webRequest.responseCode
			};
			exception.AddReason();
			throw exception;
		}

		/// <summary>
		/// Prepares the request with appropriate HTTP headers expected by Postgrest.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="headers"></param>
		/// <param name="options"></param>
		/// <param name="rangeFrom"></param>
		/// <param name="rangeTo"></param>
		/// <returns></returns>
		public static Dictionary<string, string> PrepareRequestHeaders(string method, Dictionary<string, string>? headers = null, ClientOptions? options = null, int rangeFrom = int.MinValue, int rangeTo = int.MinValue)
		{
			options ??= new ClientOptions();

			headers = headers == null ? new Dictionary<string, string>(options.Headers) : options.Headers.MergeLeft(headers);

			if (!string.IsNullOrEmpty(options.Schema))
			{
				headers.Add(method == UnityWebRequest.kHttpVerbGET ? "Accept-Profile" : "Content-Profile", options.Schema);
			}

			if (rangeFrom != int.MinValue)
			{
				var formatRangeTo = rangeTo != int.MinValue ? rangeTo.ToString() : null;

				headers.Add("Range-Unit", "items");
				headers.Add("Range", $"{rangeFrom}-{formatRangeTo}");
			}

			if (!headers.ContainsKey("X-Client-Info"))
			{
				try
				{
					// Default version to match other clients
					// https://github.com/search?q=org%3Asupabase-community+x-client-info&type=code
					headers.Add("X-Client-Info", $"postgrest-csharp/{Util.GetAssemblyVersion(typeof(Client))}");
				}
				catch (Exception)
				{
					// Fallback for when the version can't be found
					// e.g. running in the Unity Editor, ILL2CPP builds, etc.
					headers.Add("X-Client-Info", $"postgrest-csharp/session-{AppSession}");
				}
			}

			return headers;
		}
	}
}
