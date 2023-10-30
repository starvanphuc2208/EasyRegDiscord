using MCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace easy.UI
{
    public partial class frmConfigProxy : Form
    {
        JSON_Settings settings;

        public frmConfigProxy()
        {
            InitializeComponent();
            settings = new JSON_Settings("configCommon");
            LoadSettings();
        }

        private void LoadSettings()
        {
			this.ckbKhongCheckIP.Checked = this.settings.GetValueBool("ckbKhongCheckIP", false);
			this.nudChangeIpCount.Value = Convert.ToInt32((this.settings.GetValue("ip_nudChangeIpCount", "") == "" ? "1" : this.settings.GetValue("ip_nudChangeIpCount", "")));
			int num = Convert.ToInt32((this.settings.GetValue("ip_iTypeChangeIp", "") == "" ? "0" : this.settings.GetValue("ip_iTypeChangeIp", "")));
			if (num == 0)
			{
				this.rbNone.Checked = true;
			}
			else if (num == 3)
			{
				//this.rbSSH.Checked = true;
			}
			else if (num == 4)
			{
				//this.rbExpressVPN.Checked = true;
			}
			else if (num == 5)
			{
				//this.rbHotspot.Checked = true;
			}
			else if (num == 6)
			{
				//this.rbNordVPN.Checked = true;
			}
			else if (num == 7)
			{
				this.rbTinsoft.Checked = true;
			}
			else if (num == 10)
			{
				this.rbTMProxy.Checked = true;
			}
			else if (num == 11)
			{
				this.rbProxyv6.Checked = true;
			}
			else if (num == 12)
			{
				this.rbShopLike.Checked = true;
			}
			else if (num == 13)
			{
				this.rbMinproxy.Checked = true;
			}
			else if (num == 14)
			{
				this.rbProxyList.Checked = true;
			}
			//this.txtNordVPN.Text = this.settings.GetValue("ip_txtNordVPN", "");
			//this.cbbHostpot.SelectedIndex = Convert.ToInt32((this.settings.GetValue("ip_cbbHostpot", "") == "" ? "0" : this.settings.GetValue("ip_cbbHostpot", "")));
			if (this.settings.GetValueInt("typeApiTinsoft", 0) != 0)
			{
				this.rbApiProxy.Checked = true;
			}
			else
			{
				this.rbApiUser.Checked = true;
			}
			this.txtApiUser.Text = this.settings.GetValue("txtApiUser", "");
			this.txtApiProxy.Text = this.settings.GetValue("txtApiProxy", "");
			this.cbbLocationTinsoft.SelectedValue = (this.settings.GetValue("cbbLocationTinsoft", "") == "" ? "0" : this.settings.GetValue("cbbLocationTinsoft", ""));
			this.nudLuongPerIPTinsoft.Value = this.settings.GetValueInt("nudLuongPerIPTinsoft", 0);
			this.ckbWaitDoneAllTinsoft.Checked = this.settings.GetValueBool("ckbWaitDoneAllTinsoft", false);
			this.txtApiKeyTMProxy.Text = this.settings.GetValue("txtApiKeyTMProxy", "");
			this.txtMinproxy.Text = this.settings.GetValue("txtMinproxy", "");
			this.nudLuongPerIPTMProxy.Value = this.settings.GetValueInt("nudLuongPerIPTMProxy", 1);
			this.ckbWaitDoneAllTMProxy.Checked = this.settings.GetValueBool("ckbWaitDoneAllTMProxy", false);

			this.txtApiProxyv6.Text = this.settings.GetValue("txtApiProxyv6");
			this.txtListProxyv6.Text = this.settings.GetValue("txtListProxyv6");
			this.nudLuongPerIPProxyv6.Value = this.settings.GetValueInt("nudLuongPerIPProxyv6");
			this.txtApiShopLike.Text = this.settings.GetValue("txtApiShopLike");
			this.nudLuongPerIPShopLike.Value = this.settings.GetValueInt("nudLuongPerIPShopLike");

			this.CheckedChangedFull();
		}

		private void CheckedChangedFull()
		{
		}

		private void SaveSettings()
        {
			// Cấu hình đổi IP
			this.settings.Update("ckbKhongCheckIP", this.ckbKhongCheckIP.Checked);
			this.settings.Update("ip_nudChangeIpCount", this.nudChangeIpCount.Value);
			int typeChangeIP = 0;
			if (this.rbNone.Checked)
			{
				typeChangeIP = 0;
			}
			//else if (this.rbSSH.Checked)
			//{
			//	typeChangeIP = 3;
			//}
			//else if (this.rbExpressVPN.Checked)
			//{
			//	typeChangeIP = 4;
			//}
			//else if (this.rbHotspot.Checked)
			//{
			//	typeChangeIP = 5;
			//}
			//else if (this.rbNordVPN.Checked)
			//{
			//	typeChangeIP = 6;
			//}
			else if (this.rbTinsoft.Checked)
			{
				typeChangeIP = 7;
			}
			else if (this.rbTMProxy.Checked)
			{
				typeChangeIP = 10;
			}
			else if (this.rbProxyv6.Checked)
			{
				typeChangeIP = 11;
			}
			else if (this.rbShopLike.Checked)
			{
				typeChangeIP = 12;
			}
			else if (this.rbMinproxy.Checked)
			{
				typeChangeIP = 13;
			}
			else if (this.rbProxyList.Checked)
            {
				typeChangeIP = 14;
            }
			
			this.settings.Update("ip_iTypeChangeIp", typeChangeIP);
			//this.settings.Update("ip_txtNordVPN", this.txtNordVPN.Text);
			//this.settings.Update("ip_cbbHostpot", this.cbbHostpot.SelectedIndex);
			if (!this.rbApiUser.Checked)
			{
				this.settings.Update("typeApiTinsoft", 1);
			}
			else
			{
				this.settings.Update("typeApiTinsoft", 0);
			}
			this.settings.Update("txtApiUser", this.txtApiUser.Text);
			this.settings.Update("txtApiProxy", this.txtApiProxy.Text);
			this.settings.Update("cbbLocationTinsoft", this.cbbLocationTinsoft.SelectedValue);
			this.settings.Update("nudLuongPerIPTinsoft", this.nudLuongPerIPTinsoft.Value);
			this.settings.Update("ckbWaitDoneAllTinsoft", this.ckbWaitDoneAllTinsoft.Checked);
			this.settings.Update("txtApiKeyTMProxy", this.txtApiKeyTMProxy.Text);
			this.settings.Update("txtMinproxy", this.txtMinproxy.Text);
			this.settings.Update("nudLuongPerIPTMProxy", this.nudLuongPerIPTMProxy.Value);
			this.settings.Update("ckbWaitDoneAllTMProxy", this.ckbWaitDoneAllTMProxy.Checked);

			this.settings.Update("txtApiProxyv6", this.txtApiProxyv6.Text);
			this.settings.Update("txtListProxyv6", this.txtListProxyv6.Text);
			this.settings.Update("nudLuongPerIPProxyv6", this.nudLuongPerIPProxyv6.Value);
			this.settings.Update("txtApiShopLike", this.txtApiShopLike.Text);
			this.settings.Update("nudLuongPerIPShopLike", this.nudLuongPerIPShopLike.Value);

			this.settings.Update("ckbLuuTrangThai", true);
			UpdateStatus.isSaveSettings = true;

			settings.Save();
        }

        private void btnOK_Click(object sender, EventArgs e)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
			string apiTinsoft = txtApiUser.Text.Trim();
			List<string> lstKey = TinsoftProxy.GetListKey(apiTinsoft);
			if (lstKey.Count > 0)
			{
				Common.ShowMessageBox($"Đang có {lstKey.Count} proxy khả dụng!", 1);
			}
			else
			{
				Common.ShowMessageBox($"Không có proxy khả dụng!", 2);
			}
		}

        private void button8_Click(object sender, EventArgs e)
        {
			List<string> lstApiKey = new List<string>();
			List<string> lstTemp = txtApiKeyTMProxy.Lines.ToList();
			lstTemp = Common.RemoveEmptyItems(lstTemp);
			foreach (var item in lstTemp)
			{
				if (TMProxy.CheckApiProxy(item))
					lstApiKey.Add(item);
			}

			txtApiKeyTMProxy.Lines = lstApiKey.ToArray();
			if (lstApiKey.Count > 0)
			{
				Common.ShowMessageBox($"Đang có {lstApiKey.Count} proxy khả dụng!", 1);
			}
			else
			{
				Common.ShowMessageBox($"Không có proxy khả dụng!", 2);
			}
		}

        private void button5_Click(object sender, EventArgs e)
        {
			if (this.settings.GetValueInt("ip_iTypeChangeIp", 0) == 0)
			{
				Common.ShowMessageBox("Vui lòng chọn loại đổi IP", 3);
			}
			else if (!Common.ChangeIP(this.settings.GetValueInt("ip_iTypeChangeIp", 0), this.settings.GetValueInt("typeDcom", 0), this.settings.GetValue("ip_txtProfileNameDcom", ""), this.settings.GetValue("txtUrlHilink", ""), this.settings.GetValueInt("ip_cbbHostpot", 0), this.settings.GetValue("ip_txtNordVPN", "")))
			{
				Common.ShowMessageBox("Đổi IP thất bại!", 2);
			}
			else
			{
				Common.ShowMessageBox("Đổi IP thành công!", 1);
			}
		}

        private void button7_Click(object sender, EventArgs e)
        {
			List<string> lstApiKey = new List<string>();
			List<string> lstTemp = txtApiProxy.Lines.ToList();
			lstTemp = Common.RemoveEmptyItems(lstTemp);
			foreach (var item in lstTemp)
			{
				if (TinsoftProxy.CheckApiProxy(item))
					lstApiKey.Add(item);
			}

			txtApiProxy.Lines = lstApiKey.ToArray();
			if (lstApiKey.Count > 0)
			{
				Common.ShowMessageBox($"Đang có {lstApiKey.Count} proxy khả dụng!", 1);
			}
			else
			{
				Common.ShowMessageBox($"Không có proxy khả dụng!", 2);
			}
		}

        private void btnImportProxy_Click(object sender, EventArgs e)
        {
			Process.Start(Application.StartupPath + "\\input\\proxy.txt");
        }

        private void txtListProxyv6_TextChanged(object sender, EventArgs e)
        {
			List<string> list = Common.RemoveEmptyItems(this.txtListProxyv6.Lines.ToList());
			this.label43.Text = string.Format(Language.GetValue("Nhập Proxy ({0}):"), list.Count.ToString());
		}

        private void txtApiShopLike_TextChanged(object sender, EventArgs e)
        {
			List<string> list = Common.RemoveEmptyItems(this.txtApiShopLike.Lines.ToList());
			this.label47.Text = string.Format(Language.GetValue("Nhập API KEY ({0}):"), list.Count.ToString());
		}

        private void txtApiProxy_TextChanged(object sender, EventArgs e)
        {
			try
			{
				List<string> list = Common.RemoveEmptyItems(this.txtApiProxy.Lines.ToList());
				this.lblCountApiProxy.Text = "(" + list.Count + ")";
			}
			catch
			{
			}
		}

		private void LoadCbbLocation()
		{
			Dictionary<string, string> dataSource = this.TinsoftGetListLocation();
			this.cbbLocationTinsoft.DataSource = new BindingSource(dataSource, null);
			this.cbbLocationTinsoft.ValueMember = "Key";
			this.cbbLocationTinsoft.DisplayMember = "Value";
		}

        private void frmConfigProxy_Load(object sender, EventArgs e)
        {
			this.LoadCbbLocation();
		}

		public Dictionary<string, string> TinsoftGetListLocation()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> listCountryTinsoft = SetupFolder.GetListCountryTinsoft();
			for (int i = 0; i < listCountryTinsoft.Count; i++)
			{
				string[] array = listCountryTinsoft[i].Split('|');
				dictionary.Add(array[0], array[1]);
			}
			return dictionary;
		}

        private void btnCheckMinproxy_Click(object sender, EventArgs e)
        {
			List<string> list = new List<string>();
			List<string> list2 = this.txtMinproxy.Lines.ToList<string>();
			list2 = Common.RemoveEmptyItems(list2);
			foreach (string text in list2)
			{
				if (MinProxy.CheckApiProxy(text))
				{
					list.Add(text);
				}
			}
			this.txtMinproxy.Lines = list.ToArray();
			if (list.Count > 0)
			{
				Common.ShowMessageBox(string.Format(Language.GetValue("Đang có {0} proxy khả dụng!"), list.Count), 1);
			}
			else
			{
				Common.ShowMessageBox(Language.GetValue("Không có proxy khả dụng!"), 2);
			}
		}

        private void btnImportListAPI_Click(object sender, EventArgs e)
        {
			try
			{
				Process.Start(Application.StartupPath + "\\input\\listapiproxy.txt");
			}
			catch (Exception ex)
			{

			}
		}
    }
}
