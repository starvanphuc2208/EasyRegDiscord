using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keys = OpenQA.Selenium.Keys;

namespace MCommon
{
    public class Chrome
    {
        public Process process { get; set; }
        public ChromeDriver chrome { get; set; }
        /// <summary>
        /// Hide Browser(default: false)
        /// </summary>
        public bool HideBrowser { get; set; }
        public bool Incognito { get; set; }
        /// <summary>
        /// Disable Image(default: false)
        /// </summary>
        public bool DisableImage { get; set; }
        /// <summary>
        /// Disable Sound(default: false)
        /// </summary>
        public bool DisableSound { get; set; }
        public bool AutoPlayVideo { get; set; }
        /// <summary>
        /// UserAgent(default: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36)
        /// </summary>
        public string UserAgent { get; set; }
        public int PixelRatio { get; set; }
        public string ProfilePath { get; set; }
        /// <summary>
        /// Browser Size(default: (300,300))
        /// </summary>
        public Point Size { get; set; }
        /// <summary>
        /// Browser Heigh(default: 300)
        /// </summary>
        //public int Size_Heigh { get; set; }
        ///// <summary>
        ///// Browser Width(default: 300)
        ///// </summary>
        //public int Size_Width { get; set; }
        ///// <summary>
        ///// Browser Position(default: (0,0))
        ///// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// Browser PositionX(default: 0)
        /// </summary>
        //public int Position_X { get; set; }
        ///// <summary>
        ///// Browser PositionY(default: 0)
        ///// </summary>
        //public int Position_Y { get; set; }

        /// <summary>
        /// Time Wait For Searching Element (seconds - default: 0)
        /// </summary>
        public int TimeWaitForSearchingElement { get; set; }
        /// <summary>
        /// Time Wait For Loading Page (minutes - default: 5)
        /// </summary>
        public int TimeWaitForLoadingPage { get; set; }
        /// <summary>
        /// Socks5 proxy or Port 911
        /// </summary>
        public string Proxy { get; set; }
        public int TypeProxy { get; set; }
        public string App { get; set; }
        public string LinkToOtherBrowser { get; set; }
        public string PathExtension { get; set; }
        public Chrome()
        {
            HideBrowser = false;
            DisableImage = false;
            DisableSound = false;
            Incognito = false;
            UserAgent = "";
            ProfilePath = "";
            Size = new Point(300, 300);
            Size = new Point(Size.X, Size.Y);
            Proxy = "";
            TypeProxy = 0;
            Position = new Point(Position.X, Position.Y);
            TimeWaitForSearchingElement = 0;
            TimeWaitForLoadingPage = 5;
            App = "";
            AutoPlayVideo = false;
            LinkToOtherBrowser = "";
            PathExtension = "data\\extension";
        }

        //public Chrome(bool hideBrowser = false, bool disableImage = false, bool disableSound = false, string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36", 
        //    string profilePath = "", Point size = default(Point), Point position = default(Point), string proxy = "", int timeWaitForSearchingElement = 0, int timeWaitForLoadingPage = 5,string app = "https://facebook.com")
        //{
        //    this.HideBrowser = hideBrowser;
        //    this.DisableImage = disableImage;
        //    this.DisableSound = disableSound;
        //    this.UserAgent = userAgent;
        //    this.ProfilePath = profilePath;
        //    this.Size = size;
        //    this.Position = position;
        //    this.Proxy = proxy;
        //    this.TimeWaitForSearchingElement = timeWaitForSearchingElement;
        //    this.TimeWaitForLoadingPage = timeWaitForLoadingPage;
        //    this.App = app;
        //}
        public bool Open()
        {
            bool isSuccess = false;
            try
            {
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                ChromeOptions options = new ChromeOptions();
                options.AddArguments(new string[] {
                    //ninja
                    "--disable-3d-apis",
                    "--disable-background-networking",
                    "--disable-bundled-ppapi-flash",
                    "--disable-client-side-phishing-detection",
                    "--disable-default-apps",
                    "--disable-hang-monitor",
                    "--disable-prompt-on-repost",
                    "--disable-sync",
                    "--disable-webgl",
                    "--enable-blink-features=ShadowDOMV0",
                    "--enable-logging",
                    //end-ninja
                    //fplus
                    //"--disable-web-resources",
                    //"--force-fieldtrials=SiteIsolationExtensions/Control",
                    //end fplus
                    "--disable-notifications",
                    "--window-size="+Size.X+","+Size.Y,
                    "--window-position="+Position.X+","+Position.Y,
                    "--no-sandbox",
                    "--disable-gpu",// applicable to windows os only
                    "--disable-dev-shm-usage",//overcome limited resource problems       
                    "--disable-web-security",
                    "--disable-rtc-smoothness-algorithm",
                    "--disable-webrtc-hw-decoding",
                    "--disable-webrtc-hw-encoding",
                    "--disable-webrtc-multiple-routes",
                    "--disable-webrtc-hw-vp8-encoding",
                    "--enforce-webrtc-ip-permission-check",
                    "--force-webrtc-ip-handling-policy",
                    "--ignore-certificate-errors",
                    "--disable-infobars",
                    "--disable-blink-features=\"BlockCredentialedSubresources\"",
                    "--disable-popup-blocking"
                });
                //options.AddUserProfilePreference("profile.default_content_setting_values.plugins", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.popups", 0);
                options.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 0);
                //options.AddUserProfilePreference("profile.default_content_setting_values.auto_select_certificate", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.mixed_script", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.media_stream", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.media_stream_mic", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.media_stream_camera", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.protocol_handlers", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.midi_sysex", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.push_messaging", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.ssl_cert_decisions", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.metro_switch_to_desktop", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.protected_media_identifier", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.site_engagement", 1);
                //options.AddUserProfilePreference("profile.default_content_setting_values.durable_storage", 1);
                //options.AddUserProfilePreference("useAutomationExtension", true);

                if (DisableSound)
                    options.AddArguments("--mute-audio");


                if (!HideBrowser)
                {
                    if (DisableImage)
                        options.AddArguments("--blink-settings=imagesEnabled=false");

                    if (!string.IsNullOrEmpty(ProfilePath.Trim()))
                        options.AddArguments("--user-data-dir=" + ProfilePath);
                }
                else
                {
                    options.AddArguments("--blink-settings=imagesEnabled=false");
                    options.AddArguments("--headless");
                }

                if (Incognito)
                {
                    options.AddArguments("--incognito");
                }

                if (!string.IsNullOrEmpty(Proxy.Trim()))
                {
                    int dem = Proxy.Split(':').Count();
                    switch (dem)
                    {
                        case 1:
                            if (TypeProxy == 0)
                                options.AddArguments("--proxy-server= 127.0.0.1:" + Proxy);
                            else
                                options.AddArguments("--proxy-server= socks5://127.0.0.1:" + Proxy);
                            break;
                        case 2:
                            if (TypeProxy == 0)
                                options.AddArguments("--proxy-server= " + Proxy);
                            else
                                options.AddArguments("--proxy-server= socks5://" + Proxy);
                            break;
                        case 4:
                            if (TypeProxy == 0)
                            {
                                //options.AddExtension(@"extension\Proxy Auto Auth.crx");
                                //options.AddArguments(string.Format("--proxy-server={0}:{1}", Proxy.Split(':')[0], Proxy.Split(':')[1]));
                                options.AddArguments("--proxy-server= " + Proxy.Split(':')[0] + ":" + Proxy.Split(':')[1]);
                                options.AddExtension("extension\\proxy.crx");
                            }
                            else
                            {
                                //options.AddExtension(@"extension\Proxy Auto Auth.crx");
                                //options.AddArguments(string.Format("--proxy-server=socks5://{0}:{1}", Proxy.Split(':')[0], Proxy.Split(':')[1]));
                                options.AddArguments("--proxy-server= socks5://" + Proxy.Split(':')[0] + ":" + Proxy.Split(':')[1]);
                                options.AddExtension("extension\\proxy.crx");
                            }
                            break;
                        default:
                            break;
                    }
                }

                bool isHaveExtension = false;
                if (!HideBrowser && PathExtension.Trim() != "")
                {
                    if (!Directory.Exists(PathExtension))
                        Directory.CreateDirectory(PathExtension);
                    string[] paths = Directory.GetFiles(PathExtension);

                    if (paths.Length > 0)
                        isHaveExtension = true;

                    for (int i = 0; i < paths.Length; i++)
                        options.AddExtension(paths[i]);
                }

                if (!isHaveExtension)
                {
                    if (!string.IsNullOrEmpty(App.Trim()))
                    {
                        options.AddArguments("--app= " + App);
                    }
                }


                //Tùng
                if (UserAgent != "")
                {
                    if (Proxy.Split(':').Count() == 4)
                        options.AddArguments($"--user-agent={UserAgent}$PC${Proxy.Split(':')[2] + ":" + Proxy.Split(':')[3]}");
                    else
                        options.AddArguments($"--user-agent={UserAgent}");
                }

                if (AutoPlayVideo)
                {
                    options.AddArguments("--autoplay-policy=no-user-gesture-required");
                }

                if (LinkToOtherBrowser != "" && File.Exists(LinkToOtherBrowser))
                {
                    options.AddArguments("disable-infobars");
                    options.BinaryLocation = LinkToOtherBrowser;
                }

                chrome = new ChromeDriver(service, options);

                chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TimeWaitForSearchingElement);
                chrome.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(TimeWaitForLoadingPage);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                ExportError(null, ex, "chrome.Open()");
            }
            return isSuccess;
        }

        public bool CheckIsLive()
        {
            try
            {
                if (process != null)
                    return !process.HasExited;
                else
                    return !CheckChromeClosed();
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="querySelector"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns>-2: chrome closed</returns>
        public string GetCssSelector(string querySelector, string attributeName, string attributeValue)
        {
            string output = "";

            if (!CheckIsLive())
            {
                return "-2";
            }
            else
            {
                try
                {
                    output = chrome.ExecuteScript("function GetSelector(el){let path=[],parent;while(parent=el.parentNode){path.unshift(`${el.tagName}:nth-child(${[].indexOf.call(parent.children, el)+1})`);el=parent}return `${path.join('>')}`.toLowerCase()}; function GetCssSelector(selector, attribute, value){var c = document.querySelectorAll(selector); for (i = 0; i < c.length; i++) { if (c[i].getAttribute(attribute)!=null && c[i].getAttribute(attribute).includes(value)) { return GetSelector(c[i])} }; return '';}; return GetCssSelector('" + querySelector + "','" + attributeName + "','" + attributeValue + "')").ToString();
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.GetCssSelector({querySelector},{attributeName},{attributeValue})");
                }
            }
            return output;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="querySelector"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns>-2: chrome closed</returns>
        public string GetCssSelector(string querySelector)
        {
            string output = "";

            if (!CheckIsLive())
            {
                return "-2";
            }
            else
            {
                try
                {
                    output = chrome.ExecuteScript("function GetSelector(el){let path=[],parent;while(parent=el.parentNode){path.unshift(`${el.tagName}:nth-child(${[].indexOf.call(parent.children, el)+1})`);el=parent}return `${path.join('>')}`.toLowerCase()}; return GetSelector(" + querySelector + ")").ToString();
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.GetCssSelector({querySelector})");
                }
            }
            return output;
        }
        public string GetAttributeValue(string querySelector, string attributeName)
        {
            string output = "";
            if (!CheckIsLive())
            {
                return "-2";
            }
            else
            {
                try
                {
                    output = chrome.ExecuteScript("return document.querySelector('" + querySelector + "').getAttribute('" + attributeName + "')").ToString();
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.GetAttributeValue({querySelector},{attributeName})");
                }
            }
            return output;
        }

        /// <summary>
        /// 1-can scroll, 2-can not scroll
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public int ScrollSmooth(int distance)
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    int check = Convert.ToInt32(chrome.ExecuteScript("return document.querySelector('html').getBoundingClientRect().y+''").ToString());
                    chrome.ExecuteScript("window.scrollBy({ top: " + distance + ",behavior: 'smooth'});");
                    DelayTime(0.1);
                    if (check == Convert.ToInt32(chrome.ExecuteScript("return document.querySelector('html').getBoundingClientRect().y+''").ToString()))
                        return 2;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.ScrollSmooth({distance})");
                }
                return 1;
            }
        }


        public string GetUseragent()
        {
            string ua = "";
            if (!CheckIsLive())
            {
                return "-2";
            }
            else
            {
                try
                {
                    ua = chrome.ExecuteScript("return navigator.userAgent").ToString();
                }
                catch
                {
                }
            }
            return ua;
        }
        public int SendKeyDown(int typeAttribute, string attributeValue)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementById(attributeValue).SendKeys(Keys.ArrowDown);
                            break;
                        case 2:
                            chrome.FindElementByName(attributeValue).SendKeys(Keys.ArrowDown);
                            break;
                        case 3:
                            chrome.FindElementByXPath(attributeValue).SendKeys(Keys.ArrowDown);
                            break;
                        case 4:
                            chrome.FindElementByCssSelector(attributeValue).SendKeys(Keys.ArrowDown);
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendKeyDown({typeAttribute},{attributeValue})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public string GetURL()
        {
            if (!CheckIsLive())
            {
                return "-2";
            }
            else
            {
                try
                {
                    return chrome.Url;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, "chrome.GetURL()");
                }
            }
            return "";
        }
        public int GotoURL(string url)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.Navigate().GoToUrl(url);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.GotoURL({url})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int GotoURLIfNotExist(string url)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (GetURL() != url)
                        chrome.Navigate().GoToUrl(url);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.GotoURL({url})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int Refresh()
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.Navigate().Refresh();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.Refresh()");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int GotoBackPage()
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.Navigate().Back();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.GotoBackPage()");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        
        public object ExecuteScript(string script)
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    return chrome.ExecuteScript(script);
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.ExecuteScript({script})");
                }
            }
            return "";
        }
        public int Click(int typeAttribute, string attributeValue, int index = 0, int subTypeAttribute = 0, string subAttributeValue = "", int subIndex = 0)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (subTypeAttribute == 0)
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                chrome.FindElementsById(attributeValue)[index].Click();
                                break;
                            case 2:
                                chrome.FindElementsByName(attributeValue)[index].Click();
                                break;
                            case 3:
                                chrome.FindElementsByXPath(attributeValue)[index].Click();
                                break;
                            case 4:
                                chrome.FindElementsByCssSelector(attributeValue)[index].Click();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                chrome.FindElementsById(attributeValue)[index].FindElements(By.Id(subAttributeValue))[subIndex].Click();
                                break;
                            case 2:
                                chrome.FindElementsByName(attributeValue)[index].FindElements(By.Name(subAttributeValue))[subIndex].Click();
                                break;
                            case 3:
                                chrome.FindElementsByXPath(attributeValue)[index].FindElements(By.XPath(subAttributeValue))[subIndex].Click();
                                break;
                            case 4:
                                chrome.FindElementsByCssSelector(attributeValue)[index].FindElements(By.CssSelector(subAttributeValue))[subIndex].Click();
                                break;
                            default:
                                break;
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.Click({typeAttribute},{attributeValue})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }

        public int ClickWithAction(int typeAttribute, string attributeValue, int index = 0, int subTypeAttribute = 0, string subAttributeValue = "", int subIndex = 0)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (subTypeAttribute != 0)
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                new Actions(chrome).Click(chrome.FindElementsById(attributeValue)[index].FindElements(By.Id(subAttributeValue))[subIndex]).Perform();
                                break;
                            case 2:
                                new Actions(chrome).Click(chrome.FindElementsByName(attributeValue)[index].FindElements(By.Name(subAttributeValue))[subIndex]).Perform();
                                break;
                            case 3:
                                new Actions(chrome).Click(chrome.FindElementsByXPath(attributeValue)[index].FindElements(By.XPath(subAttributeValue))[subIndex]).Perform();
                                break;
                            case 4:
                                new Actions(chrome).Click(chrome.FindElementsByCssSelector(attributeValue)[index].FindElements(By.CssSelector(subAttributeValue))[subIndex]).Perform();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                new Actions(chrome).Click(chrome.FindElementsById(attributeValue)[index]).Perform();
                                break;
                            case 2:
                                new Actions(chrome).Click(chrome.FindElementsByName(attributeValue)[index]).Perform();
                                break;
                            case 3:
                                new Actions(chrome).Click(chrome.FindElementsByXPath(attributeValue)[index]).Perform();
                                break;
                            case 4:
                                new Actions(chrome).Click(chrome.FindElementsByCssSelector(attributeValue)[index]).Perform();
                                break;
                            default:
                                break;
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.ClickWithAction({typeAttribute},{attributeValue})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }


        #region SendKey
        /// <summary>
        /// Nhanh
        /// </summary>
        /// <param name="typeAttribute"></param>
        /// <param name="attributeValue"></param>
        /// <param name="content"></param>
        /// <param name="isClick"></param>
        /// <returns></returns>
        public int SendKeys(int typeAttribute, string attributeValue, string content, bool isClick = true)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (isClick)
                        Click(typeAttribute, attributeValue);
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementById(attributeValue).SendKeys(content);
                            break;
                        case 2:
                            chrome.FindElementByName(attributeValue).SendKeys(content);
                            break;
                        case 3:
                            chrome.FindElementByXPath(attributeValue).SendKeys(content);
                            break;
                        case 4:
                            chrome.FindElementByCssSelector(attributeValue).SendKeys(content);
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendKeys({typeAttribute},{attributeValue},{content},{isClick})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }

        /// <summary>
        /// Nhanh
        /// </summary>
        /// <param name="typeAttribute"></param>
        /// <param name="attributeValue"></param>
        /// <param name="content"></param>
        /// <param name="isClick"></param>
        /// <returns></returns>
        public int SendKeys(int typeAttribute, string attributeValue, int index, string content, bool isClick = true)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (isClick)
                        Click(typeAttribute, attributeValue);
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementsById(attributeValue)[index].SendKeys(content);
                            break;
                        case 2:
                            chrome.FindElementsByName(attributeValue)[index].SendKeys(content);
                            break;
                        case 3:
                            chrome.FindElementsByXPath(attributeValue)[index].SendKeys(content);
                            break;
                        case 4:
                            chrome.FindElementsByCssSelector(attributeValue)[index].SendKeys(content);
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendKeys({typeAttribute},{attributeValue},{content},{isClick})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }

        /// <summary>
        /// Bình thường
        /// </summary>
        /// <param name="typeAttribute"></param>
        /// <param name="attributeValue"></param>
        /// <param name="content"></param>
        /// <param name="timeDelay_Second"></param>
        /// <param name="isClick"></param>
        /// <param name="timeDelayAfterClick"></param>
        /// <returns></returns>
        public int SendKeys(int typeAttribute, string attributeValue, string content, double timeDelay_Second, bool isClick = true, double timeDelayAfterClick = 0.1)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (isClick)
                    {
                        Click(typeAttribute, attributeValue);
                        Thread.Sleep(Convert.ToInt32(timeDelayAfterClick * 1000));
                    }

                    for (int i = 0; i < content.Length; i++)
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                chrome.FindElementById(attributeValue).SendKeys(content[i].ToString());
                                break;
                            case 2:
                                chrome.FindElementByName(attributeValue).SendKeys(content[i].ToString());
                                break;
                            case 3:
                                chrome.FindElementByXPath(attributeValue).SendKeys(content[i].ToString());
                                break;
                            case 4:
                                chrome.FindElementByCssSelector(attributeValue).SendKeys(content[i].ToString());
                                break;
                            default:
                                break;
                        }

                        if (timeDelay_Second > 0)
                        {
                            int temp = Convert.ToInt32(timeDelay_Second * 1000);
                            if (temp < 100) temp = 100;
                            Thread.Sleep(Base.rd.Next(temp, temp + 50));
                            //Thread.Sleep(Base.rd.Next(temp, temp + 200));
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendKeys({typeAttribute},{attributeValue},{content},{timeDelay_Second},{isClick})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }

        /// <summary>
        /// Bình thường
        /// </summary>
        /// <param name="typeAttribute"></param>
        /// <param name="attributeValue"></param>
        /// <param name="content"></param>
        /// <param name="timeDelay_Second"></param>
        /// <param name="isClick"></param>
        /// <param name="timeDelayAfterClick"></param>
        /// <returns></returns>
        public int SendKeys(int typeAttribute, string attributeValue, int index, string content, double timeDelay_Second, bool isClick = true, int timeDelayAfterClick = 0)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (isClick)
                    {
                        Click(typeAttribute, attributeValue);
                        Thread.Sleep(Convert.ToInt32(timeDelayAfterClick * 1000));
                    }

                    for (int i = 0; i < content.Length; i++)
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                chrome.FindElementsById(attributeValue)[index].SendKeys(content[i].ToString());
                                break;
                            case 2:
                                chrome.FindElementsByName(attributeValue)[index].SendKeys(content[i].ToString());
                                break;
                            case 3:
                                chrome.FindElementsByXPath(attributeValue)[index].SendKeys(content[i].ToString());
                                break;
                            case 4:
                                chrome.FindElementsByCssSelector(attributeValue)[index].SendKeys(content[i].ToString());
                                break;
                            default:
                                break;
                        }

                        if (timeDelay_Second > 0)
                        {
                            int temp = Convert.ToInt32(timeDelay_Second * 1000);
                            if (temp < 100) temp = 100;
                            Thread.Sleep(Base.rd.Next(temp, temp + 50));
                            //Thread.Sleep(Base.rd.Next(temp, temp + 200));
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendKeys({typeAttribute},{attributeValue},{content},{timeDelay_Second},{isClick})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }

        /// <summary>
        /// Chậm
        /// </summary>
        /// <param name="rd"></param>
        /// <param name="typeAttribute"></param>
        /// <param name="attributeValue"></param>
        /// <param name="content"></param>
        /// <param name="timeDelay_Second"></param>
        /// <param name="isClick"></param>
        /// <param name="timeDelayAfterClick"></param>
        /// <returns></returns>
        public int SendKeys(Random rd, int typeAttribute, string attributeValue, string content, double timeDelay_Second, bool isClick = true, int timeDelayAfterClick = 0)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (isClick)
                    {
                        Click(typeAttribute, attributeValue);
                        Thread.Sleep(Convert.ToInt32(timeDelayAfterClick * 1000));
                    }

                    int lengRandom = 0;
                    int iRandom = rd.Next(1, 1000) % 3;

                    if (content.Length < 3)
                        iRandom = 2;
                    else
                        lengRandom = rd.Next(1, content.Length * 3 / 4);
                    switch (iRandom)
                    {
                        case 0:
                            string textNhap = content.Substring(0, lengRandom);

                            //điền
                            SendKeys(typeAttribute, attributeValue, textNhap, Convert.ToDouble(rd.Next(10, 100)) / 1000);
                            DelayTime(rd.Next(1, 3));

                            //xóa
                            int lengRandom1 = rd.Next(1, lengRandom);
                            for (int i = 0; i < lengRandom1; i++)
                            {
                                SendBackspace(typeAttribute, attributeValue);
                                DelayTime(Convert.ToDouble(rd.Next(1000, 2000)) / 10000);
                            }

                            string el = "";
                            switch (typeAttribute)
                            {
                                case 1:
                                    el = "#" + attributeValue;
                                    break;
                                case 2:
                                    el = "[name=\"" + attributeValue + "\"]";
                                    break;
                                case 4:
                                    el = attributeValue;
                                    break;
                                default:
                                    break;
                            }
                            textNhap = content.Substring(chrome.ExecuteScript("return document.querySelector('" + el + "').value+''").ToString().Length);

                            //điền nốt
                            DelayTime(rd.Next(1, 3));
                            SendKeys(typeAttribute, attributeValue, textNhap, Convert.ToDouble(rd.Next(100, 300)) / 1000, false);
                            DelayTime(rd.Next(1, 3));
                            break;
                        case 1:
                            //điền 2 lần
                            string textNhap1 = content.Substring(0, lengRandom);
                            string textNhap2 = content.Substring(lengRandom);

                            SendKeys(typeAttribute, attributeValue, textNhap1, Convert.ToDouble(rd.Next(10, 100)) / 1000);
                            DelayTime(rd.Next(1, 3));
                            SendKeys(typeAttribute, attributeValue, textNhap2, Convert.ToDouble(rd.Next(100, 300)) / 1000, false);
                            DelayTime(rd.Next(1, 3));
                            break;
                        case 2:
                            //Điền 1 phát
                            SendKeys(typeAttribute, attributeValue, content, Convert.ToDouble(rd.Next(100, 200)) / 1000);
                            DelayTime(rd.Next(1, 3));
                            break;
                        default:
                            break;
                    }






                    //for (int i = 0; i < content.Length; i++)
                    //{
                    //    switch (typeAttribute)
                    //    {
                    //        case 1:
                    //            chrome.FindElementById(attributeValue).SendKeys(content[i].ToString());
                    //            break;
                    //        case 2:
                    //            chrome.FindElementByName(attributeValue).SendKeys(content[i].ToString());
                    //            break;
                    //        case 3:
                    //            chrome.FindElementByXPath(attributeValue).SendKeys(content[i].ToString());
                    //            break;
                    //        case 4:
                    //            chrome.FindElementByCssSelector(attributeValue).SendKeys(content[i].ToString());
                    //            break;
                    //        default:
                    //            break;
                    //    }

                    //    Thread.Sleep(Convert.ToInt32(timeDelay_Second * 1000));
                    //}
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendKeys({typeAttribute},{attributeValue},{content},{timeDelay_Second},{isClick})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        #endregion


        public int SendBackspace(int typeAttribute, string attributeValue)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementById(attributeValue).SendKeys(Keys.Backspace);
                            break;
                        case 2:
                            chrome.FindElementByName(attributeValue).SendKeys(Keys.Backspace);
                            break;
                        case 3:
                            chrome.FindElementByXPath(attributeValue).SendKeys(Keys.Backspace);
                            break;
                        case 4:
                            chrome.FindElementByCssSelector(attributeValue).SendKeys(Keys.Backspace);
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendBackspace({typeAttribute},{attributeValue})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public void DelayThaoTacNho(int timeAdd = 0, Random rd = null)
        {
            if (rd == null)
                rd = new Random();
            DelayTime(rd.Next(timeAdd + 1, timeAdd + 4));
        }
        public void DelayRandom(int timeFrom, int timeTo)
        {
            DelayTime(Base.rd.Next(timeFrom, timeTo + 1));
        }

        public int SendEnter(int typeAttribute, string attributeValue, int index)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementsById(attributeValue)[index].SendKeys(Keys.Enter);
                            break;
                        case 2:
                            chrome.FindElementsByTagName(attributeValue)[index].SendKeys(Keys.Enter);
                            break;
                        case 3:
                            chrome.FindElementsByXPath(attributeValue)[index].SendKeys(Keys.Enter);
                            break;
                        case 4:
                            chrome.FindElementsByCssSelector(attributeValue)[index].SendKeys(Keys.Enter);
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendEnter({typeAttribute},{attributeValue},{index})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int PasteContent(int typeAttribute, string attributeValue, int index = 0, bool isClick = true, int timeDelayAfterClick = 0)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (isClick)
                    {
                        Click(typeAttribute, attributeValue);
                        Thread.Sleep(Convert.ToInt32(timeDelayAfterClick * 1000));
                    }

                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementsById(attributeValue)[index].SendKeys(Keys.Control + "v");
                            break;
                        case 2:
                            chrome.FindElementsByName(attributeValue)[index].SendKeys(Keys.Control + "v");
                            break;
                        case 3:
                            chrome.FindElementsByXPath(attributeValue)[index].SendKeys(Keys.Control + "v");
                            break;
                        case 4:
                            chrome.FindElementsByCssSelector(attributeValue)[index].SendKeys(Keys.Control + "v");
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.PasteContent({typeAttribute},{attributeValue},{isClick})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int SelectText(int typeAttribute, string attributeValue)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementById(attributeValue).SendKeys(Keys.Control + "a");
                            break;
                        case 2:
                            chrome.FindElementByName(attributeValue).SendKeys(Keys.Control + "a");
                            break;
                        case 3:
                            chrome.FindElementByXPath(attributeValue).SendKeys(Keys.Control + "a");
                            break;
                        case 4:
                            chrome.FindElementByCssSelector(attributeValue).SendKeys(Keys.Control + "a");
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SelectText({typeAttribute},{attributeValue})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int ClearText(int typeAttribute, string attributeValue)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementById(attributeValue).Clear();
                            break;
                        case 2:
                            chrome.FindElementByName(attributeValue).Clear();
                            break;
                        case 3:
                            chrome.FindElementByXPath(attributeValue).Clear();
                            break;
                        case 4:
                            chrome.FindElementByCssSelector(attributeValue).Clear();
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.ClearText({typeAttribute},{attributeValue})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int CountElement(string querySelector)
        {
            int count = 0;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    count = Convert.ToInt32(chrome.ExecuteScript("return document.querySelectorAll('" + querySelector + "').length+''").ToString());
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.CountElement({querySelector})");
                }
            }
            return count;
        }
        public int CheckExistElement(string querySelector, double timeWait_Second = 0)
        {
            bool isExist = true;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    int timeStart = Environment.TickCount;
                    while ((string)chrome.ExecuteScript("return document.querySelectorAll('" + querySelector + "').length+''") == "0")
                    {
                        if (Environment.TickCount - timeStart > timeWait_Second * 1000)
                        {
                            isExist = false;
                            break;
                        }
                        if (!CheckIsLive())
                            return -2;
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.CheckExistElement({querySelector},{timeWait_Second})");
                }
            }
            return isExist == true ? 1 : 0;
        }
        public int CheckExistElementv2(string JSPath, double timeWait_Second = 0)
        {
            bool isExist = true;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    int timeStart = Environment.TickCount;
                    while ((string)chrome.ExecuteScript("return " + JSPath + ".length+''") == "0")
                    {
                        if (Environment.TickCount - timeStart > timeWait_Second * 1000)
                        {
                            isExist = false;
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    isExist = false;
                    ExportError(null, ex, $"chrome.CheckExistElement({JSPath},{timeWait_Second})");
                }
            }
            return isExist == true ? 1 : 0;
        }
        public bool CheckChromeClosed()
        {
            if (process != null)
            {
                return process.HasExited;
            }
            else
            {
                bool isClosed = true;
                try
                {
                    var x = chrome.Title;
                    isClosed = false;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.CheckChromeClosed()");
                }
                return isClosed;
            }
        }

        /// <summary>
        /// Wait For Element Appear or Disappear
        /// </summary>
        /// <param name="chrome"></param>
        /// <param name="querySelector"></param>
        /// <param name="timeWait_Second"></param>
        /// <param name="typeSearch">0-Wait For Element Appear, 1-Wait For Element Disappear</param>
        /// <returns></returns>
        public int WaitForSearchElement(string querySelector, int typeSearch = 0, double timeWait_Second = 0)
        {
            bool isDone = true;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    int timeStart = Environment.TickCount;
                    if (typeSearch == 0)
                    {
                        while ((string)chrome.ExecuteScript("return document.querySelectorAll('" + querySelector + "').length+''") == "0")
                        {
                            if (Environment.TickCount - timeStart > timeWait_Second * 1000)
                            {
                                isDone = false;
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        while ((string)chrome.ExecuteScript("return document.querySelectorAll('" + querySelector + "').length+''") != "0")
                        {
                            if (Environment.TickCount - timeStart > timeWait_Second * 1000)
                            {
                                isDone = false;
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    isDone = false;
                    ExportError(null, ex, $"chrome.WaitForSearchElement({querySelector},{typeSearch},{timeWait_Second})");
                }
            }
            return isDone == true ? 1 : 0;
        }
        public int CheckExistElements(double timeWait_Second = 0, params string[] querySelectors)
        {
            int result = 0;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    int timeStart = Environment.TickCount;
                    while (true)
                    {
                        for (int i = 0; i < querySelectors.Length; i++)
                        {
                            int check = CheckExistElement(querySelectors[i]);
                            switch (check)
                            {
                                case -2:
                                    return -2;
                                case 1:
                                    return (i + 1);
                                default:
                                    break;
                            }
                        }

                        if (Environment.TickCount - timeStart > timeWait_Second * 1000)
                            break;
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.CheckExistElements({timeWait_Second},{string.Join("|", querySelectors)})");
                }
            }
            return result;
        }

        public int SendEnter(int typeAttribute, string attributeValue)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    switch (typeAttribute)
                    {
                        case 1:
                            chrome.FindElementById(attributeValue).SendKeys(Keys.Enter);
                            break;
                        case 2:
                            chrome.FindElementByName(attributeValue).SendKeys(Keys.Enter);
                            break;
                        case 3:
                            chrome.FindElementByXPath(attributeValue).SendKeys(Keys.Enter);
                            break;
                        case 4:
                            chrome.FindElementByCssSelector(attributeValue).SendKeys(Keys.Enter);
                            break;
                        default:
                            break;
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SendEnter({typeAttribute},{attributeValue})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int Scroll(int x, int y)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    var js1 = String.Format("window.scrollTo({0}, {1})", x, y);
                    chrome.ExecuteScript(js1);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.Scroll({x},{y})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int ScrollSmooth(string JSpath)
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.ExecuteScript(JSpath + ".scrollIntoView({ behavior: 'smooth', block: 'center'});");
                    return 1;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.ScrollSmooth({JSpath})");
                    return 0;
                }
            }
        }
        public int ScrollSmoothIfNotExistOnScreen(string JSpath)
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    if (CheckExistElementOnScreen(JSpath) != 0)
                        chrome.ExecuteScript(JSpath + ".scrollIntoView({ behavior: 'smooth', block: 'center'});");
                    return 1;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.ScrollSmoothIfNotExistOnScreen({JSpath})");
                    return 0;
                }
            }
        }

        public int CheckExistElementOnScreen(string JSpath)
        {
            int check = 0;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    check = Convert.ToInt32(chrome.ExecuteScript("var check='';x=" + JSpath + ";if(x.getBoundingClientRect().top<=0) check='-1'; else if(x.getBoundingClientRect().top+x.getBoundingClientRect().height>window.innerHeight) check='1'; else check='0'; return check;"));
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.CheckExistElementOnScreen({JSpath})");
                }
            }
            return check;
        }
        public Point GetSizeChrome()
        {
            Point point = new Point(0, 0);
            if (CheckIsLive())
            {
                try
                {
                    string temp = chrome.ExecuteScript("return window.innerHeight+'|'+window.innerWidth").ToString();
                    point.X = Convert.ToInt32(temp.Split('|')[1]);
                    point.Y = Convert.ToInt32(temp.Split('|')[0]);
                }
                catch
                {
                }
            }
            return point;
        }
        public int Close()
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    try
                    {
                        chrome.Quit();
                    }
                    catch
                    {
                    }

                    if (process != null)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                        }
                    }
                    return 1;
                }
                catch
                {
                    return 0;
                }
            }

            //try
            //{
            //    chrome.Quit();
            //}
            //catch (Exception ex)
            //{
            //    ExportError(null, ex, $"chrome.Close()");
            //}
        }


        public int ScreenCapture(string imagePath, string fileName)
        {
            bool isSuccess = false;
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    Screenshot image = ((ITakesScreenshot)chrome).GetScreenshot();
                    image.SaveAsFile(imagePath + (imagePath.EndsWith(@"\") ? "" : @"\") + fileName + ".png");
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.ScreenCapture({imagePath},{fileName})");
                }
            }
            return isSuccess == true ? 1 : 0;
        }
        public int AddCookieIntoChrome(string cookie, string domain = ".facebook.com")
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    string[] arrData = cookie.Split(';');
                    foreach (string item in arrData)
                    {
                        if (item.Trim() != "")
                        {
                            string[] pars = item.Split('=');
                            if (pars.Count() > 1 && pars[0].Trim() != "")
                            {
                                Cookie cok = new Cookie(pars[0].Trim(), item.Substring(item.IndexOf('=') + 1).Trim(), domain, "/", DateTime.Now.AddDays(10));
                                chrome.Manage().Cookies.AddCookie(cok);
                            }
                        }
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.AddCookieIntoChrome({cookie},{domain})");
                    return 0;
                }
            }
        }
        public string GetCookieFromChrome(string domain = "facebook")
        {
            string cookie = "";
            if (!CheckIsLive())
            {
                return "-2";
            }
            else
            {
                try
                {
                    var sess = chrome.Manage().Cookies.AllCookies.ToArray();
                    foreach (var item in sess)
                    {
                        if (item.Domain.Contains(domain))
                            cookie += item.Name + "=" + item.Value + ";";
                    }
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.GetCookieFromChrome({domain})");
                }
            }
            return cookie;
        }

        /// <summary>
        /// After add new tab, chrome still focus in current tab. If you want focus to new tab, please use SwitchToLastTab();
        /// </summary>
        /// <param name="url"></param>
        public int OpenNewTab(string url, bool switchToLastTab = true)
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.ExecuteScript("window.open('" + url + "', '_blank').focus();");
                    if (switchToLastTab)
                        chrome.SwitchTo().Window(chrome.WindowHandles.Last());
                    return 1;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.OpenNewTab({url},{switchToLastTab})");
                    return 0;
                }
            }
        }
        public int CloseCurrentTab()
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.CloseCurrentTab()");
                    return 0;
                }
            }
        }
        public int SwitchToFirstTab()
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.SwitchTo().Window(chrome.WindowHandles.First());
                    return 1;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SwitchToFirstTab()");
                    return 0;
                }
            }
        }
        public int SwitchToLastTab()
        {
            if (!CheckIsLive())
            {
                return -2;
            }
            else
            {
                try
                {
                    chrome.SwitchTo().Window(chrome.WindowHandles.Last());
                    return 1;
                }
                catch (Exception ex)
                {
                    ExportError(null, ex, $"chrome.SwitchToLastTab()");
                    return 0;
                }
            }
        }
        public void DelayTime(double timeDelay_Seconds)
        {
            try
            {
                if (!CheckChromeClosed())
                    Thread.Sleep(Convert.ToInt32(timeDelay_Seconds * 1000));
            }
            catch (Exception ex)
            {
                ExportError(null, ex, $"chrome.DelayTime({timeDelay_Seconds})");
            }
        }
        /// <summary>
        /// Create 1 folder name "log" and create 2 folder name "images" and "html" into "log"
        /// </summary>
        public static void ExportError(Chrome chrome, Exception ex, string error = "")
        {
            try
            {
                if (error == "chrome.Open()")//Chỉ xuất lỗi ko mở được chrome
                {
                    if (!Directory.Exists("log"))
                        Directory.CreateDirectory("log");
                    if (!Directory.Exists("log\\html"))
                        Directory.CreateDirectory("log\\html");
                    if (!Directory.Exists("log\\images"))
                        Directory.CreateDirectory("log\\images");

                    Random rrrd = new Random();
                    string fileName = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + rrrd.Next(1000, 9999);

                    if (chrome != null)
                    {
                        string html = chrome.ExecuteScript("var markup = document.documentElement.innerHTML;return markup;").ToString();
                        chrome.ScreenCapture(@"log\images\", fileName);
                        File.WriteAllText(@"log\html\" + fileName + ".html", html);
                    }

                    using (StreamWriter writer = new StreamWriter(@"log\log.txt", true))
                    {
                        writer.WriteLine("-----------------------------------------------------------------------------");
                        writer.WriteLine("Date: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        writer.WriteLine("File: " + fileName);
                        if (error != "")
                            writer.WriteLine("Error: " + error);
                        writer.WriteLine();

                        if (ex != null)
                        {
                            writer.WriteLine("Type: " + ex.GetType().FullName);
                            writer.WriteLine("Message: " + ex.Message);
                            writer.WriteLine("StackTrace: " + ex.StackTrace);
                            ex = ex.InnerException;
                        }
                    }
                }
            }
            catch { }
        }
        
    }
}
