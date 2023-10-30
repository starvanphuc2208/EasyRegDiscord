using easy;
using easy.Properties;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace easy
{
    public partial class frm_Login : Form
    {
        private string license = "";
        private LicenseDto licenseDto { get; set; }

        public frm_Login()
        {
            InitializeComponent();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.txtThietBi.Text);
            MessageBox.Show("Đã copy mã thiết bị!");
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string txtThietBi = this.txtThietBi.Text.Trim();
            string txtKey = this.txtKey.Text.Trim();


            if (txtKey != "" && txtThietBi != "")
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    this.GetLicence();

                    if (this.licenseDto != null)
                    {
                        if (this.licenseDto.error == 1)
                        {
                            MessageBox.Show(this.licenseDto.thong_bao);
                            Environment.Exit(0);
                        }
                        else
                        {
                            MessageBox.Show(string.Concat("Thiết bị của bạn đã được kích hoạt thành công!.", Environment.NewLine, Environment.NewLine, "Vui lòng mở lại phần mềm để sử dụng!"), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            Settings.Default.license = this.txtThietBi.Text;
                            Settings.Default.key = this.txtKey.Text;
                            Settings.Default.Save();
                            Environment.Exit(0);
                        }
                    } else
                    {
                        MessageBox.Show("Vui lòng liên hệ Admin để đăng ký sử dụng.!", "Thông báo");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    MessageBox.Show(this, "Vui lòng kiểm tra lại đường truyền mạng", "Thông báo");
                    Environment.Exit(0);
                }

            } else
            {
                MessageBox.Show(this, "Vui lòng nhập KEY để xác thực đăng nhập.", "Thông báo");
            }
            
        }

        private void GetLicence()
        {
            LicenseHelper licenseHelper = new LicenseHelper();
            this.licenseDto = licenseHelper.CheckLicenseV2(this.license, this.txtKey.Text);
        }

        private void Frm_Login_Load(object sender, EventArgs e)
        {
            this.license = ComputerInfo.GetComputerId();
            this.txtThietBi.Text = this.license;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.facebook.com/easysoftwarevn");
        }

        private void BtnDong_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://zalo.me/g/mvdipz665");
        }
    }
}
