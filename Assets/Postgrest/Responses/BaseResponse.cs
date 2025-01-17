using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
#pragma warning disable CS1591
namespace Supabase.Postgrest.Responses
{

	/// <summary>
	/// A wrapper class from which all Responses derive.
	/// </summary>
	public class BaseResponse
	{
		[JsonIgnore]
		public string? ResponseMessage { get; set; }

		[JsonIgnore]
		public int ResponseStatusCode { get; set; }

		[JsonIgnore]
		public string? Content { get; set; }

		[JsonIgnore]
		public Uri RequestUri { get; set; }

		[JsonIgnore]
		public Dictionary<string, string> Headers { get; set; }

		[JsonIgnore]
		public ClientOptions ClientOptions { get; set; }

        [JsonIgnore]
        public bool IsSuccessStatusCode
        {
            get
            {
                return ResponseStatusCode >= 200 && ResponseStatusCode < 300;
            }
        }

        public BaseResponse(ClientOptions clientOptions, string responseMessage, int responseStatusCode, string? content, Uri requestUri, Dictionary<string, string> headers)
		{
			ClientOptions = clientOptions;
			ResponseMessage = responseMessage;
			ResponseStatusCode = responseStatusCode;
			Content = content;
			RequestUri = requestUri;
			Headers = headers;
		}

        public void EnsureSuccessStatusCode()
        {
            if (!IsSuccessStatusCode)
            {
                throw new Exception(ResponseMessage);
            }
        }
    }
}
