using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MCommon
{
	internal class Vitech
	{
		private object k = new object();

		private object k1 = new object();

		public int typeProxy;

		private string apiKey;

		public string proxy;

		public string ip = "";

		public int dangSuDung = 0;

		public int daSuDung = 0;

		public int limit_theads_use = 3;

		public Vitech(string apiKey, string proxy, int typeProxy, int limit_theads_use)
		{
			this.apiKey = apiKey;
			this.proxy = proxy;
			this.limit_theads_use = limit_theads_use;
			this.ip = "";
			this.typeProxy = typeProxy;
		}

		public bool ChangeProxy()
		{
			bool flag = false;
			try
			{
				string str = "https://apiv2-public.vitechcheap.com/v1/public/rotate";
				string str1 = string.Concat("{\"proxy\": \"", this.proxy, "\"}");
				Vitech.RequestPost(str, this.apiKey, str1);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public bool CheckLiveProxy()
		{
			bool flag = false;
			try
			{
			}
			catch
			{
			}
			return flag;
		}

		public void DecrementDangSuDung()
		{
			lock (this.k)
			{
				this.dangSuDung--;
				if ((this.dangSuDung != 0 ? false : this.daSuDung == this.limit_theads_use))
				{
					this.daSuDung = 0;
				}
			}
		}

		private static string RequestPost(string url, string apiKey, string data)
		{
			HttpContent httpContent = null;
			string result = "";
			//try
			//{
			//	HttpClient httpClient = new HttpClient();
			//	ServicePointManager.Expect100Continue = true;
			//	ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			//	httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
			//	Task<string> task = Task.Run<string>(() => Vitech.PostURI(new Uri(url), httpContent));
			//	task.Wait();
			//	result = task.Result;
			//}
			//catch (Exception exception)
			//{
			//	Common.ExportError(exception, "Request Post");
			//	result = "";
			//}
			return result;
		}
	}
}