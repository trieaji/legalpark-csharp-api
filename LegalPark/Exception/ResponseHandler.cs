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

        /// <summary>
        /// Membuat respons sukses dengan status OK (200).
        /// </summary>
        /// <param name="responseObj">Objek data yang akan disertakan dalam respons.</param>
        /// <returns>Objek IActionResult yang berisi respons terstruktur.</returns>
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

        /// <summary>
        /// Membuat respons sukses dengan status, pesan, dan data yang disesuaikan.
        /// </summary>
        /// <param name="status">Status HTTP yang akan digunakan.</param>
        /// <param name="message">Pesan respons.</param>
        /// <param name="data">Objek data yang akan disertakan.</param>
        /// <returns>Objek IActionResult yang berisi respons terstruktur.</returns>
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

        /// <summary>
        /// Membuat respons kesalahan dengan status, objek kesalahan, dan pesan yang disesuaikan.
        /// </summary>
        /// <param name="status">Status HTTP kesalahan.</param>
        /// <param name="error">Objek yang berisi detail kesalahan.</param>
        /// <param name="message">Pesan kesalahan.</param>
        /// <returns>Objek IActionResult yang berisi respons kesalahan terstruktur.</returns>
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
