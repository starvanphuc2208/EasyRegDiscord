using easy.Helper;
using easy.Properties;
using MCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace easy.UI
{
    public partial class fViewLD : Form
    {
        public bool isRunSwap = false;

        private object lock_listPanel = new object();

        public int[] LDSize = new int[] { 240, 360, -1, -36, 240, 395 };

        public static fViewLD remote;

        public fViewLD()
        {
            InitializeComponent();
            fViewLD.remote = this;
        }

        public void AddLDIntoPanel(IntPtr MainWindowHandle, int indexDevice, int sttTaiKhoan)
        {
            try
            {
                Control control = (from Control h in this.panelListDevice.Controls
                                   where h.Tag.Equals(indexDevice)
                                   select h).FirstOrDefault<Control>();
                if (control == null)
                {
                    control = (from Control h in this.panelListDevice.Controls
                               where h.Tag.Equals(-1)
                               select h).FirstOrDefault<Control>();
                    this.UpdateInfoPanelDevice(control, indexDevice, sttTaiKhoan);
                    Application.DoEvents();
                }
                base.Invoke(new MethodInvoker(delegate ()
                {
                    User32Helper.SetParent(MainWindowHandle, control.Handle);
                    User32Helper.MoveWindow(MainWindowHandle, this.LDSize[2], this.LDSize[3], this.LDSize[4], this.LDSize[5], true);
                }));
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "AddLDIntoPanel");
            }
        }

        public void AddPanelDevice(int deviceIndex)
        {
            try
            {
                if (!this.isRunSwap)
                {
                    deviceIndex = -1;
                }
                Panel panel = new Panel()
                {
                    Name = string.Concat("dv", deviceIndex.ToString()),
                    Tag = deviceIndex,
                    Size = new Size(this.LDSize[0], this.LDSize[1] + 60),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                string str = "";
                str = (deviceIndex != -1 ? string.Concat("LDPlayer-", deviceIndex.ToString()) : "LDPlayer-None");
                Label label = new Label()
                {
                    Text = str,
                    Location = new Point(0, this.LDSize[1]),
                    Size = new Size(this.LDSize[0] - 55, 20),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    ForeColor = Color.Black,
                    Name = deviceIndex.ToString() ?? "",
                    AutoSize = false
                };
                panel.Controls.Add(label);
                label.DoubleClick += new EventHandler(this.CheckDevice);
                Label label1 = new Label()
                {
                    Text = ">",
                    Location = new Point(0, this.LDSize[1] + 20),
                    Size = new Size(this.LDSize[0], 20),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    ForeColor = Color.Black,
                    Name = deviceIndex.ToString() ?? "",
                    AutoSize = false
                };
                panel.Controls.Add(label1);
                Label label2 = new Label()
                {
                    Text = "",
                    Location = new Point(0, this.LDSize[1] + 40),
                    Size = new Size(this.LDSize[0], 20),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    ForeColor = Color.Black,
                    Name = deviceIndex.ToString() ?? "",
                    AutoSize = false
                };
                panel.Controls.Add(label2);
                PictureBox pictureBox = new PictureBox()
                {
                    //Image = Resources.icons8_multiply_20px,
                    Location = new Point(this.LDSize[0] - 25, this.LDSize[1]),
                    Name = deviceIndex.ToString() ?? "",
                    Size = new Size(20, 20),
                    Cursor = Cursors.Hand,
                    Visible = false
                };
                pictureBox.Click += new EventHandler(this.TurnOffDevice);
                panel.Controls.Add(pictureBox);
                this.toolTip1.SetToolTip(pictureBox, "Close");
                PictureBox pictureBox1 = new PictureBox()
                {
                    //Image = Resources.icons8_undo_20px,
                    Location = new Point(this.LDSize[0] - 50, this.LDSize[1]),
                    Name = deviceIndex.ToString() ?? "",
                    Size = new Size(20, 20),
                    Cursor = Cursors.Hand,
                    Visible = false
                };
                pictureBox1.Click += new EventHandler(this.Back);
                panel.Controls.Add(pictureBox1);
                this.toolTip1.SetToolTip(pictureBox1, "Back");
                PictureBox pictureBox2 = new PictureBox()
                {
                    //Image = Resources.iconmin,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(0, -30),
                    Name = "pictureBoxLogo",
                    Size = new Size(this.LDSize[0], this.LDSize[1] + 60),
                    TabIndex = 0,
                    TabStop = false
                };
                pictureBox2.BringToFront();
                panel.Controls.Add(pictureBox2);
                lock (this.lock_listPanel)
                {
                    this.panelListDevice.Invoke(new MethodInvoker(() => this.panelListDevice.Controls.Add(panel)));
                }
                for (int i = 0; i < 5 && this.panelListDevice.Controls[string.Concat("dv", deviceIndex.ToString())] == null; i++)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "AddPanelDevice");
            }
        }

        private void Back(object sender, EventArgs e)
        {
            try
            {
                string name = (sender as PictureBox).Name;
                ADBHelper.RunCMD(name, "shell input keyevent 4", 10);
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "Back");
            }
        }

        private void CheckDevice(object sender, EventArgs e)
        {
            try
            {
                int num = Convert.ToInt32((sender as Label).Name);
                this.ExportLog(num, "", "", "");
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "CheckDevice");
            }
        }

        public bool CheckExistPanelDevice(int indexDevice)
        {
            return (
                from Control h in this.panelListDevice.Controls
                where h.Tag.Equals(indexDevice)
                select h).Count<Control>() == 1;
        }

        public void ExportLog(int indexDevice, string activity = "", string html = "", string folderPath = "")
        {
            try
            {
                string str = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                string str1 = string.Concat(indexDevice.ToString(), "_", str);
                if (folderPath == "")
                {
                    folderPath = "CheckDevice";
                }
                Common.CreateFolder(folderPath);
                Common.CreateFolder(string.Concat(folderPath, "\\", indexDevice.ToString()));
                string deviceByIndex = ADBHelper.GetDeviceByIndex(indexDevice);
                if (deviceByIndex != "")
                {
                    ADBHelper.ScreenShot(deviceByIndex, string.Concat(new string[] { folderPath, "\\", indexDevice.ToString(), "\\", str1, ".png" }));
                    File.AppendAllText(string.Concat(new string[] { folderPath, "\\", indexDevice.ToString(), "\\", str1, ".txt" }), string.Concat(this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[1].Text, "\r\n"));
                    if (activity == "")
                    {
                        activity = ADBHelper.DumpActivity(deviceByIndex).Split(new char[] { '{', '}' })[1].Split(new char[] { ' ' })[2];
                    }
                    File.AppendAllText(string.Concat(new string[] { folderPath, "\\", indexDevice.ToString(), "\\", str1, ".txt" }), string.Concat(activity, "\r\n"));
                    if (html == "")
                    {
                        html = ADBHelper.GetXML(deviceByIndex);
                    }
                    File.AppendAllText(string.Concat(new string[] { folderPath, "\\", indexDevice.ToString(), "\\", str1, ".txt" }), html);
                }
            }
            catch
            {
            }
        }

        private void fViewChrome_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (Panel control in this.panelListDevice.Controls)
                {
                    int num = Convert.ToInt32(control.Name.Replace("dv", ""));
                    this.RemovePanelDevice(num);
                }
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "fViewChrome_FormClosing");
            }
        }

        public void HidePicTurnOffDevice(int indexDevice)
        {
            try
            {
                base.Invoke(new MethodInvoker(() => {
                    this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[3].Visible = false;
                    this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[4].Visible = false;
                }));
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "HidePicTurnOffDevice");
            }
        }

        public void LoadHanhDong(int indexDevice, string content)
        {
            string str = content;
            try
            {
                Application.DoEvents();
                if (str.Trim() != "")
                {
                    str = str.Replace("\"", "");
                    str = string.Concat(str, (str.EndsWith("...") ? "" : "..."));
                }
                this.panelListDevice.Invoke(new MethodInvoker(() => this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[1].Text = string.Concat(">", str)));
                this.LoadStatus(indexDevice, "");
                Application.DoEvents();
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "LoadHanhDong");
            }
        }

        public void LoadStatus(int indexDevice, string content)
        {
            string str = content;
            try
            {
                Application.DoEvents();
                if (str.Trim() != "")
                {
                    str = str.Replace("\"", "");
                    str = string.Concat(str, (str.EndsWith("...") ? "" : "..."));
                }
                this.panelListDevice.Invoke(new MethodInvoker(() => this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[2].Text = str));
                Application.DoEvents();
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "LoadStatus");
            }
        }

        public void RemovePanelDevice(int indexDevice)
        {
            try
            {
                ADBHelper.QuitDevice(ConfigHelper.GetPathLDPlayer(0), indexDevice);
                this.LoadStatus(indexDevice, "");
                this.LoadHanhDong(indexDevice, "");
                this.HidePicTurnOffDevice(indexDevice);
                if (!this.isRunSwap)
                {
                    Control item = this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())];
                    this.UpdateInfoPanelDevice(item, -1, 0);
                }
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "RemovePanelDevice");
            }
        }

        public void ShowPicTurnOffDevice(int indexDevice, string deviceId)
        {
            try
            {
                base.Invoke(new MethodInvoker(() => {
                    this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[3].Visible = true;
                    this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[4].Visible = true;
                    this.panelListDevice.Controls[string.Concat("dv", indexDevice.ToString())].Controls[4].Name = deviceId;
                }));
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "ShowPicTurnOffDevice");
            }
        }

        private void TurnOffDevice(object sender, EventArgs e)
        {
            try
            {
                this.RemovePanelDevice(Convert.ToInt32((sender as PictureBox).Name));
            }
            catch (Exception exception)
            {
                Common.ExportError(exception, "TurnOffDevice");
            }
        }

        public void UpdateInfoPanelDevice(Control control, int deviceIndex, int sttTaiKhoan = 0)
        {
            try
            {
                control.Invoke(new MethodInvoker(() => {
                    control.Name = string.Concat("dv", deviceIndex.ToString());
                    control.Tag = deviceIndex;
                    if (deviceIndex != -1)
                    {
                        control.Controls[0].Text = string.Concat("LDPlayer-", deviceIndex.ToString());
                    }
                    else
                    {
                        control.Controls[0].Text = "LDPlayer-None";
                    }
                    if (sttTaiKhoan > 0)
                    {
                        Control item = control.Controls[0];
                        item.Text = string.Concat(item.Text, ": Tài khoản ", sttTaiKhoan.ToString());
                    }
                    control.Controls[0].Name = deviceIndex.ToString();
                    control.Controls[1].Name = deviceIndex.ToString();
                    control.Controls[2].Name = deviceIndex.ToString();
                    control.Controls[3].Name = deviceIndex.ToString();
                }));
            }
            catch
            {
            }
        }
    }
}
