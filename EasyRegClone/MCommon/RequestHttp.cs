using HttpRequest;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace MCommon
{
	public class RequestHttp
	{
		public RequestHTTP request;

		private string UserAgent;

		private string Proxy;

		public RequestHttp(string cookie = "", string userAgent = "", string proxy = "", int typeProxy = 0)
		{
			if (userAgent != "")
			{
				this.UserAgent = userAgent;
			}
			else
			{
				this.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";
			}
			this.request = new RequestHTTP();
			this.request.SetSSL(SecurityProtocolType.Tls12);
			this.request.SetKeepAlive(true);
			this.request.SetDefaultHeaders(new string[] { "content-type: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", string.Concat("user-agent: ", this.UserAgent) });
			if (cookie != "")
			{
				this.AddCookie(cookie);
			}
			this.Proxy = proxy;
		}

		public void AddCookie(string cookie)
		{
			string[] strArrays = cookie.Split(new char[] { ';' });
			string str = "";
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str1 = strArrays1[i];
				string[] strArrays2 = str1.Split(new char[] { '=' });
				if (strArrays2.Count<string>() > 1)
				{
					try
					{
						str = string.Concat(new string[] { str, strArrays2[0], "=", strArrays2[1], ";" });
					}
					catch
					{
					}
				}
			}
			this.request.SetDefaultHeaders(new string[] { "content-type: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8;charset=UTF-8", string.Concat("user-agent: ", this.UserAgent), string.Concat("cookie: ", str) });
		}

		public string GetCookie()
		{
			return this.request.GetCookiesString();
		}

		public string RequestGet(string url)
		{
			string str;
			if (this.Proxy == "")
			{
				str = this.request.Request("GET", url, null, null, true, null, 60000).ToString();
			}
			else
			{
				str = (!this.Proxy.Contains(":") ? this.request.Request("GET", url, null, null, true, new WebProxy("127.0.0.1", Convert.ToInt32(this.Proxy)), 60000).ToString() : this.request.Request("GET", url, null, null, true, new WebProxy(this.Proxy.Split(new char[] { ':' })[0], Convert.ToInt32(this.Proxy.Split(new char[] { ':' })[1])), 60000).ToString());
			}
			return str;
		}

		public string RequestPost(string url, string data = "")
		{
			string str;
			if (this.Proxy == "")
			{
				str = this.request.Request("POST", url, null, Encoding.ASCII.GetBytes(data), true, null, 60000).ToString();
			}
			else
			{
				str = (!this.Proxy.Contains(":") ? this.request.Request("POST", url, null, Encoding.ASCII.GetBytes(data), true, new WebProxy("127.0.0.1", Convert.ToInt32(this.Proxy)), 60000).ToString() : this.request.Request("POST", url, null, Encoding.ASCII.GetBytes(data), true, new WebProxy(this.Proxy.Split(new char[] { ':' })[0], Convert.ToInt32(this.Proxy.Split(new char[] { ':' })[1])), 60000).ToString());
			}
			return str;
		}
	}
}