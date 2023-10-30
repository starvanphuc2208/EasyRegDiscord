using HtmlAgilityPack;
using Newtonsoft.Json;
using Starksoft.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace easy.MCommon
{
    public class Tlmix
    {
        public static TcpClient TcpHandlerManage(string address, int port)
        {
            TcpClient tcpClient1;
            if (!string.IsNullOrEmpty(DataManage.ProxyAddress))
            {
                tcpClient1 = new HttpProxyClient(DataManage.ProxyAddress, Convert.ToInt32(DataManage.ProxyPort)).CreateConnection(address, port);
            }
            else
            {
                TcpClient tcpClient2 = new TcpClient();
                tcpClient2.Connect(address, port);
                tcpClient1 = tcpClient2;
            }
            return tcpClient1;
        }

        public static async Task<string> GetLoginCodeWeb(TelegramClient telegramClient)
        {
            try
            {
                TLAbsDialogs tlAbsDialogs = await telegramClient.GetUserDialogsAsync();
                TLDialogs tLAbsDialogs = (TLDialogs)tlAbsDialogs;
                tlAbsDialogs = (TLAbsDialogs)null;
                TLUser telegramService = tLAbsDialogs.Users.ToList<TLAbsUser>().OfType<TLUser>().FirstOrDefault<TLUser>((Func<TLUser, bool>)(c => c.Phone == "42777"));
                TLInputPeerUser peerTelegramService = new TLInputPeerUser()
                {
                    UserId = telegramService.Id,
                    AccessHash = telegramService.AccessHash.Value
                };
                TLAbsMessages tlAbsMessages1 = await telegramClient.GetHistoryAsync((TLAbsInputPeer)peerTelegramService, maxId: -1);
                TLMessages tlAbsMessages2 = (TLMessages)tlAbsMessages1;
                tlAbsMessages1 = (TLAbsMessages)null;
                TLMessage lastTLMessage = tlAbsMessages2.Messages.ToList<TLAbsMessage>().OfType<TLMessage>().FirstOrDefault<TLMessage>((Func<TLMessage, bool>)(message => message.Message.Contains("my.telegram.org")));
                if (lastTLMessage == null)
                    return (string)null;
                string lastMessage = lastTLMessage.Message;
                lastMessage = lastMessage.Substring(lastMessage.IndexOf(":"));
                lastMessage = lastMessage.Replace(":", "");
                lastMessage = lastMessage.Trim();
                lastMessage = lastMessage.Substring(0, lastMessage.IndexOf("\n"));
                return lastMessage;
            }
            catch (Exception ex)
            {
                return (string)null;
            }
        }

        public static async Task<string> SendCodeMyTelegramAsync(string phone, WebProxy proxy = null)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                if (proxy != null)
                {
                    handler.Proxy = (IWebProxy)proxy;
                    handler.UseProxy = true;
                }
                string url = "https://my.telegram.org/auth/send_password?phone=" + phone;
                HttpClient client = new HttpClient();
                string responseString = await client.GetStringAsync(url);
                if (!responseString.Contains("random_hash"))
                    return responseString;
                string randomHash = Regex.Match(responseString, "random_hash\":\"(.*?)\"").Groups[1].Value.Trim();
                return randomHash;
            }
            catch (Exception ex)
            {
                return (string)null;
            }
        }

        public static async Task<CookieCollection> GetCookieCollectionMyTelegramAsync(
          string phoneNumber,
          string randomHash,
          string loginCode,
          WebProxy proxy = null)
        {
            try
            {
                CookieContainer cookieContainer = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                if (proxy != null)
                {
                    handler.Proxy = (IWebProxy)proxy;
                    handler.UseProxy = true;
                }
                handler.CookieContainer = cookieContainer;
                string url = string.Format("https://my.telegram.org/auth/login?phone={0}&random_hash={1}&password={2}", phoneNumber, randomHash, loginCode);
                HttpClient client = new HttpClient((HttpMessageHandler)handler);
                string responseString = await client.GetStringAsync(url);
                if (responseString != "true")
                    return (CookieCollection)null;
                Uri uri = new Uri(url);
                IEnumerable<Cookie> responseCookies = cookieContainer.GetCookies(uri).Cast<Cookie>();
                CookieCollection cookieCollection = new CookieCollection();
                foreach (Cookie cookie1 in responseCookies)
                {
                    Cookie cookie = cookie1;
                    cookieCollection.Add(cookie);
                    cookie = (Cookie)null;
                }
                return cookieCollection;
            }
            catch (Exception ex)
            {
                return (CookieCollection)null;
            }
        }

        public static async Task<KeyValuePair<string, string>> GetAppInfoMyTelegramAsync(
          CookieCollection cookieCollection,
          WebProxy proxy = null)
        {
            try
            {
                CookieContainer cookieContainer = new CookieContainer();
                foreach (Cookie cookie1 in cookieCollection)
                {
                    Cookie cookie = cookie1;
                    cookieContainer.Add(cookie);
                    cookie = (Cookie)null;
                }
                HttpClientHandler handler = new HttpClientHandler();
                if (proxy != null)
                {
                    handler.Proxy = (IWebProxy)proxy;
                    handler.UseProxy = true;
                }
                handler.CookieContainer = cookieContainer;
                string url = "https://my.telegram.org/apps";
                HttpClient client = new HttpClient((HttpMessageHandler)handler);
                string responseString = await client.GetStringAsync(url);
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(responseString);
                HtmlNodeCollection nodeApiId = htmlDocument.DocumentNode.SelectNodes("//*[@onclick= 'this.select();']");
                HtmlNodeCollection nodeApiHash = htmlDocument.DocumentNode.SelectNodes("//*[@onclick= 'this.select();']");
                if (nodeApiId == null)
                {
                    HtmlNode nodeHash = htmlDocument.DocumentNode.SelectNodes("//*[@name= 'hash']").First<HtmlNode>();
                    string hash = nodeHash.Attributes["value"].Value.ToString();
                    return new KeyValuePair<string, string>("hash", hash);
                }
                string ApiId = nodeApiId.First<HtmlNode>().InnerText;
                string ApiHash = nodeApiHash.Last<HtmlNode>().InnerText;
                return new KeyValuePair<string, string>(ApiId, ApiHash);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<string, string>();
            }
        }

        public static async Task<string> CreateAppMyTelegramAsync(
          CookieCollection cookieCollection,
          string hash,
          WebProxy proxy = null)
        {
            try
            {
                CookieContainer cookieContainer = new CookieContainer();
                foreach (Cookie cookie1 in cookieCollection)
                {
                    Cookie cookie = cookie1;
                    cookieContainer.Add(cookie);
                    cookie = (Cookie)null;
                }
                HttpClientHandler handler = new HttpClientHandler();
                if (proxy != null)
                {
                    handler.Proxy = (IWebProxy)proxy;
                    handler.UseProxy = true;
                }
                handler.CookieContainer = cookieContainer;
                string url = string.Format("https://my.telegram.org/apps/create?hash={0}&app_title={1}&app_shortname={2}&app_url&app_platform=desktop&app_desc", hash, RandomStr.RandomStringStr(10), RandomStr.RandomNumberStr(5));
                HttpClient client = new HttpClient((HttpMessageHandler)handler);
                string responseString = await client.GetStringAsync(url);

                url = "https://my.telegram.org/apps";
                responseString = await client.GetStringAsync(url);

                return !string.IsNullOrEmpty(responseString) ? responseString : "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
