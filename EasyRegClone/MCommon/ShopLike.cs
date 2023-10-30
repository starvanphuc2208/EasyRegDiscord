using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MCommon
{
	internal class ShopLike
	{
		private Random rd = new Random();

		private object k = new object();

		public int dangSuDung = 0;

		public int daSuDung = 0;

		public int limit_theads_use = 3;

		public string api_key
		{
			get;
			set;
		}

		public string ip
		{
			get;
			set;
		}

		public int port
		{
			get;
			set;
		}

		public string proxy
		{
			get;
			set;
		}

		public int typeProxy
		{
			get;
			set;
		}

		public ShopLike(string api_key, int typeProxy, int limit_theads_use)
		{
			this.api_key = api_key;
			this.proxy = "";
			this.ip = "";
			this.port = 0;
			this.typeProxy = typeProxy;
			this.limit_theads_use = limit_theads_use;
			this.dangSuDung = 0;
			this.daSuDung = 0;
		}

		public bool ChangeProxy()
		{
			bool flag;
			this.proxy = "";
			this.ip = "";
			this.port = 0;
			string str = ShopLike.RequestGet(string.Concat("http://proxy.shoplike.vn/Api/getNewProxy?access_token=", this.api_key));
			if (str != "")
			{
				try
				{
					JObject jObject = JObject.Parse(str);
					if (jObject["status"].ToString() == "success")
					{
						if (this.typeProxy == 0)
						{
							this.proxy = jObject["data"]["proxy"].ToString();
							string[] strArrays = this.proxy.Split(new char[] { ':' });
							this.ip = strArrays[0];
							this.port = int.Parse(strArrays[1]);
						}
						flag = true;
						return flag;
					}
				}
				catch
				{
				}
			}
			flag = false;
			return flag;
		}

		public bool CheckStatusProxy()
		{
			bool flag;
			this.proxy = "";
			this.ip = "";
			this.port = 0;
			string str = ShopLike.RequestGet(string.Concat("http://proxy.shoplike.vn/Api/getCurrentProxy?access_token=", this.api_key));
			if (str != "")
			{
				try
				{
					JObject jObject = JObject.Parse(str);
					if (jObject["status"].ToString() == "success")
					{
						this.proxy = jObject["data"]["proxy"].ToString();
						string[] strArrays = this.proxy.Split(new char[] { ':' });
						this.ip = strArrays[0];
						this.port = int.Parse(strArrays[1]);
						flag = true;
						return flag;
					}
				}
				catch
				{
				}
			}
			flag = false;
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

		public static string GetShopLikeProxy (string api)
        {
			string str = ShopLike.RequestGet(string.Concat("http://proxy.shoplike.vn/Api/getNewProxy?access_token=", api));
			if (str != "")
			{
				try
				{
					JObject jObject = JObject.Parse(str);
					if (jObject["status"].ToString() == "success")
					{
						return jObject["data"]["proxy"].ToString();
					} else
                    {
						str = ShopLike.RequestGet(string.Concat("http://proxy.shoplike.vn/Api/getCurrentProxy?access_token=", api));
						jObject = JObject.Parse(str);
						if (jObject["status"].ToString() == "success")
                        {
							return jObject["data"]["proxy"].ToString();
						}
					}
				}
				catch
				{
				}
			}
			return "";
		}

		public string GetProxy()
		{
			while (!this.CheckStatusProxy())
			{
			}
			return this.proxy;
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

		public static string RequestGet(string url)
		{
			string result = "";
            try
            {
                HttpClient httpClient = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Task<string> task = Task.Run<string>(() => ShopLike.GetURI(new Uri(url)));
                task.Wait();
                result = task.Result;
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "Request get");
                result = "";
            }
            return result;
		}
	}
}