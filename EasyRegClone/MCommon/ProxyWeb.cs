using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace MCommon
{
	internal class ProxyWeb
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

		public ProxyWeb(string apiKey, string proxy, int typeProxy, int limit_theads_use)
		{
			this.apiKey = apiKey;
			this.proxy = proxy;
			this.limit_theads_use = limit_theads_use;
			this.ip = "";
			this.typeProxy = typeProxy;
		}

		public bool ChangeProxy()
		{
			bool flag;
			bool flag1 = false;
			try
			{
				string str = string.Concat("https://api.proxyv6.net/api/reset-ip-manual?api_key=", this.apiKey);
				string[] strArrays = new string[] { "{\"host\": \"", null, null, null, null };
				strArrays[1] = this.proxy.Split(new char[] { ':' })[0];
				strArrays[2] = "\", \"port\": ";
				strArrays[3] = this.proxy.Split(new char[] { ':' })[1];
				strArrays[4] = "}";
				string str1 = string.Concat(strArrays);
				if (JObject.Parse((new RequestXNet("", "", "", 0)).RequestPost(str, str1, "application/json"))["message"].ToString() == "SUCCESS")
				{
					int num = 0;
					while (num < 120)
					{
						if (this.CheckLiveProxy())
						{
							Thread.Sleep(1000);
							flag = true;
							return flag;
						}
						else
						{
							Thread.Sleep(1000);
							num++;
						}
					}
				}
			}
			catch
			{
				flag1 = false;
			}
			flag = flag1;
			return flag;
		}

		public bool CheckLiveProxy()
		{
			bool flag = false;
			try
			{
				string str = string.Concat("https://api.proxyv6.net/api/check-list-proxy?api_key=", this.apiKey);
				string str1 = string.Concat(new string[] { this.proxy.Split(new char[] { ':' })[2], ":", this.proxy.Split(new char[] { ':' })[3], "@", this.proxy.Split(new char[] { ':' })[0], ":", this.proxy.Split(new char[] { ':' })[1] });
				string str2 = string.Concat("{\"proxies\": [\"", str1, "\"]}");
				RequestXNet requestXNet = new RequestXNet("", "", "", 0);
				string str3 = requestXNet.RequestPost(str, str2, "application/json");
				flag = Convert.ToBoolean((JObject.Parse(str3)["message"].ToString() != "SUCCESS" ? false : JObject.Parse(str3)["data"]["ip"].ToString() != ""));
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
	}
}