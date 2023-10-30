using MCommon;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace easy.UI
{
    public partial class frmConfig : Form
    {
        JSON_Settings settings;

        public frmConfig()
        {
            InitializeComponent();
            settings = new JSON_Settings("configCommon");
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Cấu hình LD
            txtLD.Text = settings.GetValue("txtLD");
            nudThread.Value = settings.GetValueInt("nudThread");
            nudDelayOpenDeviceFrom.Value = settings.GetValueInt("nudDelayOpenDeviceFrom");
            nudDelayOpenDeviceTo.Value = settings.GetValueInt("nudDelayOpenDeviceTo");
            nudDelayCloseDeviceFrom.Value = settings.GetValueInt("nudDelayCloseDeviceFrom");
            nudDelayCloseDeviceTo.Value = settings.GetValueInt("nudDelayCloseDeviceTo");
            nudTimeOTP.Value = settings.GetValueInt("nudTimeOTP", 60);

            this.ckbExportTdata.Checked = settings.GetValueBool("ckbExportTdata", false);
            this.ckbAvatar.Checked = settings.GetValueBool("ckbAvatar", false);
            this.ckbCTSCNew.Checked = settings.GetValueBool("ckbCTSCNew", false);

            if (this.settings.GetValueInt("typeOpenDevice", 0) == 0)
            {
                this.rbMoLanLuot.Checked = true;
            }
            else
            {
                this.rbMoCachNhau.Checked = true;
            }

            // Cấu hình đăng ký
            this.ckbKhongAddVaoFormView.Checked = settings.GetValueBool("ckbKhongAddVaoFormView", false);

            // API SIM
            if (settings.GetValueInt("typePhone") == 0)
            {
                this.rbChoThueSimCode.Checked = true;
                this.txtApiSimCode.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 2)
            {
                this.rbTempSMS.Checked = true;
                this.txtTempSMS.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 3)
            {
                this.rbSimFast.Checked = true;
                this.txtSimFastVN.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 4)
            {
                this.rbCodeSim.Checked = true;
                this.txtAPICodeSim.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 5)
            {
                this.rbViOtp.Checked = true;
                this.txtViOtp.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 6)
            {
                this.rb2ndLine.Checked = true;
                this.txt2ndLine.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 7)
            {
                this.rbSMSActivate.Checked = true;
                this.txtSMSActivate.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 8)
            {
                this.rbAhasim.Checked = true;
                this.txtApiAhasim.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 10)
            {
                this.rbJSNguyenLieu.Checked = true;
                this.txtJSNguyenLieu.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 11)
            {
                this.rbOtpmm.Checked = true;
                this.txtOtpmm.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 12)
            {
                this.rbCustomSimThue.Checked = true;
                this.txtApiCustomSimThue.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 13)
            {
                this.rbTempCodeCo.Checked = true;
                this.txtTempCodeCo.Text = settings.GetValue("apiPhone");
            }
            else if (settings.GetValueInt("typePhone") == 14)
            {
                this.rbCodeText247.Checked = true;
                this.txtCodeText247.Text = settings.GetValue("apiPhone");
            } else if (settings.GetValueInt("typePhone") == 15)
            {
                this.rbListAPI.Checked = true;
            }
            else if (settings.GetValueInt("typePhone") == 16)
            {
                this.rbKhach01.Checked = true;
            }
            else if (settings.GetValueInt("typePhone") == 17)
            {
                this.rbCustomAPIPhp.Checked = true;
            }
            else if (settings.GetValueInt("typePhone") == 18)
            {
                this.rbKhachHang02.Checked = true;
            }

            // 
            this.txt2ndLine.Text = settings.GetValue("txt2ndLine", "");
            this.txtViOtp.Text = settings.GetValue("txtViOtp", "");
            this.txtSMSActivate.Text = settings.GetValue("txtSMSActivate", "");
            this.txtJSNguyenLieu.Text = settings.GetValue("txtJSNguyenLieu", "");
            this.txtAPICodeSim.Text = settings.GetValue("txtAPICodeSim", "");
            this.txtOtpmm.Text = settings.GetValue("txtOtpmm", "");
            this.txtApiAhasim.Text = settings.GetValue("txtApiAhasim", "");
            this.txtApiCustomSimThue.Text = settings.GetValue("txtApiCustomSimThue", "");
            this.txtApiSimCode.Text = settings.GetValue("txtApiSimCode", "");
            this.txtServiceID.Text = settings.GetValue("txtServiceID", "");
            this.txtTempCodeCo.Text = settings.GetValue("txtTempCodeCo", "");
            this.txtCodeText247.Text = settings.GetValue("txtCodeText247", "");
            this.txtKhach01.Text = settings.GetValue("txtKhach01");
            this.txtKhachHang02.Text = settings.GetValue("txtKhachHang02");
            this.txtCustomAPIPhp.Text = settings.GetValue("txtCustomAPIPhp");

            // Đầu số
            this.txtCTSCDauSo.Text = settings.GetValue("txtCTSCDauSo", "");
            this.txtCTSCNhaMang.Text = settings.GetValue("txtCTSCNhaMang", "");
            this.ckbFakeProxy.Checked = settings.GetValueBool("ckbFakeProxy", true);

            // Cấu hình name
            if (settings.GetValueInt("typeName") == 0)
            {
                this.rbNameVN.Checked = true;
            } else
            {
                this.rbNameUS.Checked = true;
            }


            // Cấu hình mật khẩu
            if (settings.GetValueInt("typePass") == 0)
            {
                this.rbPassMacDinh.Checked = true;
            }
            else if (settings.GetValueInt("typePass") == 1)
            {
                this.rbPassRandom.Checked = true;
            }
            txtPassword.Text = settings.GetValue("txtPassword");

            this.ckbFakeProxy.Checked = settings.GetValueBool("ckbTdata", false);

            // Kiểu chạy
            if (settings.GetValueInt("typeRun") == 0)
            {
                this.rbTelegramThuong.Checked = true;
            } else
            {
                this.rbTelegramX.Checked = true;
            }

            // Kiểu change proxy
            if (settings.GetValueInt("typeChangeProxy") == 0)
            {
                this.rbTunProxy.Checked = true;
            }
            else
            {
                this.rbCollege.Checked = true;
            }

            this.txtHint.Text = settings.GetValue("txtHint", "easysoftware");
        }

        private void SaveSettings()
        {
            // Cấu hình LD
            settings.Update("txtLD", txtLD.Text);
            settings.Update("nudThread", nudThread.Value);
            settings.Update("nudDelayOpenDeviceFrom", nudDelayOpenDeviceFrom.Value);
            settings.Update("nudDelayOpenDeviceTo", nudDelayOpenDeviceTo.Value);
            settings.Update("nudDelayCloseDeviceFrom", nudDelayCloseDeviceFrom.Value);
            settings.Update("nudDelayCloseDeviceTo", nudDelayCloseDeviceTo.Value);
            settings.Update("nudTimeOTP", nudTimeOTP.Value);
            settings.Update("ckbTdata", this.ckbFakeProxy.Checked);
            settings.Update("txtHint", this.txtHint.Text);
            settings.Update("ckbCTSCNew", this.ckbCTSCNew.Checked);

            settings.Update("ckbExportTdata", this.ckbExportTdata.Checked);
            settings.Update("ckbAvatar", this.ckbAvatar.Checked);

            if (this.rbMoCachNhau.Checked)
            {
                this.settings.Update("typeOpenDevice", 1);
            }
            else
            {
                this.settings.Update("typeOpenDevice", 0);
            }

            // Kiểu chạy
            if (this.rbTelegramThuong.Checked)
            {
                settings.Update("typeRun", 0);
            } else
            {
                settings.Update("typeRun", 1);
            }

            // Kiểu chạy
            if (this.rbTunProxy.Checked)
            {
                settings.Update("typeChangeProxy", 0);
            }
            else
            {
                settings.Update("typeChangeProxy", 1);
            }

            // Cấu hình đăng ký
            settings.Update("ckbKhongAddVaoFormView", this.ckbKhongAddVaoFormView.Checked);

            // Cấu hình phone
            // Chức năng veryphone
            int typePhone = 0;
            string apiPhone = "";
            if (this.rbChoThueSimCode.Checked)
            {
                typePhone = 0;
                apiPhone = this.txtApiSimCode.Text;
            }
            else if (this.rbTempSMS.Checked)
            {
                typePhone = 2;
                apiPhone = this.txtTempSMS.Text;
            }
            else if (this.rbSimFast.Checked)
            {
                typePhone = 3;
                apiPhone = this.txtSimFastVN.Text;
            }
            else if (this.rbCodeSim.Checked)
            {
                typePhone = 4;
                apiPhone = this.txtAPICodeSim.Text;
            }
            else if (this.rbViOtp.Checked)
            {
                typePhone = 5;
                apiPhone = this.txtViOtp.Text;
                settings.Update("txtViOtp", txtViOtp.Text);
            }
            else if (this.rb2ndLine.Checked)
            {
                typePhone = 6;
                apiPhone = this.txt2ndLine.Text;
                settings.Update("txt2ndLine", txt2ndLine.Text);
            }
            else if (this.rbSMSActivate.Checked)
            {
                typePhone = 7;
                apiPhone = this.txtSMSActivate.Text;
            }
            else if (this.rbAhasim.Checked)
            {
                typePhone = 8;
                apiPhone = this.txtApiAhasim.Text;
            }
            else if (this.rbJSNguyenLieu.Checked)
            {
                // JS Nguyenlieu
                typePhone = 10;
                apiPhone = this.txtJSNguyenLieu.Text;
            } else if (this.rbOtpmm.Checked)
            {
                // otpmm.com
                typePhone = 11;
                apiPhone = this.txtOtpmm.Text;
            }
            else if (this.rbCustomSimThue.Checked)
            {
                // http://103.142.139.33/simthue
                typePhone = 12;
                apiPhone = this.txtApiCustomSimThue.Text;
            }
            else if (this.rbTempCodeCo.Checked)
            {
                // tempcode.co
                typePhone = 13;
                apiPhone = this.txtTempCodeCo.Text;
            }
            else if (this.rbCodeText247.Checked)
            {
                // codetext247
                typePhone = 14;
                apiPhone = this.txtCodeText247.Text;
            } else if (this.rbListAPI.Checked)
            {
                // list api
                typePhone = 15;
            }
            else if (this.rbKhach01.Checked)
            {
                // Khách 01
                typePhone = 16;
                apiPhone = this.txtKhach01.Text;
            }
            else if (this.rbCustomAPIPhp.Checked)
            {
                // List custom
                typePhone = 17;
                apiPhone = this.txtCustomAPIPhp.Text;
            }
            else if (this.rbKhachHang02.Checked)
            {
                // Khách 02
                typePhone = 18;
                apiPhone = this.txtKhachHang02.Text;
            }

            settings.Update("typePhone", typePhone);
            settings.Update("apiPhone", apiPhone);


            // Cấu hình mật khẩu
            int typePass = 0;
            if (this.rbPassMacDinh.Checked)
            {
                typePass = 0;
            }
            else if (this.rbPassRandom.Checked)
            {
                typePass = 1;
            }
            settings.Update("typePass", typePass);
            settings.Update("txtPassword", txtPassword.Text);

            // Cấu hình tên
            if (this.rbNameVN.Checked)
            {
                settings.Update("typeName", 0);
            } else
            {
                settings.Update("typeName", 1);
            }

            settings.Update("ckbFakeProxy", this.ckbFakeProxy.Checked);
            settings.Update("txtSMSActivate", this.txtSMSActivate.Text);
            settings.Update("txtJSNguyenLieu", this.txtJSNguyenLieu.Text);
            settings.Update("txtAPICodeSim", txtAPICodeSim.Text);
            settings.Update("txtOtpmm", txtOtpmm.Text);
            settings.Update("txtApiAhasim", txtApiAhasim.Text);
            settings.Update("txtApiCustomSimThue", txtApiCustomSimThue.Text);
            settings.Update("txtApiSimCode", this.txtApiSimCode.Text);
            settings.Update("txtServiceID", this.txtServiceID.Text);
            settings.Update("txtTempCodeCo", this.txtTempCodeCo.Text);
            settings.Update("txtCodeText247", this.txtCodeText247.Text);
            settings.Update("txtKhach01", this.txtKhach01.Text);
            settings.Update("txtKhachHang02", this.txtKhachHang02.Text);
            settings.Update("txtCustomAPIPhp", this.txtCustomAPIPhp.Text);

            settings.Save();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSettings();

                if (MessageBox.Show("Lưu thành công, bạn có muốn đóng cửa sổ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    this.Close();
            }
            catch
            {
                Common.ShowMessageBox("Vui lòng thử lại sau!", 2);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpenLD_Click(object sender, EventArgs e)
        {
            base.Invoke(new Action(delegate ()
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                DialogResult dialogResult = folderBrowserDialog.ShowDialog();
                bool flag = dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath);
                if (flag)
                {
                    this.txtLD.Text = folderBrowserDialog.SelectedPath;
                }
            }));
        }

        public void RunCMD(string string_0, string ldconsole, string path)
        {
            string_0 = path + ldconsole + string_0;
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + string_0,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            }.Start();
        }

        private void btnOpenNameUS_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\input");
        }

        private void btnOpenNameVN_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\input");
        }

        private void btnCheck2ndLine_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalance2ndLine(this.txt2ndLine.Text.Trim()) + " $");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalance2ndLine(string apikey)
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("https://2ndline.io/apiv1/getbalance?apikey=" + apikey);
            JObject jobject = JObject.Parse(json);
            if (Convert.ToBoolean(jobject["status"]))
            {
                try
                {
                    value = jobject["balance"].ToString();
                }
                catch
                {
                }
            }
            return (value == "") ? "" : value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalanceViotp(this.txtViOtp.Text.Trim()) + " VNĐ");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalanceViotp(string apikey)
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("https://api.viotp.com/users/balance?token=" + apikey);
            JObject jobject = JObject.Parse(json);
            if (json.Contains("successful"))
            {
                try
                {
                    value = jobject["data"]["balance"].ToString();
                }
                catch
                {
                }
            }
            return (value == "") ? "" : value;
        }

        private void btnCheckTempSMS_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + CheckBalanceTempSMS(this.txtTempSMS.Text.Trim()) + " đ");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public static string CheckBalanceTempSMS(string apikey)
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("https://api.tempsms.co/money?api_key=" + apikey);
            JObject jobject = JObject.Parse(json);
            if (jobject["message"].ToString() == "Success")
            {
                try
                {
                    value = jobject["money"].ToString();
                }
                catch
                {
                }
            }
            return value;
        }

        private void btnCheckSimFast_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + CheckBalanceSimFast(this.txtSimFastVN.Text.Trim()) + " đ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public static string CheckBalanceSimFast(string apikey)
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("https://access.simfast.vn/api/ig/balance?api_token=" + apikey);
            JObject jobject = JObject.Parse(json);
            if (jobject["success"].ToString() == "True")
            {
                try
                {
                    value = jobject["data"]["balance"].ToString();
                }
                catch
                {
                }
            }
            return value;
        }

        private void btnCheckChoThueSimCode_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalanceChoThueSimCode(this.txtApiSimCode.Text.Trim()) + " đ");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalanceChoThueSimCode(string apiKey)
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("https://chothuesimcode.com/api?act=account&apik=" + Uri.EscapeDataString(apiKey));
            JObject jobject = JObject.Parse(json);
            if (jobject["Msg"].ToString() == "OK")
            {
                try
                {
                    value = jobject["Result"]["Balance"].ToString();
                }
                catch
                {
                }
            }
            return (value == "") ? "" : value;
        }

        private void btnCheckSMSActivate_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalanceSmsActivate(this.txtSMSActivate.Text.Trim()) + "$");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalanceSmsActivate(string apiKey)
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("https://api.sms-activate.org/stubs/handler_api.php?api_key="+apiKey+"&action=getBalance");
            try
            {
                value = json.Replace("ACCESS_BALANCE:", "");
            }
            catch
            {
            }
            return (value == "") ? "" : value;
        }

        private void btnCheckOtpMM_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalanceOtpMM(this.txtOtpmm.Text.Trim()) + " VNĐ");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalanceOtpMM(string apikey)
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            value = requestXNet.RequestGet("http://api.otpmm.com/?Accesskey=" + apikey + "&Method=Balance");
            return (value == "") ? "" : value;
        }

        private void btnCheckAhasim_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalanceAhasim(this.txtApiAhasim.Text.Trim()) + " đ");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalanceAhasim(string apikey) /////////////ahasim
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("http://ahasim.com/api/user/balance?token=" + apikey);
            JObject jobject = JObject.Parse(json);
            if (jobject["success"].ToString() == "True")
            {
                try
                {
                    value = jobject["data"]["balance"].ToString();
                }
                catch
                {
                }
            }
            return value;
        }

        private void btnImportProxy_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\input\\sock5.txt");
        }

        private void btnCheckCustomSimThue_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalanceCustomSimThue(this.txtApiCustomSimThue.Text.Trim()) + " đ");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalanceCustomSimThue(string apikey) /////////////ahasim
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("http://103.142.139.33:1337/V1/balance?token=" + apikey);
            JObject jobject = JObject.Parse(json);
            if (jobject["success"].ToString() == "True")
            {
                try
                {
                    value = jobject["balance"].ToString();
                }
                catch
                {
                }
            }
            return value;
        }

        private void btnUpdateChrome_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread((ThreadStart)delegate
            {
                try
                {
                    var cService = ChromeDriverService.CreateDefaultService();
                    cService.HideCommandPromptWindow = true;
                    var options = new ChromeOptions();
                    options.AddArguments("--headless");
                    var _webDriver = new ChromeDriver(cService, options);
                    _webDriver.Navigate().GoToUrl("https://google.com/");
                    _webDriver.Close();
                    _webDriver.Quit();
                    try
                    {
                        CleanTemporaryFolders();
                    }

                    catch
                    {

                    }
                    MessageBox.Show("Phiên bản chromedriver khả dụng!", "Thông báo");

                }
                catch
                {
                    MessageBox.Show("Phiên bản không khả dụng, bấm ok để cập nhật!", "Thông báo");
                    DownloadLatestVersionOfChromeDriver();
                    try
                    {
                        File.Delete(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip");
                    }
                    catch { }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        //update chromedriver

        public void DownloadLatestVersionOfChromeDriver()
        {
            string path = DownloadLatestVersionOfChromeDriverGetVersionPath();
            var version = DownloadLatestVersionOfChromeDriverGetChromeVersion(path);
            var urlToDownload = DownloadLatestVersionOfChromeDriverGetURLToDownload(version);
            DownloadLatestVersionOfChromeDriverKillAllChromeDriverProcesses();
            DownloadLatestVersionOfChromeDriverDownloadNewVersionOfChrome(urlToDownload);
        }

        public string DownloadLatestVersionOfChromeDriverGetVersionPath()
        {
            //Path originates from here: https://chromedriver.chromium.org/downloads/version-selection            
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe"))
            {
                if (key != null)
                {
                    Object o = key.GetValue("");
                    if (!String.IsNullOrEmpty(o.ToString()))
                    {
                        return o.ToString();
                    }
                    else
                    {
                        throw new ArgumentException("Unable to get version because chrome registry value was null");
                    }
                }
                else
                {
                    throw new ArgumentException("Unable to get version because chrome registry path was null");
                }
            }
        }

        public string DownloadLatestVersionOfChromeDriverGetChromeVersion(string productVersionPath)
        {
            if (String.IsNullOrEmpty(productVersionPath))
            {
                throw new ArgumentException("Unable to get version because path is empty");
            }

            if (!File.Exists(productVersionPath))
            {
                throw new FileNotFoundException("Unable to get version because path specifies a file that does not exists");
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(productVersionPath);
            if (versionInfo != null && !String.IsNullOrEmpty(versionInfo.FileVersion))
            {
                return versionInfo.FileVersion;
            }
            else
            {
                throw new ArgumentException("Unable to get version from path because the version is either null or empty: " + productVersionPath);
            }
        }

        public string DownloadLatestVersionOfChromeDriverGetURLToDownload(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                throw new ArgumentException("Unable to get url because version is empty");
            }

            //URL's originates from here: https://chromedriver.chromium.org/downloads/version-selection
            string html = string.Empty;
            string urlToPathLocation = @"https://chromedriver.storage.googleapis.com/LATEST_RELEASE_" + String.Join(".", version.Split('.').Take(3));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToPathLocation);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            if (String.IsNullOrEmpty(html))
            {
                throw new WebException("Unable to get version path from website");
            }

            return "https://chromedriver.storage.googleapis.com/" + html + "/chromedriver_win32.zip";
        }

        public void DownloadLatestVersionOfChromeDriverKillAllChromeDriverProcesses()
        {
            //It's important to kill all processes before attempting to replace the chrome driver, because if you do not you may still have file locks left over
            var processes = Process.GetProcessesByName("chromedriver");
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                    //We do our best here but if another user account is running the chrome driver we may not be able to kill it unless we run from a elevated user account + various other reasons we don't care about
                }
            }
        }

        public void DownloadLatestVersionOfChromeDriverDownloadNewVersionOfChrome(string urlToDownload)
        {
            if (String.IsNullOrEmpty(urlToDownload))
            {
                throw new ArgumentException("Unable to get url because urlToDownload is empty");
            }

            //Downloaded files always come as a zip, we need to do a bit of switching around to get everything in the right place
            using (var client = new WebClient())
            {
                if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip"))
                {
                    File.Delete(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip");
                }

                client.DownloadFile(urlToDownload, "chromedriver.zip");

                if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip") && File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.exe"))
                {
                    File.Delete(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.exe");
                }

                if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip"))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip", System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                }
            }
        }

        private void CleanTemporaryFolders()
        {
            String tempFolder = Environment.ExpandEnvironmentVariables("%TEMP%");
            String recent = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + "\\Recent";
            String prefetch = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%") + "\\Prefetch";
            EmptyFolderContents(tempFolder);
            EmptyFolderContents(recent);
            EmptyFolderContents(prefetch);
        }

        private void EmptyFolderContents(string folderName)
        {
            foreach (var folder in Directory.GetDirectories(folderName))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception excep)
                {
                    System.Diagnostics.Debug.WriteLine(excep);
                }
            }
            foreach (var file in Directory.GetFiles(folderName))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception excep)
                {
                    System.Diagnostics.Debug.WriteLine(excep);
                }
            }
        }

        private void btnCheckCodeText247_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(" Tài khoản còn: " + this.CheckBalanceCodeText247(this.txtCodeText247.Text.Trim()) + " VNĐ");
            }
            catch
            {
                MessageBox.Show("Có lỗi rồi, vui lòng kiểm tra lại!\n( Error, Please check back)");
            }
        }

        public string CheckBalanceCodeText247(string apikey) 
        {
            string value = "";
            RequestHttp requestXNet = new RequestHttp("", "", "");
            string json = requestXNet.RequestGet("https://administrator.codetext247.com/api/balance?token=" + apikey);
            JObject jobject = JObject.Parse(json);
            try
            {
                value = jobject["balance"].ToString();
            }
            catch
            {
            }
            return value;
        }

        private void btnImportListAPI_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Application.StartupPath + "\\input\\listapi.txt");
            }
            catch (Exception ex)
            {

            }
           
        }
    }
}
