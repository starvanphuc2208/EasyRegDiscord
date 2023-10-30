using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MCommon
{
	internal class XProxy
	{
		public XProxy()
		{
		}

		public static bool CheckLiveProxy(string ServicesURL, string proxy)
		{
			bool flag = false;
			try
			{
				ServicesURL = ServicesURL.TrimEnd(new char[] { '/' });
				string str = string.Concat(ServicesURL, "/status?proxy=", proxy);
				RequestXNet requestXNet = new RequestXNet("", "", "", 0);
				string str1 = requestXNet.RequestGet(str);
				flag = Convert.ToBoolean(JObject.Parse(str1)["status"].ToString());
			}
			catch
			{
			}
			return flag;
		}

		public static List<string> CloneList(List<string> lstFrom)
		{
			List<string> strs = new List<string>();
			try
			{
				for (int i = 0; i < lstFrom.Count; i++)
				{
					strs.Add(lstFrom[i]);
				}
			}
			catch
			{
			}
			return strs;
		}

		public static int ResetAllProxy(string ServicesURL, List<string> lstProxy)
		{
			int num;
			int num1 = 0;
			try
			{
				ServicesURL = ServicesURL.TrimEnd(new char[] { '/' });
				string str = string.Concat(ServicesURL, "/reset_all");
				if (Convert.ToBoolean(JObject.Parse((new RequestXNet("", "", "", 0)).RequestGet(str))["status"].ToString()))
				{
					int num2 = 0;
					while (num2 < 120)
					{
						for (int i = 0; i < lstProxy.Count; i++)
						{
							if (XProxy.CheckLiveProxy(ServicesURL, lstProxy[i]))
							{
								int num3 = i;
								i = num3 - 1;
								lstProxy.RemoveAt(num3);
							}
						}
						if (lstProxy.Count == 0)
						{
							num = 1;
							return num;
						}
						else
						{
							Common.DelayTime(1);
							num2++;
						}
					}
				}
			}
			catch
			{
				num1 = -1;
			}
			num = num1;
			return num;
		}

		public static int ResetProxy(string ServicesURL, string proxy)
		{
			int num;
			int num1 = 0;
			try
			{
				ServicesURL = ServicesURL.TrimEnd(new char[] { '/' });
				string str = string.Concat(ServicesURL, "/reset?proxy=", proxy);
				if (JObject.Parse((new RequestXNet("", "", "", 0)).RequestGet(str))["msg"].ToString() == "command_sent")
				{
					int num2 = 0;
					while (num2 < 120)
					{
						if (XProxy.CheckLiveProxy(ServicesURL, proxy))
						{
							num = 1;
							return num;
						}
						else
						{
							Common.DelayTime(1);
							num2++;
						}
					}
				}
			}
			catch
			{
				num1 = -1;
			}
			num = num1;
			return num;
		}

		public static int ResetProxy(string ServicesURL, List<string> lstXProxy)
		{
			int num;
			int num1 = 0;
			try
			{
				List<string> strs = XProxy.CloneList(lstXProxy);
				for (int i = 0; i < strs.Count; i++)
				{
					XProxy.ResetProxy(ServicesURL, strs[i]);
				}
				int num2 = 0;
				while (num2 < 120)
				{
					for (int j = 0; j < strs.Count; j++)
					{
						if (XProxy.CheckLiveProxy(ServicesURL, strs[j]))
						{
							int num3 = j;
							j = num3 - 1;
							strs.RemoveAt(num3);
						}
					}
					if (strs.Count == 0)
					{
						num = 1;
						return num;
					}
					else
					{
						Common.DelayTime(1);
						num2++;
					}
				}
			}
			catch
			{
				num1 = -1;
			}
			num = num1;
			return num;
		}
	}
}