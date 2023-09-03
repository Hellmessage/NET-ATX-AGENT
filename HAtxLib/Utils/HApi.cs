using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace HAtxLib.Utils {
    internal static class HApi {

        public static HttpWebResponse Get(string url) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 5 * 60 * 1000;
                request.KeepAlive = false;
                return (HttpWebResponse)request.GetResponse();
            } catch (WebException e) {
                return e.Response as HttpWebResponse;
            } catch (Exception) {
                return default;
            }
        }

        public static HttpWebResponse Delete(string url) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "DELETE";
                request.Timeout = 5 * 60 * 1000;
                request.KeepAlive = false;
                return (HttpWebResponse)request.GetResponse();
            } catch (WebException e) {
                return e.Response as HttpWebResponse;
            } catch (Exception) {
                return default;
            }
        }

        public static HttpWebResponse Post(string url, string data) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 5 * 60 * 1000;
                request.KeepAlive = false;
                if (!string.IsNullOrEmpty(data)) {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    Stream stream = request.GetRequestStream();
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                    stream.Close();
                }
                return (HttpWebResponse)request.GetResponse();
            } catch (Exception) {
                return default;
            }
        }

        public static HttpWebResponse Post(string url, JObject data) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 5 * 60 * 1000;
                request.KeepAlive = false;
                SetHeaderValue(request.Headers, "Content-Type", "application/json; charset=UTF-8");
                if (data != null) {
                    string postdata = data.ToString();
                    byte[] buffer = Encoding.UTF8.GetBytes(postdata);
                    Stream stream = request.GetRequestStream();
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                    stream.Close();
                }
                return (HttpWebResponse)request.GetResponse();
            } catch (Exception) {
                return default;
            }
        }

        public static string DecodeDefault(this HttpWebResponse response) {
            try {
                using (Stream stream = response.GetResponseStream()) {
                    using (StreamReader sr = new StreamReader(stream, Encoding.UTF8)) {
                        return sr.ReadToEnd();
                    }
                }
            } catch (Exception) {
                return null;
            }
        }

        private static void SetHeaderValue(WebHeaderCollection header, string name, string value) {
            var type = typeof(WebHeaderCollection);
            if (type == null) {
                return;
            }
            PropertyInfo property = type.GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null) {
                NameValueCollection collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
    }
}
