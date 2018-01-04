using System;

namespace CodeElements.UpdateSystem.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        ///     Adds the specified parameter to the Query String.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="paramName">Name of the parameter to add.</param>
        /// <param name="paramValue">Value for the parameter to add.</param>
        /// <returns>Url with added parameter.</returns>
        public static Uri AddQueryParameters(this Uri uri, string paramName, string paramValue)
        {
            var uriBuilder = new UriBuilder(uri);
            if (string.IsNullOrEmpty(uriBuilder.Query))
                uriBuilder.Query = $"{paramName}={Uri.EscapeDataString(paramValue)}";
            else //for some reasons, the uri builder adds a '?' before the value when setting the property
                uriBuilder.Query = uriBuilder.Query.Remove(0, 1) + $"&{paramName}={Uri.EscapeDataString(paramValue)}";

            return uriBuilder.Uri;
        }
    }
}