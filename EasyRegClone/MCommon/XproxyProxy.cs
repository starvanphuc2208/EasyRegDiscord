using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;

namespace MCommon
{
	internal class XproxyProxy
	{
		private object k = new object();

		private object k1 = new object();

		public int typeProxy;

		private string ServicesURL;

		public string proxy;

		public string ip = "";

		public bool isProxyLive = true;

		public int dangSuDung = 0;

		public int daSuDung = 0;

		public int limit_theads_use = 3;

		public XproxyProxy(string ServicesURL, string proxy, int typeProxy, int limit_theads_use)
		{
			this.ServicesURL = ServicesURL;
			this.proxy = proxy;
			this.limit_theads_use = limit_theads_use;
			this.ip = "";
			this.typeProxy = typeProxy;
		}

		public bool ChangeProxy()
		{
			bool flag;
			bool flag1;
			int valueInt = (new JSON_Settings("configGeneral", false)).GetValueInt("nudDelayResetXProxy", 5) * 60;
			bool flag2 = false;
			try
			{
				int tickCount = Environment.TickCount;
				this.ServicesURL = this.ServicesURL.TrimEnd(new char[] { '/' });
				string str = string.Concat(this.ServicesURL, "/reset?proxy=", this.proxy);
				RequestXNet requestXNet = new RequestXNet("", "", "", 0);
				string str1 = requestXNet.RequestGet(str);
				this.ExportToFile(string.Concat(str, ": ", str1));
				JObject jObject = JObject.Parse(str1);
				bool flag3 = false;
				if (!jObject.ContainsKey("msg"))
				{
					flag1 = false;
				}
				else
				{
					flag1 = (JObject.Parse(str1)["msg"].ToString() == "command_sent" || JObject.Parse(str1)["msg"].ToString() == "OK" ? true : JObject.Parse(str1)["msg"].ToString().ToLower() == "ok");
				}
				if (flag1)
				{
					flag3 = true;
				}
				else if ((!jObject.ContainsKey("error_code") ? false : JObject.Parse(str1)["error_code"].ToString().ToLower() == "0"))
				{
					flag3 = true;
				}
				if (flag3)
				{
					while (!this.CheckLiveProxy())
					{
						Thread.Sleep(1000);
						if (Environment.TickCount - tickCount >= valueInt * 1000)
						{
							goto Xong;
						}
					}
					Thread.Sleep(1000);
					flag = true;
					return flag;
				}
			Xong:
				Console.WriteLine("Xong");
			}
			catch
			{
				flag2 = false;
			}
			flag = flag2;
			return flag;
		}

		public bool CheckLiveProxy()
		{
			bool flag = false;
			try
			{
				this.ServicesURL = this.ServicesURL.TrimEnd(new char[] { '/' });
				string str = string.Concat(this.ServicesURL, "/status?proxy=", this.proxy);
				RequestXNet requestXNet = new RequestXNet("", "", "", 0);
				string str1 = requestXNet.RequestGet(str);
				this.ExportToFile(string.Concat(str, ": ", str1));
				try
				{
					if ((str1.Contains("public_ip_v6") ? true : str1.Contains("public_ip")))
					{
						string str2 = this.proxy.Split(new char[] { ':' })[1];
						if ((str2.StartsWith("4") ? true : str2.StartsWith("5")))
						{
							flag = (JObject.Parse(str1)["public_ip"].ToString() == "" ? false : JObject.Parse(str1)["public_ip"].ToString() != "CONNECT_INTERNET_ERROR");
						}
						else if ((str2.StartsWith("6") ? true : str2.StartsWith("7")))
						{
							flag = (JObject.Parse(str1)["public_ip_v6"].ToString() == "" ? false : JObject.Parse(str1)["public_ip_v6"].ToString() != "CONNECT_INTERNET_ERROR");
						}
					}
					else
					{
						flag = Convert.ToBoolean(JObject.Parse(str1)["status"].ToString());
					}
				}
				catch
				{
					flag = JObject.Parse(str1)["error_code"].ToString() == "0";
				}
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

		private void ExportToFile(string content)
		{
			try
			{
				File.AppendAllText("GetProxy.txt", string.Concat(content, "\r\n"));
			}
			catch
			{
			}
		}
	}
}