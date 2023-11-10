using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DevExpress.XtraPrinting;
using ZXing;
using Emgu.CV.CvEnum;
using static DevExpress.Xpo.DB.DataStoreLongrunnersWatch;

namespace easy
{
    internal class captchaSolve
    {
        public string APIKey { get; private set; }

        public captchaSolve(string apiKey)
        {
            APIKey = apiKey;
        }

        public bool SolveClickCaptcha(string base64Image,out string status, out List<Coordinate> co)
        {
            string jsonCreateTask = "{\"clientKey\":\"" + APIKey + "\",\"task\": { \"type\":\"CoordinatesTask\",\"body\":\"" + base64Image + "\"}}";
            string createTask = postRequest("http://2captcha.com/createTask", jsonCreateTask);
            dynamic jsonResCreateTask = JsonConvert.DeserializeObject(createTask);
            if(jsonResCreateTask.errorId == 0)
            {
                string taskId = jsonResCreateTask.taskId;
                string jsonExecTask = "{\"clientKey\":\"" + APIKey + "\",\"taskId\": " + taskId + "}";
				Thread.Sleep(3000);
                recall:
                string callExecTask = postRequest("https://api.2captcha.com/getTaskResult", jsonExecTask);
                dynamic jsonResExecTask = JsonConvert.DeserializeObject(callExecTask);

                if (jsonResExecTask.errorId == 0)
                {
                    if(jsonResExecTask.status == "processing")
                    {
                        Thread.Sleep(1000);
                        goto recall;
                    } else if (jsonResExecTask.status == "ready")
                    {
                        List<Coordinate> coorList = new List<Coordinate>();
                        foreach (var obj in jsonResExecTask.solution.coordinates)
                        {
                            Coordinate c = new Coordinate();
                            c.x = obj.x;
                            c.y = obj.y;
                            coorList.Add(c);
                        }
                        co = coorList;
                        status = "Success";
                        return true;
                    }
                    else
                    {
                        co = null;
                        status = "error";
                        return false;
                    }
                }
                else
                {
                    co = null;
                    status = jsonResExecTask.errorCode + " - " + jsonResExecTask.status;
                    return false;
                }
            }
            else
            {
                co = null;
                status = "Fail";
                return false;
            }
        }

        private string postRequest(string url, string jsonBody)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonBody);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var res = streamReader.ReadToEnd();
                return res;

            }
        }
    }

    public class Coordinate
    {
        public int x { get; set;}
        public int y { get; set;}
    }

}