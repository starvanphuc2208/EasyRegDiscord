using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

namespace MCommon
{
    internal class TinsoftProxy
    {
        private object k1 = new object();

        private object k = new object();

        public string errorCode = "";

        private string svUrl = "http://proxy.tinsoftsv.com";

        private int lastRequest = 0;

        public bool canChangeIP = true;

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

        public TinsoftProxy(string api_key, int limit_theads_use, int location = 0)
        {
            this.api_key = api_key;
            this.proxy = "";
            this.ip = "";
            this.port = 0;
            this.timeout = 0;
            this.next_change = 0;
            this.location = location;
            this.limit_theads_use = limit_theads_use;
            this.dangSuDung = 0;
            this.daSuDung = 0;
            this.canChangeIP = true;
        }

        public bool ChangeProxy()
        {
            bool flag;
            lock (this.k)
            {
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
                    string sVContent = this.getSVContent(string.Concat(new object[] { this.svUrl, "/api/changeProxy.php?key=", this.api_key, "&location=", this.location }));
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
                                this.next_change = int.Parse(jObjects["next_change"].ToString());
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
            }
            return flag;
        }

        public static bool CheckApiProxy(string apiProxy)
        {
            bool flag;
            string sVContent = TinsoftProxy.GetSVContent(string.Concat("http://proxy.tinsoftsv.com/api/getKeyInfo.php?key=", apiProxy));
            flag = (!(sVContent != "") || !bool.Parse(JObject.Parse(sVContent)["success"].ToString()) ? false : true);
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

        public bool CheckStatusProxy()
        {
            bool flag;
            lock (this.k)
            {
                this.errorCode = "";
                this.next_change = 0;
                this.proxy = "";
                this.ip = "";
                this.port = 0;
                this.timeout = 0;
                string sVContent = this.getSVContent(string.Concat(new object[] { this.svUrl, "/api/getProxy.php?key=", this.api_key }));
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
                            if (jObjects["next_change"] != null)
                            {
                                this.next_change = int.Parse(jObjects["next_change"].ToString());
                            }
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
                    catch (Exception exception)
                    {
                    }
                }
                flag = false;
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

        public static List<string> GetListKey(string api_user)
        {
            List<string> strs = new List<string>();
            try
            {
                RequestHttp requestXNet = new RequestHttp("", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)", "", 0);
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

        public int GetNextChange()
        {
            while (!this.CheckStatusProxy())
            {
            }
            return this.next_change;
        }

        public string GetProxy()
        {
            while (!this.CheckStatusProxy())
            {
            }
            return this.proxy;
        }

        private string getSVContent(string url)
        {
            Console.WriteLine(url);
            string str = "";
            try
            {
                using (WebClient webClient = new WebClient())
                {
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

        private static string GetSVContent(string url)
        {
            Console.WriteLine(url);
            string str = "";
            try
            {
                using (WebClient webClient = new WebClient())
                {
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

        public int GetTimeOut()
        {
            while (!this.CheckStatusProxy())
            {
            }
            return this.timeout;
        }

        public string TryToGetMyIP()
        {
            string str;
            lock (this.k1)
            {
                if (this.dangSuDung == 0)
                {
                    if ((this.daSuDung <= 0 ? false : this.daSuDung < this.limit_theads_use))
                    {
                        if (this.GetTimeOut() < 30 && !this.ChangeProxy())
                        {
                            str = "0";
                            return str;
                        }
                    }
                    else if (!this.ChangeProxy())
                    {
                        str = "0";
                        return str;
                    }
                }
                else if (this.daSuDung >= this.limit_theads_use)
                {
                    str = "2";
                    return str;
                }
                else if (this.GetTimeOut() < 30 && !this.ChangeProxy())
                {
                    str = "0";
                    return str;
                }
                this.daSuDung++;
                this.dangSuDung++;
                str = "1";
            }
            return str;
        }
    }
}