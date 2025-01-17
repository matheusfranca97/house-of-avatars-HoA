using Newtonsoft.Json;
using System;
namespace Supabase.Gotrue.Responses
{
    /// <summary>
    /// A wrapper class from which all Responses derive.
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// The HTTP response message.
        /// </summary>
        [JsonIgnore]
        public string ResponseMessage { get; set; }

        [JsonIgnore]
        public int ResponseStatusCode { get; set; }

        /// <summary>
        /// The HTTP response content as a string.
        /// </summary>
        [JsonIgnore]
        public string? Content { get; set; }

        [JsonIgnore]
        public bool IsSuccessStatusCode 
        { 
            get
            {
                return ResponseStatusCode >= 200 && ResponseStatusCode < 300;
            } 
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
