using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Serialization;

namespace LegalPark.Exception
{
    public class ResponseHandler
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("status")]
        public HttpStatusCode? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public object? Data { get; set; }

        [JsonPropertyName("error")]
        public object? Error { get; set; }

        
        public static IActionResult GenerateResponseSuccess(object responseObj)
        {
            var response = new ResponseHandler
            {
                Code = (int)HttpStatusCode.OK,
                Status = HttpStatusCode.OK,
                Message = "success",
                Data = responseObj,
                Error = null
            };

            return new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        
        public static IActionResult GenerateResponseSuccess(HttpStatusCode status, string message, object data)
        {
            var response = new ResponseHandler
            {
                Code = (int)status,
                Status = status,
                Message = message,
                Data = data,
                Error = null
            };

            return new ObjectResult(response)
            {
                StatusCode = (int)status
            };
        }

        
        public static IActionResult GenerateResponseError(HttpStatusCode status, object error, string message)
        {
            var response = new ResponseHandler
            {
                Code = (int)status,
                Status = status,
                Message = message,
                Error = error,
                Data = null
            };

            return new ObjectResult(response)
            {
                StatusCode = (int)status
            };
        }
    }
}
