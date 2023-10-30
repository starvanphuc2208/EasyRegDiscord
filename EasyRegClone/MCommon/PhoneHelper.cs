using System;
using System.Text.RegularExpressions;
using System.Threading;
using MCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace easy.MCommon
{
    class PhoneHelper
    {
		//public static string GetPhoneChoThueSimCode(string apiKey, string nha_mang, string dau_so)
		//{
		//	string result = "";

  //          for (int i = 0; i < 100; i++)
  //          {
		//		try
		//		{
		//			xNet.HttpRequest httpRequest = new xNet.HttpRequest();

		//			string url = "https://chothuesimcode.com/api?act=number&apik=" + apiKey + "&appId=1006";

		//			if (nha_mang != "")
		//			{
		//				url = url + "&carrier=" + nha_mang;
		//			}

		//			if (dau_so != "")
		//			{
		//				url = url + "&prefix=" + dau_so;
		//			}

		//			string str1 = httpRequest.Get(url).ToString();

		//			string requestID = Regex.Match(str1, "\"Id\":(\\d+),").Groups[1].Value;
		//			string phoneNumber = Regex.Match(str1, "\"Number\":\"(\\d+)\",").Groups[1].Value;

		//			if (!(requestID != string.Empty) || !(phoneNumber != string.Empty))
		//			{
		//				result = "";
		//				Common.DelayTime(5);
		//			}
		//			else
		//			{
		//				result = string.Concat(requestID, "|", phoneNumber);
		//				goto Xong;
		//			}
		//		}
		//		catch
		//		{
		//		}
		//	}

		//	Xong:
		//	return result;
		//}

		//public static bool HuyChoThueSimCode(string api, string request_id)
		//{
		//	string result = "";
		//	RequestHttp requestXNet = new RequestHttp("", "", "");
  //          try
  //          {
		//		result = requestXNet.RequestGet("https://chothuesimcode.com/api?act=expired&apik="+api+"&id=" + request_id).ToString();
		//		if (result.Contains("OK"))
  //              {
		//			return true;
  //              } else
  //              {
		//			return false;
  //              }
		//	}
  //          catch (Exception ex)
  //          {
		//		Common.ExportError(ex, "HuyChoThueSimCode()");
  //          }
		//	return false;
		//}

		//public static string GetOTPChoThueSimCode(string api, string request_id, int timeOut = 120)
		//{
		//	string result = "";
		//	RequestHttp requestXNet = new RequestHttp("", "", "");
		//	int tickCount = Environment.TickCount;
		//	while (Environment.TickCount - tickCount <= timeOut * 1000)
		//	{
		//		string text = requestXNet.RequestGet(string.Concat("https://chothuesimcode.com/api?act=code&apik=", api, "&id=", request_id.Split('|')[0])).ToString();
		//		try
		//		{
		//			JObject jobject = JObject.Parse(text);
		//			result = jobject["Result"]["Code"].ToString();
		//			if (result != "")
		//			{
		//				break;
		//			}
		//			Thread.Sleep(3000);
		//		}
		//		catch (Exception)
		//		{

		//		}
		//	}
		//	return result;
		//}

		public static string RequestGet(string url)
		{
			return (new xNet.HttpRequest()).Get(url, null).ToString();
		}

		public static string GetPhoneOtpSim(string apiKey, int timeOut, string nhamang, string dauso)
		{
			string result = "";
			for (int i = 0; i < timeOut / 2; i++)
			{
				try
				{
					string.Concat(new string[] { "http://otpsim.com/api/phones/request?token=", apiKey, "&service=4&network=", nhamang, "&prefix=", dauso });
					string html = PhoneHelper.RequestGet(string.Concat(new string[] { "http://otpsim.com/api/phones/request?token=", apiKey, "&service=4&network=", nhamang, "&prefix=", dauso }));
					dynamic obj = JsonConvert.DeserializeObject(html);
					string requestID = (string)obj["data"]["session"];
					string phoneNumber = (string)obj["data"]["phone_number"];
					if ((!string.IsNullOrEmpty(requestID) ? requestID.Length > 4 : false))
					{
						result = requestID + "|" + phoneNumber;
						break;
					}
					else
					{
						Thread.Sleep(2000);
					}
				}
				catch
				{
					Thread.Sleep(2000);
				}
			}
			return result;
		}

		public static string GetOtpOtpSim(string apiKey, string requestID, int timeOut)
		{
			string value = "";
			for (int i = 0; i < timeOut / 3; i++)
			{
				try
				{
					string str = string.Concat("http://otpsim.com/api/sessions/", requestID, "?token=", apiKey);
					dynamic obj = JsonConvert.DeserializeObject(PhoneHelper.RequestGet(str));
					value = (string)obj["data"]["messages"][0]["otp"].ToString();
					if ((!string.IsNullOrEmpty(value) ? value.ToString() == "[]" : true))
					{
						Thread.Sleep(3000);
					}
					else
					{
						value = Regex.Match(value.Replace(" ", ""), "\\d+").Value;
						if (!string.IsNullOrEmpty(value))
						{
							break;
						}
					}
				}
				catch (Exception exception)
				{
					Thread.Sleep(3000);
				}
			}
			return value;
		}

		public static string GetOtpCTSC(string apiKey, string requestID, int timeOut)
		{
			string value = "";
			for (int i = 0; i < timeOut / 3; i++)
			{
				try
				{
					string str = string.Concat("https://chothuesimcode.com/api?act=code&apik=", apiKey, "&id=", requestID.Split(new char[] { '|' })[0]);
					dynamic obj = JsonConvert.DeserializeObject(PhoneHelper.RequestGet(str));
					value = (string)obj["Result"]["Code"];
					if ((!string.IsNullOrEmpty(value) ? value.ToString() != "[]" : false))
					{
						value = Regex.Match(value.Replace(" ", ""), "\\d+").Value;
						if (!string.IsNullOrEmpty(value))
						{
							break;
						}
					}
					else
					{
						Thread.Sleep(3000);
					}
				}
				catch (Exception exception)
				{
					Thread.Sleep(3000);
				}
			}
			return value;
		}

		public static string GetPhoneCTSC(string apiKey, int timeOut, string nhamang, string dauso)
		{
			string str = "";
			string str1 = "";
			for (int i = 0; i < timeOut / 2; i++)
			{
				try
				{
					if (Base.rd.Next(0, 2) == 0)
					{
					}
					string str2 = PhoneHelper.RequestGet(string.Concat(new string[] { "https://chothuesimcode.com/api?act=number&apik=", apiKey, "&appId=1002&carrier=", nhamang, "&prefix=", dauso }));
					dynamic obj = JsonConvert.DeserializeObject(str2);
					str = (string)obj["Result"]["Id"];
					str1 = (string)obj["Result"]["Number"];
					if ((!string.IsNullOrEmpty(str) ? str.Length <= 4 : true))
					{
						Thread.Sleep(2000);
					}
					else
					{
						break;
					}
				}
				catch
				{
					Thread.Sleep(2000);
				}
			}
			return string.Concat(str, "|0", str1);
		}

		public static string GetOtpOtpSimV2(string apiKey, string requestID, int timeOut)
		{
			string value = "";
			for (int i = 0; i < timeOut / 3; i++)
			{
				try
				{
					string str = string.Concat("http://otpsim.com/api/sessions/", requestID, "?token=", apiKey);
					dynamic obj = JsonConvert.DeserializeObject(PhoneHelper.RequestGet(str));
					value = (string)obj["data"]["messages"][0]["otp"].ToString();
					if ((!string.IsNullOrEmpty(value) ? value.ToString() != "[]" : false))
					{
						value = Regex.Match(value.Replace(" ", ""), "\\d+").Value;
						if (!string.IsNullOrEmpty(value))
						{
							break;
						}
					}
					else
					{
						Thread.Sleep(3000);
					}
				}
				catch (Exception exception)
				{
					Thread.Sleep(3000);
				}
			}
			return value;
		}

		public static string GetPhoneCTSCOld(string string_0, int int_0, string string_1)
		{
			string str = "";
			for (int i = 0; i < int_0 / 2; i++)
			{
				try
				{
					string str1 = PhoneHelper.RequestGet(string.Concat("https://chothuesimcode.com/api?act=number&apik=", string_0, "&appId=1002&number=", string_1));
					dynamic obj = JsonConvert.DeserializeObject(str1);
					str = (string)obj["Result"]["Id"];
					if ((!string.IsNullOrEmpty(str) ? str.Length <= 4 : true))
					{
						Thread.Sleep(2000);
					}
					else
					{
						break;
					}
				}
				catch
				{
					Thread.Sleep(2000);
				}
			}
			return str;
		}

		public static string GetPhoneOtpSimOld(string apiKey, int timeOut, string phoneNumber)
		{
			string str = "";
			for (int i = 0; i < timeOut / 2; i++)
			{
				try
				{
					string str1 = PhoneHelper.RequestGet(string.Concat("http://otpsim.com/api/phones/request?token=", apiKey, "&service=4&number=", phoneNumber));
					dynamic obj = JsonConvert.DeserializeObject(str1);
					str = (string)obj["data"]["session"];
					if ((!string.IsNullOrEmpty(str) ? str.Length <= 4 : true))
					{
						Thread.Sleep(2000);
					}
					else
					{
						break;
					}
				}
				catch
				{
					Thread.Sleep(2000);
				}
			}
			return str;
		}
	}
}
