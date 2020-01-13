using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CommonUtils
{
    public static partial class Extension
    {
        public static string ReadString(this HttpResponseMessage message)
        {
            try
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    return default;
                return message.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                LogUtil.Print("HttpResponseMessage ReadString Error: " + ex.Message);
                return default;
            }
        }
        public static string GetString(this HttpResponseMessage message)
        => message.ReadString();

        public static JToken ReadJToken(this HttpResponseMessage message)
        {
            try
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    return default;
                return message.Content.ReadAsStringAsync().Result.ParseToJToken();
            }
            catch (Exception ex)
            {
                LogUtil.Print("HttpResponseMessage ReadJToken Error: " + ex.Message);
                return default;
            }
        }
        public static JToken GetJToken(this HttpResponseMessage message)
        => message.ReadJToken();

        public static JObject ReadJObject(this HttpResponseMessage message)
        {
            try
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    //因为framework不支持ApiResult，所以这里直接用json结构
                    return new JObject {
                        { "Code", (int)message.StatusCode },
                        { "Message", message.StatusCode.ToString() },
                        { "Data", message.ReasonPhrase }
                    };
                return message.Content.ReadAsStringAsync().Result.ParseToJObject();
            }
            catch (Exception ex)
            {
                LogUtil.Print("HttpResponseMessage ReadJObject Error: " + ex.Message);
                return default;
            }
        }
        public static JObject GetJObject(this HttpResponseMessage message)
        => message.ReadJObject();

        public static JArray ReadJArray(this HttpResponseMessage message)
        {
            try
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    return default;
                return message.Content.ReadAsStringAsync().Result.ParseToJArray();
            }
            catch (Exception ex)
            {
                LogUtil.Print("HttpResponseMessage ReadJArray Error: " + ex.Message);
                return default;
            }
        }
        public static JArray GetJArray(this HttpResponseMessage message)
        => message.ReadJArray();

        public static Stream ReadStream(this HttpResponseMessage message)
        {
            try
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    return default;
                return message.Content.ReadAsStreamAsync().Result;
            }
            catch (Exception ex)
            {
                LogUtil.Print("HttpResponseMessage ReadStream Error: " + ex.Message);
                return default;
            }
        }
        public static Stream GetStream(this HttpResponseMessage message)
        => message.ReadStream();

        public static byte[] ReadBytes(this HttpResponseMessage message)
        {
            try
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    return default;
                return message.Content.ReadAsByteArrayAsync().Result;
            }
            catch (Exception ex)
            {
                LogUtil.Print("HttpResponseMessage ReadBytes Error: " + ex.Message);
                return default;
            }
        }
        public static byte[] GetBytes(this HttpResponseMessage message)
        => message.ReadBytes();
    }

    public static class HttpUtil
    {
        private static HttpClientHandler _handlerWithoutSslCheck;

        public static void InitHandler()
        {
            _handlerWithoutSslCheck = new HttpClientHandler();
            _handlerWithoutSslCheck.ServerCertificateCustomValidationCallback = delegate { return true; };
        }

        static HttpUtil()
        {
            InitHandler();
        }

        public static string GetFromCookies(string url, string name)
        {
            var cookies = _handlerWithoutSslCheck.CookieContainer.GetCookies(new Uri(url));
            var cookie = cookies[name];
            if (cookie == null)
                return null;
            return cookie.Value;
        }

        public static void ClearCookies()
        => InitHandler();

        public enum Method
        {
            Get,
            Post,
            Delete,
            Put,
            Patch,
        }

        public static HttpResponseMessage Send(Method method, string url, HttpContent content = null, Dictionary<string, string> headers = null, bool log = true)
        {
            var message = new HttpResponseMessage();
            message.StatusCode = HttpStatusCode.ExpectationFailed;

            var client = new HttpClient(_handlerWithoutSslCheck);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            //ConsoleUtil.Print("RequestHeaders:");
            //client.DefaultRequestHeaders.PrintJson();

            try
            {
                switch (method)
                {
                    case Method.Get:
                        message = client.GetAsync(url).Result;
                        break;

                    case Method.Post:
                        message = client.PostAsync(url, content).Result;
                        break;

                    case Method.Delete:
                        message = client.DeleteAsync(url).Result;
                        break;

                    case Method.Put:
                        message = client.PutAsync(url, content).Result;
                        break;

                    case Method.Patch:
                        //message = client.PatchAsync(url, content).Result;
                        break;
                }
            }
            catch (Exception ex)
            {
                message.ReasonPhrase = ex.Message;
                if (log)
                    LogUtil.Error(url, ex);
                else
                    LogUtil.Print(url + ex.Message);
            }

            //ConsoleUtil.Print("ResponseHeaders:");
            //message.Headers.PrintJson();

            ConsoleUtil.WriteLine(method + " " + url + " " + (int)message.StatusCode + " " + message.StatusCode);
            return message;
        }

        public static HttpResponseMessage Get(string url, Dictionary<string, string> headers = null, bool log = true)
        => Send(Method.Get, url, null, headers, log);

        public static HttpResponseMessage Post(string url, HttpContent content = null, Dictionary<string, string> headers = null, bool log = true)
        => Send(Method.Post, url, content, headers, log);

        public static HttpResponseMessage Delete(string url, Dictionary<string, string> headers = null, bool log = true)
        => Send(Method.Delete, url, null, headers, log);

        public static HttpResponseMessage Put(string url, HttpContent content = null, Dictionary<string, string> headers = null, bool log = true)
        => Send(Method.Put, url, content, headers, log);

        public static HttpResponseMessage Patch(string url, HttpContent content = null, Dictionary<string, string> headers = null, bool log = true)
        => Send(Method.Patch, url, content, headers, log);

        public static HttpStatusCode GetStatusCode(string url, Dictionary<string, string> headers = null, bool log = true)
        => Get(url, headers, log).StatusCode;

        public static string GetString(string url, Dictionary<string, string> headers = null, bool log = true)
        => Get(url, headers, log).ReadString();

        public static JToken GetJToken(string url, Dictionary<string, string> headers = null, bool log = true)
        => Get(url, headers, log).ReadJToken();

        public static JObject GetJObject(string url, Dictionary<string, string> headers = null, bool log = true)
        => Get(url, headers, log).ReadJObject();

        public static JArray GetJArray(string url, Dictionary<string, string> headers = null, bool log = true)
        => Get(url, headers, log).ReadJArray();

        public static Stream GetStream(string url, Dictionary<string, string> headers = null)
        => Get(url, headers).ReadStream();

        public static byte[] GetBytes(string url, Dictionary<string, string> headers = null)
        => Get(url, headers).ReadBytes();

        public static bool Download(string url, string path, Dictionary<string, string> headers = null)
        {
            var message = Get(url, headers);
            if (message.StatusCode == HttpStatusCode.OK)
            {
                FileUtil.Save(path, message.ReadStream());
                Console.WriteLine("Saved to " + path);
                return true;
            }
            return false;
        }

        public static HttpResponseMessage PostJson(string url, string data, Encoding encoding = null, Dictionary<string, string> headers = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return Post(url, new StringContent(data, encoding, ContentType.Json), headers);
        }

        public static HttpResponseMessage PostForm(string url, string data, Encoding encoding = null, Dictionary<string, string> headers = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return Post(url, new StringContent(data, encoding, ContentType.Form), headers);
        }

        public static HttpResponseMessage PostFile(string url, string path, bool nameEncode = false, Dictionary<string, string> headers = null)
        {
            var streamContent = new StreamContent(File.OpenRead(path));
            var fileName = FileUtil.GetName(path);
            fileName = nameEncode ? fileName.UrlEncode() : fileName;

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(streamContent, "file", fileName);

            return Post(url, multipartFormDataContent, headers);
        }

        public static HttpResponseMessage PostFileByBase64(string url, string path)
        => PostForm(url, string.Format("name={0}&base64={1}", FileUtil.GetName(path), FileUtil.GetBase64(path)));

        public static HttpResponseMessage PostFileByJson(string url, string path)
        => PostJson(url, new { Name = FileUtil.GetName(path), Base64 = FileUtil.GetBase64(path) }.ToJson(true));

        public static HttpResponseMessage Upload(string url, string path, bool nameEncode = false, Dictionary<string, string> headers = null)
        => PostFile(url, path, nameEncode, headers);

        public static HttpResponseMessage PutJson(string url, string data, Encoding encoding = null, Dictionary<string, string> headers = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return Put(url, new StringContent(data, encoding, ContentType.Json), headers);
        }

        public static HttpResponseMessage PatchJson(string url, string data, Encoding encoding = null, Dictionary<string, string> headers = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return Patch(url, new StringContent(data, encoding, ContentType.Json), headers);
        }
    }
}
