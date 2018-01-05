using System;
using System.Net.Http;
using System.Threading.Tasks;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Core.Internal;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Extensions
{
    public static class UpdateSystemResponseExtensions
    {
        public static async Task<Exception> GetResponseException(HttpResponseMessage response,
            IUpdateController updateController)
        {
            var result = await response.Content.ReadAsStringAsync();

            RestError[] errors;
            try
            {
                errors = JsonConvert.DeserializeObject<RestError[]>(result, updateController.JsonSerializerSettings);
            }
            catch (Exception)
            {
                throw new HttpRequestException(result);
            }

            if (errors == null)
                throw new HttpRequestException($"Invalid response (status code: {response.StatusCode}): {result}");

            var error = errors[0];
            switch (error.Type)
            {
                case ErrorTypes.ValidationError:
                    return new ArgumentException(error.Message);
                default:
                    return new UpdateSystemRequestException(error);
            }
        }
    }
}