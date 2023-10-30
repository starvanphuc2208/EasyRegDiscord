using easy.Helper;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using MCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace easy.MCommon
{
	internal class DeviceV2
    {
		public static async Task<bool> ExportError(string phoneNumber, bool isAddPrefix, string folder, string deviceID)
		{
			string prefix = (isAddPrefix ? "+" : "");
			string output = folder + "/" + prefix + phoneNumber;
			if (!Directory.Exists(output))
			{
				Directory.CreateDirectory(output);
			}
			return await runCMD("pull /sdcard/Download/SessionsOrigin/" + prefix + phoneNumber + ".session " + output, deviceID);
		}

		public static async Task<bool> XoaSessionLD(string phoneNumber, bool bool_0, string deviceID)
		{
			if (!bool_0)
			{
				return await runCMD("su -c rm -rf /sdcard/Download/Sessions/" + phoneNumber, deviceID);
			}
			return await runCMD("su -c rm -rf /sdcard/Download/Sessions/+" + phoneNumber, deviceID);
		}

		public static async Task<KeyValuePair<bool, string>> CreateSessionAndTdata(string phoneNumber, string string_1, string password, string hint, bool ckbAddPrefix, bool ckbExportTdata, int typeAPI, bool ckbImage, string apiID, string apiHash, string deviceID)
		{
			string isAddPrefix = (ckbAddPrefix ? "1" : "0");
			string isExportTdata = (ckbExportTdata ? "1" : "0");
			string isImage = (ckbImage ? "1" : "0");
			apiID = ((typeAPI != 2) ? "1" : apiID);
			apiHash = ((typeAPI != 2) ? "a" : apiHash);
			return await runCMDV2($"CreateSessionAndTdata chiLnoVsoftware {phoneNumber} {string_1} {password} {hint} {isAddPrefix} {isExportTdata} {typeAPI} {isImage} {apiID} {apiHash}", deviceID, 180000);
		}

		public static async Task<KeyValuePair<bool, string>> runCMDV2(string cmd, string deviceID = null, int timeout = -1)
		{
			string script = "";
			if (deviceID == null)
			{
				script = "shell \"" + cmd + "\" 2>&1";
			}
			else
			{
				script = "-s " + deviceID + " shell \"" + cmd + "\" 2>&1";
			}
			KeyValuePair<bool, string> result;
			if (timeout != -1)
			{
				Task<KeyValuePair<bool, string>> task = Task.Run(async delegate
				{
					Process process2 = (Process)Activator.CreateInstance(typeof(Process));
					process2.StartInfo = (ProcessStartInfo)Activator.CreateInstance(typeof(ProcessStartInfo));
					process2.StartInfo.UseShellExecute = false;
					process2.StartInfo.CreateNoWindow = true;
					process2.StartInfo.RedirectStandardOutput = true;
					process2.StartInfo.FileName = "adb";
					process2.StartInfo.Arguments = script;
					process2.Start();
					string value2 = process2.StandardOutput.ReadToEnd();
					process2.WaitForExit();
					try
					{
						if (process2.ExitCode == 0)
						{
							return new KeyValuePair<bool, string>(key: true, value2);
						}
						return new KeyValuePair<bool, string>(key: false, value2);
					}
					catch (Exception)
					{
						return new KeyValuePair<bool, string>(key: false, value2);
					}
				});
				result = ((await Task.WhenAny(task, Task.Delay(timeout)) != task) ? new KeyValuePair<bool, string>(key: false, "Quá thời gian chuyển đổi session/tdata") : task.Result);
			}
			else
			{
				result = await Task.Run(async delegate
				{
					Process process = (Process)Activator.CreateInstance(typeof(Process));
					process.StartInfo = (ProcessStartInfo)Activator.CreateInstance(typeof(ProcessStartInfo));
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.FileName = "adb";
					process.StartInfo.Arguments = cmd;
					process.Start();
					string value = process.StandardOutput.ReadToEnd();
					process.WaitForExit();
					try
					{
						if (process.ExitCode == 0)
						{
							return new KeyValuePair<bool, string>(key: true, value);
						}
						return new KeyValuePair<bool, string>(key: false, value);
					}
					catch (Exception)
					{
						return new KeyValuePair<bool, string>(key: false, value);
					}
				});
			}
			return result;
		}

		public static async Task<bool> PullSessionToDevice(string phoneNumber, bool isAddPrefix, string folderOut, string deviceID)
		{
			string prefix = (isAddPrefix ? "+" : "");
			string string_3 = ((!(folderOut == "SessionsTdata")) ? ("pull /sdcard/Download/Sessions/" + prefix + phoneNumber + "  " + folderOut) : ("pull /sdcard/Download/Sessions/" + prefix + phoneNumber + " " + folderOut));
			Common.OutData(string_3);
			return (await runCMDV3(string_3, deviceID)).Key;
		}

		public static async Task<bool> MkDirSessionOrigin(string deviceID)
		{
			return await runCMD("mkdir -p /sdcard/Download/SessionsOrigin/", deviceID);
		}

		public static async Task<bool> PushSessionOrigin(string sessionPath, string deviceID)
		{
			return await runCMDV4("push " + sessionPath + " /sdcard/Download/SessionsOrigin/", deviceID);
		}

		public static async Task<bool> runCMDV4(string cmd, string deviceID = null)
		{
			return (await runCMDV3(cmd, deviceID)).Key;
		}

		public static async Task<string> GetTgnet(string deviceID = null)
		{
			return await runCMDV5("getTgnet", deviceID);
		}

		private static async Task<string> runCMDV5(string cmd, string devideID = null)
		{
			string string_2 = "";
			if (devideID != null)
			{
				string_2 = "-s " + devideID + " shell \"" + cmd + "\"";
			}
			else
			{
				string_2 = "shell \"" + cmd + "\"";
			}
			return await Task.Run(delegate
			{
				Process process = (Process)Activator.CreateInstance(typeof(Process));
				process.StartInfo = (ProcessStartInfo)Activator.CreateInstance(typeof(ProcessStartInfo));
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.FileName = "adb";
				process.StartInfo.Arguments = string_2;
				process.Start();
				string text = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				try
				{
					string[] array = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
					if (string.IsNullOrEmpty(array[array.Length - 1]))
					{
						array = array.Take(array.Count() - 1).ToArray();
					}
					if (array.Length < 1)
					{
						return null;
					}
					return array[0];
				}
				catch (Exception)
				{
					return null;
				}
			});
		}

		private static async Task<KeyValuePair<bool, string>> runCMDV3(string cmd, string deviceID = null)
		{
			string string_2 = "";
			if (deviceID == null)
			{
				string_2 = cmd ?? "";
			}
			else
			{
				string_2 = "-s " + deviceID + " " + cmd;
			}
			return await Task.Run(delegate
			{
				Process process = (Process)Activator.CreateInstance(typeof(Process));
				process.StartInfo = (ProcessStartInfo)Activator.CreateInstance(typeof(ProcessStartInfo));
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.FileName = "adb";
				process.StartInfo.Arguments = string_2;
				process.Start();
				string value = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				try
				{
					if (process.ExitCode == 0)
					{
						return new KeyValuePair<bool, string>(key: true, value);
					}
					return new KeyValuePair<bool, string>(key: false, value);
				}
				catch (Exception)
				{
					return new KeyValuePair<bool, string>(key: false, value);
				}
			});
		}


		public static async Task<List<string>> GetDevice()
		{
			string[] array = (await runCMDV3("devices")).Value.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
			List<string> list = (List<string>)Activator.CreateInstance(typeof(List<string>));
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.EndsWith("device"))
				{
					string item = text.Split('\t')[0];
					list.Add(item);
				}
			}
			return list;
		}


		public static async Task<XDocument> GetXML(string string_0 = null)
		{
			string cmd = "";
			string text = "uiautomator dump /dev/tty";
			if (string_0 != null)
			{
				cmd = "-s " + string_0 + " shell \"" + text + "\"";
			}
			else
			{
				cmd = "shell \"" + text + "\"";
			}
			return await Task.Run(delegate
			{
				Process process = (Process)Activator.CreateInstance(typeof(Process));
				process.StartInfo = (ProcessStartInfo)Activator.CreateInstance(typeof(ProcessStartInfo));
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.FileName = "adb";
				process.StartInfo.Arguments = cmd;
				process.Start();
				string text2 = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				if (!text2.Contains("UI hierchary dumped to: /dev/tty"))
				{
					return null;
				}
				text2 = text2.Replace("UI hierchary dumped to: /dev/tty", "");
				return XDocument.Parse(text2);
			});
		}

		public static async Task<bool> runCMD(string cmd, string deviceID = null)
		{
			string script = "";
			if (deviceID != null)
			{
				script = "-s " + deviceID + " shell \"" + cmd + " 2>&1 > /dev/null; echo $?\"";
			}
			else
			{
				script = "shell \"" + cmd + " 2>&1 > /dev/null; echo $?\"";
			}
			//if (deviceID != null)
			//{
			//    script = "-s " + deviceID + " shell " + cmd;
			//}
			//else
			//{
			//    script = "shell \"" + cmd;
			//}
			string text = await Task.Run(async delegate
			{
				Process process = (Process)Activator.CreateInstance(typeof(Process));
				process.StartInfo = (ProcessStartInfo)Activator.CreateInstance(typeof(ProcessStartInfo));
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.FileName = "adb";
				process.StartInfo.Arguments = script;
				process.Start();
				string result2 = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				return result2;
			});
			bool result = default(bool);
			int num;
			try
			{
				string[] array = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
				if (string.IsNullOrEmpty(array[array.Length - 1]))
				{
					array = array.Take(array.Count() - 1).ToArray();
				}
				if (array.Length > 1)
				{
					result = false;
					return result;
				}
				if (int.Parse(array[array.Length - 1]) != 0)
				{
					result = false;
					return result;
				}
				result = true;
				return result;
			}
			catch (Exception)
			{
				num = 1;
			}
			if (num != 1)
			{
				return result;
			}
			return await runCMD(cmd, deviceID);
		}

		public static async Task<bool> TouchSreen(UIElement element, string deviceID = null)
		{
			return await runCMD($"input tap {element.X} {element.Y}", deviceID);
		}

		public static async Task<bool> ClearData(string package, string deviceID = null)
		{
			return await runCMD("pm clear " + package, deviceID);
		}

		public static async Task<bool> Swipe (string deviceID = null, int x1 = 0, int y1 = 0, int x2 = 0, int y2 = 0, int duration = 100)
		{
            return await runCMD(string.Format("input touchscreen swipe {0} {1} {2} {3} {4}", new object[]
                {
                    x1,
                    y1,
                    x2,
                    y2,
                    duration
                }), deviceID);
        }

		public static async Task<bool> Grant(string package, string[] lstGrant, string deviceID = null)
		{
			int num = 0;
			while (true)
			{
				if (num < lstGrant.Length)
				{
					string text = lstGrant[num];
					if (!(await runCMD("pm grant " + package + " android.permission." + text, deviceID)))
					{
						break;
					}
					num++;
					continue;
				}
				return true;
			}
			return false;
		}

		public static async Task<bool> OpenApp(string package, string deviceID = null)
		{
			return await runCMD("monkey -p " + package + " -c android.intent.category.LAUNCHER 1", deviceID);
		}

		public static async Task<bool> InputTextNormal(string message, string deviceID)
		{
			return await runCMD("input text " + message, deviceID);
		}

		public static async Task<bool> InputText(string message, string deviceID)
		{
			// Chuyển
			ADBHelper.SwitchAdbkeyboard(deviceID);

			return await runCMD("am broadcast -a ADB_INPUT_B64 --es msg '" + Convert.ToBase64String(Encoding.UTF8.GetBytes(message.ToString())) + "'", deviceID);
			//return await runCMD("am broadcast -a ADB_INPUT_TEXT --es msg '" + Convert.ToBase64String(Encoding.UTF8.GetBytes(message.ToString())) + "'", deviceID);
		}

		public static async Task<bool> SendKey(string keyevent, string deviceID)
		{
			return await runCMD("input keyevent " + keyevent, deviceID);
		}

        public static bool CheckExistImage(string imagePath, Bitmap bitmap_screen = null, int timeWait = 0, string deviceID = null)
        {
            try
            {
                string boundsByImage = DeviceV2.GetBoundsByImage(imagePath, bitmap_screen, timeWait, deviceID);
                return boundsByImage != "";
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static string GetBoundsByImage(string ImagePath, Bitmap bitmap_screen = null, int timeWait_Second = 0, string deviceID = null)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(ImagePath);
                FileInfo[] files = directoryInfo.GetFiles();

                int timeStart = Environment.TickCount;
                while (true)
                {
                    if (bitmap_screen == null)
                        bitmap_screen = DeviceV2.ScreenShoot(deviceID);
                    Point? point = new Point?();
                    foreach (FileInfo fileInfo in files)
                    {
                        Bitmap subBitmap = (Bitmap)Image.FromFile(fileInfo.FullName);
                        point = DeviceV2.FindOutPoint(bitmap_screen, subBitmap, 0.9);
                        if (point != null)
                        {
                            var value = point.Value;
                            if ((value.X != 0) && (value.Y != 0))
                                return $"[{value.X},{value.Y}][{value.X + subBitmap.Width},{value.Y + subBitmap.Height}]";
                        }
                    }
                    if (Environment.TickCount - timeStart >= timeWait_Second * 1000)
                        break;
                    DeviceV2.DelayTime(1);
                    bitmap_screen = DeviceV2.ScreenShoot(deviceID);
                }
            }
            catch (Exception ex)
            {
            }

            return "";
        }

        public static Bitmap ScreenShoot(string deviceID = null)
        {
            Bitmap result = null;
            try
            {
                string fileName = DeviceV2.ScreenShoot("", DeviceV2.CreateRandomString(10, "a", null) + ".png", deviceID);
                result = DeviceV2.GetBitmapFromFile(fileName, true);
            }
            catch
            {
            }
            return result;
        }

        public static void DelayTime(double second)
        {
            Application.DoEvents();
            Thread.Sleep(Convert.ToInt32(second * 1000.0));
        }

        public static string ScreenShoot(string pathFolder = "", string fileName = "*.png", string deviceID = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
                fileName = Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName);
                ADBHelper.ScreenCap(deviceID, "/sdcard/" + fileName);
                if (string.IsNullOrEmpty(pathFolder))
                {
                    pathFolder = FileHelper.GetPathToCurrentFolder();
                }
                DeviceV2.PullFile("/sdcard/" + fileName, pathFolder, deviceID);
                DeviceV2.ExecuteCMD("shell rm /sdcard/*.png", 10, deviceID);
                return pathFolder + "\\" + fileName;
            }
            catch
            {
            }
            return "";
        }

        public static string ExecuteCMD(string cmd, int timeout = 10, string deviceID = null)
        {
            string result;
            result = ADBHelper.RunCMD(deviceID, cmd, timeout);
            return result;
        }

        public static string PullFile(string fromFilePath, string toFilePath, string deviceID = null)
        {
            return ADBHelper.PullFile(deviceID, fromFilePath, toFilePath);
        }

        private static Bitmap GetBitmapFromFile(string fileName, bool isDeleteFile = true)
        {
            Bitmap result = null;
            try
            {
                using (FileStream fileStream = File.OpenRead(fileName))
                {
                    result = (Bitmap)Image.FromStream(fileStream);
                }
            }
            catch
            {
            }
            if (isDeleteFile)
            {
                Common.DeleteFile(fileName);
            }
            return result;
        }

        public static string CreateRandomString(int lengText)
        {
            string text = "";
            string text2 = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < lengText; i++)
            {
                Random rd = new Random();
                text += text2[rd.Next(0, text2.Length)].ToString();
            }
            return text;
        }

        public static string CreateRandomString(int lengText = 32, string format = "0_a_A", Random random = null)
        {
            string text = "";
            string[] source = format.Split(new char[]
            {
                '_'
            });
            if (source.Contains("A"))
            {
                text += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            if (source.Contains("a"))
            {
                text += "abcdefghijklmnopqrstuvwxyz";
            }
            if (source.Contains("0"))
            {
                text += "0123456789";
            }
            char[] array = new char[lengText];
            if (random == null)
            {
                Random rd = new Random();
                random = rd;
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = text[random.Next(text.Length)];
            }
            return new string(array);
        }

        private static Point? FindOutPoint(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
        {
            Point? result;
            try
            {
                var image = new Image<Bgr, byte>(mainBitmap);
                var image2 = new Image<Bgr, byte>(subBitmap);
                Point? point = null;
                using (var image3 = image.MatchTemplate(image2, (TemplateMatchingType)5))
                {
                    double[] array;
                    double[] array2;
                    Point[] array3;
                    Point[] array4;
                    image3.MinMax(out array, out array2, out array3, out array4);
                    if (array2[0] > percent) point = array4[0];
                }

                result = point;
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
        private List<Point> FindOutPoints(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(mainBitmap);
            Image<Bgr, byte> image2 = new Image<Bgr, byte>(subBitmap);
            List<Point> list = new List<Point>();
            for (; ; )
            {
                using (Image<Gray, float> image3 = image.MatchTemplate(image2, TemplateMatchingType.CcoeffNormed))
                {
                    double[] array;
                    double[] array2;
                    Point[] array3;
                    Point[] array4;
                    image3.MinMax(out array, out array2, out array3, out array4);
                    if (array2[0] <= percent)
                    {
                        break;
                    }
                    Rectangle rect = new Rectangle(array4[0], image2.Size);
                    image.Draw(rect, new Bgr(Color.Blue), -1, LineType.EightConnected, 0);
                    list.Add(array4[0]);
                }
            }
            return list;
        }


    }
}
