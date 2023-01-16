using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace CoreData.Common
{
    public static class HttpExtensions
    {
        #region HttpRequest

        public static string ReadBody(this HttpRequest request, JsonSerializerSettings settings = null)
        {
            return ReadBody<string>(request, settings);
        }

        public static T ReadBody<T>(this HttpRequest request, JsonSerializerSettings settings = null) where T : class
        {
            return ReadBodyAsync<T>(request, settings).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<string> ReadBodyAsync(this HttpRequest request, JsonSerializerSettings settings = null)
        {
            return await ReadBodyAsync<string>(request, settings);
        }

        public static async Task<T> ReadBodyAsync<T>(this HttpRequest request, JsonSerializerSettings settings = null) where T : class
        {
            try
            {
                settings ??= CustomJsonSerializerSettings.Logging;

                if (request.Body.CanSeek)
                    request.Body.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(request.Body))
                {
                    var requestBody = await streamReader.ReadToEndAsync();

                    request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

                    if (typeof(T) == typeof(string))
                        return requestBody as T;

                    return JsonConvert.DeserializeObject<T>(requestBody, settings);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "HttpExtensions.ReadHttpRequestBodyAsync");
                return default;
            }
        }

        #endregion

        #region Response

        public static string ReadBody(this HttpResponse response, Stream originalBody, JsonSerializerSettings settings = null)
        {
            return ReadBody<string>(response, originalBody, settings);
        }

        public static T ReadBody<T>(this HttpResponse response, Stream originalBody, JsonSerializerSettings settings = null) where T : class
        {
            return ReadBodyAsync<T>(response, originalBody, settings).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<string> ReadBodyAsync(this HttpResponse response, Stream originalBody, JsonSerializerSettings settings = null)
        {
            return await ReadBodyAsync<string>(response, originalBody, settings);
        }

        public static async Task<T> ReadBodyAsync<T>(this HttpResponse response, Stream originalBody, JsonSerializerSettings settings = null) where T : class
        {
            try
            {
                if (!response.Body.CanRead)
                    return null;

                settings ??= CustomJsonSerializerSettings.Logging;

                //reset the buffer and read out the contents
                if (response.Body.CanSeek)
                    response.Body.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(response.Body))
                {
                    string responseBody = await streamReader.ReadToEndAsync();

                    //reset the buffer and read out the contents
                    if (response.Body.CanSeek)
                        response.Body.Seek(0, SeekOrigin.Begin);

                    //copy our content to the original stream and put it back
                    await response.Body.CopyToAsync(originalBody);
                    response.Body = originalBody;

                    if (typeof(T) == typeof(string))
                        return responseBody as T;

                    return JsonConvert.DeserializeObject<T>(responseBody, settings);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "HttpExtensions.ReadHttpResponseBodyAsync");
                return default;
            }
        }

        public static MemoryStream PrepareToRead(this HttpResponse response, out Stream originalBody)
        {
            var buffer = new MemoryStream();

            //replace the context response with our buffer
            originalBody = response.Body;

            response.Body = buffer;

            return buffer;
        }

        #endregion

        public static StringContent AsStringContent(this string content, Encoding encoding = null, string mediaTypeName = null)
        {
            encoding ??= Encoding.UTF8;
            mediaTypeName ??= MediaTypeNames.Application.Json;

            return new StringContent(content, encoding, mediaTypeName);
        }
    }
}