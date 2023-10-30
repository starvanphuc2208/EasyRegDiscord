using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using easy.Properties;
using HttpRequest;
using KAutoHelper;
using Newtonsoft.Json.Linq;
using OtpNet;

namespace EasyRegClone
{
	// Token: 0x02000006 RID: 6
	public class Helper
	{
		// Token: 0x06000096 RID: 150 RVA: 0x000156A4 File Offset: 0x000138A4
		public static string InstallAPK(string index, string pathAPK)
		{
			return Helper.ExecuteLD_Result("installapp --" + index + " --filename " + pathAPK, 0);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000156D0 File Offset: 0x000138D0
		public static string UnistallAPK(string index, string package)
		{
			return Helper.ExecuteLD_Result("uninstallapp --" + index + " --packagename " + package, 0);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000156FC File Offset: 0x000138FC
		public static bool FindImage(string index, Bitmap LinkIMG, int exitwhile, int timeout = 2, bool tap = true, int x = 0, int y = 0)
		{
			int num = 0;
			bool result;
			for (; ; )
			{
				try
				{
					Bitmap bitmap = Helper.ScreenShoot_Index(index, true);
					try
					{
						Point? point = ImageScanOpenCV.FindOutPoint(bitmap, LinkIMG, 0.9);
						bool flag = point != null;
						if (flag)
						{
							if (tap)
							{
								Helper.Tap(index, point.Value.X + x, point.Value.Y + y);
							}
							result = true;
							break;
						}
						num++;
						bool flag2 = num == exitwhile;
						if (flag2)
						{
							result = false;
							break;
						}
						Thread.Sleep(1500);
					}
					catch (Exception ex)
					{
						if (bitmap != null)
						{
							((IDisposable)bitmap).Dispose();
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000157D4 File Offset: 0x000139D4
		public static void OpenLDP(string index)
		{
			Helper.ExecuteLDP(string.Format("launch --{0}", index));
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000157F4 File Offset: 0x000139F4
		public static string Pull(string index, string path1, string path2)
		{
			return Helper.ExcuteCMD(index, string.Concat(new string[]
			{
				"pull \"",
				path1,
				"\" \"",
				path2,
				"\""
			}), 0);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00015838 File Offset: 0x00013A38
		public static string Push(string index, string path1, string path2)
		{
			return Helper.ExcuteCMD(index, string.Concat(new string[]
			{
				"push \"",
				path1,
				"\" \"",
				path2,
				"\""
			}), 0);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0001587C File Offset: 0x00013A7C
		public static string Get2FA(string Key_2FA)
		{
			Key_2FA = Key_2FA.Replace(" ", "");
			byte[] secretKey = Base32Encoding.ToBytes(Key_2FA);
			Totp totp = new Totp(secretKey, 30, OtpHashMode.Sha1, 6, null);
			return totp.ComputeTotp();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000158BC File Offset: 0x00013ABC
		public static void CloseLDP(string index)
		{
			Helper.ExecuteLDP(string.Format("quit  --{0}", index));
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000158DC File Offset: 0x00013ADC
		public static void CloseAllLDP()
		{
			Helper.ExecuteLDP("quitall");
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000158F8 File Offset: 0x00013AF8
		public static void ClearPackage(string index, string package)
		{
			string cmd = "shell pm clear " + package;
			Helper.ExcuteCMD(index, cmd, 0);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0001591C File Offset: 0x00013B1C
		public static string KillPackage(string index, string package)
		{
			return Helper.ExecuteLD_Result("killapp --" + index + " --packagename " + package, 0);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00015948 File Offset: 0x00013B48
		public static string OpenPackage(string index, string package)
		{
			return Helper.ExcuteCMD(index, "shell monkey -p " + package + " -c android.intent.category.LAUNCHER 1", 0);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00015974 File Offset: 0x00013B74
		public static void Tap(string index, int x, int y)
		{
			string cmd = "shell input tap " + x.ToString() + " " + y.ToString();
			Helper.ExcuteCMD(index, cmd, 0);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000159AC File Offset: 0x00013BAC
		public static Bitmap ScreenShoot_Index(string index, bool isDeleteImageAfterCapture = true)
		{
			string fileName = "screenShoot.png";

			Bitmap bitmap;
			string str = index.Replace(".", "_").Replace(":", "_").Replace("-", "_").Replace(" ", "");
			string str1 = string.Concat(Path.GetFileNameWithoutExtension(fileName), str, Path.GetExtension(fileName));
			while (File.Exists(str1))
			{
				try
				{
					File.Delete(str1);
					break;
				}
				catch (Exception ex)
				{
				}
			}

			Helper.ExcuteCMD(index, string.Concat("shell screencap -p \"/sdcard/", str1, "\""), 0);
			Helper.ExcuteCMD(index, string.Concat("pull \"/sdcard/", str1, "\""), 0);
			Helper.ExcuteCMD(index, string.Concat("shell rm -f \"/sdcard/", str1, "\""), 0);

			//Helpers.ExecuteADB(deviceID, string.Concat("shell screencap -p \"/sdcard/", str1, "\""), 5000, true);
			//Helpers.ExecuteADB(deviceID, string.Concat("pull \"/sdcard/", str1, "\""), 5000, true);
			//Helpers.ExecuteADB(deviceID, string.Concat("shell rm -f \"/sdcard/", str1, "\""), 5000, true);
			using (Bitmap bitmap1 = new Bitmap(str1))
			{
				bitmap = new Bitmap(bitmap1);
			}
			if (isDeleteImageAfterCapture)
			{
				try
				{
					File.Delete(str1);
				}
				catch
				{
				}
			}
			return bitmap;

			//string text = Path.GetFileNameWithoutExtension(path) + index.Replace(" ", "") + Path.GetExtension(path);
			//string text2 = Directory.GetCurrentDirectory() + "\\dump\\" + text;
			//if (isDeleteImageAfterCapture)
			//{
			//	string text3 = Directory.GetCurrentDirectory() + "\\dump";
			//	string text4 = "/sdcard/" + text;
			//	string cmd = "shell screencap -p /sdcard/" + text;
			//	string cmd2 = string.Concat(new string[]
			//	{
			//		"pull \"",
			//		text4,
			//		"\" \"",
			//		text2,
			//		"\""
			//	});
			//	try
			//	{
			//		File.Delete(text);
			//	}
			//	catch (Exception ex)
			//	{
			//	}
			//	Helper.ExcuteCMD(index, cmd, 0);
			//	Helper.ExcuteCMD(index, cmd2, 0);
			//}
			//Bitmap result = null;
			//try
			//{
			//	Bitmap bitmap = new Bitmap(text2);
			//	try
			//	{
			//		result = new Bitmap(bitmap);
			//	}
			//	catch
			//	{
			//		if (bitmap != null)
			//		{
			//			((IDisposable)bitmap).Dispose();
			//		}
			//	}
			//}
			//catch
			//{
			//}
			//return result;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00015AD4 File Offset: 0x00013CD4
		public static string ExecuteLD_Result(string cmdCommand, int timeout = 0)
		{
			string result;
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = Helper.pathLD,
					Arguments = cmdCommand,
					CreateNoWindow = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardInput = true,
					RedirectStandardOutput = true
				};
				process.Start();
				bool flag = timeout == 0;
				if (flag)
				{
					process.WaitForExit();
				}
				else
				{
					process.WaitForExit(timeout);
				}
				string text = process.StandardOutput.ReadToEnd();
				result = text;
				process.Close();
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00015B8C File Offset: 0x00013D8C
		public static string ExcuteCMD(string index, string cmd, int timeout = 0)
		{
			return Helper.ExecuteLD_Result(string.Format("adb --{0} --command \"{1}\"", index, cmd), timeout);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00015BB0 File Offset: 0x00013DB0
		public static void ExecuteLDP(string cmd)
		{
			Process process = new Process();
			process.StartInfo.FileName = Helper.pathLD;
			process.StartInfo.Arguments = cmd;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.EnableRaisingEvents = true;
			process.Start();
			process.WaitForExit();
			process.Close();
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00015C28 File Offset: 0x00013E28
		public static bool CheckLDRunning(string index, string pathLD)
		{
			pathLD += "\\ldconsole.exe";
			string arguments = string.Format("adb --{0} --command \"{1}\"", index, "shell input tap 0 0");
			int num = 0;
			Process process = new Process();
			process.StartInfo = new ProcessStartInfo
			{
				FileName = pathLD,
				Arguments = arguments,
				CreateNoWindow = true,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				RedirectStandardInput = true,
				RedirectStandardOutput = true
			};
			for (; ; )
			{
				process.Start();
				process.WaitForExit(3000);
				string text = process.StandardOutput.ReadToEnd();
				string a = text;
				bool flag = a == "";
				if (flag)
				{
					break;
				}
				Thread.Sleep(2000);
				num++;
				bool flag2 = num == 30;
				if (flag2)
				{
					goto Block_2;
				}
			}
			return true;
			Block_2:
			return false;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00015D08 File Offset: 0x00013F08
		public static void Delay(int time)
		{
			while (time > 0)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1.0));
				time--;
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00015D3C File Offset: 0x00013F3C
		public static List<string> GetIndex(int thread)
		{
			List<string> list = new List<string>();
			for (int i = 1; i < thread + 1; i++)
			{
				list.Add("index " + i.ToString());
			}
			return list;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00015D84 File Offset: 0x00013F84
		public static void RunADB(string cmd, int timeout = 0, bool returnresult = true)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					WorkingDirectory = currentDirectory,
					FileName = "cmd.exe",
					CreateNoWindow = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					Verb = "runas"
				};
				process.Start();
				process.StandardInput.WriteLine(cmd);
				process.StandardInput.Flush();
				process.StandardInput.Close();
				bool flag = timeout > 0;
				if (flag)
				{
					process.WaitForExit(timeout);
				}
				else
				{
					process.WaitForExit();
				}
				if (returnresult)
				{
					string text = process.StandardOutput.ReadToEnd();
				}
			}
			catch
			{
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00015E6C File Offset: 0x0001406C
		public static List<string> GetInfoAccount(string dmmail, bool ranname = true, bool nameviet = true, bool ranpass = true, string defaultpwd = "")
		{
			List<string> list = new List<string>();
			string text = Helper.rdmgmail(6);
			string[] array = new string[]
			{
				"Bui",
				"Cao",
				"Chu",
				"Chung",
				"Chuong",
				"Cu",
				"Diep",
				"Doan",
				"Duong",
				"Dao",
				"Dang",
				"Dau",
				"Dinh",
				"Dien",
				"Doan",
				"Do",
				"Duong",
				"Giang",
				"Giao",
				"Giap",
				"Gia",
				"Hoang",
				"Huynh",
				"Ha",
				"Ho",
				"Hong",
				"Khuong",
				"Khong",
				"Kim",
				"Kieu",
				"La",
				"Le",
				"Lo",
				"Ly",
				"Luu",
				"Luong",
				"Mai",
				"Mac",
				"Nguyen",
				"Nong",
				"Ong",
				"Phan",
				"Phung",
				"Phuong",
				"Pham",
				"Quach",
				"Thai",
				"Tieu",
				"Trieu",
				"Truong",
				"Tran",
				"To",
				"Tang",
				"Ta",
				"Tong",
				"Vuong",
				"An",
				"Anh",
				"Binh",
				"Bich",
				"Bang",
				"Bach",
				"Bao",
				"Ca",
				"Chi",
				"Chinh",
				"Chieu",
				"Chau",
				"Cat",
				"Cuc",
				"Cuong",
				"Cam",
				"Dao",
				"Di",
				"Diem",
				"Dieu",
				"Du",
				"Dung",
				"Duyen",
				"Dan",
				"Hieu",
				"Hien",
				"Hiep",
				"Hoa",
				"Hoan",
				"Hoai",
				"Hoan",
				"Huyen",
				"Hue",
				"Han",
				"Hoa",
				"Huong",
				"Huong",
				"Hanh",
				"Hai",
				"Hao",
				"Hau",
				"Hang",
				"Hop",
				"Khai",
				"Khanh",
				"Khuyen",
				"Khue",
				"Khanh",
				"Khe",
				"Khoi",
				"Lam",
				"Lan",
				"Linh",
				"Lien",
				"Lieu",
				"Loan",
				"Ly",
				"Lam",
				"Le",
				"Le",
				"Loc",
				"Loi",
				"Mi",
				"Minh",
				"Mien",
				"My",
				"Man",
				"My",
				"Nga",
				"Nghi",
				"Nguyen",
				"Nguyet",
				"Nga",
				"Ngan",
				"Ngon",
				"Ngoc",
				"Nhi",
				"Nhien",
				"Nhung",
				"Nhan",
				"Nhan",
				"Nha",
				"Nhu",
				"Nuong",
				"Nu",
				"Oanh",
				"Phi",
				"Phong"
			};
			Random random = new Random();
			int num = random.Next(1, 100);
			int num2 = random.Next(1, 100);
			int num3 = random.Next(1, 100);
			string[] array2 = new string[]
			{
				"41",
				"25",
				"30",
				"41",
				"55",
				"66",
				"47",
				"89",
				"89",
				"10",
				"11",
				"12",
				"13",
				"14",
				"15",
				"16",
				"17",
				"18",
				"19",
				"20",
				"21",
				"22",
				"23",
				"24",
				"25",
				"26",
				"27",
				"28",
				"29",
				"30"
			};
			Random random2 = new Random();
			int num4 = random2.Next(1, 29);
			int num5 = random2.Next(1, 29);
			int num6 = random2.Next(1, 29);
			string str = array2[num4];
			string str2 = array2[num5];
			string str3 = array2[num6];
			string text2 = array[num3];
			string str4 = array[num3];
			bool flag = !ranname;
			string text3;
			string text4;
			if (flag)
			{
				string path;
				if (nameviet)
				{
					path = "DATA/NAME/VN.txt";
				}
				else
				{
					path = "DATA/NAME/US.txt";
				}
				string[] array3 = File.ReadAllLines(path);
				Random random3 = new Random();
				int num7 = random3.Next(0, array3.Length - 1);
				text3 = array3[num7];
				int num8 = random3.Next(0, array3.Length - 1);
				text4 = array3[num8];
			}
			else
			{
				text3 = array[num];
				text4 = array[num2];
				char[] array4 = text3.ToCharArray();
				char[] array5 = text4.ToCharArray();
			}
			string item;
			if (ranpass)
			{
				item = str + str2 + text2 + str3;
			}
			else
			{
				item = defaultpwd;
			}
			Random random4 = new Random();
			string item2 = text2 + str4 + random4.Next(10, 199).ToString() + dmmail;
			list.Add(text3 + "|" + text4);
			list.Add(item);
			list.Add(item2);
			return list;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00016668 File Offset: 0x00014868
		public static void Vietcodau(string index, string text)
		{
			string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
			Helper.ExcuteCMD(index, "shell ime set com.android.adbkeyboard/.AdbIME", 0);
			Helper.ExcuteCMD(index, "shell am broadcast -a ADB_INPUT_B64 --es msg " + str, 0);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000166A8 File Offset: 0x000148A8
		public static void ChangeDcomApp(string nameDcom)
		{
			bool flag = false;
			try
			{
				string text = Helper.RunCmdDcom("\"" + nameDcom + "\"");
				bool flag2 = text.Contains("Successfully connected to ");
				if (!flag2)
				{
					bool flag3 = text.Contains("You are already connected to ");
					if (flag3)
					{
						for (int i = 0; i < 3; i++)
						{
							text = Helper.RunCmdDcom("\"" + nameDcom + "\" /disconnect");
							bool flag4 = text.Trim() == "Command completed successfully.";
							if (flag4)
							{
								flag = true;
								break;
							}
							Helper.Delay(1);
						}
						bool flag5 = flag;
						if (flag5)
						{
							for (int j = 0; j < 3; j++)
							{
								text = Helper.RunCmdDcom("\"" + nameDcom + "\"");
								bool flag6 = text.Contains("Successfully connected to ");
								if (flag6)
								{
									break;
								}
								Helper.Delay(1);
							}
						}
						Helper.Delay(1);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000167C4 File Offset: 0x000149C4
		public static int ChangeDcomHilink(string linkDcom = "192.168.0.1")
		{
			int result = -1;
			try
			{
				string str = "http" + Regex.Match(linkDcom, "http(.*?)/html").Groups[1].Value;
				Helper.RequestHttp requestHttp = new Helper.RequestHttp("", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36", "", 0);
				string text = requestHttp.RequestGet(linkDcom);
				string str2 = "";
				try
				{
					str2 = Regex.Matches(text, "csrf_token\" content=\"(.*?)\"")[1].Groups[1].Value;
				}
				catch
				{
					str2 = Regex.Match(requestHttp.RequestGet(str + "/api/webserver/token"), "<token>(.*?)</token>").Groups[1].Value;
				}
				text = requestHttp.RequestGet(str + "/api/dialup/mobile-dataswitch");
				requestHttp.request.SetDefaultHeaders(new string[]
				{
					"__RequestVerificationToken: " + str2,
					"Accept: */*",
					"Accept-Encoding: gzip, deflate",
					"Accept-Language: vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5",
					"Content-Type: application/x-www-form-urlencoded; charset=UTF-8",
					"X-Requested-With: XMLHttpRequest",
					"content-type: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
					"user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36"
				});
				bool flag = text.Contains("dataswitch>1");
				bool flag2 = flag;
				string data;
				if (flag2)
				{
					data = text.Replace("response", "request").Replace("dataswitch>1", "dataswitch>0");
				}
				else
				{
					bool flag3 = text.Contains("dataswitch>0");
					bool flag4 = !flag3;
					if (flag4)
					{
						return -1;
					}
					data = text.Replace("response", "request").Replace("dataswitch>0", "dataswitch>1");
				}
				string text2 = requestHttp.RequestPost(str + "/api/dialup/mobile-dataswitch", data);
				bool flag5 = text2.Contains("OK");
				bool flag6 = flag5;
				if (flag6)
				{
					text = requestHttp.RequestGet(str + "/api/dialup/mobile-dataswitch");
					bool flag7 = text.Contains("dataswitch>1<");
					bool flag8 = flag7;
					if (flag8)
					{
						for (int i = 0; i < 10; i++)
						{
							string text3 = requestHttp.RequestGet(str + "/api/monitoring/traffic-statistics");
							bool flag9 = !text3.Contains("<CurrentUpload>0");
							bool flag10 = flag9;
							if (flag10)
							{
								break;
							}
							Thread.Sleep(1000);
						}
					}
					return Convert.ToInt32(Regex.Match(text, "dataswitch>(.*?)<").Groups[1].Value);
				}
				result = -1;
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00016A78 File Offset: 0x00014C78
		public static string RunCmdDcom(string cmdCommand)
		{
			string result;
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = "rasdial.exe",
					Arguments = cmdCommand,
					CreateNoWindow = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardInput = true,
					RedirectStandardOutput = true
				};
				process.Start();
				process.WaitForExit();
				string text = process.StandardOutput.ReadToEnd();
				result = text;
				process.Close();
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00016B18 File Offset: 0x00014D18
		public static void InputText(string index, string text)
		{
			Helper.ExcuteCMD(index, "shell input text \"" + text + "\"", 0);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00016B40 File Offset: 0x00014D40
		public static void ImportContact(string index)
		{
			string[] array = File.ReadAllLines("contacts\\phone.txt");
			string[] array2 = File.ReadAllLines("contacts\\namecontacts.txt");
			string str = index.Replace(" ", "_");
			string text = Directory.GetCurrentDirectory() + "\\contacts\\contact_" + str + ".vcf";
			Random random = new Random();
			try
			{
				File.Delete(text);
			}
			catch
			{
			}
			string text2 = "";
			int num = new Random().Next(40, 60);
			for (int i = 0; i < num; i++)
			{
				string str2 = array[random.Next(0, array.Length - 1)];
				string str3 = array2[random.Next(0, array2.Length - 1)] + " " + random.Next().ToString();
				text2 += "BEGIN:VCARD\n";
				text2 += "VERSION:2.1\n";
				text2 = text2 + "N:;" + str3 + ";;;\n";
				text2 = text2 + "FN:" + str3 + "\n";
				text2 = text2 + "TEL;CELL:" + str2 + "\n";
				text2 += "END:VCARD\n";
			}
			File.AppendAllText(text, text2);
			Helper.ClearPackage(index, "com.android.providers.contacts");
			string text3 = Helper.ExcuteCMD(index, "push \"" + text + "\" \"/sdcard/Contacts.vcf\"", 2000);
			text3 = Helper.ExcuteCMD(index, "shell am start -t text/vcard -d file:///sdcard/Contacts.vcf -a android.intent.action.VIEW com.android.contacts", 2000);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00016CD4 File Offset: 0x00014ED4
		public static void InputUnicode(string index, string textpull)
		{
			Helper.ExcuteCMD(index, "shell ime set com.android.adbkeyboard/.AdbIME", 0);
			char[] array = textpull.ToCharArray();
			foreach (char c in array)
			{
				Helper.ExcuteCMD(index, "shell am broadcast -a ADB_INPUT_B64 --es msg \"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(c.ToString())) + "\"", 0);
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00016D3C File Offset: 0x00014F3C
		public static void KeyEvent(string index, int key)
		{
			Helper.ExcuteCMD(index, "shell input keyevent " + key.ToString(), 0);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00016D64 File Offset: 0x00014F64
		public static string Swipe(string index, int x1, int y1, int x2, int y2, int duration = 500)
		{
			return Helper.ExcuteCMD(index, string.Format("shell input swipe {0} {1} {2} {3} {4}", new object[]
			{
				x1,
				y1,
				x2,
				x2,
				duration
			}), 0);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00016DBC File Offset: 0x00014FBC
		private static string rdmgmail(int gmaillenght)
		{
			string text = "abcdefghijklmnopqrstuvwxyz";
			string text2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string text3 = "0123456789";
			string text4 = text + text2 + text3;
			Random random = new Random();
			string text5 = "";
			text5 = string.Concat(new string[]
			{
				text5,
				text[random.Next(0, text.Length)].ToString(),
				text[random.Next(0, text.Length)].ToString(),
				text[random.Next(0, text.Length)].ToString(),
				text[random.Next(0, text.Length)].ToString(),
				text[random.Next(0, text.Length)].ToString()
			});
			text5 = text5 + text2[random.Next(0, text2.Length)].ToString() + text2[random.Next(0, text2.Length)].ToString() + text2[random.Next(0, text2.Length)].ToString();
			text5 = text5 + text3[random.Next(0, text3.Length)].ToString() + text3[random.Next(0, text3.Length)].ToString() + text3[random.Next(0, text3.Length)].ToString();
			for (int i = 0; i <= gmaillenght - 9; i++)
			{
				text5 += text4[random.Next(0, text4.Length)].ToString();
			}
			string text6 = text5;
			string str = "";
			Random random2 = new Random();
			while (text6.Length > 0)
			{
				int startIndex = random2.Next(0, text6.Length);
				str += text6.Substring(startIndex, 1);
				text6 = text6.Remove(startIndex, 1);
			}
			return text5;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00017008 File Offset: 0x00015208
		public static void ChangeInfoLD(string index)
		{
			Random random = new Random();
			string[] array = "+8486|+8496|+8497|+8498|+8432|+8433|+8434|+8435|+8436|+8437|+8438|+8439|+8488|+8491|+8494|+8483|+8484|+8485|+8481|+8482|+8489|+8490|+8493|+8470|+8479|+8477|+8476|+8478|+8492|+8456|+8458|+8499|+8459".Split(new char[]
			{
				'|'
			});
			string text = array[random.Next(array.Length)] + Helper.CreateRandomNumber(7, random);
			string text2 = "86516602" + Helper.CreateRandomNumber(7, random);
			string text3 = "46000" + Helper.CreateRandomNumber(10, random);
			string text4 = "898600" + Helper.CreateRandomNumber(14, random);
			string text5 = Helper.Md5Encode(Helper.CreateRandomStringNumber(32, random), "x2").Substring(random.Next(0, 16), 16);
			string path = "DATA/deviceinfo.json";
			string json = File.ReadAllText(path);
			JToken jtoken = JToken.Parse(json);
			JToken jtoken2 = jtoken[random.Next(0, jtoken.Count<JToken>() - 1)];
			JToken jtoken3 = jtoken2["manufacturer"];
			JToken jtoken4 = jtoken2["models"];
			JToken jtoken5 = jtoken4[random.Next(0, jtoken4.Count<JToken>() - 1)];
			string cmd = string.Concat(new object[]
			{
				"modify --",
				index,
				" --imei ",
				text2,
				" --model \"",
				jtoken5.ToString(),
				"\" --manufacturer ",
				jtoken3.ToString(),
				" --pnumber ",
				text,
				" --imsi ",
				text3,
				" --simserial ",
				text4,
				" --androidid ",
				text5,
				" --resolution 320,480,120",
				" --cpu 1 --memory 1024",
				" --mac"
			});
			Helper.ExecuteLDP(cmd);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000171C0 File Offset: 0x000153C0
		public static string CreateRandomStringNumber(int lengText, Random rd = null)
		{
			string text = "";
			bool flag = rd == null;
			bool flag2 = flag;
			if (flag2)
			{
				rd = new Random();
			}
			string text2 = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			for (int i = 0; i < lengText; i++)
			{
				text += text2[rd.Next(0, text2.Length)].ToString();
			}
			return text;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00017230 File Offset: 0x00015430
		public static string Md5Encode(string text, string type = "X2")
		{
			MD5 md = MD5.Create();
			byte[] array = md.ComputeHash(Encoding.UTF8.GetBytes(text));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString(type));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00017294 File Offset: 0x00015494
		public static string CreateRandomNumber(int leng, Random rd = null)
		{
			string text = "";
			bool flag = rd == null;
			bool flag2 = flag;
			if (flag2)
			{
				rd = new Random();
			}
			string text2 = "0123456789";
			for (int i = 0; i < leng; i++)
			{
				text += text2[rd.Next(0, text2.Length)].ToString();
			}
			return text;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00017304 File Offset: 0x00015504
		public static void PullImg(string index, string path)
		{
			string str = index.Replace(" ", "");
			string text = "screenShoot" + str + ".png";
			Helper.ExcuteCMD(index, "shell mount -o rw,remount /", 0);
			Thread.Sleep(200);
			Helper.ExcuteCMD(index, "shell rm -rR /sdcard/launcher/ad", 0);
			Helper.ExcuteCMD(index, "shell rm -rR /sdcard/DCIM/Foleravt", 0);
			Helper.ExcuteCMD(index, "shell am broadcast -a android.intent.action.MEDIA_MOUNTED -d file://sdcard", 0);
			Thread.Sleep(200);
			Thread.Sleep(200);
			Helper.ExcuteCMD(index, "shell mkdir /sdcard/DCIM/Foleravt", 0);
			string arguments = string.Concat(new string[]
			{
				"adb --",
				index,
				" --command \"push \"",
				Helper.GetFilesFrom(path),
				"\" \"/sdcard/DCIM/Foleravt\"\""
			});
			Process process = new Process();
			process.StartInfo = new ProcessStartInfo
			{
				FileName = Helper.pathLD,
				Arguments = arguments,
				CreateNoWindow = true,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				RedirectStandardInput = true,
				RedirectStandardOutput = true
			};
			process.Start();
			string text2 = process.StandardOutput.ReadToEnd();
			process.Close();
			Thread.Sleep(200);
			Helper.ExcuteCMD(index, "shell am broadcast -a android.intent.action.MEDIA_MOUNTED -d file://sdcard/DCIM", 0);
			Thread.Sleep(200);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00017458 File Offset: 0x00015658
		public static string GetFilesFrom(string path)
		{
			string[] array = new string[]
			{
				"jpg",
				"png"
			};
			List<string> list = new List<string>();
			SearchOption searchOption = SearchOption.TopDirectoryOnly;
			foreach (string arg in array)
			{
				list.AddRange(Directory.GetFiles(path, string.Format("*.{0}", arg), searchOption));
			}
			return list[new Random().Next(list.Count)];
		}

		// Token: 0x040000F7 RID: 247
		public static string pathLD = "" + "\\ldconsole.exe";

		// Token: 0x0200001A RID: 26
		public class RequestHttp
		{
			// Token: 0x060001AE RID: 430 RVA: 0x0001C558 File Offset: 0x0001A758
			public RequestHttp(string cookie = "", string userAgent = "", string proxy = "", int typeProxy = 0)
			{
				bool flag = userAgent == "";
				bool flag2 = flag;
				if (flag2)
				{
					this.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";
				}
				else
				{
					this.UserAgent = userAgent;
				}
				this.request = new RequestHTTP();
				this.request.SetSSL(SecurityProtocolType.Tls12);
				this.request.SetKeepAlive(true);
				this.request.SetDefaultHeaders(new string[]
				{
					"content-type: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
					"user-agent: " + this.UserAgent
				});
				bool flag3 = cookie != "";
				bool flag4 = flag3;
				if (flag4)
				{
					this.AddCookie(cookie);
				}
				this.Proxy = proxy;
			}

			// Token: 0x060001AF RID: 431 RVA: 0x0001C610 File Offset: 0x0001A810
			public string RequestGet(string url)
			{
				bool flag = this.Proxy != "";
				bool flag2 = flag;
				string result;
				if (flag2)
				{
					bool flag3 = this.Proxy.Contains(":");
					bool flag4 = flag3;
					if (flag4)
					{
						result = this.request.Request("GET", url, null, null, true, new WebProxy(this.Proxy.Split(new char[]
						{
							':'
						})[0], Convert.ToInt32(this.Proxy.Split(new char[]
						{
							':'
						})[1])), 60000).ToString();
					}
					else
					{
						result = this.request.Request("GET", url, null, null, true, new WebProxy("127.0.0.1", Convert.ToInt32(this.Proxy)), 60000).ToString();
					}
				}
				else
				{
					result = this.request.Request("GET", url, null, null, true, null, 60000).ToString();
				}
				return result;
			}

			// Token: 0x060001B0 RID: 432 RVA: 0x0001C710 File Offset: 0x0001A910
			public string RequestPost(string url, string data = "")
			{
				bool flag = this.Proxy != "";
				bool flag2 = flag;
				string result;
				if (flag2)
				{
					bool flag3 = this.Proxy.Contains(":");
					bool flag4 = flag3;
					if (flag4)
					{
						result = this.request.Request("POST", url, null, Encoding.ASCII.GetBytes(data), true, new WebProxy(this.Proxy.Split(new char[]
						{
							':'
						})[0], Convert.ToInt32(this.Proxy.Split(new char[]
						{
							':'
						})[1])), 60000).ToString();
					}
					else
					{
						result = this.request.Request("POST", url, null, Encoding.ASCII.GetBytes(data), true, new WebProxy("127.0.0.1", Convert.ToInt32(this.Proxy)), 60000).ToString();
					}
				}
				else
				{
					result = this.request.Request("POST", url, null, Encoding.ASCII.GetBytes(data), true, null, 60000).ToString();
				}
				return result;
			}

			// Token: 0x060001B1 RID: 433 RVA: 0x0001C830 File Offset: 0x0001AA30
			public void AddCookie(string cookie)
			{
				string[] array = cookie.Split(new char[]
				{
					';'
				});
				string text = "";
				foreach (string text2 in array)
				{
					string[] array3 = text2.Split(new char[]
					{
						'='
					});
					bool flag = array3.Count<string>() > 1;
					bool flag2 = flag;
					if (flag2)
					{
						try
						{
							text = string.Concat(new string[]
							{
								text,
								array3[0],
								"=",
								array3[1],
								";"
							});
						}
						catch
						{
						}
					}
				}
				this.request.SetDefaultHeaders(new string[]
				{
					"content-type: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8;charset=UTF-8",
					"user-agent: " + this.UserAgent,
					"cookie: " + text
				});
			}

			// Token: 0x060001B2 RID: 434 RVA: 0x0001C928 File Offset: 0x0001AB28
			public string GetCookie()
			{
				return this.request.GetCookiesString();
			}

			// Token: 0x04000141 RID: 321
			public RequestHTTP request;

			// Token: 0x04000142 RID: 322
			private string UserAgent;

			// Token: 0x04000143 RID: 323
			private string Proxy;
		}
	}
}
