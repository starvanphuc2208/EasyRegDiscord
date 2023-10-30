namespace MCommon
{
    using AE.Net.Mail;
    using Newtonsoft.Json.Linq;
    using OtpNet;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using File = System.IO.File;
    using Ionic.Zip;

    public class Common
    {
        private static readonly string[] VietnameseSigns = new string[]
       {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
       };

        public static string CreateRandomStringAndInt(int lengText, Random rd = null)
        {
            string text = "";
            if (rd == null)
            {
                rd = new Random();
            }
            string text2 = "abcdefghik1236574";
            for (int i = 0; i < lengText; i++)
            {
                text += text2[rd.Next(0, text2.Length)].ToString();
            }
            return text;
        }

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        public static string ConvertNgayHetHan(string input)
        {
            string result = "";
            DateTime oDate = Convert.ToDateTime(input);
            return oDate.Day + "/" + oDate.Month + "/" + oDate.Year;
        }

        public static string GetRandomItemFromListNoDel(ref List<string> lst, Random rd)
        {
            string item = "";
            try
            {
                item = lst[rd.Next(0, lst.Count)];
            }
            catch (Exception)
            {

                throw;
            }
            return item;
        }

        public static bool CheckWithPercent(int percent)
        {
            int num = Base.rd.Next(1, 101);
            return num <= percent;
        }

        public static bool CheckFormIsOpenning(string nameForm)
        {
            try
            {
                FormCollection openForms = Application.OpenForms;
                foreach (object obj in openForms)
                {
                    Form form = (Form)obj;
                    if (form.Name == nameForm)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public static string ConvertListParamsToString(object lstParams)
        {
            string text = "";
            try
            {
                foreach (PropertyInfo propertyInfo in lstParams.GetType().GetProperties())
                {
                    string str = text;
                    object value = propertyInfo.GetValue(lstParams);
                    text = str + ((value != null) ? value.ToString() : null) + ",";
                }
                text = text.TrimEnd(new char[]
                {
                    ','
                });
            }
            catch
            {
            }
            return text;
        }

        public static string GetStatusDataGridView(DataGridView dgv, int row, string colName)
        {
            string output = "";

            try
            {
                try
                {
                    dgv.Invoke(new MethodInvoker(delegate ()
                    {
                        output = dgv.Rows[row].Cells[colName].Value.ToString();
                    }));
                }
                catch
                {
                    output = dgv.Rows[row].Cells[colName].Value.ToString();
                }
            }
            catch
            {
            }

            return output;
        }

        public static void SetStatusDataGridView(DataGridView dgv, int row, string colName, object status)
        {
            try
            {
                Application.DoEvents();
                dgv.Invoke(new MethodInvoker(delegate ()
                {
                    dgv.Rows[row].Cells[colName].Value = status;
                }));
            }
            catch { }
        }

        public static List<string> GetIntersectItemBetweenTwoList(List<string> lstRoot, List<string> lstCompare)
        {
            List<string> result = new List<string>();
            try
            {
                result = lstRoot.Intersect(lstCompare).ToList<string>();
            }
            catch
            {
            }
            return result;
        }

        public static List<string> GetExceptItemBetweenTwoList(List<string> lstRoot, List<string> lstCompare)
        {
            List<string> result = new List<string>();
            try
            {
                result = lstRoot.Except(lstCompare).ToList<string>();
            }
            catch
            {
            }
            return result;
        }

        private static void Enable(string interfaceName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" enable");
            new Process
            {
                StartInfo = startInfo
            }.Start();
        }

        private static void Disable(string interfaceName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" disable");
            new Process
            {
                StartInfo = startInfo
            }.Start();
        }

        public static string GetDateCreatFolder(string pathFolder)
        {
            try
            {
                return Directory.GetCreationTime(pathFolder).ToString("yyyy/MM/dd HH:mm:ss");
            }
            catch (Exception)
            {
            }
            return "";
        }

        public static string GetDateCreatFile(string pathFile)
        {
            try
            {
                return File.GetCreationTime(pathFile).ToString("yyyy/MM/dd HH:mm:ss");
            }
            catch (Exception)
            {
            }
            return "";
        }

        public static string GetRandomItemFromList(ref List<string> lst, Random rd)
        {
            string text = "";
            try
            {
                text = lst[rd.Next(0, lst.Count)];
                lst.Remove(text);
            }
            catch (Exception)
            {
                throw;
            }
            return text;
        }

        public static bool CloseForm(string nameForm)
        {
            try
            {
                IEnumerator enumerator = Application.OpenForms.GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            Form current = (Form)enumerator.Current;
                            if (current.Name == nameForm)
                            {
                                current.Invoke(new MethodInvoker(() => current.Close()));
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public static string CheckAccountHotmail(string username, string password)
        {
            int num = 0;
            string result;
            for (; ; )
            {
                try
                {
                    string text = "";
                    if (username.EndsWith("@hotmail.com") || username.EndsWith("@outlook.com"))
                    {
                        text = "outlook.office365.com";
                    }
                    else if (username.EndsWith("@yandex.com"))
                    {
                        text = "imap.yandex.com";
                    }
                    if (text == "")
                    {
                        result = "0";
                        break;
                    }
                    ImapClient imapClient = new ImapClient(text, username, password, AuthMethods.Login, 993, true, false);
                    imapClient.Dispose();
                    result = "1";
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("The remote certificate is invalid according to the validation procedure"))
                    {
                        num++;
                        if (num < 10)
                        {
                            continue;
                        }
                    }
                    result = "0";
                    break;
                }
            }
            return result;
        }

        public static string CheckAccountEmail(string username, string password, string imap)
        {
            int num = 0;
            string result;
            for (; ; )
            {
                try
                {
                    ImapClient imapClient = new ImapClient(imap, username, password, AuthMethods.Login, 993, true, false);
                    imapClient.Dispose();
                    result = "1";
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("The remote certificate is invalid according to the validation procedure"))
                    {
                        num++;
                        if (num < 10)
                        {
                            continue;
                        }
                    }
                    result = "0";
                    break;
                }
            }
            return result;
        }

        public static string ConvertSecondsToTime(int seconds)
        {
            string result;
            try
            {
                TimeSpan.FromSeconds((double)seconds);
                if (seconds < 60)
                {
                    result = TimeSpan.FromSeconds((double)seconds).ToString("ss");
                }
                else if (seconds < 3600)
                {
                    result = TimeSpan.FromSeconds((double)seconds).ToString("mm\\:ss");
                }
                else
                {
                    result = TimeSpan.FromSeconds((double)seconds).ToString("hh\\:mm\\:ss");
                }
            }
            catch
            {
                result = "";
            }
            return result;
        }

        public static int CompareImage(string pathFile1, string pathFile2)
        {
            int result = 0;
            try
            {
                List<bool> hash = Common.GetHash(new Bitmap(pathFile1));
                List<bool> hash2 = Common.GetHash(new Bitmap(pathFile1));
                result = hash.Zip(hash2, (bool i, bool j) => i == j).Count((bool eq) => eq);
            }
            catch
            {
            }
            return result;
        }

        public static bool SetTextToClipboard(string content)
        {
            bool isSuccess = false;
            try
            {
                Thread thread = new Thread(delegate ()
                {
                    try
                    {
                        Clipboard.SetText(content);
                        isSuccess = true;
                    }
                    catch
                    {
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
            catch
            {
            }
            return isSuccess;
        }

        public static List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> list = new List<bool>();
            Bitmap bitmap = new Bitmap(bmpSource, new Size(16, 16));
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    list.Add(bitmap.GetPixel(j, i).GetBrightness() < 0.5f);
                }
            }
            return list;
        }

        public static List<string> CloneList(List<string> lstFrom)
        {
            List<string> list = new List<string>();
            try
            {
                for (int i = 0; i < lstFrom.Count; i++)
                {
                    list.Add(lstFrom[i]);
                }
            }
            catch
            {
            }
            return list;
        }

        public static string SpinText(string text, Random rand)
        {
            int num = -1;
            char[] anyOf = new char[]
            {
                '{',
                '}'
            };
            text += "~";
            do
            {
                int num2 = num;
                num = -1;
                while ((num2 = text.IndexOf('{', num2 + 1)) != -1)
                {
                    int num3 = num2;
                    while ((num3 = text.IndexOfAny(anyOf, num3 + 1)) != -1 && text[num3] != '}')
                    {
                        if (num == -1)
                        {
                            num = num2;
                        }
                        num2 = num3;
                    }
                    if (num3 != -1)
                    {
                        string[] array = text.Substring(num2 + 1, num3 - 1 - (num2 + 1 - 1)).Split(new char[]
                        {
                            '|'
                        });
                        text = text.Remove(num2, num3 - (num2 - 1)).Insert(num2, array[rand.Next(array.Length)]);
                    }
                }
            }
            while (num-- != -1);
            return text.Remove(text.Length - 1);
        }

        public static DateTime ConvertTimeStampToDateTime(double timestamp)
        {
            DateTime result = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            result = result.AddSeconds(timestamp).ToLocalTime();
            return result;
        }

        public static Form GetFormByName(string name, string para)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (Type type in executingAssembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.FullName == "System.Windows.Forms.Form" && type.FullName == name)
                {
                    return Activator.CreateInstance(Type.GetType(name), new object[]
                    {
                        "",
                        1,
                        para
                    }) as Form;
                }
            }
            return null;
        }

        public static void CreateFile(string pathFile)
        {
            try
            {
                if (!File.Exists(pathFile))
                {
                    File.AppendAllText(pathFile, "");
                }
            }
            catch
            {
            }
        }

        public static void ClearSelectedOnDatagridview(DataGridView dtgv)
        {
            for (int i = 0; i < dtgv.RowCount; i++)
            {
                dtgv.Rows[i].Selected = false;
            }
        }

        public static void CreateFolder(string pathFolder)
        {
            try
            {
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
            }
            catch
            {
            }
        }

        public static void ShowForm(Form f)
        {
            try
            {
                f.ShowInTaskbar = false;
                f.ShowDialog();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        public static void ShowMessageBox(object s, int type)
        {
            switch (type)
            {
                case 1:
                    MessageBox.Show(s.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    break;
                case 2:
                    MessageBox.Show(s.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    break;
                case 3:
                    MessageBox.Show(s.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
        }

        public static List<int> ShuffleList(List<int> lst)
        {
            int i = lst.Count;
            while (i != 0)
            {
                int index = Base.rd.Next(0, lst.Count);
                i--;
                int value = lst[i];
                lst[i] = lst[index];
                lst[index] = value;
            }
            return lst;
        }

        public static List<string> ShuffleList(List<string> lst)
        {
            int i = lst.Count;
            while (i != 0)
            {
                int index = Base.rd.Next(0, lst.Count);
                i--;
                string value = lst[i];
                lst[i] = lst[index];
                lst[index] = value;
            }
            return lst;
        }

        public static List<string> RemoveEmptyItems(List<string> lst)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < lst.Count; i++)
            {
                string text = lst[i].Trim();
                if (text != "")
                {
                    list.Add(text);
                }
            }
            return list;
        }

        public static string RunCMD(string fileName, string cmd, int timeout = 10)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = cmd;
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

        public static bool ResetDcom(string profileDcom)
        {
            bool flag = false;
            string str = Common.RunCMD("rasdial.exe", string.Concat("\"", profileDcom, "\""), 3);
            if (str.Contains("Successfully connected to "))
            {
                flag = true;
            }
            else if (!str.Contains("You are already connected to "))
            {
                flag = false;
            }
            else
            {
                int num = 0;
                while (true)
                {
                    if (num < 3)
                    {
                        str = Common.RunCMD("rasdial.exe", string.Concat("\"", profileDcom, "\" /disconnect"), 3);
                        if (str.Trim() == "Command completed successfully.")
                        {
                            flag = true;
                            break;
                        }
                        else
                        {
                            Common.DelayTime(1);
                            num++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                Common.DelayTime(1);
                if (flag)
                {
                    int num1 = 0;
                    while (num1 < 3)
                    {
                        str = Common.RunCMD("rasdial.exe", string.Concat("\"", profileDcom, "\""), 3);
                        if (str.Contains("Successfully connected to "))
                        {
                            flag = true;
                            goto Label0;
                        }
                        else
                        {
                            Common.DelayTime(1);
                            num1++;
                        }
                    }
                }
            Label0:
                Common.DelayTime(1);
            }
            return flag;
        }

        public static string TrimEnd(string text, string value)
        {
            string result;
            if (!text.EndsWith(value))
            {
                result = text;
            }
            else
            {
                result = text.Remove(text.LastIndexOf(value));
            }
            return result;
        }

        public static void SaveDatagridview(DataGridView dgv, string FilePath, char splitChar = '|')
        {
            List<string> list = new List<string>();
            for (int i = 0; i < dgv.RowCount; i++)
            {
                string text = "";
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    object value = dgv.Rows[i].Cells[j].Value;
                    text += ((value == null) ? splitChar.ToString() : (((value != null) ? value.ToString() : null) + splitChar.ToString()));
                }
                text = text.TrimEnd(new char[]
                {
                    splitChar
                });
                list.Add(text);
            }
            File.WriteAllLines(FilePath, list);
        }

        public static void LoadDatagridview(DataGridView dgv, string namePath, char splitChar = '|')
        {
            if (!File.Exists(namePath))
            {
                Common.CreateFile(namePath);
            }
            List<string> list = File.ReadAllLines(namePath).ToList<string>();
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string text = list[i];
                    DataGridViewRowCollection rows = dgv.Rows;
                    object[] values = text.Split(new char[]
                    {
                        splitChar
                    });
                    rows.Add(values);
                }
            }
        }

        public static string SelectFolder()
        {
            string result = "";
            try
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    DialogResult dialogResult = folderBrowserDialog.ShowDialog();
                    if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        result = folderBrowserDialog.SelectedPath;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public static string SelectFile(string title = "Chọn File txt", string typeFile = "txt Files (*.txt)|*.txt|")
        {
            string result = "";
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = typeFile + "All files (*.*)|*.*";
                    openFileDialog.InitialDirectory = "C:\\";
                    openFileDialog.Title = title;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        result = openFileDialog.FileName;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public static void KillProcess(string nameProcess)
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName(nameProcess))
                {
                    process.Kill();
                }
            }
            catch
            {
            }
        }

        public static bool CheckBasicString(string text)
        {
            bool result = true;
            foreach (char c in text)
            {
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '.')
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }

        public static string RemoveCharNotLatin(string text)
        {
            string text2 = "";
            foreach (char c in text)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    text2 += c.ToString();
                }
            }
            return text2;
        }

        public static string ConvertToUTF8(string text)
        {
            byte[] bytes = Encoding.Default.GetBytes(text);
            text = Encoding.UTF8.GetString(bytes);
            return text;
        }

        public static bool IsNumber(string pValue)
        {
            bool result;
            if (pValue == "")
            {
                result = false;
            }
            else
            {
                foreach (char c in pValue)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }
                result = true;
            }
            return result;
        }

        public static bool IsContainNumber(string pValue)
        {
            foreach (char c in pValue)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }

        public static void ReadHtmlText(string html)
        {
            string text = "zzz999.html";
            File.WriteAllText(text, html);
            Process.Start(text);
        }

        public static string ReadHTMLCode(string Url)
        {
            string result;
            try
            {
                result = new RequestXNet("", "", "", 0).RequestGet(Url);
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public static bool IsValidMail(string emailaddress)
        {
            bool result;
            try
            {
                new MailAddress(emailaddress);
                result = true;
            }
            catch (FormatException)
            {
                result = false;
            }
            return result;
        }

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

        public static string Base64Encode(string base64Decoded)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(base64Decoded);
            return Convert.ToBase64String(bytes);
        }

        public static string Base64Decode(string base64Encoded)
        {
            byte[] bytes = Convert.FromBase64String(base64Encoded);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string CreateRandomStringNumber(int lengText, Random rd = null)
        {
            string text = "";
            if (rd == null)
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

        public static string CreateRandomString(int lengText, Random rd = null)
        {
            string text = "";
            if (rd == null)
            {
                rd = new Random();
            }
            string text2 = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < lengText; i++)
            {
                text += text2[rd.Next(0, text2.Length)].ToString();
            }
            return text;
        }

        public static string CreateRandomNumber(int leng, Random rd = null)
        {
            string text = "";
            if (rd == null)
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

        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string input = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(input, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
        }

        public static string RunCMD(string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + cmd;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string text = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            string result;
            if (string.IsNullOrEmpty(text))
            {
                result = "";
            }
            else
            {
                result = text;
            }
            return result;
        }

        public static void DelayTime(double second)
        {
            Application.DoEvents();
            Thread.Sleep(Convert.ToInt32(second * 1000.0));
        }

        public static string HtmlDecode(string text)
        {
            return WebUtility.HtmlDecode(text);
        }

        public static string HtmlEncode(string text)
        {
            return WebUtility.HtmlEncode(text);
        }

        public static string UrlDecode(string text)
        {
            return WebUtility.UrlDecode(text);
        }

        public static string UrlEncode(string text)
        {
            return WebUtility.UrlEncode(text);
        }

        public static Point GetSizeChrome(int column, int row)
        {
            JSON_Settings json_Settings = new JSON_Settings("configChrome", false);
            if (json_Settings.GetValueInt("width", 0) == 0)
            {
                json_Settings.Update("width", Common.getWidthScreen);
                json_Settings.Update("heigh", Common.getHeightScreen);
                json_Settings.Save("");
            }
            Common.getWidthScreen = json_Settings.GetValueInt("width", 0);
            Common.getHeightScreen = json_Settings.GetValueInt("heigh", 0);
            int x = Common.getWidthScreen / column + 15;
            int y = Common.getHeightScreen / row + 10;
            return new Point(x, y);
        }

        public static Point GetPointFromIndexPosition(int indexPos, int column, int row)
        {
            JSON_Settings json_Settings = new JSON_Settings("configChrome", false);
            if (json_Settings.GetValueInt("width", 0) == 0)
            {
                json_Settings.Update("width", Common.getWidthScreen);
                json_Settings.Update("heigh", Common.getHeightScreen);
                json_Settings.Save("");
            }
            Common.getWidthScreen = json_Settings.GetValueInt("width", 0);
            Common.getHeightScreen = json_Settings.GetValueInt("heigh", 0);
            Point result = default(Point);
            while (indexPos >= column * row)
            {
                indexPos -= column * row;
            }
            switch (row)
            {
                case 1:
                    result.Y = 0;
                    break;
                case 2:
                    if (indexPos < column)
                    {
                        result.Y = 0;
                    }
                    else if (indexPos < column * 2)
                    {
                        result.Y = Common.getHeightScreen / 2;
                        indexPos -= column;
                    }
                    break;
                case 3:
                    if (indexPos < column)
                    {
                        result.Y = 0;
                    }
                    else if (indexPos < column * 2)
                    {
                        result.Y = Common.getHeightScreen / 3;
                        indexPos -= column;
                    }
                    else if (indexPos < column * 3)
                    {
                        result.Y = Common.getHeightScreen / 3 * 2;
                        indexPos -= column * 2;
                    }
                    break;
                case 4:
                    if (indexPos < column)
                    {
                        result.Y = 0;
                    }
                    else if (indexPos < column * 2)
                    {
                        result.Y = Common.getHeightScreen / 4;
                        indexPos -= column;
                    }
                    else if (indexPos < column * 3)
                    {
                        result.Y = Common.getHeightScreen / 4 * 2;
                        indexPos -= column * 2;
                    }
                    else if (indexPos < column * 4)
                    {
                        result.Y = Common.getHeightScreen / 4 * 3;
                        indexPos -= column * 3;
                    }
                    break;
                case 5:
                    if (indexPos < column)
                    {
                        result.Y = 0;
                    }
                    else if (indexPos < column * 2)
                    {
                        result.Y = Common.getHeightScreen / 5;
                        indexPos -= column;
                    }
                    else if (indexPos < column * 3)
                    {
                        result.Y = Common.getHeightScreen / 5 * 2;
                        indexPos -= column * 2;
                    }
                    else if (indexPos < column * 4)
                    {
                        result.Y = Common.getHeightScreen / 5 * 3;
                        indexPos -= column * 3;
                    }
                    else
                    {
                        result.Y = Common.getHeightScreen / 5 * 4;
                        indexPos -= column * 4;
                    }
                    break;
            }
            int num = Common.getWidthScreen / column;
            result.X = indexPos * num - 10;
            return result;
        }

        public static int GetIndexOfPossitionApp(ref List<int> lstPossition)
        {
            int result = 0;
            List<int> obj = lstPossition;
            lock (obj)
            {
                for (int i = 0; i < lstPossition.Count; i++)
                {
                    if (lstPossition[i] == 0)
                    {
                        result = i;
                        lstPossition[i] = 1;
                        break;
                    }
                }
            }
            return result;
        }

        public static void FillIndexPossition(ref List<int> lstPossition, int indexPos)
        {
            List<int> obj = lstPossition;
            lock (obj)
            {
                lstPossition[indexPos] = 0;
            }
        }



        public static double ConvertDatetimeToTimestamp(DateTime value)
        {
            return (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
        }

        public static string CheckProxy(string proxy, int typeProxy)
        {
            string text = "";
            try
            {
                RequestHttp requestXNet = new RequestHttp("", SetupFolder.GetUseragentIPhone(Common.rd), proxy, typeProxy);
                text = requestXNet.RequestGet("https://whatismyv6.com/api/");
                text = text.Split(new char[]
                {
                    ','
                })[1];
            }
            catch
            {
                text = Common.CheckProxy3(proxy, typeProxy);
            }
            return text;
        }

        public static string CheckProxy3(string proxy, int typeProxy)
        {
            string result = "";
            try
            {
                RequestHttp requestXNet = new RequestHttp("", SetupFolder.GetUseragentIPhone(Common.rd), proxy, typeProxy);
                result = requestXNet.RequestGet("https://api64.ipify.org/");
            }
            catch (Exception)
            {
                result = Common.CheckProxy2(proxy, typeProxy);
            }
            return result;
        }

        public static string CheckProxy2(string proxy, int typeProxy)
        {
            string result = "";
            try
            {
                RequestHttp requestXNet = new RequestHttp("", SetupFolder.GetUseragentIPhone(Common.rd), proxy, typeProxy);
                string input = requestXNet.RequestGet("https://showip.net/");
                result = Regex.Match(input, "value=\"(.*?)\"").Groups[1].Value;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "Check Proxy2");
            }
            return result;
        }

        public static string CheckProxyOnlyV4(string proxy, int typeProxy)
        {
            string result = "";
            try
            {
                RequestXNet requestXNet = new RequestXNet("", "", proxy, typeProxy);
                string json = requestXNet.RequestGet("http://lumtest.com/myip.json");
                result = JObject.Parse(json)["ip"].ToString();
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "Check Proxy");
            }
            return result;
        }

        public static string CheckIP()
        {
            string result = "";
            try
            {
                RequestXNet requestXNet = new RequestXNet("", "", "", 0);
                string json = requestXNet.RequestGet("http://lumtest.com/myip.json");
                result = JObject.Parse(json)["ip"].ToString();
            }
            catch
            {
            }
            return result;
        }

        public static string SendMessageTelegram(string text)
        {
            string apilToken = "5420589976:AAGWzCpNuOlRR30aLZwUx0mof-wL0HR83UU";
            string destID = "-638018095";
            string urlString = $"https://api.telegram.org/bot{apilToken}/sendMessage?chat_id={destID}&text={text}";

            WebClient webclient = new WebClient();

            return webclient.DownloadString(urlString);
        }

        public static void OutData(string error = "")
        {
            try
            {
                if (!Directory.Exists("log"))
                {
                    Directory.CreateDirectory("log");
                }
                using (StreamWriter streamWriter = new StreamWriter("log\\log_data.txt", true))
                {
                    streamWriter.WriteLine("-----------------------------------------------------------------------------");
                    streamWriter.WriteLine("Date: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    if (error != "")
                    {
                        streamWriter.WriteLine(error);
                    }
                    streamWriter.WriteLine();
                }
            }
            catch
            {
            }
        }

        public static void ExportError(Exception ex, string error = "")
        {
            try
            {
                if (!Directory.Exists("log"))
                {
                    Directory.CreateDirectory("log");
                }
                if (!Directory.Exists("log\\html"))
                {
                    Directory.CreateDirectory("log\\html");
                }
                if (!Directory.Exists("log\\images"))
                {
                    Directory.CreateDirectory("log\\images");
                }
                Random random = new Random();
                string text = string.Concat(new string[]
                {
                    DateTime.Now.Day.ToString(),
                    "_",
                    DateTime.Now.Month.ToString(),
                    "_",
                    DateTime.Now.Year.ToString(),
                    "_",
                    DateTime.Now.Hour.ToString(),
                    "_",
                    DateTime.Now.Minute.ToString(),
                    "_",
                    DateTime.Now.Second.ToString(),
                    "_",
                    random.Next(1000, 9999).ToString()
                });
                using (StreamWriter streamWriter = new StreamWriter("log\\log.txt", true))
                {
                    streamWriter.WriteLine("-----------------------------------------------------------------------------");
                    streamWriter.WriteLine("Date: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    streamWriter.WriteLine("File: " + text);
                    if (error != "")
                    {
                        streamWriter.WriteLine("Error: " + error);
                    }
                    streamWriter.WriteLine();
                    if (ex != null)
                    {
                        streamWriter.WriteLine("Type: " + ex.GetType().FullName);
                        streamWriter.WriteLine("Message: " + ex.Message);
                        streamWriter.WriteLine("StackTrace: " + ex.StackTrace);
                        ex = ex.InnerException;
                    }
                }
            }
            catch
            {
            }
        }

        public static bool ChangeIP(int typeChangeIP, int typeDcom, string profileDcom, string urlHilink, int iTypeHotspot, string sLinkNord)
        {
            bool result = false;
            try
            {
                if (typeChangeIP == 0)
                {
                    return true;
                }
                if (typeChangeIP == 1)
                {
                    string b = Common.CheckIP();
                    IntPtr intPtr = AutoControl.FindWindowHandle(null, "HMA VPN");
                    AutoControl.BringToFront(intPtr);
                    AutoControl.SendClickOnPosition(AutoControl.FindHandle(intPtr, "Chrome_RenderWidgetHostHWND", "Chrome Legacy Window"), 356, 286, EMouseKey.LEFT, 1);
                    Thread.Sleep(5000);
                    string b2 = Common.CheckIP();
                    AutoControl.SendClickOnPosition(AutoControl.FindHandle(intPtr, "Chrome_RenderWidgetHostHWND", "Chrome Legacy Window"), 356, 286, EMouseKey.LEFT, 1);
                    int tickCount = Environment.TickCount;
                    string a;
                    do
                    {
                        a = Common.CheckIP();
                    }
                    while (Environment.TickCount - tickCount <= 20000 && (a == b || a == b2));
                    if (a != b)
                    {
                        result = true;
                    }
                }
                else if (typeChangeIP == 2)
                {
                    if (typeDcom == 0)
                    {
                        result = Common.ResetDcom(profileDcom);
                    }
                    else
                    {
                        int num = Common.ResetHilink(urlHilink);
                        if (num == 0)
                        {
                            Thread.Sleep(2000);
                            num = Common.ResetHilink(urlHilink);
                        }
                        result = (num == 1);
                    }
                }
                else if (typeChangeIP == 4 || typeChangeIP == 5 || typeChangeIP == 6)
                {
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "Error ChangeIP");
            }
            return result;
        }

        public static int ResetHilink(string urlHilink)
        {
            int result = -1;
            try
            {
                string str = "http" + Regex.Match(urlHilink, "http(.*?)/html").Groups[1].Value;
                RequestHttp requestHttp = new RequestHttp("", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36", "", 0);
                string text = requestHttp.RequestGet(urlHilink);
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
                string data;
                if (text.Contains("dataswitch>1"))
                {
                    data = text.Replace("response", "request").Replace("dataswitch>1", "dataswitch>0");
                }
                else
                {
                    if (!text.Contains("dataswitch>0"))
                    {
                        return -1;
                    }
                    data = text.Replace("response", "request").Replace("dataswitch>0", "dataswitch>1");
                }
                string text2 = requestHttp.RequestPost(str + "/api/dialup/mobile-dataswitch", data);
                if (text2.Contains("OK"))
                {
                    text = requestHttp.RequestGet(str + "/api/dialup/mobile-dataswitch");
                    if (text.Contains("dataswitch>1<"))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            string text3 = requestHttp.RequestGet(str + "/api/monitoring/traffic-statistics");
                            if (!text3.Contains("<CurrentUpload>0"))
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

        public static DateTime ConvertStringToDatetime(string datetime, string format = "dd/MM/yyyy HH:mm:ss")
        {
            return DateTime.ParseExact(datetime, format, CultureInfo.InvariantCulture);
        }

        public static string GetTotp(string input)
        {
            string text = Common.GetTotpServer(input);
            if (text == "")
            {
                text = Common.GetTotpClient(input);
            }
            return text;
        }

        public static string GetTotpServer(string input)
        {
            string str = "";
            try
            {
                string str1 = "";
                input = input.Replace(" ", "").Trim();
                string str2 = string.Concat("http://app.minsoftware.vn/api/2fa1?secret=", input);
                string str3 = string.Concat("http://2fa.live/tok/", input);
                int num = 0;
                while (num < 5)
                {
                    str = "";
                    try
                    {
                        str1 = Common.ReadHTMLCode(str3);
                        if (str1.Contains("token"))
                        {
                            JObject jObject = JObject.Parse(str1);
                            str = jObject["token"].ToString().Trim();
                        }
                    }
                    catch (Exception exception)
                    {
                        Common.ExportError(exception, str3);
                    }
                    try
                    {
                        if (str.Trim() == "")
                        {
                            str = Common.ReadHTMLCode(str2);
                        }
                    }
                    catch (Exception exception1)
                    {
                        Common.ExportError(exception1, str2);
                    }
                    if ((str == "" ? false : Common.IsNumber(str)))
                    {
                        for (int i = str.Length; i < 6; i++)
                        {
                            str = string.Concat("0", str);
                        }
                    }
                    else
                    {
                        Common.DelayTime(1);
                        num++;
                    }
                }
            }
            catch
            {
            }
            return str;
        }

        public static string GetTotpClient(string input)
        {
            try
            {
                byte[] secretKey = Base32Encoding.ToBytes(input.Trim().Replace(" ", ""));
                Totp totp = new Totp(secretKey, 30, OtpHashMode.Sha1, 6, null);
                return totp.ComputeTotp(DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "GetTotp(" + input + ")");
            }
            return "";
        }

        private static int RemainingSeconds()
        {
            return 30 - (int)((DateTime.UtcNow.Ticks - 621355968000000000L) / 10000000L % 30L);
        }

        private static byte[] GetBigEndianBytes(long input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            Array.Reverse(bytes);
            return bytes;
        }

        private static long CalculateTime30FromTimestamp(DateTime timestamp)
        {
            long num = (timestamp.Ticks - 621355968000000000L) / 10000000L;
            return num / 30L;
        }

        private static string Digits(long input, int digitCount)
        {
            return ((int)input % (int)Math.Pow(10.0, (double)digitCount)).ToString().PadLeft(digitCount, '0');
        }

        private static byte[] ToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }
            input = input.TrimEnd(new char[]
            {
                '='
            });
            int num = input.Length * 5 / 8;
            byte[] array = new byte[num];
            byte b = 0;
            byte b2 = 8;
            int num2 = 0;
            foreach (char c in input)
            {
                int num3 = Common.CharToValue(c);
                if (b2 > 5)
                {
                    int num4 = num3 << (int)(b2 - 5);
                    b = (byte)((int)b | num4);
                    b2 -= 5;
                }
                else
                {
                    int num4 = num3 >> (int)(5 - b2);
                    b = (byte)((int)b | num4);
                    array[num2++] = b;
                    b = (byte)(num3 << (int)(3 + b2));
                    b2 += 3;
                }
            }
            if (num2 != num)
            {
                array[num2] = b;
            }
            return array;
        }

        private static int CharToValue(char c)
        {
            int result;
            if (c < '[' && c > '@')
            {
                result = (int)(c - 'A');
            }
            else if (c < '8' && c > '1')
            {
                result = (int)(c - '\u0018');
            }
            else
            {
                if (c >= '{' || c <= '`')
                {
                    throw new ArgumentException("Character is not a Base32 character.", "c");
                }
                result = (int)(c - 'a');
            }
            return result;
        }

        public static DataTable ShuffleDataTable(DataTable dt)
        {
            DataTable result = new DataTable();
            try
            {
                result = (from DataRow r in dt.Rows
                          orderby Base.rd.Next()
                          select r).CopyToDataTable<DataRow>();
            }
            catch
            {
            }
            return result;
        }

        public static bool CopyFolder(string pathFrom, string pathTo)
        {
            try
            {
                Common.CreateFolder(pathTo);
                foreach (string text in Directory.GetDirectories(pathFrom, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(text.Replace(pathFrom, pathTo));
                }
                foreach (string text2 in Directory.GetFiles(pathFrom, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(text2, text2.Replace(pathFrom, pathTo), true);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static bool MoveFolder(string pathFrom, string pathTo)
        {
            try
            {
                Directory.Move(pathFrom, pathTo);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static bool DeleteFile(string pathFile)
        {
            try
            {
                File.Delete(pathFile);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool DeleteFolder(string pathFolder)
        {
            try
            {
                Directory.Delete(pathFolder, true);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static int GetRandInt(int min, int max)
        {
            if (min > max)
            {
                max = min;
            }
            return new Random().Next(min, max);
        }

        public static bool ChangeIP(int typeChangeIP, string profileDcom, int iTypeHotspot, string sLinkNord)
        {
            bool isSuccess = false;
            string ip_new = "";
            string ip_old = CheckIP();
            try
            {
                if (typeChangeIP == 0)
                {
                    return true;
                }
                else if (typeChangeIP == 1)
                {
                    IntPtr app = AutoControl.FindWindowHandle(null, "HMA VPN");
                    AutoControl.BringToFront(app);
                    AutoControl.SendClickOnPosition(AutoControl.FindHandle(app, "Chrome_RenderWidgetHostHWND", "Chrome Legacy Window"), 356, 286);
                    Thread.Sleep(5000);
                    string ipLan = CheckIP();
                    AutoControl.SendClickOnPosition(AutoControl.FindHandle(app, "Chrome_RenderWidgetHostHWND", "Chrome Legacy Window"), 356, 286);
                    //Thread.Sleep(15000);
                    int timeStart = Environment.TickCount;
                    do
                    {
                        ip_new = CheckIP();
                        if (Environment.TickCount - timeStart > 20000)
                            break;
                    } while (ip_new == ip_old || ip_new == ipLan);

                    if (ip_new != ip_old)
                        isSuccess = true;
                }
                else if (typeChangeIP == 2)
                {
                    ResetDcom(profileDcom);
                    ip_new = CheckIP();
                    if (ip_new != ip_old)
                        isSuccess = true;
                }
                else if (typeChangeIP == 4)
                {

                }
                else if (typeChangeIP == 5)
                {

                }
                else if (typeChangeIP == 6)
                {
                    //nordvpn
                    Random rd = new Random();
                    string[] arrGroupName = new string[]
                    {
                    "Albania",
                    "Argentina",
                    "Australia",
                    "Austria",
                    "Belgium",
                    "Bosnia and Herzegovina",
                    "Brazil",
                    "Bulgaria",
                    "Canada",
                    "Chile",
                    "Costa Rica",
                    "Croatia",
                    "Cyprus",
                    "Czech Republic",
                    "Denmark",
                    "Estonia",
                    "Finland",
                    "France",
                    "Georgia",
                    "Germany",
                    "Greece",
                    "Hong Kong",
                    "Hungary",
                    "Iceland",
                    "India",
                    "Indonesia",
                    "Ireland",
                    "Israel",
                    "Italy",
                    "Japan",
                    "Latvia",
                    "Luxembourg",
                    "Mexico",
                    "Moldova",
                    "Netherlands",
                    "New Zealand",
                    "North Macedonia",
                    "Norway",
                    "Poland",
                    "Portugal",
                    "Romania",
                    "Serbia",
                    "Singapore",
                    "Slovakia",
                    "Slovenia",
                    "South Africa",
                    "South Korea",
                    "Spain",
                    "Sweden",
                    "Switzedland",
                    "Taiwan",
                    "Thailand",
                    "Turkey",
                    "Ukraine",
                    "United Kingdom",
                    "United States",
                    "Vietnam"
                    };
                    RunCMD("\"" + sLinkNord + "\\NordVPN.exe\" -d");
                    Thread.Sleep(5000);
                    RunCMD("\"" + sLinkNord + "\\NordVPN.exe\" -c -g" + "P2P");
                    int timeStart = Environment.TickCount;
                    Thread.Sleep(8000);
                    ip_new = CheckIP();
                    if (ip_new != ip_old)
                        isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ExportError(ex, "Error ChangeIP");
            }
            return isSuccess;
        }

        public static void ZipFolder(string pathFolderFrom, string pathFileZip, string password = "tungmin")
        {
            try
            {
                using (ZipFile zipFile = new ZipFile())
                {
                    if (password != "")
                    {
                        zipFile.Password = password;
                    }
                    zipFile.AddDirectory(pathFolderFrom);
                    zipFile.Save(pathFileZip);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void UnZipFolder(string pathFileZip, string pathFolderTo, string password = "tungmin")
        {
            try
            {
                using (ZipFile zipFile = new ZipFile(pathFileZip, Encoding.UTF8))
                {
                    if (password != "")
                    {
                        zipFile.Password = password;
                    }
                    zipFile.ExtractAll(pathFolderTo, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception)
            {
            }
        }

        private static Random rd = new Random();

        private static int getWidthScreen = Screen.PrimaryScreen.WorkingArea.Width;

        private static int getHeightScreen = Screen.PrimaryScreen.WorkingArea.Height;

        private static object k = new object();
    }
}
