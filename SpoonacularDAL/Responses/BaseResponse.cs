using System;
using System.Net;
using Newtonsoft.Json;
using HomeUtilities.Common.Interfaces;

namespace SpoonacularDAL.Responses
{
    public class BaseResponse : IBaseResponse
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public bool Success
        {
            get { return (string.IsNullOrWhiteSpace(ErrorCode)); }
            set { }
        }

        public Exception DALException { get; set; }

        public CookieContainer Cookies { get; set; }

        public CookieContainer DALCookies { get; set; }
    }
}
