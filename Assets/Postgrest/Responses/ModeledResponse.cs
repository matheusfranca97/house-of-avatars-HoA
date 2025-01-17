﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Supabase.Postgrest.Extensions;
using Supabase.Postgrest.Models;

namespace Supabase.Postgrest.Responses
{

	/// <summary>
	/// A representation of a successful Postgrest response that transforms the string response into a C# Modelled response.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ModeledResponse<T> : BaseResponse where T : BaseModel, new()
	{
		/// <summary>
		/// The first model in the response.
		/// </summary>
		public T? Model => Models.FirstOrDefault();

		/// <summary>
		/// A list of models in the response.
		/// </summary>
		public List<T> Models { get; } = new();
		
		/// <inheritdoc />
		public ModeledResponse(BaseResponse baseResponse, JsonSerializerSettings serializerSettings, Func<Dictionary<string, string>>? getHeaders = null, bool shouldParse = true) 
			: base(baseResponse.ClientOptions, baseResponse.ResponseMessage, baseResponse.ResponseStatusCode, baseResponse.Content, baseResponse.RequestUri, baseResponse.Headers)
		{
			Content = baseResponse.Content;
			ResponseMessage = baseResponse.ResponseMessage;

			if (!shouldParse || string.IsNullOrEmpty(Content)) return;

			var token = JToken.Parse(Content!);

			switch (token)
			{
				// A List of models has been returned
				case JArray: {
					var deserialized = JsonConvert.DeserializeObject<List<T>>(Content!, serializerSettings);

					if (deserialized != null)
						Models = deserialized;

					foreach (var model in Models)
					{
						model.BaseUrl = baseResponse.RequestUri.GetInstanceUrl().Replace(model.TableName, "").TrimEnd('/');
						model.RequestClientOptions = ClientOptions;
						model.GetHeaders = getHeaders;
					}

					break;
				}
				// A single model has been returned
				case JObject: {
					Models.Clear();

					var obj = JsonConvert.DeserializeObject<T>(Content!, serializerSettings);

					if (obj != null)
					{
						obj.BaseUrl = baseResponse.RequestUri.GetInstanceUrl().Replace(obj.TableName, "").TrimEnd('/');
						obj.RequestClientOptions = ClientOptions;
						obj.GetHeaders = getHeaders;

						Models.Add(obj);
					}

					break;
				}
			}

			Debugger.Instance.Log(this, $"Response: [{baseResponse.ResponseStatusCode}]\n" + $"Parsed Models <{typeof(T).Name}>:\n\t{JsonConvert.SerializeObject(Models)}\n");
		}
	}
}
