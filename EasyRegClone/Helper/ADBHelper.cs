namespace easy.Helper
{
    using global::MCommon;
    using MCommon;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class ADBHelper
    {
        public static List<string> GetListPackagesByUser(string deviceId)
        {
            List<string> result = new List<string>();
            try
            {
                string text = ADBHelper.RunCMD(deviceId, ADBCommands.LIST_PACKAGES_USER_INSTALL, 10).Replace("package:", "").Trim();
                if (text != "")
                {
                    result = text.Split(new char[]
                    {
                        '\n'
                    }).ToList<string>();
                }
            }
            catch
            {
            }
            return result;
        }

        public static List<string> GetListPackages(string deviceId)
        {
            List<string> result = new List<string>();
            try
            {
                string text = ADBHelper.RunCMD(deviceId, ADBCommands.LIST_PACKAGES, 10).Replace("package:", "").Trim();
                if (text != "")
                {
                    result = text.Split(new char[]
                    {
                        '\n'
                    }).ToList<string>();
                }
            }
            catch
            {
            }
            return result;
        }

        public static string UninstallApp(string deviceId, string package)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.UNINSTALL_APP, package), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string InstallApp(string deviceId, string filePathFromComputer)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.INSTALL_APP, filePathFromComputer), 60);
            }
            catch
            {
            }
            return "";
        }

        public static string CloseApp(string deviceId, string package)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.CLOSE_APP, package), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string OpenAppChange(string deviceId, string package)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.OPEN_APP_CHANGE, package), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string OpenApp(string deviceId, string package)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.OPEN_APP, package), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string Tap(string deviceId, int x, int y)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.TAP, x, y), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string TapLong(string deviceId, int x, int y, int duration = 100)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.INPUT_SWIPE, new object[]
                {
                    x,
                    y,
                    x,
                    y,
                    duration
                }), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string SwitchAndroidKeyboard(string deviceId, List<string> lstPackage)
        {
            try
            {
                string text = "com.android.inputmethod.pinyin";
                if (lstPackage.Count > 0 && !lstPackage.Contains(text))
                {
                    text = "com.android.emu.inputservice";
                }
                return ADBHelper.RunCMD(deviceId, "shell ime set " + text + "/.InputService", 10);
            }
            catch
            {
            }
            return "";
        }

        public static string SwitchAdbkeyboard(string deviceId)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, ADBCommands.SWITCH_ADBKEYBOARD, 10);
            }
            catch
            {
            }
            return "";
        }

        public static string InputText(string deviceId, string text)
        {
            try
            {
                text = text.Replace(" ", "%s").Replace("&", "\\&").Replace("<", "\\<").Replace(">", "\\>").Replace("?", "\\?").Replace(":", "\\:").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace("|", "\\|");
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.INPUT_TEXT, text), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string Swipe(string deviceId, int x1, int y1, int x2, int y2, int duration = 100)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.INPUT_SWIPE, new object[]
                {
                    x1,
                    y1,
                    x2,
                    y2,
                    duration
                }), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string RemoveHttpProxy(string deviceId)
        {
            try
            {
                ADBHelper.ConnectHttpProxy(deviceId, ":0");
                ADBHelper.RunCMD(deviceId, ADBCommands.DELETE_HTTP_PROXY, 10);
                ADBHelper.RunCMD(deviceId, ADBCommands.DELETE_HTTP_PROXY_HOST, 10);
                ADBHelper.RunCMD(deviceId, ADBCommands.DELETE_HTTP_PROXY_PORT, 10);
            }
            catch
            {
            }
            return "";
        }

        public static string ConnectHttpProxy(string deviceId, string proxy)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.PUT_HTTP_PROXY, proxy), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string View(string deviceId, string link)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.CURL, link), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string Curl(string deviceId, string link)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.CURL, link), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string ScreenCap(string deviceId, string filePath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.SCREENCAP, filePath), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string PullFile(string deviceId, string fromFilePath, string toFilePath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.PULL_FILE, fromFilePath, toFilePath), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string PushFile(string deviceId, string fromFilePath, string toFilePath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.PUSH_FILE, fromFilePath, toFilePath), 60);
            }
            catch
            {
            }
            return "";
        }

        public static string ImportContact(string deviceId, string filePath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.IMPORT_CONTACT, filePath), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string ZipFile(string deviceId, string sourceFile, string toFile, int timeOut)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.ZIP_FILE, toFile, sourceFile), timeOut);
            }
            catch
            {
            }
            return "";
        }

        public static string UnZipFile(string deviceId, string filePath, int timeOut)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.UNZIP_FILE, filePath), timeOut);
            }
            catch
            {
            }
            return "";
        }

        public static string ClearDataApp(string deviceId, string package)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.CLEAR_DATA_APP, package), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string GetXMLFull(string deviceId)
        {
            string text = "";
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    text = ADBHelper.RunCMD(deviceId, "adb shell uiautomator dump && adb shell cat /sdcard/window_dump.xml && adb shell rm /sdcard/window_dump.xml", 10);
                    if (text.Trim() != "ui hierchary dumped to: /sdcard/window_dump.xml" && text.Trim() != "")
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch
            {
            }
            return Regex.Match(text, "<\\?xml(.*?)</hierarchy>").Value;
        }

        public static string GetXML(string deviceId)
        {
            string text = "";
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    text = ADBHelper.RunCMD(deviceId, "adb shell uiautomator dump && adb shell cat /sdcard/window_dump.xml && adb shell rm /sdcard/window_dump.xml", 10).ToLower();
                    if (text.Trim() != "ui hierchary dumped to: /sdcard/window_dump.xml" && text.Trim() != "")
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch
            {
            }
            return Regex.Match(text, "<\\?xml(.*?)</hierarchy>").Value;
        }

        public static string ScreenShot(string deviceId, string filePathToSave)
        {
            string result = "";
            try
            {
                string fileName = Path.GetFileName(filePathToSave);
                ADBHelper.ScreenCap(deviceId, "/sdcard/" + fileName);
                ADBHelper.PullFile(deviceId, "/sdcard/" + fileName, filePathToSave);
                ADBHelper.DeleteFile(deviceId, "/sdcard/" + fileName);
            }
            catch
            {
            }
            return result;
        }

        public static string DumpScreen(string deviceId, string filePath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, "shell uiautomator dump && adb shell cat /sdcard/window_dump.xml", 10);
            }
            catch
            {
            }
            return "";
        }

        public static string DumpActivity(string deviceId)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, ADBCommands.DUMP_ACTIVITY, 10);
            }
            catch
            {
            }
            return "";
        }

        public static string DeleteFile(string deviceId, string filePath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.DELETE_FILE, filePath), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string DeleteFolder(string deviceId, string folderPath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.DELETE_FOLDER, folderPath), 10);
            }
            catch
            {
            }
            return "";
        }

        public static string ReadFile(string deviceId, string filePath)
        {
            try
            {
                return ADBHelper.RunCMD(deviceId, string.Format(ADBCommands.READ_FILE, filePath), 10);
            }
            catch
            {
            }
            return "";
        }

        public static void DisconnectDevice(string deviceId)
        {
            try
            {
                ADBHelper.RunCMD("adb disconnect " + deviceId, 10);
            }
            catch
            {
            }
        }

        public static void ConnectDevice(string deviceId)
        {
            try
            {
                ADBHelper.RunCMD("adb connect " + deviceId, 10);
            }
            catch
            {
            }
        }

        public static string GetClipboard(string deviceId)
        {
            string text = ADBHelper.RunCMD(deviceId, "shell am broadcast -a clipper.get");
            return text;
        }

        public static List<string> GetDevices()
        {
            List<string> list = new List<string>();
            string text = ADBHelper.RunCMD("adb devices", 10);
            string[] array = text.Replace("List of devices attached", "").Trim().Split(new string[]
            {
                "\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].Contains("offline") && array[i].Contains("device"))
                {
                    list.Add(array[i].Trim().Split(new char[]
                    {
                        '\t'
                    })[0]);
                }
            }
            return list;
        }

        public static List<string> GetListNameLDPlayer(string pathLd)
        {
            List<string> list = new List<string>();
            try
            {
                string text = ADBHelper.RunCMD(pathLd + "\\dnconsole.exe list2", 10);
                if (text.Trim() != "")
                {
                    List<string> list2 = text.Trim().Split(new char[]
                    {
                        '\n'
                    }).ToList<string>();
                    for (int i = 0; i < list2.Count; i++)
                    {
                        string item = list2[i].Split(new char[]
                        {
                            ','
                        })[1];
                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "GetListLDPlayer(" + pathLd + ")");
            }
            return list;
        }

        public static List<string> GetListIndexLDPlayer(string pathLd)
        {
            List<string> list = new List<string>();
            try
            {
                string text = ADBHelper.RunCMD(pathLd + "\\dnconsole.exe list2", 10);
                if (text.Trim() != "")
                {
                    List<string> list2 = text.Trim().Split(new char[]
                    {
                        '\n'
                    }).ToList<string>();
                    for (int i = 0; i < list2.Count; i++)
                    {
                        list.Add(list2[i].Split(new char[]
                        {
                            ','
                        })[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "GetListIndexLDPlayer(" + pathLd + ")");
            }
            return list;
        }

        public static void RestoreDevice(string pathLD, int indexDevice, string filePathBackup)
        {
            try
            {
                ADBHelper.RunCMD(string.Concat(new string[]
                {
                    pathLD,
                    "\\dnconsole.exe restore --index ",
                    indexDevice.ToString(),
                    " --file \"",
                    filePathBackup,
                    "\""
                }), 300);
            }
            catch
            {
            }
        }

        public static void CopyDevice(string pathLD)
        {
            try
            {
                ADBHelper.RunCMD(pathLD + "\\dnconsole.exe copy --from 0", 30);
            }
            catch
            {
            }
        }

        public static void AddDevice(string pathLD)
        {
            try
            {
                ADBHelper.RunCMD(pathLD + "\\dnconsole.exe add", 30);
            }
            catch
            {
            }
        }

        public static void LaunchDevice(string pathLD, int indexDevice)
        {
            try
            {
                ADBHelper.RunCMD(pathLD + "\\dnconsole.exe launch --index " + indexDevice.ToString(), 10);
            }
            catch
            {
            }
        }

        public static void RemoveDevice(string pathLD, int indexDevice)
        {
            try
            {
                ADBHelper.RunCMD(pathLD + "\\dnconsole.exe remove --index " + indexDevice.ToString(), 10);
            }
            catch
            {
            }
        }


        public static void QuitDevice(string pathLD, int indexDevice)
        {
            try
            {
                ADBHelper.RunCMD(pathLD + "\\dnconsole.exe quit --index " + indexDevice.ToString(), 10);
            }
            catch
            {
            }
        }

        public static void QuitAllDevice(string pathLD)
        {
            try
            {
                ADBHelper.RunCMD(pathLD + "\\dnconsole.exe quitall", 10);
            }
            catch
            {
            }
        }

        public static string RunCMD(string deviceId, string cmd, int timeout = 10)
        {
            if (!string.IsNullOrEmpty(deviceId))
            {
                string newValue = "adb -s " + deviceId;
                if (!cmd.StartsWith("adb"))
                {
                    cmd = "adb -s " + deviceId + " " + cmd;
                }
                else
                {
                    cmd = cmd.Replace("adb", newValue);
                }
            }
            return ADBHelper.RunCMD(cmd, timeout);
        }

        //public static KeyValuePair<bool, string> RunCMD(string cmd, int timeout = 10)
        //{
        //    Process process = new Process();
        //    process.StartInfo = new ProcessStartInfo();
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.FileName = "adb";
        //    process.StartInfo.Arguments = cmd;
        //    process.Start();
        //    string value = process.StandardOutput.ReadToEnd();
        //    process.WaitForExit();
        //    try
        //    {
        //        if (process.ExitCode == 0)
        //        {
        //            return new KeyValuePair<bool, string>(key: true, value);
        //        }
        //        return new KeyValuePair<bool, string>(key: false, value);
        //    }
        //    catch (Exception)
        //    {
        //        return new KeyValuePair<bool, string>(key: false, value);
        //    }
        //}

        public static string RunCMD(string cmd, int timeout = 10)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + cmd;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            StringBuilder output = new StringBuilder();
            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    output.Append(e.Data + "\n");
                }
            };
            process.Start();
            process.BeginOutputReadLine();
            if (timeout < 0)
            {
                process.WaitForExit();
            }
            else
            {
                process.WaitForExit(timeout * 1000);
            }
            process.Close();
            return output.ToString();
        }

        public static string CreateRandomString(int lengText)
        {
            string text = "";
            Random random = new Random();
            string text2 = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < lengText; i++)
            {
                text += text2[random.Next(0, text2.Length)].ToString();
            }
            return text;
        }

        public static string GetDeviceByIndex(int IndexDevice)
        {
            string text = "";
            try
            {
                List<string> devices = ADBHelper.GetDevices();
                List<string> lstDeviceIdCheck = new List<string>
                {
                    "127.0.0.1:" + (IndexDevice * 2 + 5555).ToString(),
                    "emulator-" + (IndexDevice * 2 + 5554).ToString()
                };
                text = (from x in devices
                        where lstDeviceIdCheck.Contains(x)
                        select x).FirstOrDefault<string>();
                if (string.IsNullOrEmpty(text))
                {
                    for (int i = 0; i < lstDeviceIdCheck.Count; i++)
                    {
                        ADBHelper.DisconnectDevice(lstDeviceIdCheck[i]);
                    }
                    ADBHelper.ConnectDevice(lstDeviceIdCheck[0]);
                }
            }
            catch
            {
            }
            return text;
        }
    }
}
