namespace easy.Helper
{
    using Newtonsoft.Json.Linq;
    using System.IO;
    using System.Linq;

    public class ConfigHelper
    {
        public static string GetPathProfile()
        {
            JsonHelper jsonHelper = new JsonHelper("configGeneral", false);
            string text = jsonHelper.GetValue("txbPathProfile", "");
            if (!text.Contains('\\'))
            {
                text = FileHelper.GetPathToCurrentFolder() + "\\" + ((jsonHelper.GetValue("txbPathProfile", "") == "") ? "profiles" : jsonHelper.GetValue("txbPathProfile", ""));
            }
            return text;
        }

        public static string GetPathBackup()
        {
            return FileHelper.GetPathToCurrentFolder() + "\\backup";
        }

        public static string GetLanguage()
        {
            JsonHelper jsonHelper = new JsonHelper("configCommon", false);
            string result = jsonHelper.GetValue("txtLanguage", "vi");

            return result;
        }

        public static string GetPathLDPlayer(int type = 0)
        {
            JsonHelper jsonHelper = new JsonHelper("configCommon", false);
            string result = jsonHelper.GetValue("txtLD");
            
            return result;
        }

        public static string GetPathPictureLDPlayer()
        {
            string result = "";
            try
            {
                string path = ConfigHelper.GetPathLDPlayer(0) + "\\vms\\config\\leidian0.config";
                string text = ConfigHelper.GetPathLDPlayer(0) + "\\vms\\config\\leidian1.config";
                if (File.Exists(text))
                {
                    path = text;
                }
                JObject jobject = JObject.Parse(File.ReadAllText(path));
                result = jobject["statusSettings.sharedPictures"].ToString();
            }
            catch
            {
            }
            return result;
        }
    }
}
