using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MCommon
{
    internal class Tinsoft
    {
        public string errorCode = "";

        private string svUrl = "http://proxy.tinsoftsv.com";

        private int lastRequest = 0;

        public string api_key
        {
            get;
            set;
        }

        public int connecting
        {
            get;
            set;
        }

        public int countConnected
        {
            get;
            set;
        }

        public string ip
        {
            get;
            set;
        }

        public int location
        {
            get;
            set;
        }

        public int next_change
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

        public int timeout
        {
            get;
            set;
        }

        public Tinsoft(string api_key, int location = 0)
        {
            this.api_key = api_key;
            this.proxy = "";
            this.ip = "";
            this.port = 0;
            this.timeout = 0;
            this.next_change = 0;
            this.location = location;
        }

        public bool changeProxy()
        {
            bool flag;
            if (!this.checkLastRequest())
            {
                this.errorCode = "Request so fast!";
            }
            else
            {
                this.errorCode = "";
                this.next_change = 0;
                this.proxy = "";
                this.ip = "";
                this.port = 0;
                this.timeout = 0;
                string[] apiKey = new string[] { this.svUrl, "/api/changeProxy.php?key=", this.api_key, "&location=", null };
                apiKey[4] = this.location.ToString();
                string sVContent = this.getSVContent(string.Concat(apiKey));
                if (sVContent == "")
                {
                    this.errorCode = "request server timeout!";
                }
                else
                {
                    try
                    {
                        JObject jObjects = JObject.Parse(sVContent);
                        if (!bool.Parse(jObjects["success"].ToString()))
                        {
                            this.errorCode = jObjects["description"].ToString();
                        }
                        else
                        {
                            this.proxy = jObjects["proxy"].ToString();
                            string[] strArrays = this.proxy.Split(new char[] { ':' });
                            this.ip = strArrays[0];
                            this.port = int.Parse(strArrays[1]);
                            this.timeout = int.Parse(jObjects["timeout"].ToString());
                            this.next_change = int.Parse(jObjects["next_change"].ToString());
                            this.errorCode = "";
                            flag = true;
                            return flag;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            flag = false;
            return flag;
        }

        private bool checkLastRequest()
        {
            bool flag;
            try
            {
                DateTime dateTime = new DateTime(2001, 1, 1);
                long ticks = DateTime.Now.Ticks - dateTime.Ticks;
                int totalSeconds = (int)(new TimeSpan(ticks)).TotalSeconds;
                if (totalSeconds - this.lastRequest >= 10)
                {
                    this.lastRequest = totalSeconds;
                    flag = true;
                    return flag;
                }
            }
            catch
            {
            }
            flag = false;
            return flag;
        }

        public static List<string> GetListKey(string api_user)
        {
            List<string> strs = new List<string>();
            try
            {
                RequestXNet requestXNet = new RequestXNet("", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)", "", 0);
                string str = requestXNet.RequestGet(string.Concat("http://proxy.tinsoftsv.com/api/getUserKeys.php?key=", api_user));
                foreach (JToken item in (IEnumerable<JToken>)JObject.Parse(str)["data"])
                {
                    if (!Convert.ToBoolean(item["success"].ToString()))
                    {
                        continue;
                    }
                    strs.Add(item["key"].ToString());
                }
            }
            catch
            {
            }
            return strs;
        }

        public static string GetTinsoftProxy (string api)
        {
            string result = "";

            string[] apiKey = new string[] { "http://proxy.tinsoftsv.com", "/api/changeProxy.php?key=", api, "&location=", null };
            apiKey[4] = "0";
            string sVContent = Tinsoft.getSVContent2(string.Concat(apiKey));

            string next_change_temp = Regex.Match(JObject.Parse(sVContent)["next_change"].ToString(), "\\d+").Value;
            int next_change = next_change_temp == "" ? 0 : int.Parse(next_change_temp);

            if (sVContent == "")
            {
                Common.DelayTime(5);
            }
            else
            {
                if (sVContent.Contains("for next change"))
                {
                    Common.DelayTime(next_change);
                    return "";
                }

                try
                {
                    JObject jObjects = JObject.Parse(sVContent);
                    if (!bool.Parse(jObjects["success"].ToString()))
                    {
                        // Lỗi
                        result = "";
                    }
                    else
                    {
                        result = jObjects["proxy"].ToString();
                        if (result != "")
                        {
                            return jObjects["proxy"].ToString();
                        }
                        else
                        {
                            Common.DelayTime(5);
                        }
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        public bool getProxyStatus()
        {
            bool flag;
            if (!this.checkLastRequest())
            {
                this.errorCode = "Request so fast!";
            }
            else
            {
                this.errorCode = "";
                this.proxy = "";
                this.ip = "";
                this.port = 0;
                this.timeout = 0;
                string sVContent = this.getSVContent(string.Concat(this.svUrl, "/api/getProxy.php?key=", this.api_key));
                if (sVContent != "")
                {
                    try
                    {
                        JObject jObjects = JObject.Parse(sVContent);
                        if (!bool.Parse(jObjects["success"].ToString()))
                        {
                            this.errorCode = jObjects["description"].ToString();
                        }
                        else
                        {
                            this.proxy = jObjects["proxy"].ToString();
                            string[] strArrays = this.proxy.Split(new char[] { ':' });
                            this.ip = strArrays[0];
                            this.port = int.Parse(strArrays[1]);
                            this.timeout = int.Parse(jObjects["timeout"].ToString());
                            this.next_change = int.Parse(jObjects["next_change"].ToString());
                            this.errorCode = "";
                            flag = true;
                            return flag;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            flag = false;
            return flag;
        }

        private static string getSVContent2(string url)
        {
            //Console.WriteLine(url);
            string str = "";
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    str = webClient.DownloadString(url);
                }
                if (string.IsNullOrEmpty(str))
                {
                    str = "";
                }
            }
            catch
            {
                str = "";
            }
            return str;
        }

        private string getSVContent(string url)
        {
            Console.WriteLine(url);
            string str = "";
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    str = webClient.DownloadString(url);
                }
                if (string.IsNullOrEmpty(str))
                {
                    str = "";
                }
            }
            catch
            {
                str = "";
            }
            return str;
        }

        public void stopProxy()
        {
            this.errorCode = "";
            this.proxy = "";
            this.ip = "";
            this.port = 0;
            this.timeout = 0;
            if (this.api_key != "")
            {
                this.getSVContent(string.Concat(this.svUrl, "/api/stopProxy.php?key=", this.api_key));
            }
        }
    }
}