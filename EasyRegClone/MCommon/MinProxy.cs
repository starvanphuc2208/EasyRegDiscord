using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MCommon
{
    class MinProxy
    {
		public static string RequestGet(string url)
		{
			string result = "";
			try
			{
				new HttpClient();
				ServicePointManager.Expect100Continue = true;
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				Task<string> task = Task.Run<string>(() => MinProxy.GetURI(new Uri(url)));
				task.Wait();
				result = task.Result;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "Request get");
				result = "";
			}
			return result;
		}

		public static bool CheckApiProxy(string apiProxy)
		{
			string text = MinProxy.RequestGet("http://dash.minproxy.vn/api/rotating/v1/proxy/get-status?api_key=" + apiProxy);
			if (text != "")
			{
				try
				{
					JObject jobject = JObject.Parse(text);
					if (jobject["code"].ToString() == "1")
					{
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		private static async Task<string> GetURI(Uri u)
		{
			string response = string.Empty;
			using (HttpClient client = new HttpClient())
			{
				HttpResponseMessage result = await client.GetAsync(u);
				if (result.IsSuccessStatusCode)
				{
					response = await result.Content.ReadAsStringAsync();
				}
			}
			return response;
		}

		public static string GetMinproxy(string api)
		{
			string text = MinProxy.RequestGet("http://dash.minproxy.vn/api/rotating/v1/proxy/get-new-proxy?api_key=" + api);
			if (text != "")
			{
				try
				{
					if (text.Contains("api expired"))
					{
						return "";
					}
					if (text.Contains("api wrong") || text.Contains("error"))
					{
						return "";
					}


					JObject jobject = JObject.Parse(text);
					string value = Regex.Match(jobject["data"]["next_request"].ToString(), "\\d+").Value;
					int next_change = ((value == "") ? 0 : int.Parse(value));

					if (next_change > 0)
					{
						// Lấy ra IP cũ
						text = MinProxy.RequestGet("http://dash.minproxy.vn/api/rotating/v1/proxy/get-current-proxy?api_key=" + api);
						if (text != "")
						{
							try
							{
								jobject = JObject.Parse(text);
								return jobject["data"]["http_proxy"].ToString() + ":" + jobject["data"]["username"].ToString() + ":" + jobject["data"]["password"].ToString();
							}
							catch
							{
							}
						}

					}
					else
					{
						// Lấy ra IP mới
						return jobject["data"]["http_proxy"].ToString() + ":" + jobject["data"]["username"].ToString() + ":" + jobject["data"]["password"].ToString();
					}

				}
				catch
				{
				}
			}

			return "";
		}
	}
}
