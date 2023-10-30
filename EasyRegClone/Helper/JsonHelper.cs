namespace easy.Helper
{
    using global::MCommon;
    using MCommon;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class JsonHelper
    {
        public JsonHelper(string jsonStringOrPathFile, bool isJsonString = false)
        {
            if (isJsonString)
            {
                if (jsonStringOrPathFile.Trim() == "")
                {
                    jsonStringOrPathFile = "{}";
                }
                this.json = JObject.Parse(jsonStringOrPathFile);
            }
            else
            {
                try
                {
                    this.PathFileSetting = "settings\\" + jsonStringOrPathFile + ".json";
                    if (!File.Exists(this.PathFileSetting))
                    {
                        using (File.AppendText(this.PathFileSetting))
                        {
                        }
                    }
                    this.json = JObject.Parse(File.ReadAllText(this.PathFileSetting));
                }
                catch
                {
                    this.json = new JObject();
                }
            }
        }

        public static Dictionary<string, object> ConvertToDictionary(JObject jObject)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                dic = jObject.ToObject<Dictionary<string, object>>();

                var JObjectKeys = (from r in dic
                                   let key = r.Key
                                   let value = r.Value
                                   where value.GetType() == typeof(JObject)
                                   select key).ToList();

                var JArrayKeys = (from r in dic
                                  let key = r.Key
                                  let value = r.Value
                                  where value.GetType() == typeof(JArray)
                                  select key).ToList();

                JArrayKeys.ForEach(key => dic[key] = ((JArray)dic[key]).Values().Select(x => ((JValue)x).Value).ToArray());
                JObjectKeys.ForEach(key => dic[key] = ConvertToDictionary(dic[key] as JObject));
            }
            catch
            {
            }

            return dic;
        }

        public JsonHelper()
        {
            this.json = new JObject();
        }

        public string GetValue(string key, string valueDefault = "")
        {
            string result = valueDefault;
            try
            {
                result = ((this.json[key] == null) ? valueDefault : this.json[key].ToString());
            }
            catch
            {
            }
            return result;
        }

        public List<string> GetValueList(string key, int typeSplitString = 0)
        {
            List<string> list = new List<string>();
            try
            {
                if (typeSplitString == 0)
                {
                    list = this.GetValue(key, "").Split(new char[]
                    {
                        '\n'
                    }).ToList<string>();
                }
                else
                {
                    list = this.GetValue(key, "").Split(new string[]
                    {
                        "\n|\n"
                    }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                }
                list = Common.RemoveEmptyItems(list);
            }
            catch
            {
            }
            return list;
        }

        public int GetValueInt(string key, int valueDefault = 0)
        {
            int result = valueDefault;
            try
            {
                result = ((this.json[key] == null) ? valueDefault : Convert.ToInt32(this.json[key].ToString()));
            }
            catch
            {
            }
            return result;
        }

        public bool GetValueBool(string key, bool valueDefault = false)
        {
            bool result = valueDefault;
            try
            {
                result = ((this.json[key] == null) ? valueDefault : Convert.ToBoolean(this.json[key].ToString()));
            }
            catch
            {
            }
            return result;
        }

        public void Add(string key, string value)
        {
            try
            {
                if (!this.json.ContainsKey(key))
                {
                    this.json.Add(key, value);
                }
                else
                {
                    this.json[key] = value;
                }
            }
            catch (Exception)
            {
            }
        }

        public void Update(string key, object value)
        {
            try
            {
                this.json[key] = value.ToString();
            }
            catch
            {
            }
        }

        public void Update(string key, List<string> lst)
        {
            try
            {
                this.json[key] = string.Join("\n", lst).ToString();
            }
            catch
            {
            }
        }

        public void Remove(string key)
        {
            try
            {
                this.json.Remove(key);
            }
            catch
            {
            }
        }

        public void Save(string pathFileSetting = "")
        {
            try
            {
                if (pathFileSetting == "")
                {
                    pathFileSetting = this.PathFileSetting;
                }
                File.WriteAllText(pathFileSetting, this.json.ToString());
            }
            catch
            {
            }
        }

        public string GetFullString()
        {
            string result = "";
            try
            {
                result = this.json.ToString().Replace("\r\n", "");
            }
            catch (Exception)
            {
            }
            return result;
        }

        private string PathFileSetting;

        private JObject json;
    }
}
