using MCommon;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace easy
{
    public class LicenseHelper
    {
        public static string SHA1(string input)
        {
            string result;
            using (MD5 md = MD5.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                byte[] array = md.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    stringBuilder.Append(array[i].ToString("X2"));
                }
                result = stringBuilder.ToString().ToLower();
            }
            return result;
        }

        // Token: 0x060000E9 RID: 233 RVA: 0x0000E164 File Offset: 0x0000C364
        public static string SHAKE256(string randomString)
        {
            HashAlgorithm hashAlgorithm = new SHA256Managed();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(randomString)))
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString().ToLower();
        }

        public LicenseDto CheckLicense(string license, string key)
        {
            LicenseDto licenseDto = new LicenseDto();

            try
            {
                string url = "http://api.jxteamsw.com/api/easykey/check";
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if (!string.IsNullOrEmpty(license))
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{\"license\":\"" + license + "\"," + "\"key\":\"" + key + "\"}";
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        File.WriteAllText(Application.StartupPath + "\\log\\log_key.txt", result);
                        licenseDto = JsonConvert.DeserializeObject<LicenseDto>(result);
                    }
                }

            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "CheckLicense()");
                return null;
            }

            return licenseDto;
        }

        public LicenseDto CheckLicenseV2(string license, string key)
        {
            LicenseDto licenseDto = new LicenseDto();

            try
            {
                //license = "REGTREGTELEGRAM-E2FF-4BF1-D9E3-2247-D1B4-BB01-0AFE-5277";
               // key = "camon";
                string url = "http://app.jxteamsw.com/api/checkkeyv1.php?easy_key="+ Common.UrlEncode(license) + "&pass=" + Common.UrlEncode(key);
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (!string.IsNullOrEmpty(license))
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        File.WriteAllText(Application.StartupPath + "\\log\\log_key1.txt", url + "\n" + result);
                        licenseDto = JsonConvert.DeserializeObject<LicenseDto>(result);
                    }
                }

            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "CheckLicense()");
                return null;
            }

            return licenseDto;
        }

        public LicenseDto Register(string license)
        {
            LicenseDto licenseDto = new LicenseDto();

            try
            {
                string url = "http://api.jxteamsw.com/api/easyspamgroup/register";
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if (!string.IsNullOrEmpty(license))
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{\"license\":\"" + license + "\"}";
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        RegisterDto registerDto = JsonConvert.DeserializeObject<RegisterDto>(result);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }

            return licenseDto;
        }
    }
}
