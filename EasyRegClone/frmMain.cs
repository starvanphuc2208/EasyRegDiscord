using DevExpress.XtraPrinting;
using easy.Helper;
using easy.MCommon;
using easy.Properties;
using easy.UI;
using EasySignupFB.Library.UiAutomation;
using IronOcr;
using MCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TeleSharp.TL;
using TeleSharp.TL.Account;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Network;
using update;

namespace easy
{
	public partial class frmMain : Form
	{
		#region Khai báo biến

		private bool isStop = false;

		JSON_Settings settings_common;

		private int checkDelayLD;
		private List<Device> lstDevice;
		public bool isResetAdb;
		private List<Thread> lstThread;
		private bool isOpeningDevice;
		private List<TinsoftProxy> listTinsoft;

		private List<XproxyProxy> listxProxy;

		private List<ProxyWeb> listProxyWeb;

		private List<ShopLike> listShopLike;

		private List<string> listApiTinsoft;

		private List<string> listProxyXproxy;

		List<TMProxy> listTMProxy = null;
		List<string> listProxyTMProxy = null;

		private List<string> listProxyProxyv6;

		private List<string> listProxyShopLike;
		List<string> listProxyMinproxy = null;

		Random rd = new Random();

		#endregion

		public frmMain()
		{
			InitializeComponent();

			//size form
			Base.width = Width;
			Base.heigh = Height;

			Control.CheckForIllegalCrossThreadCalls = false;
			LoadConfig();

			// Setup
			SetupFolder.StartApplication();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			bool check = Width == Screen.PrimaryScreen.WorkingArea.Width && Height == Screen.PrimaryScreen.WorkingArea.Height;
			if (check)
			{
				Width = Base.width;
				Height = Base.heigh;
				Top = Base.top;
				Left = Base.left;
			}
			else
			{
				Base.top = Top;
				Base.left = Left;

				Top = 0;
				Left = 0;
				Width = Screen.PrimaryScreen.WorkingArea.Width;
				Height = Screen.PrimaryScreen.WorkingArea.Height;
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			try
			{
				Common.KillProcess("EasyRegTelegramUpdate");
				Environment.Exit(0);
			}
			catch
			{
				this.Close();
			}
		}

		private void LoadSetting()
		{
			this.settings_common = new JSON_Settings("configCommon", false);
		}

		void LoadConfig()
		{
			settings_common = new JSON_Settings("configCommon");
		}

		List<string> lstPhoneNumber = new List<string>();
		List<string> lstFirstNameVN = new List<string>();
		List<string> lstFirstNameEN = new List<string>();
		List<string> lstLastNameVN = new List<string>();
		List<string> lstLastNameEN = new List<string>();
		List<string> lstProxy = new List<string>();
		List<string> lstSock5 = new List<string>();
		List<string> lstProxyAPI = new List<string>();

		int typePhone = 0;
		string apiPhone = "";

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.lstDevice = new List<Device>();
			this.dtgvAcc.Rows.Clear();
			Thread threadRefreshDeviceId = null;
			try
			{
				threadRefreshDeviceId = new Thread(delegate ()
				{
					for (; ; )
					{
						try
						{
							if (this.isResetAdb && this.lstDevice.Count > 0)
							{
								bool flag = false;
								while (!flag)
								{
									flag = true;
									for (int num2 = 0; num2 < this.lstDevice.Count; num2++)
									{
										string deviceByIndex = ADBHelper.GetDeviceByIndex(this.lstDevice[num2].IndexDevice);
										if (string.IsNullOrEmpty(deviceByIndex))
										{
											flag = false;
										}
										else
										{
											this.lstDevice[num2].DeviceId = deviceByIndex;
										}
									}
								}
								this.isResetAdb = false;
							}
						}
						catch
						{
						}
						Common.DelayTime(5.0);
					}
				});
				threadRefreshDeviceId.IsBackground = true;
				threadRefreshDeviceId.Start();
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "");
			}
			try
			{
				this.LoadSetting();
				bool isRunSwap = false;
				string pathLD = settings_common.GetValue("txtLD");
				if (!Directory.Exists(pathLD))
				{
					MessageBoxHelper.ShowMessageBox("Đường dẫn LDPlayer không hợp lệ!", 3);
					return;
				}
				int maxThread = settings_common.GetValueInt("nudThread", 1);

				// Very phone
				typePhone = settings_common.GetValueInt("typePhone", 0);
				apiPhone = settings_common.GetValue("apiPhone", "");

				// Lấy ra danh sách phone
				lstPhoneNumber = File.ReadAllLines(@"contacts\phone.txt").ToList();

				// Lấy ra danh sách proxy
				if (settings_common.GetValueInt("ip_iTypeChangeIp") == 9)
				{
					lstProxy = File.ReadAllLines(@"input\proxy.txt").ToList();

					if (lstProxy.Count == 0)
					{
						MessageBoxHelper.ShowMessageBox("Vui lòng nhập danh sách Proxy!", 3);
						return;
					}
				}

				// Lấy danh sách fake proxy
				if (settings_common.GetValueBool("ckbFakeProxy"))
				{
					lstSock5 = File.ReadAllLines(@"input\sock5.txt").ToList();

					if (lstSock5.Count == 0)
					{
						MessageBoxHelper.ShowMessageBox("Vui lòng nhập danh sách Sock5!", 3);
						return;
					}
				}

				// Lấy ra tên 
				lstFirstNameVN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\FirstName_VN.txt").ToList());
				lstLastNameVN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\LastName_VN.txt").ToList());

				lstFirstNameEN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\FirstName_EN.txt").ToList());
				lstLastNameEN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\LastName_EN.txt").ToList());

				// Reset gia tri
				//this.lbtongacc.Text = "0";
				//this.lblive.Text = "0";
				//this.lb2fa.Text = "0";
				this.lbdie.Text = "0";

				// Khởi tạo convert session
				dicIPDC = new Dictionary<uint, string>();
				this.dicIPDC.Add(1U, "149.154.175.53");
				this.dicIPDC.Add(2U, "149.154.167.51");
				this.dicIPDC.Add(3U, "149.154.175.100");
				this.dicIPDC.Add(4U, "149.154.167.91");
				this.dicIPDC.Add(5U, "91.108.56.130");

				switch (this.settings_common.GetValueInt("ip_iTypeChangeIp", 0))
				{
					case 7:
						this.listApiTinsoft = this.GetListKeyTinsoft();
						if (this.listApiTinsoft.Count == 0)
						{
							MessageBoxHelper.ShowMessageBox("Proxy Tinsoft không đủ, vui lòng mua thêm!", 2);
							return;
						}
						this.listTinsoft = new List<TinsoftProxy>();
						for (int i = 0; i < this.listApiTinsoft.Count; i++)
						{
							TinsoftProxy item = new TinsoftProxy(this.listApiTinsoft[i], this.settings_common.GetValueInt("nudLuongPerIPTinsoft", 0), this.settings_common.GetValueInt("cbbLocationTinsoft", 0));
							this.listTinsoft.Add(item);
						}
						if (this.listApiTinsoft.Count * this.settings_common.GetValueInt("nudLuongPerIPTinsoft", 0) < maxThread)
						{
							maxThread = this.listApiTinsoft.Count * this.settings_common.GetValueInt("nudLuongPerIPTinsoft", 0);
						}
						break;
					case 8:
						this.listProxyXproxy = this.settings_common.GetValueList("txtListProxy", 0);
						if (this.listProxyXproxy.Count == 0)
						{
							MessageBoxHelper.ShowMessageBox("Proxy không đủ, vui lòng cấu hình lại!", 2);
							return;
						}
						this.listxProxy = new List<XproxyProxy>();
						for (int j = 0; j < this.listProxyXproxy.Count; j++)
						{
							XproxyProxy item2 = new XproxyProxy(this.settings_common.GetValue("txtServiceURLXProxy", ""), this.listProxyXproxy[j], this.settings_common.GetValueInt("typeProxy", 0), this.settings_common.GetValueInt("nudLuongPerIPXProxy", 0));
							this.listxProxy.Add(item2);
						}
						if (this.listProxyXproxy.Count * this.settings_common.GetValueInt("nudLuongPerIPXProxy", 0) < maxThread)
						{
							maxThread = this.listProxyXproxy.Count * this.settings_common.GetValueInt("nudLuongPerIPXProxy", 0);
						}
						break;
					case 10:
						// TMProxy
						listProxyTMProxy = settings_common.GetValueList("txtApiKeyTMProxy");
						if (listProxyTMProxy.Count == 0)
						{
							Common.ShowMessageBox("TMProxy không đủ, vui lòng mua thêm!", 2);
							return;
						}

						listTMProxy = new List<TMProxy>();
						for (int ts = 0; ts < listProxyTMProxy.Count; ts++)
						{
							TMProxy tmproxy = new TMProxy(listProxyTMProxy[ts], 0, settings_common.GetValueInt("nudLuongPerIPTMProxy"));
							listTMProxy.Add(tmproxy);
						}

						if (listProxyTMProxy.Count * settings_common.GetValueInt("nudLuongPerIPTMProxy") < maxThread)
							maxThread = listProxyTMProxy.Count * settings_common.GetValueInt("nudLuongPerIPTMProxy");
						break;
					case 11:
						this.listProxyProxyv6 = this.settings_common.GetValueList("txtListProxyv6", 0);
						if (this.listProxyProxyv6.Count == 0)
						{
							MessageBoxHelper.ShowMessageBox("Proxy không đủ, vui lòng cấu hình lại!", 2);
							return;
						}
						this.listProxyWeb = new List<ProxyWeb>();
						for (int l = 0; l < this.listProxyProxyv6.Count; l++)
						{
							ProxyWeb item4 = new ProxyWeb(this.settings_common.GetValue("txtApiProxyv6", ""), this.listProxyProxyv6[l], 0, this.settings_common.GetValueInt("nudLuongPerIPProxyv6", 0));
							this.listProxyWeb.Add(item4);
						}
						if (this.listProxyProxyv6.Count * this.settings_common.GetValueInt("nudLuongPerIPProxyv6", 0) < maxThread)
						{
							maxThread = this.listProxyProxyv6.Count * this.settings_common.GetValueInt("nudLuongPerIPProxyv6", 0);
						}
						break;
					case 12:
						this.listProxyShopLike = this.settings_common.GetValueList("txtApiShopLike", 0);
						if (this.listProxyShopLike.Count == 0)
						{
							MessageBoxHelper.ShowMessageBox("Proxy không đủ, vui lòng mua thêm!", 2);
							return;
						}
						this.listShopLike = new List<ShopLike>();
						for (int m = 0; m < this.listProxyShopLike.Count; m++)
						{
							ShopLike item5 = new ShopLike(this.listProxyShopLike[m], 0, this.settings_common.GetValueInt("nudLuongPerIPShopLike", 0));
							this.listShopLike.Add(item5);
						}
						if (this.listProxyShopLike.Count * this.settings_common.GetValueInt("nudLuongPerIPShopLike", 0) < maxThread)
						{
							maxThread = this.listProxyShopLike.Count * this.settings_common.GetValueInt("nudLuongPerIPShopLike", 0);
						}
						break;
				}
				List<int> lstPossition = new List<int>();
				for (int n = 0; n < maxThread; n++)
				{
					lstPossition.Add(0);
				}
				this.isOpeningDevice = false;
				this.checkDelayLD = 0;
				this.cControl("start");
				if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
				{
					this.OpenFormViewLD(isRunSwap);
					for (int num = 0; num < maxThread; num++)
					{
						fViewLD.remote.AddPanelDevice(num + 1);
					}
				}
				this.isStop = false;
				int curChangeIp = 0;
				bool isChangeIPSuccess = false;
				this.lstThread = new List<Thread>();

				new Thread(delegate ()
				{
					try
					{
						int num3 = 0;
						List<string> list = new List<string>();
						int num2 = 1;
						while (num3 < num2)
						{
							int currentIndex = 0;
							while (currentIndex < 3000 && !this.isStop)
							{
								if (this.isStop)
								{
									break;
								}
								if (this.lstThread.Count < maxThread)
								{
									if (this.isStop)
									{
										break;
									}
									int row = currentIndex++;
									Thread thread = new Thread(async delegate ()
									{
										try
										{
											int indexOfPossitionApp = 0;

											try
											{
												indexOfPossitionApp = Common.GetIndexOfPossitionApp(ref lstPossition);
											}
											catch (Exception ex)
											{
												Common.ExportError(ex, "ThreadNew1()");
											}

											// this.ExcuteOneThread(row, indexOfPossitionApp + 1, pathLD);
											this.ExcuteOneThread(row, indexOfPossitionApp, pathLD);

											try
											{
												Common.FillIndexPossition(ref lstPossition, indexOfPossitionApp);
											}
											catch (Exception ex)
											{
												Common.ExportError(ex, "ThreadNew2()");
											}

										}
										catch (Exception ex3)
										{
											Common.ExportError(ex3, "ThreadNew()");
										}
									});
									thread.Name = row.ToString();
									lock (this.lstThread)
									{
										this.lstThread.Add(thread);
									}
									Common.DelayTime(1.0);
									thread.Start();
								}
								else
								{
									if (this.isStop)
									{
										break;
									}
									if ((this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) == 7 && this.settings_common.GetValueBool("ckbWaitDoneAllTinsoft", false)) || (this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) == 8 && this.settings_common.GetValueBool("ckbWaitDoneAllXproxy", false)) || (this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) == 10 && this.settings_common.GetValueBool("ckbWaitDoneAllTMProxy", false)))
									{
										for (int iThread = 0; iThread < this.lstThread.Count; iThread++)
										{
											this.lstThread[iThread].Join();
											List<Thread> obj2 = this.lstThread;
											lock (obj2)
											{
												this.lstThread.RemoveAt(iThread--);
											}
										}
									}
									else if (this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 0 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 7 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 8 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 9 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 10 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 11 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 12)
									{
										for (int num7 = 0; num7 < this.lstThread.Count; num7++)
										{
											this.lstThread[num7].Join();
											List<Thread> obj3 = this.lstThread;
											lock (obj3)
											{
												this.lstThread.RemoveAt(num7--);
											}
										}
										if (this.isStop)
										{
											break;
										}
										Interlocked.Increment(ref curChangeIp);
										if (curChangeIp >= this.settings_common.GetValueInt("ip_nudChangeIpCount", 1))
										{
											for (int num8 = 0; num8 < 3; num8++)
											{
												isChangeIPSuccess = Common.ChangeIP(this.settings_common.GetValueInt("ip_iTypeChangeIp", 0), this.settings_common.GetValueInt("typeDcom", 0), this.settings_common.GetValue("ip_txtProfileNameDcom", ""), this.settings_common.GetValue("txtUrlHilink", ""), this.settings_common.GetValueInt("ip_cbbHostpot", 0), this.settings_common.GetValue("ip_txtNordVPN", ""));
												if (isChangeIPSuccess)
												{
													break;
												}
											}
											if (!isChangeIPSuccess)
											{
												MessageBoxHelper.ShowMessageBox("Không thể đổi ip!", 2);
												goto IL_E4C;
											}
											curChangeIp = 0;
										}
									}
									else
									{
										for (int num9 = 0; num9 < this.lstThread.Count; num9++)
										{
											if (!this.lstThread[num9].IsAlive)
											{
												lock (this.lstThread)
												{
													this.lstThread.RemoveAt(num9--);
												}
											}
										}
									}
								}

								if (this.isStop)
								{
									break;
								}
							}
							for (int iThread = 0; iThread < this.lstThread.Count; iThread++)
							{
								this.lstThread[iThread].Join();
							}

							if (this.isStop)
							{
								break;
							}
						}
					}
					catch (Exception ex2)
					{
						Common.ExportError(ex2, "");
					}
				IL_E4C:
					if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
					{
						this.CloseFormViewLD();
					}
					this.cControl("stop");
					try
					{
						threadRefreshDeviceId.Abort();
					}
					catch
					{
					}
				}).Start();
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "");
				this.cControl("stop");
			}
		}

		object lock_GetLoginCode1 = new object();
		object lock_GetLoginCode2 = new object();
		object lock_StartProxy = new object();
		object lock_FinishProxy = new object();
		object lock_restoreDevice = new object();
		object lock_checkDelayLD = new object();
		private object lock_Output_Otp = new object();
		private object lock_Output_2FA = new object();
		private object lock_Output_Success = new object();
		private object lock_Output_Fail = new object();

		#region Backup code
		private void ExcuteOneThread_Old(int indexRow, int indexPos, string pathLD)
		{
			Device device = null;
			int iResult = 0;

			string ip = "";
			string statusProxy = "";
			string proxy = "";
			int typeProxy = 0;

			TinsoftProxy tinsoft = null;
			XproxyProxy xproxy = null;
			TMProxy tmproxy = null;

			try
			{
				// Kiểm tra dừng hay chưa
				if (this.isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					iResult = 1;
				}

			// Đổi proxy
			#region Proxy            
			getProxy:
				if (isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					iResult = 1;
					goto Xong;
				}

				switch (settings_common.GetValueInt("ip_iTypeChangeIp"))
				{
					case 7:
						this.SetStatusAccount(indexRow, "Đang lấy proxy Tinsoft...", null);
						lock (lock_StartProxy)
						{
							do
							{
								if (isStop)
									break;
								tinsoft = null;

								do
								{
									if (isStop)
										break;
									foreach (var item in listTinsoft)
									{
										if (tinsoft == null || item.daSuDung < tinsoft.daSuDung)
											tinsoft = item;
									}
								} while (tinsoft.daSuDung == tinsoft.limit_theads_use);

								if (isStop)
									break;
								if (tinsoft.daSuDung > 0 || tinsoft.ChangeProxy())
								{
									proxy = tinsoft.proxy;
									if (proxy == "")
										proxy = tinsoft.GetProxy();
									tinsoft.dangSuDung++;
									tinsoft.daSuDung++;
									break;
								}
							} while (true);
							if (isStop)
							{
								this.SetStatusAccount(indexRow, "Đã dừng!", null);
								iResult = 1;
								goto Xong;
							}

							this.SetStatusAccount(indexRow, "Check IP Tinsoft ...", null);

							for (int i = 0; i < 30; i++)
							{
								Common.DelayTime(1);
								ip = Common.CheckProxy(proxy, typeProxy);
								if (ip != "")
									break;
							}

							if (ip == "")
							{
								tinsoft.dangSuDung--;
								tinsoft.daSuDung--;
								goto getProxy;
							}
						}
						break;
					case 8:
						//xproxy
						this.SetStatusAccount(indexRow, "Đang lấy Xproxy ...", null);
						lock (lock_StartProxy)
						{
							do
							{
								if (isStop)
									break;
								xproxy = null;

								do
								{
									if (isStop)
										break;
									foreach (var item in listxProxy)
									{
										if (xproxy == null || item.daSuDung < xproxy.daSuDung)
											xproxy = item;
									}
								} while (xproxy.daSuDung == xproxy.limit_theads_use);

								if (isStop)
									break;
								if (xproxy.daSuDung > 0 || xproxy.ChangeProxy())
								{
									proxy = xproxy.proxy;
									typeProxy = xproxy.typeProxy;
									xproxy.dangSuDung++;
									xproxy.daSuDung++;
									break;
								}
							} while (true);
							if (isStop)
							{
								this.SetStatusAccount(indexRow, "Đã dừng!", null);
								iResult = 1;
								goto Xong;
							}

							this.SetStatusAccount(indexRow, "Check IP Xproxy...", null);

							for (int i = 0; i < 30; i++)
							{
								Common.DelayTime(1);
								ip = Common.CheckProxy(proxy, typeProxy);
								if (ip != "")
									break;
							}

							if (ip == "")
							{
								xproxy.dangSuDung--;
								xproxy.daSuDung--;
								goto getProxy;
							}
						}
						break;
					case 9:
						proxy = Common.GetRandomItemFromListNoDel(ref lstProxy, new Random());
						typeProxy = 0;

						for (int i = 0; i < 30; i++)
						{
							Common.DelayTime(1);
							ip = Common.CheckProxy(proxy, typeProxy);
							if (ip != "")
								break;
						}

						if (ip == "")
						{
							this.SetStatusAccount(indexRow, statusProxy + "Không thể kết nối Proxy!");
							iResult = 1;
							goto Xong;
						}
						break;
					case 10:
						//tmproxy
						this.SetStatusAccount(indexRow, "Đang lấy TMProxy ...");
						lock (lock_StartProxy)
						{
							do
							{
								if (isStop)
									break;
								tmproxy = null;

								do
								{
									if (isStop)
										break;
									foreach (var item in listTMProxy)
									{
										if (tmproxy == null || item.daSuDung < tmproxy.daSuDung)
											tmproxy = item;
									}
								} while (tmproxy.daSuDung == tmproxy.limit_theads_use);

								if (isStop)
									break;
								if (tmproxy.daSuDung > 0 || tmproxy.ChangeProxy())
								{
									proxy = tmproxy.proxy;
									if (proxy == "")
										proxy = tmproxy.GetProxy();
									tmproxy.dangSuDung++;
									tmproxy.daSuDung++;
									break;
								}
							} while (true);
							if (isStop)
							{
								this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!");
								iResult = 1;
								goto Xong;
							}
							statusProxy = $"(IP: {proxy.Split(':')[0]}) ";
							this.SetStatusAccount(indexRow, statusProxy + "Check IP...");

							ip = Common.CheckProxy(proxy, 0);
							if (ip == "")
							{
								tmproxy.dangSuDung--;
								tmproxy.daSuDung--;
								goto getProxy;
							}
						}
						break;
					default:

						break;
				}

				if (isStop)
				{
					this.SetStatusAccount(indexRow, "Đã dừng", null);
					iResult = 1;
					goto Xong;
				}
				#endregion

				// Thêm dòng
				base.Invoke(new Action(delegate ()
				{
					this.dtgvAcc.Rows.Add("");
				}));
				Common.DelayTime(1);


				// Kiểm tra Proxy
				if (!this.settings_common.GetValueBool("ckbKhongCheckIP", false))
				{
					if (this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 7 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 8 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 10 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 11 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 12)
					{
						if (ip != "")
						{
							statusProxy = "(IP: " + ip.Split(new char[]
							{
										':'
							})[0] + ") ";
						}
						this.SetStatusAccount(indexRow, statusProxy + "Check IP...", null);
						bool checkProxy = false;
						int k = 0;
						while (k < 30)
						{
							Common.DelayTime(1.0);
							proxy = Common.CheckProxy(ip, typeProxy);
							if (!(proxy != ""))
							{
								if (this.isStop)
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
									iResult = 1;
									goto Xong;
								}
								k++;
							}
							else
							{
								checkProxy = true;
								break;
							}
						}
						if (!checkProxy)
						{
							if (ip != "")
							{
								this.SetStatusAccount(indexRow, statusProxy + "Không thể kết nối proxy!", null);
							}
							else
							{
								this.SetStatusAccount(indexRow, statusProxy + "Không có kết nối Internet!", null);
							}
							iResult = 1;
							goto Xong;
						}
						statusProxy = "(IP: " + proxy + ") ";
					}
				}

				if (settings_common.GetValueInt("ip_iTypeChangeIp") == 9)
				{
					statusProxy = "(IP: " + ip.Split(new char[]
					{
								':'
					})[0] + ") ";
				}

				if (this.isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					iResult = 1;
					goto Xong;
				}
				else
				{
					// Mở thiết bị
					device = new Device(pathLD, indexPos.ToString() ?? "");
					if (!Directory.Exists(pathLD + "\\vms\\leidian" + indexPos.ToString()))
					{
						this.SetStatusAccount(indexRow, statusProxy + "Đang tạo thiết bị...", null);
						for (int l = 0; l < 2; l++)
						{
							// ADBHelper.AddDevice(pathLD);
							ADBHelper.CopyDevice(pathLD);
							if (Directory.Exists(pathLD + "\\vms\\leidian" + indexPos.ToString()))
							{
								break;
							}
						}
						this.SetStatusAccount(indexRow, statusProxy + "Đang cấu hình thiết bị...", null);
						lock (this.lock_restoreDevice)
						{
							device.Restore();
						}
						device.ChangeHardwareLDPlayer2();
						device.ChangeFileConfig();
					}

					// Change thiết bị

					// Chờ tới lượt
					this.SetStatusAccount(indexRow, statusProxy + "Chờ đến lượt...", null);
					lock (this.lock_checkDelayLD)
					{
						if (this.settings_common.GetValueInt("typeOpenDevice", 0) == 0)
						{
							while (this.isOpeningDevice)
							{
								Common.DelayTime(0.5);
								if (this.isStop)
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
									iResult = 1;
									goto Xong;
								}
							}
							this.isOpeningDevice = true;
						}
						else if (this.checkDelayLD > 0)
						{
							int delayOpenDeviceTime = this.rd.Next(this.settings_common.GetValueInt("nudDelayOpenDeviceFrom", 1), this.settings_common.GetValueInt("nudDelayOpenDeviceTo", 1) + 1);
							if (delayOpenDeviceTime > 0)
							{
								int tickCount = Environment.TickCount;
								while ((Environment.TickCount - tickCount) / 1000 - delayOpenDeviceTime < 0)
								{
									this.SetStatusAccount(indexRow, statusProxy + "Mở thiết bị sau {time}s...".Replace("{time}", (delayOpenDeviceTime - (Environment.TickCount - tickCount) / 1000).ToString()), null);
									device.LoadStatusLD("Open device after {time}s...".Replace("{time}", (delayOpenDeviceTime - (Environment.TickCount - tickCount) / 1000).ToString()));
									Common.DelayTime(0.5);
									if (this.isStop)
									{
										this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
										iResult = 1;
										goto Xong;
									}
								}
							}
						}
						else
						{
							this.checkDelayLD++;
						}
					}

					// Mở thiết bị
					this.SetStatusAccount(indexRow, statusProxy + "Mở thiết bị...", null);
					device.LoadStatusLD("Open device...");
					device.Open(120);
					if (device.process == null)
					{
						this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Mở thiết bị!", null);
						this.isOpeningDevice = false;
						iResult = 1;
						goto Xong;
					}
					else
					{
						if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
						{
							fViewLD.remote.AddLDIntoPanel(device.process.MainWindowHandle, device.IndexDevice, indexRow);
						}

						if (!device.CheckOpenedDevice(60))
						{
							this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Mở thiết bị!", null);
							this.isOpeningDevice = false;
							iResult = 1;
						}
						else
						{
							this.isOpeningDevice = false;
							this.SetStatusAccount(indexRow, statusProxy + "Mở thiết bị thành công!", device);
							device.LoadStatusLD("Open device success...");
							this.lstDevice.Add(device);

							if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
							{
								fViewLD.remote.ShowPicTurnOffDevice(device.IndexDevice, device.DeviceId);
							}

							// Cài đặt ứng dụng
							this.SetStatusAccount(indexRow, statusProxy + "Đang xóa app Telegram...", device);
							device.ClearDataApp("org.telegram.messenger");

							for (int n = 0; n < 5; n++)
							{
								device.lstPackages = device.GetListPackages();
								if (device.lstPackages.Contains("com.android.adbkeyboard") && device.lstPackages.Contains("org.telegram.messenger"))
								{
									break;
								}
								if (!device.lstPackages.Contains("com.android.adbkeyboard"))
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đang cài app Keyboard...", device);
									device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\ADBKeyboard.apk");
								}
								if (!device.lstPackages.Contains("org.telegram.messenger"))
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đang cài app Telegram...", device);
									device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\telegram.apk");
								}
							}

							// Kiểm tra cài đặt
							if (!device.lstPackages.Contains("com.android.adbkeyboard"))
							{
								this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Install App!", device);
								iResult = 1;
							}
							else
							{
								if (this.settings_common.GetValueBool("ckbEnableGPS", false))
								{
									device.ExecuteCMD("shell settings put secure location_providers_allowed +gps", 10);
								}
								else
								{
									device.ExecuteCMD("shell settings put secure location_providers_allowed -gps", 10);
								}
								device.RemoveProxy();

								// Đổi IP
								if (proxy != "" && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 0)
								{
									device.LoadStatusLD("Connect proxy...");
									this.SetStatusAccount(indexRow, statusProxy + "Connect proxy...", null);
									if (proxy.Split(new char[]
									{
												':'
									}).Length == 2)
									{
										device.ConnectProxy(proxy);
									}
									else
									{
										if (!Base.isUseProxyUserPass)
										{
											this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Chưa hỗ trợ proxy dạng user pass!", null);
											iResult = 1;
											goto Xong;
										}
										for (int num5 = 0; num5 < 5; num5++)
										{
											device.lstPackages = device.GetListPackages();
											if (device.lstPackages.Contains("com.cell47.College_Proxy"))
											{
												break;
											}
											if (!device.lstPackages.Contains("com.cell47.College_Proxy"))
											{
												this.SetStatusAccount(indexRow, statusProxy + "Install App Proxy...", null);
												device.LoadStatusLD("Install App Proxy...");
												device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\collegeproxy.apk");
											}
										}
										if (!device.lstPackages.Contains("com.cell47.College_Proxy"))
										{
											this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Install App Proxy!", null);
											goto Xong;
										}
										device.ClearDataApp("com.cell47.College_Proxy");
										this.SetStatusAccount(indexRow, statusProxy + "Connect proxy...", null);
										device.LoadStatusLD("Connect proxy...");
										if (!this.ConnectProxy(device, proxy))
										{
											this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Connect proxy!", null);
											goto Xong;
										}
										device.InputKey(Device.KeyEvent.KEYCODE_HOME);
									}
								}
							}

							// Chạy đăng ký
							// string outputDangKy = this.ChayDangKy(device, indexRow, statusProxy);
							string outputDangKy = this.ChayDangKyTelegramX(device, indexRow, statusProxy, proxy);


							if (outputDangKy.StartsWith("1|"))
							{
								// Thành công
								string phone = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPhone");
								string password = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPassword");
								string api_id = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cAPIID");
								string api_hash = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cAPIHash");
								lock (this.lock_Output_Success)
								{
									File.AppendAllText(string.Concat("output//success-", DateTime.Now.ToString("dd-MM-yyyy"), ".txt"), string.Concat(phone, "|", password, "|", api_id, "|", api_hash, "\r\n"));
								}
							}
							else
							{
								// Lỗi
								string phone = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPhone");
								string password = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPassword");
								lock (this.lock_Output_Fail)
								{
									File.AppendAllText(string.Concat("output//failed-", DateTime.Now.ToString("dd-MM-yyyy"), ".txt"), string.Concat(phone, "|", password, "|", outputDangKy.Split('|')[1], DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), "\r\n"));
								}

								this.SetStatusAccount(indexRow, statusProxy + "[LỖI] " + outputDangKy.Split('|')[1], null);
								iResult = 1;
								goto Xong;
							}

							goto Xong;
						}
					}

				}
			}
			catch (Exception ex)
			{
				device.ExportError(ex, "Lỗi không xác định!");
				this.SetStatusAccount(indexRow, statusProxy + "Lỗi không xác định!", null);
				iResult = 1;
				Common.ExportError(ex, "Lỗi không xác định!");
			}

		Xong:

			// Đóng device
			try
			{
				int delayTimeCloseLD = this.rd.Next(this.settings_common.GetValueInt("nudDelayCloseDeviceFrom", 0), this.settings_common.GetValueInt("nudDelayCloseDeviceTo", 0) + 1);
				if (delayTimeCloseLD > 0)
				{
					int tickCount2 = Environment.TickCount;
					while ((Environment.TickCount - tickCount2) / 1000 - delayTimeCloseLD < 0)
					{
						if (this.isStop)
						{
							this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
							break;
						}
						//this.SetStatusAccount(indexRow, statusProxy +"Đóng LD sau {time}s...".Replace("{time}", (delayTimeCloseLD - (Environment.TickCount - tickCount2) / 1000).ToString()), null);
						Common.DelayTime(0.5);
					}
				}

				// Đóng app
				device.CloseApp("org.telegram.messenger");
				if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
				{
					fViewLD.remote.RemovePanelDevice(device.IndexDevice);
				}
				else
				{
					// device.Close();
				}
			}
			catch
			{
			}

			// Xóa LD
			// device.Remove();
			this.lstDevice.Remove(device);

			switch (iResult)
			{
				case 1:
					// Thất bại
					this.ThemThatBai();
					this.dtgvAcc.Rows[indexRow].DefaultCellStyle.BackColor = Color.LightBlue;
					this.dtgvAcc.Rows[indexRow].DefaultCellStyle.ForeColor = Color.White;
					break;
				default:
					// Thành công
					this.dtgvAcc.Rows[indexRow].DefaultCellStyle.BackColor = Color.Green;
					this.dtgvAcc.Rows[indexRow].DefaultCellStyle.ForeColor = Color.White;
					this.ThemThanhCong();
					break;
			}

			lock (lock_FinishProxy)
			{
				switch (settings_common.GetValueInt("ip_iTypeChangeIp"))
				{
					case 7:
						//tinsoft
						if (tinsoft != null)
							tinsoft.DecrementDangSuDung();
						break;
					case 8:
						//xproxy
						if (xproxy != null)
							xproxy.DecrementDangSuDung();
						break;
					case 10:
						// tmproxy

						if (tmproxy != null)
							tmproxy.DecrementDangSuDung();
						break;
				}
			}
		}

		#endregion
		private async void ExcuteOneThread(int indexRow, int indexPos, string pathLD)
		{
			Device device = null;
			Chrome chrome = null;
			int iResult = 0;

			string ip = "";
			string statusProxy = "";
			string proxy = "";
			int typeProxy = 0;

			TinsoftProxy tinsoft = null;
			XproxyProxy xproxy = null;
			TMProxy tmproxy = null;
			ProxyWeb proxyWeb = null;
			ShopLike shopLike = null;
			Console.WriteLine(string.Format("Index row: {0} - {1}", indexRow, indexPos));
			try
			{
				// Kiểm tra dừng hay chưa
				if (this.isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					iResult = 1;
				}

			// Đổi proxy
			#region Proxy            
			getProxy:
				if (isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					iResult = 1;
					goto Xong;
				}

				switch (settings_common.GetValueInt("ip_iTypeChangeIp"))
				{
					case 7:
						this.SetStatusAccount(indexRow, "Đang lấy proxy Tinsoft...", null);
						lock (lock_StartProxy)
						{
							do
							{
								if (isStop)
									break;
								tinsoft = null;

								do
								{
									if (isStop)
										break;
									foreach (var item in listTinsoft)
									{
										if (tinsoft == null || item.daSuDung < tinsoft.daSuDung)
											tinsoft = item;
									}
								} while (tinsoft.daSuDung == tinsoft.limit_theads_use);

								if (isStop)
									break;
								if (tinsoft.daSuDung > 0 || tinsoft.ChangeProxy())
								{
									proxy = tinsoft.proxy;
									if (proxy == "")
										proxy = tinsoft.GetProxy();
									tinsoft.dangSuDung++;
									tinsoft.daSuDung++;
									break;
								}
							} while (true);
							if (isStop)
							{
								this.SetStatusAccount(indexRow, "Đã dừng!", null);
								iResult = 1;
								goto Xong;
							}

							statusProxy = $"(IP: {proxy.Split(':')[0]}) ";
							SetStatusAccount(indexRow, statusProxy + "Check IP...");

							for (int i = 0; i < 30; i++)
							{
								Common.DelayTime(1);
								ip = Common.CheckProxy(proxy, typeProxy);
								if (ip != "")
									break;
							}

							if (ip == "")
							{
								tinsoft.dangSuDung--;
								tinsoft.daSuDung--;
								goto getProxy;
							}
						}
						break;
					case 8:
						//xproxy
						this.SetStatusAccount(indexRow, "Đang lấy Xproxy ...", null);
						lock (lock_StartProxy)
						{
							do
							{
								if (isStop)
									break;
								xproxy = null;

								do
								{
									if (isStop)
										break;
									foreach (var item in listxProxy)
									{
										if (xproxy == null || item.daSuDung < xproxy.daSuDung)
											xproxy = item;
									}
								} while (xproxy.daSuDung == xproxy.limit_theads_use);

								if (isStop)
									break;
								if (xproxy.daSuDung > 0 || xproxy.ChangeProxy())
								{
									proxy = xproxy.proxy;
									typeProxy = xproxy.typeProxy;
									xproxy.dangSuDung++;
									xproxy.daSuDung++;
									break;
								}
							} while (true);
							if (isStop)
							{
								this.SetStatusAccount(indexRow, "Đã dừng!", null);
								iResult = 1;
								goto Xong;
							}

							this.SetStatusAccount(indexRow, "Check IP Xproxy...", null);

							for (int i = 0; i < 30; i++)
							{
								Common.DelayTime(1);
								ip = Common.CheckProxy(proxy, typeProxy);
								if (ip != "")
									break;
							}

							if (ip == "")
							{
								xproxy.dangSuDung--;
								xproxy.daSuDung--;
								goto getProxy;
							}
						}
						break;
					case 9:
						proxy = Common.GetRandomItemFromListNoDel(ref lstProxy, new Random());
						typeProxy = 0;

						for (int i = 0; i < 30; i++)
						{
							Common.DelayTime(1);
							try
							{
								ip = proxy.Split(':')[0] + ":" + proxy.Split(':')[1];
							}
							catch { }

							//ip = Common.CheckProxy(proxy, typeProxy);
							if (ip != "")
								break;
						}

						if (ip == "")
						{
							this.SetStatusAccount(indexRow, statusProxy + "Không thể kết nối Proxy!");
							iResult = 1;
							goto Xong;
						}
						break;
					case 10:
						//tmproxy
						LoadStatusDatagridView(indexRow, "Đang lấy TMProxy ...");
						lock (lock_StartProxy)
						{
							do
							{
								if (isStop)
									break;
								tmproxy = null;

                                do
                                {
                                    if (isStop)
                                        break;
                                    foreach (var item in listTMProxy)
                                    {
                                        if (tmproxy == null || item.daSuDung < tmproxy.daSuDung)
                                            tmproxy = item;
                                    }
                                } while (tmproxy.daSuDung == tmproxy.limit_theads_use);
								
								if (isStop)
									break;
								if (tmproxy.daSuDung > 0 || tmproxy.ChangeProxy(indexRow))
								{
									proxy = tmproxy.proxy;
									if (proxy == "")
										proxy = tmproxy.GetProxy();
									tmproxy.dangSuDung++;
									tmproxy.daSuDung++;
									break;
								}
							} while (true);
							if (isStop)
							{
								SetStatusAccount(indexRow, statusProxy + "Đã dừng!");
								iResult = 1;
								goto Xong;
							}
							statusProxy = $"(IP: {proxy.Split(':')[0]}) ";
							SetStatusAccount(indexRow, statusProxy + "Check IP...");

							ip = Common.CheckProxy(proxy, 0);
							if (ip == "")
							{
								tmproxy.dangSuDung--;
								tmproxy.daSuDung--;
								goto getProxy;
							}
						}
						break;
					case 11:
						this.SetStatusAccount(indexRow, Language.GetValue("Đang lấy Proxyv6..."));
						lock (this.lock_StartProxy)
						{
							while (!this.isStop)
							{
								proxyWeb = null;
								while (!this.isStop)
								{
									foreach (ProxyWeb item4 in this.listProxyWeb)
									{
										if (proxyWeb == null || item4.daSuDung < proxyWeb.daSuDung)
										{
											proxyWeb = item4;
										}
									}
									if (proxyWeb.daSuDung != proxyWeb.limit_theads_use)
									{
										break;
									}
								}
								if (this.isStop)
								{
									break;
								}
								if (proxyWeb.daSuDung > 0 || proxyWeb.ChangeProxy())
								{
									statusProxy = proxyWeb.proxy;
									typeProxy = proxyWeb.typeProxy;
									proxyWeb.dangSuDung++;
									proxyWeb.daSuDung++;
									break;
								}
								bool flag5 = true;
							}
							if (this.isStop)
							{
								this.SetStatusAccount(indexRow, statusProxy + Language.GetValue("Đã dừng!"));
								iResult = 1;
								break;
							}
							bool flag6 = true;
							if (this.settings_common.GetValueInt("nudDelayCheckIP") > 0)
							{
								this.SetStatusAccount(indexRow, statusProxy + "Delay check IP...");
								Common.DelayTime(this.settings_common.GetValueInt("nudDelayCheckIP"));
							}
							if (!this.settings_common.GetValueBool("ckbKhongCheckIP"))
							{
								statusProxy = "(IP: " + proxy.Split(':')[0] + ") ";
								this.SetStatusAccount(indexRow, statusProxy + "Check IP...");
								int num4 = 0;
								while (true)
								{
									if (num4 >= 30)
									{
										goto IL_15c3;
									}
									Common.DelayTime(1.0);
									ip = Common.CheckProxy(proxy, typeProxy);
									if (ip != "")
									{
										goto IL_15c3;
									}
									if (this.isStop)
									{
										this.SetStatusAccount(indexRow, statusProxy + Language.GetValue("Đã dừng!"));
										iResult = 1;
										goto Xong;
									}
									num4++;
									continue;
								IL_15c3:
									if (ip == "")
									{
										flag6 = false;
									}
									break;
								}
							}
							if (!flag6)
							{
								proxyWeb.dangSuDung--;
								proxyWeb.daSuDung--;
							}
							goto default;
						}
					case 12:
						this.SetStatusAccount(indexRow, Language.GetValue("Đang lấy Proxy ShopLike ..."));
						lock (this.lock_StartProxy)
						{
							while (!this.isStop)
							{
								shopLike = null;
								while (!this.isStop)
								{
									foreach (ShopLike item5 in this.listShopLike)
									{
										if (shopLike == null || item5.daSuDung < shopLike.daSuDung)
										{
											shopLike = item5;
										}
									}
									if (shopLike.daSuDung != shopLike.limit_theads_use)
									{
										break;
									}
								}
								if (this.isStop)
								{
									break;
								}
								if (shopLike.daSuDung > 0 || shopLike.ChangeProxy())
								{
									proxy = shopLike.proxy;
									if (proxy == "")
									{
										proxy = shopLike.GetProxy();
									}
									shopLike.dangSuDung++;
									shopLike.daSuDung++;
									break;
								}
								bool flag3 = true;
							}
							if (this.isStop)
							{
								this.SetStatusAccount(indexRow, statusProxy + Language.GetValue("Đã dừng!"));
								iResult = 1;
								break;
							}
							bool flag4 = true;
							if (this.settings_common.GetValueInt("nudDelayCheckIP") > 0)
							{
								this.SetStatusAccount(indexRow, statusProxy + "Delay check IP...");
								Common.DelayTime(this.settings_common.GetValueInt("nudDelayCheckIP"));
							}
							if (!this.settings_common.GetValueBool("ckbKhongCheckIP"))
							{
								statusProxy = "(IP: " + proxy.Split(':')[0] + ") ";
								this.SetStatusAccount(indexRow, statusProxy + "Check IP...");
								ip = Common.CheckProxy(proxy, 0);
								if (ip == "")
								{
									flag4 = false;
								}
							}
							if (!flag4)
							{
								shopLike.dangSuDung--;
								shopLike.daSuDung--;
							}
							goto default;
						}
					default:
						break;
				}

				if (isStop)
				{
					this.SetStatusAccount(indexRow, "Đã dừng", null);
					iResult = 1;
					goto Xong;
				}
				#endregion

				// Thêm dòng
				base.Invoke(new Action(delegate ()
				{
					this.dtgvAcc.Rows.Add();
				}));
				Common.DelayTime(1);

				//goto Xong;

				//this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
				//iResult = 1;
				//goto Xong;


				// Kiểm tra Proxy
				if (!this.settings_common.GetValueBool("ckbKhongCheckIP", false))
				{
					if (this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 7 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 8 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 10 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 11 && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 12)
					{
						if (ip != "")
						{
							statusProxy = "(IP: " + ip.Split(new char[]
							{
										':'
							})[0] + ") ";
						}
						this.SetStatusAccount(indexRow, statusProxy + "Check IP...", null);
						bool checkProxy = false;
						int k = 0;
						while (k < 30)
						{
							Common.DelayTime(1.0);
							proxy = Common.CheckProxy(ip, typeProxy);
							if (!(proxy != ""))
							{
								if (this.isStop)
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
									iResult = 1;
									goto Xong;
								}
								k++;
							}
							else
							{
								checkProxy = true;
								break;
							}
						}
						if (!checkProxy)
						{
							if (ip != "")
							{
								this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Không thể kết nối proxy!", null);
							}
							else
							{
								this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Không có kết nối Internet!", null);
							}
							iResult = 1;
							goto Xong;
						}
						statusProxy = "(IP: " + proxy + ") ";
					}
				}

				if (settings_common.GetValueInt("ip_iTypeChangeIp") == 9)
				{
					statusProxy = "(IP: " + ip + ") ";
				}

				if (this.isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					iResult = 1;
					goto Xong;
				}
				else
				{
					// Mở thiết bị
					// device = new Device(pathLD, indexPos.ToString() ?? "");
					try
					{
						device = new Device(pathLD, (indexPos + 1).ToString() ?? "");
						if (!Directory.Exists(pathLD + "\\vms\\leidian" + (indexPos + 1).ToString()))
						{
							this.SetStatusAccount(indexRow, statusProxy + "Đang tạo thiết bị...", null);
							for (int l = 0; l < 2; l++)
							{
								// ADBHelper.AddDevice(pathLD);
								ADBHelper.CopyDevice(pathLD);
								if (Directory.Exists(pathLD + "\\vms\\leidian" + (indexPos + 1).ToString()))
								{
									break;
								}
							}
							this.SetStatusAccount(indexRow, statusProxy + "Đang cấu hình thiết bị...", null);
							lock (this.lock_restoreDevice)
							{
								device.Restore();
							}
							device.ChangeHardwareLDPlayer2();
							device.ChangeFileConfig();
						}
						else
						{
							// Change thiết bị
							device.ChangeHardwareLDPlayer2();
							device.ChangeFileConfig();
						}
					}
					catch (Exception ex)
					{
						Common.ExportError(ex, "");
					}

					// Chờ tới lượt
					this.SetStatusAccount(indexRow, statusProxy + "Chờ đến lượt...", null);
					lock (this.lock_checkDelayLD)
					{
						if (this.settings_common.GetValueInt("typeOpenDevice", 0) == 0)
						{
							while (this.isOpeningDevice)
							{
								Common.DelayTime(0.5);
								if (this.isStop)
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
									iResult = 1;
									goto Xong;
								}
							}
							this.isOpeningDevice = true;
						}
						else if (this.checkDelayLD > 0)
						{
							int delayOpenDeviceTime = this.rd.Next(this.settings_common.GetValueInt("nudDelayOpenDeviceFrom", 1), this.settings_common.GetValueInt("nudDelayOpenDeviceTo", 1) + 1);
							if (delayOpenDeviceTime > 0)
							{
								int tickCount = Environment.TickCount;
								while ((Environment.TickCount - tickCount) / 1000 - delayOpenDeviceTime < 0)
								{
									this.SetStatusAccount(indexRow, statusProxy + "Mở thiết bị sau {time}s...".Replace("{time}", (delayOpenDeviceTime - (Environment.TickCount - tickCount) / 1000).ToString()), null);
									device.LoadStatusLD("Open device after {time}s...".Replace("{time}", (delayOpenDeviceTime - (Environment.TickCount - tickCount) / 1000).ToString()));
									Common.DelayTime(0.5);
									if (this.isStop)
									{
										this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
										iResult = 1;
										goto Xong;
									}
								}
							}
						}
						else
						{
							this.checkDelayLD++;
						}
					}

					// Mở thiết bị
					this.SetStatusAccount(indexRow, statusProxy + "Mở thiết bị...", null);
					device.LoadStatusLD("Open device...");
					device.Open(120);
					if (device.process == null)
					{
						this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Lỗi mở thiết bị!", null);
						this.isOpeningDevice = false;
						iResult = 1;
                        try
                        {
							device.Close();
                        }
                        catch { }
						goto Xong;
					}
					else
					{
						if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
						{
							fViewLD.remote.AddLDIntoPanel(device.process.MainWindowHandle, device.IndexDevice, indexRow);
						}

						if (!device.CheckOpenedDevice(60))
						{
							this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Lỗi mở thiết bị!", null);
							this.isOpeningDevice = false;
							iResult = 1;
							try
							{
								device.Close();
							}
							catch { }

							// Ẩn
							this.dtgvAcc.Invoke(new MethodInvoker(() =>
							{
								dtgvAcc.Rows[indexRow].Visible = false;
								this.dtgvAcc.Refresh();
							}));
						}
						else
						{
							this.isOpeningDevice = false;
							this.SetStatusAccount(indexRow, statusProxy + "Mở thiết bị thành công!", device);
							device.LoadStatusLD("Open device success...");
							this.lstDevice.Add(device);

							if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
							{
								fViewLD.remote.ShowPicTurnOffDevice(device.IndexDevice, device.DeviceId);
							}

							// Cài đặt ứng dụng
							//this.SetStatusAccount(indexRow, statusProxy + "Đang xóa app Telegram...", device);
							//device.ClearDataApp("org.telegram.messenger");

							for (int n = 0; n < 5; n++)
							{
								device.lstPackages = device.GetListPackages();
								if (device.lstPackages.Contains("com.android.adbkeyboard") && device.lstPackages.Contains("org.telegram.messenger"))
								{
									break;
								}
								if (!device.lstPackages.Contains("com.android.adbkeyboard"))
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đang cài app Keyboard...", device);
									device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\ADBKeyboard.apk");
								}
								if (!device.lstPackages.Contains("org.telegram.messenger"))
								{
									this.SetStatusAccount(indexRow, statusProxy + "Đang cài app Telegram...", device);
									device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\Telegram.apk");
								}
							}

							// Kiểm tra cài đặt
							if (!device.lstPackages.Contains("com.android.adbkeyboard"))
							{
								this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Lỗi Install App!", device);
								iResult = 1;
							}
							else
							{
								if (this.settings_common.GetValueBool("ckbEnableGPS", false))
								{
									device.ExecuteCMD("shell settings put secure location_providers_allowed +gps", 10);
								}
								else
								{
									device.ExecuteCMD("shell settings put secure location_providers_allowed -gps", 10);
								}
								device.RemoveProxy();

								// Đổi IP version 2
								if (proxy != "" && this.settings_common.GetValueInt("ip_iTypeChangeIp", 0) != 0)
                                {
									device.LoadStatusLD("Connect proxy...");
									this.SetStatusAccount(indexRow, statusProxy + "Connect proxy...", null);

									if (proxy.Split(new char[]
									{
												':'
									}).Length == 2)
									{

										// Kết nối
										for (int num5 = 0; num5 < 5; num5++)
										{
											device.lstPackages = device.GetListPackages();
											if (device.lstPackages.Contains("com.cell47.College_Proxy"))
											{
												break;
											}
											if (!device.lstPackages.Contains("com.cell47.College_Proxy"))
											{
												this.SetStatusAccount(indexRow, statusProxy + "Install App Proxy...", null);
												device.LoadStatusLD("Install App Proxy...");
												device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\collegeproxy.apk");
											}
										}
										if (!device.lstPackages.Contains("com.cell47.College_Proxy"))
										{
											this.SetStatusAccount(indexRow, statusProxy + "Lỗi Install App Proxy!", null);
											goto Xong;
										}
										device.ClearDataApp("com.cell47.College_Proxy");
										this.SetStatusAccount(indexRow, statusProxy + "Đang kết nối Proxy...", null);
										device.LoadStatusLD("Đang kết nối Proxy...");
										if (!this.ConnectProxyV2(device, proxy))
										{
											this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Kết nối proxy lỗi!", null);

											// Ẩn
											this.dtgvAcc.Invoke(new MethodInvoker(() =>
											{
												dtgvAcc.Rows[indexRow].Visible = false;
												this.dtgvAcc.Refresh();
											}));

											goto Xong;
										}
										device.InputKey(Device.KeyEvent.KEYCODE_HOME);

									}

								}

							}

							// Chạy đăng ký
							string outputDangKy = "";
							if (settings_common.GetValueInt("typeRun") == 0)
							{
								// outputDangKy = this.ChayDangKyTest(chrome, device, indexRow, statusProxy, proxy);
								outputDangKy = await this.ChayDangKy(chrome, device, indexRow, statusProxy, proxy);

								// Xóa app
								// device.UninstallApp("org.telegram.messenger");
								device.CloseApp("com.sollyu.xposed.hook.model");
							}
							else
							{
								outputDangKy = this.ChayDangKyTelegramX(device, indexRow, statusProxy, proxy);
							}

							if (outputDangKy.StartsWith("1|"))
							{
								// Thành công
								string phone = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPhone");
								string password = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPassword");
								string api_id = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cAPIID");
								string api_hash = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cAPIHash");
								string username = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cName");
								lock (this.lock_Output_Success)
								{
									phone = "+84" + phone.Substring(1);

									File.AppendAllText(string.Concat("output//success-", DateTime.Now.ToString("dd-MM-yyyy"), ".txt"), string.Concat(phone, "|", password, "|", username, "|", api_id, "|", api_hash, "\r\n"));

								}
							}
							else
							{
                                // Xóa thiết bị
                                //if (outputDangKy.ToLower().Contains("thiết bị đã bị khóa"))
                                //{
                                //	device.Close();
                                //	device.Remove();
                                //}

                                // Ẩn
                                this.dtgvAcc.Invoke(new MethodInvoker(() =>
                                {
                                    dtgvAcc.Rows[indexRow].Visible = false;
									this.dtgvAcc.Refresh();
                                }));

                                // Lỗi
                                string phone = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPhone");
								string password = Common.GetStatusDataGridView(dtgvAcc, indexRow, "cPassword");
								lock (this.lock_Output_Fail)
								{
									File.AppendAllText(string.Concat("output//failed-", DateTime.Now.ToString("dd-MM-yyyy"), ".txt"), string.Concat(phone, "|", password, "|", outputDangKy.Split('|')[1], DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), "\r\n"));
								}

								this.SetStatusAccount(indexRow, statusProxy + "[LỖI] " + outputDangKy.Split('|')[1], null);
								iResult = 1;
								goto Xong;
							}

							goto Xong;
						}
					}

				}
			}
			catch (Exception ex)
			{
				device.ExportError(ex, "[LỖI] Lỗi khác:" + ex.Message.ToString());
				this.SetStatusAccount(indexRow, statusProxy + "[LỖI] Lỗi khác:" + ex.Message.ToString(), null);
				iResult = 1;
				Common.ExportError(ex, "ExcuteOneThread()");
				this.dtgvAcc.Invoke(new MethodInvoker(() =>
				{
					dtgvAcc.Rows[indexRow].Visible = false;
					this.dtgvAcc.Refresh();
				}));
			}

		Xong:

			// Đóng device
			try
			{
				int delayTimeCloseLD = this.rd.Next(this.settings_common.GetValueInt("nudDelayCloseDeviceFrom", 0), this.settings_common.GetValueInt("nudDelayCloseDeviceTo", 0) + 1);
				if (delayTimeCloseLD > 0)
				{
					int tickCount2 = Environment.TickCount;
					while ((Environment.TickCount - tickCount2) / 1000 - delayTimeCloseLD < 0)
					{
						if (this.isStop)
						{
							this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
							break;
						}
						//this.SetStatusAccount(indexRow, statusProxy +"Đóng LD sau {time}s...".Replace("{time}", (delayTimeCloseLD - (Environment.TickCount - tickCount2) / 1000).ToString()), null);
						Common.DelayTime(0.5);
					}
				}

				// Đóng app
				if (!this.settings_common.GetValueBool("ckbKhongAddVaoFormView", false))
				{
					fViewLD.remote.RemovePanelDevice(device.IndexDevice);
				}
				else
				{
					// device.Close();
				}
			}
			catch
			{
			}


			try
			{
				// Xóa LD
				// device.Remove();
				this.lstDevice.Remove(device);
				// ThemTotal();

				switch (iResult)
				{
					case 1:
						// Thất bại
						this.ThemThatBai();
						this.dtgvAcc.Rows[indexRow].DefaultCellStyle.BackColor = Color.LightBlue;
						this.dtgvAcc.Rows[indexRow].DefaultCellStyle.ForeColor = Color.White;
						break;
					default:
						// Thành công
						this.dtgvAcc.Rows[indexRow].DefaultCellStyle.BackColor = Color.Green;
						this.dtgvAcc.Rows[indexRow].DefaultCellStyle.ForeColor = Color.White;
						this.ThemThanhCong();
						break;
				}

				// Xử lý xoá các dòng lỗi
				//this.dtgvAcc.Invoke(new MethodInvoker(() => {
				//	try
				//	{
				//		this.dtgvAcc.Rows.RemoveAt(indexRow);
				//		this.dtgvAcc.Refresh();
				//	}
				//	catch (Exception ex)
				//	{

				//	}

				//}));

				lock (lock_FinishProxy)
				{
					switch (settings_common.GetValueInt("ip_iTypeChangeIp"))
					{
						case 7:
							//tinsoft
							if (tinsoft != null)
								tinsoft.DecrementDangSuDung();
							break;
						case 8:
							//xproxy
							if (xproxy != null)
								xproxy.DecrementDangSuDung();
							break;
						case 10:
							// tmproxy

							if (tmproxy != null)
								tmproxy.DecrementDangSuDung();
							break;
					}
				}
			}
			catch (Exception)
			{

			}

			
		}

		private void ThemTotal()
		{
			//this.lbtongacc.GetCurrentParent().Invoke(new MethodInvoker(() => {
			//	try
			//	{
			//		int current = Int16.Parse(this.lbtongacc.Text.Trim());
			//		this.lbtongacc.Text = (current + 1).ToString();
			//	}
			//	catch (Exception ex)
			//	{

			//	}

			//}));
		}

		private void Them2FA()
		{
			this.lb2fa.GetCurrentParent().Invoke(new MethodInvoker(() => {
				try
				{
					int current = Int16.Parse(this.lb2fa.Text.Trim());
					this.lb2fa.Text = (current + 1).ToString();
				}
				catch (Exception ex)
				{

				}

			}));
		}


		private void ThemThanhCong()
		{
			this.lblive.GetCurrentParent().Invoke(new MethodInvoker(() => {
				try
				{
					int current = Int16.Parse(this.lblive.Text.Trim());
					this.lblive.Text = (current + 1).ToString();
				}
				catch (Exception ex)
				{

				}

			}));
		}

		private void ThemThatBai()
		{
			this.lbdie.GetCurrentParent().Invoke(new MethodInvoker(() => {
				try
				{
					int current = Int16.Parse(this.lbdie.Text.Trim());
					this.lbdie.Text = (current + 1).ToString();
				}
				catch (Exception ex)
				{

				}

			}));
		}

		public string RandomFirstName(int type)
		{
			List<string> FirstName = new List<string>();
			Random rd = new Random();
			if (type == 1)
			{
				// Tiếng Việt
				FirstName = lstFirstNameVN;
			}
			else
			{
				// Tiếng Anh
				FirstName = lstFirstNameEN;
			}
			string firstName = FirstName[rd.Next(0, FirstName.Count<string>() - 1)];
			return firstName;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x001AA9E8 File Offset: 0x001A8BE8
		public string RandomLastName(int type)
		{
			List<string> LastName = new List<string>();
			Random rd = new Random();
			if (type == 1)
			{
				// Tiếng Việt
				LastName = lstLastNameVN;

			}
			else
			{
				// Tiếng Anh
				LastName = lstLastNameEN;
			}
			string lastName = LastName[rd.Next(0, LastName.Count<string>() - 1)];
			return lastName;
		}

		private const string TELEGRAM_PACKAGE = "org.telegram.messenger";
		private const string MOBILE_FAKER_PACKAGE = "com.unique.mobilefaker";
		private const string DEVICE_EMULATOR_PACKAGE = "com.device.emulator.pro";

		/// <summary>
		/// Hàm đăng ký Telegram X
		/// </summary>
		/// <param name="device"></param>
		/// <param name="indexRow"></param>
		/// <param name="statusProxy"></param>
		/// <returns></returns>
		private string ChayDangKyTelegramX(Device device, int indexRow, string statusProxy, string proxy)
		{
			string output = "";
			string phoneSimThue = "";
			string phoneNumber = "";
			string requestID = "";
			string otpCode = "";
			string firtname = "";
			string lastname = "";
			string password = "123456Ac";
			string username = "";
			string api_id = "";
			string api_hash = "";
			try
			{
				// Bấm ok nếu lỗi
				device.TapByText("ok", "", 5);
				Thread.Sleep(500);
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cSTT", indexRow);
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cLD", "LDPLayer-" + device.IndexDevice);
				device.LoadStatusLD(statusProxy + "Đang fake thiết bị...");
				this.SetStatusAccount(indexRow, statusProxy + "Đang fake thiết bị...", null);

				device.OpenApp("com.sollyu.xposed.hook.model");
				Thread.Sleep(2000);
				device.WaitForImageAppear("Telegram\\Device", 10, true);
				device.WaitForImageAppear("Telegram\\Device2", 10, true);
				device.WaitForImageAppear("Telegram\\RandomInfo", 10, true);
				device.WaitForImageAppear("Telegram\\SaveInfo", 10, true);
				Thread.Sleep(500);
				device.OpenApp(DEVICE_EMULATOR_PACKAGE);
				device.WaitForImageAppear("DataClick\\IMG_RANDOM_DEVICE", 10, true);
				Thread.Sleep(500);
				device.OpenApp(MOBILE_FAKER_PACKAGE);
				device.WaitForImageAppear("DataClick\\IMG_RANDOM_INFO", 10, true);
				device.WaitForImageAppear("DataClick\\IMG_RANDOM_INFO_APPLY", 10, true);
				Thread.Sleep(1000);

				// Đăng ký
				device.LoadStatusLD(statusProxy + "Đang mở telegram...");
				this.SetStatusAccount(indexRow, statusProxy + "Đang mở telegram...", null);
				device.OpenApp("org.thunderdog.challegram");
				//device.DeviceId = "emulator-5556";
				device.DelayTime(5);

				// Kiểm tra update
				device.TapByImage("TelegramX\\Cancel", null, 5);
				device.DelayTime(1);

				// device.DeviceId = "emulator-5556";
				device.TapByImage("TelegramX\\Allow", null, 5);
				device.DelayTime(1);

				device.WaitForImageAppear("TelegramX\\Menu2", 10, true);
				device.DelayTime(3);

				if (device.WaitForImageAppear("TelegramX\\AddAcc", 05, true))
				{
					goto NhapPhone;
				}
				else
				{
					device.Tap(408, 223, 1);
					device.DelayTime(3);
					device.WaitForImageAppear("TelegramX\\AddAcc", 10, true);
					device.DelayTime(2);
				}

			NhapPhone:
				// Clear và nhập phone
				for (int j = 0; j < 10; j++)
				{
					device.InputKeyBackspace();
				}
				device.ClearText("");
				device.DelayTime(1);

				// Lấy phone
				device.LoadStatusLD(statusProxy + "Đang lấy phone...");
				this.SetStatusAccount(indexRow, statusProxy + "Đang lấy phone...", null);
				string nhamang = settings_common.GetValue("txtNhaMang");
				string dauso = settings_common.GetValue("txtDauSo");
				if (typePhone == 0)
				{
					nhamang = settings_common.GetValue("txtCTSCNhaMang");
					dauso = settings_common.GetValue("txtCTSCDauSo");
					phoneSimThue = this.GetPhoneChoThueSimCode(this.apiPhone, nhamang, dauso, indexRow, statusProxy, "", false, 200);
				}
				else if (typePhone == 1)
				{
					phoneSimThue = PhoneHelper.GetPhoneOtpSim(this.apiPhone, 60, nhamang, dauso);
				}
				else if (typePhone == 2)
				{
					// tempsms.co
					phoneSimThue = this.GetPhoneTempSMS(this.apiPhone, 60);
				}
				else if (typePhone == 3)
				{
					// simfast.vn
					phoneSimThue = this.GetPhoneSimfast(this.apiPhone, 60);
				}
				else if (typePhone == 4)
				{
					// codesim.net
					phoneSimThue = this.GetPhoneCodeSim(this.apiPhone, 60);
				}
				else if (typePhone == 5)
				{
					// viotp
					phoneSimThue = this.GetPhoneViotp(this.apiPhone, 60);
				}
				else if (typePhone == 6)
				{
					// 2ndline
					phoneSimThue = this.GetPhone2ndLine(this.apiPhone, 60);
				}
				else if (typePhone == 7)
				{
					// sms-active
					phoneSimThue = this.GetPhoneSMSActivate(this.apiPhone, 60);
				}
				else if (typePhone == 8)
				{
					// ahasim
					phoneSimThue = GetPhoneJSNguyenLieu(this.apiPhone);
				}
				else if (typePhone == 9)
				{
					// ahasim
					phoneSimThue = GetPhoneAhasim(this.apiPhone);
				}
				else if (typePhone == 10)
				{
					// thuesimxyz
					phoneSimThue = GetPhoneThuesimXyz(this.apiPhone);
				}


				device.LoadStatusLD(statusProxy + "PhoneSimThue : " + phoneSimThue);
				this.SetStatusAccount(indexRow, statusProxy + "PhoneSimThue : " + phoneSimThue, null);

				if (phoneSimThue.Contains("|"))
				{
					requestID = phoneSimThue.Split(new char[] { '|' })[0];
					phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
				}
				else
				{
					if (phoneSimThue.Contains("tạm hết"))
					{
						return "0|Kho số cho ứng dụng đang tạm hết!";

					}
					else
					{
						return "0|Không lấy được phone, kiểm tra API!";

					}

				}

				if (phoneNumber == "" || phoneNumber == "0")
				{
					return "0|Không lấy được phone -> API lỗi hoặc hết số!";
				}
				else
				{
					// Kiểm tra Blacklist
					if (BlackList.IsBlackList(phoneNumber))
					{
						// Hủy bỏ giao dịch
						if (this.typePhone == 4)
						{
							this.HuyCodeSim(this.apiPhone, requestID, 10);
						}

						return "0|Phone đã đăng ký rồi -> Blacklist!";
					}

					// Thành công
					Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", phoneNumber);
					BlackList.Add(phoneNumber);
					device.LoadStatusLD(statusProxy + "Phone : " + phoneNumber);
					this.SetStatusAccount(indexRow, statusProxy + "Phone : " + phoneNumber, null);
					device.SendText("84" + phoneNumber);
					device.DelayTime(2);
					device.WaitForImageAppear("TelegramX\\PhoneNext", 10, true);
					device.DelayTime(2);


					if (device.WaitForImageAppear("Telegram\\PhoneBanned", 5, false))
					{
						// Hủy bỏ giao dịch
						if (this.typePhone == 4)
						{
							this.HuyCodeSim(this.apiPhone, requestID, 10);
						}

						if (output == "")
							output = "0|Sim thuê đã bị khóa!";
						return output;
					}
					if (device.WaitForImageAppear("Telegram\\DeviceBanned", 5, false))
					{
						if (output == "")
							output = "0|Thiết bị đã bị khóa!";
						return output;
					}

					///// Case lỗi để out
					if (device.WaitForImageAppear("Telegram\\LoiPhone", 5, false))
					{
						// Hủy bỏ giao dịch
						if (this.typePhone == 4)
						{
							this.HuyCodeSim(this.apiPhone, requestID, 10);
						}

						if (output == "")
							output = "0|Site sim lỗi rồi!";
						return output;
					}

					// Kiểm tra gửi code
					// device.DeviceId = "emulator-5556";
					//if (device.WaitForImageAppear("TelegramX\\SMS_CODE", 10, false))
					//{
					//	// Đợi lấy code
					//	goto GET_OTP;
					//} else
					//               {
					//	output = "0|Không truy cập được màn hình lấy SMS Code!";
					//	return output;
					//}

					// Lấy otp
					//GET_OTP:
					device.LoadStatusLD(statusProxy + "Đang lấy OTP...");
					this.SetStatusAccount(indexRow, statusProxy + "Đang lấy OTP...", null);
					int limitTimeOTP = settings_common.GetValueInt("nudTimeOTP", 60);

					// Lấy ra otp
					if (this.typePhone == 0)
					{
						// Chợ thue sim code
						otpCode = this.GetOTPChoThueSimCode(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 1)
					{
						// Otpsim
						otpCode = this.GetOTPOtpSim(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 2)
					{
						// tempsms.co
						otpCode = this.GetOTPTempSMS(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 3)
					{
						// simfast.vn
						otpCode = this.GetOTPSimfast(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 4)
					{
						// codesim.net
						otpCode = this.GetOTPCodeSim(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 5)
					{
						// Viotp
						otpCode = this.GetOTPViotp(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 6)
					{
						// 2ndLine
						otpCode = this.GetOTP2ndLine(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 7)
					{
						// sms-activate
						otpCode = this.GetOTPSMSActivate(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 8)
					{
						// JS Nguyen Lieu
						otpCode = GetOTPJSNguyenLieu(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 9)
					{
						// Ahasim
						otpCode = GetOTPAhasim(this.apiPhone, requestID, limitTimeOTP);
					}

					if ((otpCode == null ? true : otpCode == ""))
					{
						// Hủy bỏ giao dịch
						if (this.typePhone == 4)
						{
							this.HuyCodeSim(this.apiPhone, requestID, 10);
						}

						return "0|Phone không trả về OTP Code!";
					}
					else
					{
						// Lấy code thành công
						device.LoadStatusLD(statusProxy + "OTP : " + otpCode);
						this.SetStatusAccount(indexRow, statusProxy + "OTP : " + otpCode, null);
						char[] charArr = otpCode.ToCharArray();
						device.SendText(charArr[0].ToString());
						device.SendText(charArr[1].ToString());
						device.SendText(charArr[2].ToString());
						device.SendText(charArr[3].ToString());
						device.SendText(charArr[4].ToString());
						device.DelayTime(3);

						// Lỗi pass
						if (device.WaitForImageAppear("Telegram\\InvalidPass", 10, false))
						{
							if (output == "")
								output = "0|Số lỗi đã có pass!";
							return output;
						}

						if (device.GetHtml().Contains("invalid code, please try again."))
						{
							return "0|Invalid code, please try again.";
						}
						else
						{
							// Nhập thành công
							if (settings_common.GetValueInt("typeName") == 0)
							{
								firtname = RandomFirstName(1);
								lastname = RandomLastName(1);
							}
							else
							{
								firtname = RandomFirstName(2);
								lastname = RandomLastName(2);
							}

							username = firtname + this.rd.Next(0, 99).ToString() + this.rd.Next(0, 99).ToString() + this.rd.Next(10, 9999).ToString();
							username = username.ToLower();

							device.LoadStatusLD(statusProxy + "Đang đăng ký...");
							this.SetStatusAccount(indexRow, statusProxy + "Đang đăng ký...", null);

							// Lấy ra lastname
							device.SendText(firtname);
							device.DelayTime(1);
							device.InputKey(Device.KeyEvent.KEYCODE_TAB);
							device.DelayTime(1);
							// Lấy ra firstname
							device.SendText(lastname);
							device.DelayTime(1);
							device.WaitForImageAppear("TelegramX\\PhoneNext", 10, true);
							device.DelayTime(2);

							if (device.WaitForImageAppear("Telegram\\InvalidPass", 10, false))
							{
								if (output == "")
									output = "0|Số Lỗi Đã Có Pass!";
								return output;
							}

							device.Tap(429, 737, 1);
							device.DelayTime(2);
							device.Tap(430, 596, 1);
							device.DelayTime(2);

							// Thành công và đổi mật khẩu
							device.LoadStatusLD(statusProxy + "Đang bật 2FA...");
							this.SetStatusAccount(indexRow, statusProxy + "Đang bật 2FA...", null);
							device.TapByImage("TelegramX\\Menu2", null, 5);
							device.DelayTime(2);

							// Tìm nút Cài đặt để click
							for (int j = 0; j < 10; j++)
							{
								if (device.WaitForImageAppear("TelegramX\\Caidat", 5, true))
								{
									device.DelayTime(1);
									goto NHAP_USERNAME;
								}
								else
								{
									device.Swipe(60, 700, 60, 300, 1000);
								}
							}

						NHAP_USERNAME:
							// device.DeviceId = "emulator-5556";
							device.TapByImage("TelegramX\\SetUsername", null, 5);
							device.DelayTime(2);

							// Nhập username
							device.LoadStatusLD(statusProxy + "Username: " + username);
							this.SetStatusAccount(indexRow, statusProxy + "Username: " + username, null);
							device.SendText(username);

							device.DelayTime(2);
							device.Tap(469, 893, 1);
							device.DelayTime(2);
							Common.SetStatusDataGridView(dtgvAcc, indexRow, "cName", username);

							device.Swipe(505, 848, 515, 332, 1000);
							device.Swipe(505, 848, 515, 332, 1000);
							device.DelayTime(1);
							device.TapByImage("TelegramX\\TwoStep", null, 5);
							device.DelayTime(1);
							device.TapByImage("TelegramX\\TwoStep2", null, 5);
							device.DelayTime(1);
							device.Tap(245, 708, 1);  ///click nút giữa
							device.DelayTime(2);

							// Nhập mật khẩu
							if (settings_common.GetValueInt("typePass") == 0)
							{
								password = settings_common.GetValue("txtPassword");
							}
							else
							{
								password = Common.CreateRandomString(15);
							}

							device.LoadStatusLD(statusProxy + "Password: " + password);
							this.SetStatusAccount(indexRow, statusProxy + "Password: " + password, null);
							device.SendText(password);
							device.DelayTime(1);
							device.InputKey(Device.KeyEvent.KEYCODE_ENTER);
							device.DelayTime(2);
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.InputKeyBackspace();
							device.DelayTime(2);
							device.SendText("easysoftware");
							device.DelayTime(1);
							device.Tap(484, 896, 1);  ///click nút next
							device.DelayTime(2);
							device.Tap(35, 921, 1);  ///click skip
							device.DelayTime(2);
							device.Tap(432, 625, 1); ///click nút ok
							device.DelayTime(2);
							device.TapByImage("TelegramX\\Back", null, 5);
							device.DelayTime(1);
							device.TapByImage("TelegramX\\Back", null, 5);
							device.DelayTime(1);
							device.TapByImage("TelegramX\\Back", null, 5);
							device.DelayTime(1);
							device.TapByImage("TelegramX\\Back", null, 5);
							device.DelayTime(1);
							device.TapByImage("TelegramX\\Back", null, 5);
							device.DelayTime(1);

							// Kiểm tra có ra ngoài màn hình Home hay không.


							//// Upload avatar
							//device.TapByImage("TelegramX\\Menu2", null, 5);
							//device.DelayTime(3);

							//// Tìm nút Cài đặt để click
							//for (int j = 0; j < 10; j++)
							//{
							//	if (device.WaitForImageAppear("TelegramX\\Caidat", 5, true))
							//	{
							//		device.DelayTime(1);
							//		goto NHAP_AVATAR;
							//	}
							//	else
							//	{
							//		device.Swipe(60, 700, 60, 300, 1000);
							//	}
							//}
							//NHAP_AVATAR:
							//device.Tap(276, 230, 1);/// click nút thêm ảnh
							//device.DelayTime(3);
							//device.Tap(115, 921, 1);
							//device.DelayTime(3);

							//if (device.WaitForImageAppear("TelegramX\\Avatar", 5, false))
							//                     {
							//	goto CLICK_DOWNLOAD;
							//                     } else
							//                     {
							//	device.Tap(395, 586, 1);///click nút cho phép
							//	device.DelayTime(3);
							//}

							//CLICK_DOWNLOAD:
							//device.Tap(291, 391, 1);  ////vào thư mục dowload
							//device.DelayTime(3);
							//device.Tap(260, 501, 1);  /// chọn ảnh

							//device.DelayTime(3);
							Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPassword", password);
							Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", phoneNumber);


						}
					}

					// Tạo sesesion mới
					if (this.settings_common.GetValueBool("ckbTdata"))
					{
						// Thực hiện xóa 
						// device.DeviceId = "emulator-5556";
						device.TapByImage("TelegramX\\MessageTelegram", null, 5);
						device.DelayTime(1);
						device.TapByImage("TelegramX\\MessageTelegramSetting", null, 5);
						device.DelayTime(1);
						device.TapByImage("TelegramX\\ClearHistory", null, 5);
						device.DelayTime(1);
						device.TapByImage("TelegramX\\HistoryDelete", null, 5);
						device.DelayTime(1);

						RequestHttp request = new RequestHttp("", "", "");
						//string phone = "+84" + "0344646974".Substring(1);
						//string phone = "+84" + "0859065624".Substring(1);
						string phone = "+84" + phoneNumber.Substring(1);


						// Xóa session (nếu có)
						Common.DeleteFile(Application.StartupPath + "\\session\\" + phone + ".session");

						// string phone = "+84" + phoneNumber.Substring(1); // phoneNumber.Substring(1)
						string path = "";
						if (proxy.Contains(":"))
						{
							path = "http://127.0.0.1:5005/GetSession?phone=" + Common.UrlEncode(phone) + "&password=" + Common.UrlEncode(password) + "&code=" + "" + "&proxy=" + proxy.Split(':')[0] + "&port=" + proxy.Split(':')[1];
						}
						else
						{
							path = "http://127.0.0.1:5005/GetSession?phone=" + Common.UrlEncode(phone) + "&password=" + Common.UrlEncode(password) + "&code=" + "";
						}

						// Lấy code
						Task task = new Task(() =>
						{
							// Lặp lại để lấy code
							for (int j = 0; j < 30; j++)
							{
								// Đoạn lấy code
								device.DelayTime(2);
								device.ScreenShoot("log", phone + ".png");

								var Result = new IronTesseract().Read(Application.StartupPath + "\\log\\" + phone + ".png");
								string code = Regex.Match(Result.Text, "Login code: (.*?)\\.").Groups[1].Value.Trim();
								Console.WriteLine(string.Format("Code {0}: {1}", j, code));
								if (code != "")
								{
									try
									{
										File.WriteAllText(Application.StartupPath + "\\log\\" + phone + ".txt", code);
										break;
									}
									catch (Exception ex)
									{

									}
								}
							}
						});
						task.Start();

						string result = request.RequestGet(@path);

						result = result.Replace("\"", "").Replace("\\", "").Trim();
						if (result.Contains("OK"))
						{
							// Thêm qua bảng thành công
							lock (this.lock_Output_Success)
							{
								//this.dtgvAcc01.Rows.Add("", firtname + " " + lastname, phoneNumber, password, api_id, api_hash, DateTime.Now.ToString("yyyy'/'MM'/'dd'T'HH':'mm':'ss"), statusProxy + "[OK] Đăng ký thành công!");

								// Ghi logs
								this.WriteLog();
							}

							device.LoadStatusLD(statusProxy + "[OK] Đăng ký thành công!");
							this.SetStatusAccount(indexRow, statusProxy + "[OK] Đăng ký thành công!", null);

							return "1|[OK] Đăng ký thành công!";
						}
						else
						{
							output = "0|Đăng ký OK nhưng tạo session lỗi 2!";
							goto Xong;
						}

					}

					// Chạy process cũ
					//if (this.settings_common.GetValueBool("ckbTdata"))
					//{
					//                   // Mở Telegram
					//                   // device.DeviceId = "emulator-5556";
					//                   device.Tap(41, 78, 1);
					//                   device.DelayTime(2);
					//                   device.Tap(275, 185, 1);
					//                   device.DelayTime(2);

					//                   device.LoadStatusLD(statusProxy + "Create tdata...");
					//	this.SetStatusAccount(indexRow, statusProxy + "Create tdata...", null);
					//	Common.CreateFolder(Application.StartupPath + "\\output\\" + phoneNumber + "\\data_tdata");
					//	Common.CreateFolder(Application.StartupPath + "\\output\\" + phoneNumber + "\\data_session");
					//	Common.CreateFile(Application.StartupPath + "\\output\\" + phoneNumber + "\\data_session\\api_info.txt");
					//	Common.DelayTime(1);
					//	File.Copy(Application.StartupPath + "\\output\\Telegram.exe", Application.StartupPath + "\\output\\" + phoneNumber + "\\data_tdata\\Telegram.exe");
					//	Common.DelayTime(1);

					//	// Mở lên
					//	Process.Start(Application.StartupPath + "\\output\\" + phoneNumber + "\\data_tdata\\Telegram.exe");
					//	// Common.SetStatusDataGridView(dtgvAcc, indexRow, "cTdata", Application.StartupPath + "\\output\\" + phoneNumber + "\\data_tdata\\Telegram.exe");
					//	int timeWait = settings_common.GetValueInt("nudTimeWaitTdata", 60);

					//	for (int j = 0; j < timeWait; j++)
					//	{
					//		// Kiểm tra
					//		if (Common.GetStatusDataGridView(dtgvAcc, indexRow, "cFinish") == "OK")
					//		{
					//			Common.DelayTime(0.5);
					//			break;
					//		}

					//		device.LoadStatusLD(statusProxy + string.Format("[LẦN {0}/{1}] Đang chờ...", j, timeWait));
					//		this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] Đang chờ...", j, timeWait), null);
					//		Common.DelayTime(1);
					//	}

					//	// Nhâp xong thông báo thành công
					//	device.LoadStatusLD(statusProxy + "[OK] Đăng ký thành công!");
					//	this.SetStatusAccount(indexRow, statusProxy + "[OK] Đăng ký thành công!", null);
					//	Common.SetStatusDataGridView(dtgvAcc, indexRow, "cNgay", DateTime.Now.ToString("yyyy'/'MM'/'dd'T'HH':'mm':'ss"));

					//	// Xóa folder
					//	if (settings_common.GetValueBool("ckbKhongXoa") == false)
					//	{
					//		try
					//		{
					//			Common.DeleteFile(Application.StartupPath + "\\output\\" + phoneNumber + "\\data_tdata\\Telegram.exe");
					//			Common.DeleteFile(Application.StartupPath + "\\output\\" + phoneNumber + "\\data_tdata\\log.txt");
					//		}
					//		catch
					//		{

					//		}
					//	}

					//	// Tạo session
					//	RequestHttp request = new RequestHttp("", "", "");
					//	string tdataFolder = Application.StartupPath + "\\output\\" + phoneNumber + "\\data_tdata\\tdata";
					//	string deskFolder = Application.StartupPath + "\\output\\" + phoneNumber + "\\data_session\\";

					//	string path = "http://127.0.0.1:5005/Telegram?tdataFolder=" + Common.UrlEncode(tdataFolder) + "&destFolder=" + Common.UrlEncode(deskFolder) + "&password=" + Common.UrlEncode(password) + "&phone=" + Common.UrlEncode(phoneNumber);
					//	string result = request.RequestGet(@path);
					//	result = result.Replace("\"", "").Replace("\\", "").Trim();

					//	try
					//	{
					//		api_id = Regex.Match(result, @"api_id:(.*?)}").Groups[1].Value.Trim().ToString();
					//		api_hash = Regex.Match(result, @"api_hash:(.*?),").Groups[1].Value.Trim().ToString();

					//		Common.SetStatusDataGridView(dtgvAcc, indexRow, "cAPIID", api_id);
					//		Common.SetStatusDataGridView(dtgvAcc, indexRow, "cAPIHash", api_hash);

					//	}
					//	catch
					//	{

					//	}
					//}


				}
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "ChayDangKy()");
				return "0|Đăng ký lỗi: " + ex.Message.ToString();
			}
		Xong:
			if (output == "")
				output = "0|Đăng ký thất bại!";
			return output;
		}

		private string ChayDangKyTest(Chrome chrome, Device device, int indexRow, string statusProxy, string proxy)
		{
			string output = "";
			string phoneSimThue = "";
			string phoneNumber = "";
			string password = "";
			string requestID = "";
			Random rd = new Random();
			string otpCode = "";

			try
			{
				// Đăng ký
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cSTT", indexRow);
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cLD", "LDPLayer-" + device.IndexDevice);
				device.TapByText("ok", "", 3);
				//Thread.Sleep(500);
				//device.ClearDataApp("org.telegram.messenger");
				//device.LoadStatusLD(statusProxy + "Đang cài đặt Telegram...");
				//this.SetStatusAccount(indexRow, statusProxy + "Đang cài đặt Telegram...", null);
				//this.SetStatusAccount(indexRow, statusProxy + "Đang đăng ký...", device);
				//device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\Telegram.apk");
				//Thread.Sleep(500);
				//device.LoadStatusLD(statusProxy + "Đang fake thiết bị...");
				//this.SetStatusAccount(indexRow, statusProxy + "Đang fake thiết bị...", null);
				//// 

				//device.OpenApp("com.sollyu.xposed.hook.model");
				//device.WaitForImageAppear("Telegram\\Hook", 10, true);
				//device.WaitForImageAppear("Telegram\\HookAll", 10, true);
				//device.WaitForImageAppear("Telegram\\Device2", 10, true);
				//device.WaitForImageAppear("Telegram\\RandomInfo", 10, true);
				//device.WaitForImageAppear("Telegram\\SaveInfo", 10, true);

				////// mở app telegramX
				//device.LoadStatusLD(statusProxy + "Open Telegram...");
				//this.SetStatusAccount(indexRow, statusProxy + "Open Telegram...", null);
				device.OpenApp(TELEGRAM_PACKAGE);
				device.DelayTime(1);////////////////// Mở zalo

				string phone = "+84589017488";

				RequestHttp requestHttp = new RequestHttp("", "", "", 0);
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", phone);
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPassword", password);
				Common.DeleteFile(Application.StartupPath + "\\session\\" + phone + ".session");


				string path = "http://127.0.0.1:5005/GetSession?phone=" + Common.UrlEncode(phone) + "&password=" + Common.UrlEncode(password);

				// Lấy code

				Task task = new Task(() =>
				{
					// Lặp lại để lấy codel
					for (int j = 0; j < 5; j++)
					{
						// Đoạn lấy code
						// device.ScreenShoot("log", phone + ".png");
						device.DelayTime(1);

						// Kiểm tra có ảnh
						if (device.CheckExistImage(""))
						{
							try
							{
								File.WriteAllText(Application.StartupPath + "\\log\\" + phone + ".txt", "");
								string code = GetLoginCode(null, device, indexRow, "", phone);

								device.LoadStatusLD(statusProxy + "Login OTP : " + code);
								this.SetStatusAccount(indexRow, statusProxy + "Login OTP : " + code, null);
								if (code != "")
								{
									try
									{
										File.WriteAllText(Application.StartupPath + "\\log\\" + phone + ".txt", code);
										break;
									}
									catch { }
								}
								else
								{
									Common.DelayTime(1);
								}
							}
							catch (Exception ex)
							{
								Common.ExportError(ex, "ChayDangKy()");
							}
						}
						else
						{
							device.LoadStatusLD(statusProxy + string.Format("Đổi ảnh lần {0}/{1}", j, 5));
							this.SetStatusAccount(indexRow, statusProxy + string.Format("Đổi ảnh lần {0}/{1}", j, 5), null);
							device.DelayTime(3);
						}
					}
				});
				task.Start();

				Console.WriteLine("@path :" + @path);
				string result = requestHttp.RequestGet(@path);

				result = result.Replace("\"", "").Replace("\\", "").Trim();
				Console.WriteLine("result=" + result);
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "ChayDangKy()");
				return "0|Lỗi khác: " + ex.Message.ToString();
			}
		Xong:
			if (output == "")
				output = "0|Đăng ký thất bại!";
			return output;
		}

		private Dictionary<uint, string> dicIPDC = new Dictionary<uint, string>();
		public KeyValuePair<uint, string> keyValuePair_0;


		internal bool method_0(int int_0)
		{
			return int_0 % 2 == 0;
		}

		internal byte method_00(int int_0)
		{
			return Convert.ToByte(this.keyValuePair_0.Value.Substring(int_0, 2), 16);
		}

		public async Task<bool> OutSessionData(string phoneNumber, bool isAddPrefix, string folderOut, string deviceID)
		{
			string prefix = (isAddPrefix ? "+" : "");
			string cmd = ((!(folderOut == "SessionsTdata")) ? ("pull /sdcard/Download/Sessions/" + prefix + phoneNumber + "  " + folderOut) : ("pull /sdcard/Download/Sessions/" + prefix + phoneNumber + " " + folderOut));
			return (await runCMDToKeyValuePair(cmd, deviceID)).Key;
		}

		private async Task<string> CreateTempSession (Device device, string phone, string password, string hint, bool exportTdata = false, bool imageAvatar = false, int indexRow = 0, string statusProxy = "", int typeAPI = 3, string selectMode = "1")
        {
			string output = "";
            try
            {
				// string tgnet = ADBHelper.RunCMD(device.DeviceId, "shell getTgnet", 10);
				string tgnet = await runCMD("getTgnet", device.DeviceId);
				Class5 @class = new Class5(tgnet, phone, false, "SessionTemp");
				if (!@class.CreateSessionFile())
				{
					output = "0|Có lỗi khi chuyển tạo file session. Vui lòng iên hệ với developer!";
					return output;
				}

				string sessionPath = @class.sessionPath;
				ADBHelper.RunCMD(device.DeviceId, "shell mkdir -p /sdcard/Download/SessionsOrigin/", 10);

				// Push
				ADBHelper.RunCMD(device.DeviceId, "push " + sessionPath + " /sdcard/Download/SessionsOrigin/", 10);

				// Tạo tdata
				KeyValuePair<bool, string> keyValuePair = await CreateSessionAndTdata(device.DeviceId, phone, selectMode, password, hint, false, exportTdata, typeAPI, imageAvatar, "", "");

				// Copy dữ liệu
				string folderOutput = "";
				string cmd = "";
				if (keyValuePair.Key)
                {
					// Thành công
					folderOutput = (exportTdata ? "SessionsTdata" : "Sessions") + "/" + phone;
					try
					{
						if (!Directory.Exists(folderOutput))
						{
							Directory.CreateDirectory(folderOutput);
						}
						cmd = ((!(folderOutput == "SessionsTdata")) ? ("pull /sdcard/Download/Sessions/" + phone + "  " + folderOutput) : ("pull /sdcard/Download/Sessions/" + phone + " " + folderOutput));
                        ADBHelper.RunCMD(device.DeviceId, cmd, 10);
						output = "1|Tạo session thành công!";
						return output;
					}
					catch { }
				} else
                {
					// Thất bại
					folderOutput = (exportTdata ? "SessionsTdataFail" : "SessionsFail") + "/" + phone;
					try
					{
						if (!Directory.Exists(folderOutput))
						{
							Directory.CreateDirectory(folderOutput);
						}

						// Copy dữ liệu
						cmd = ((!(folderOutput == "SessionsTdata")) ? ("pull /sdcard/Download/Sessions/" + phone + "  " + folderOutput) : ("pull /sdcard/Download/Sessions/" + phone + " " + folderOutput));
						ADBHelper.RunCMD(device.DeviceId, cmd, 10);
						output = "0|Đã xuất session lỗi ra thư mục!";
						return output;
					}
					catch { }
				}

			}
            catch (Exception ex)
            {
				output = "0|Lỗi khác -> " + ex.Message.ToString();
				Common.ExportError(ex, "CreateTempSession()");
            }
			Xong:
			if (output == "")
            {
				output = "0|Xuất session lỗi gì đó rồi!";
			}
			return output;
			
		}

		private async Task<KeyValuePair<bool, string>> CreateSessionAndTdata(string deviceID, string phoneNumber, string selectMode, string password, string hint, bool ckbAddPrefix, bool ckbExportTdata, int typeAPI, bool ckbImage, string apiID, string apiHash)
        {
			string isAddPrefix = (ckbAddPrefix ? "1" : "0");
			string isExportTdata = (ckbExportTdata ? "1" : "0");
			string isImage = (ckbImage ? "1" : "0");
			apiID = ((typeAPI != 2) ? "1" : apiID);
			apiHash = ((typeAPI != 2) ? "a" : apiHash);
			//return ADBHelper.RunCMD(deviceID, $"shell CreateSessionAndTdata chiLnoVsoftware {phoneNumber} {selectMode} {password} {hint} {isAddPrefix} {isExportTdata} {typeAPI} {isImage} {apiID} {apiHash}", 180000);
			return await runCMDToKeyValuePair($"CreateSessionAndTdata chiLnoVsoftware {phoneNumber} {selectMode} {password} {hint} {isAddPrefix} {isExportTdata} {typeAPI} {isImage} {apiID} {apiHash}", deviceID, 180000);
		}

		private bool HuyPhone(int typePhone = 0, string requestID = "", string apiListType = "", string apiListPhone = "")
        {
			bool isResultHuy = false;
			if (typePhone == 0)
			{
				// CTSC
				bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
				isResultHuy = this.HuyChoThueSimCode(this.apiPhone, requestID, isNewAPI);

			}
			else if (typePhone == 2)
			{
				//Tempsms
				isResultHuy = this.HuyTempSMS(this.apiPhone, requestID);
			} else if (typePhone == 4)
            {
				// Code sim
				isResultHuy = this.HuyCodeSim(this.apiPhone, requestID, 10);
			} else if (typePhone == 15)
            {
				if (apiListType == "1")
                {
					// CTSC
					bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
					isResultHuy = this.HuyChoThueSimCode(apiListPhone, requestID, isNewAPI);
				} else if (apiListType == "2")
                {
					// Viotp

				}
				else if (apiListType == "3")
				{
					// Codesim
					isResultHuy = this.HuyCodeSim(apiListPhone, requestID, 10);
				}
				else if (apiListType == "5")
				{
					//Tempsms
					isResultHuy = this.HuyTempSMS(apiListPhone, requestID);
				}
			}
			return isResultHuy; 
		}

		private string GetAllPhone (Device device, int indexRow = 0, string statusProxy = "")
        {
			string phoneSimThue = "";
			string phoneNumber = "";
			string requestID = "";
			string apiType = "0";

			device.LoadStatusLD(statusProxy + "Đang lấy phone...");
			this.SetStatusAccount(indexRow, statusProxy + "Đang lấy phone...", null);
			string nhamang = settings_common.GetValue("txtNhaMang");
			string dauso = settings_common.GetValue("txtDauSo");
			string serviceID = settings_common.GetValue("txtServiceID", "").Trim();
			if (typePhone == 0)
			{
				// Chothuesimcode
				nhamang = settings_common.GetValue("txtCTSCNhaMang");
				dauso = settings_common.GetValue("txtCTSCDauSo");
				bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
				phoneSimThue = this.GetPhoneChoThueSimCode(this.apiPhone, nhamang, dauso, indexRow, statusProxy, serviceID, isNewAPI, 200);
			}
			else if (typePhone == 1)
			{
				phoneSimThue = PhoneHelper.GetPhoneOtpSim(this.apiPhone, 60, nhamang, dauso);
			}
			else if (typePhone == 2)
			{
				// tempsms.co
				phoneSimThue = this.GetPhoneTempSMS(this.apiPhone, 60, indexRow, statusProxy, serviceID, 200);
			}
			else if (typePhone == 3)
			{
				// simfast.vn
				phoneSimThue = this.GetPhoneSimfast(this.apiPhone, 60);
			}
			else if (typePhone == 4)
			{
				// codesim.net
				phoneSimThue = this.GetPhoneCodeSim(this.apiPhone, 60, indexRow, statusProxy, 100);
			}
			else if (typePhone == 5)
			{
				// viotp
				phoneSimThue = this.GetPhoneViotp(this.apiPhone, 60, serviceID, indexRow, statusProxy, 200);
			}
			else if (typePhone == 6)
			{
				// 2ndline
				phoneSimThue = this.GetPhone2ndLine(this.apiPhone, 60, serviceID);
			}
			else if (typePhone == 7)
			{
				// sms-active
				phoneSimThue = this.GetPhoneSMSActivate(this.apiPhone, 60);
			}
			///////////////////////////////// ahasim
			else if (typePhone == 8)
			{
				// Ahasim
				phoneSimThue = GetPhoneAhasim(this.apiPhone, indexRow, statusProxy, serviceID, 200);
			}
			///////////////////////////////// thuesimxyz
			else if (typePhone == 9)
			{
				phoneSimThue = GetPhoneThuesimXyz(this.apiPhone);
			}
			else if (typePhone == 10)
			{
				phoneSimThue = GetPhoneJSNguyenLieu(this.apiPhone, indexRow, statusProxy, serviceID);
			}
			else if (typePhone == 11)
			{
				phoneSimThue = GetPhoneOtpMM(this.apiPhone);
			}
			else if (typePhone == 12)
			{
				phoneSimThue = GetPhoneCustomSimThue(this.apiPhone);
			}
			else if (typePhone == 13)
			{
				// Tempcocde.co
				phoneSimThue = GetPhoneTempCodeCo(this.apiPhone);
			}
			else if (typePhone == 14)
			{
				// codetext247
				phoneSimThue = GetPhoneCodeText247(this.apiPhone, indexRow, statusProxy, 100);
			} else if (typePhone == 15)
            {
				// List API
				List<string> listAPI = File.ReadAllLines(@"input\listapi.txt").ToList();
				listAPI = Common.RemoveEmptyItems(listAPI);
				if (listAPI.Count == 0)
                {
					return "|";
                }
				return GetPhoneByListAPI(listAPI, device, indexRow, statusProxy);

			}
			else if (typePhone == 16)
			{
				// Khách 01
				phoneSimThue = GetPhoneKhach01(this.apiPhone);
			}
			else if (typePhone == 17)
			{
				// Custom PHP API
				phoneSimThue = GetPhoneCustomAPIPhp(this.apiPhone, 60, indexRow, statusProxy);
			}
			else if (typePhone == 18)
			{
				// Khách 02
				phoneSimThue = GetPhoneKhach02(this.apiPhone, indexRow, statusProxy, 100);
			}

			if (phoneSimThue.Contains("|"))
			{
				requestID = phoneSimThue.Split(new char[] { '|' })[0];
				phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];

				// Lấy API type với List
				if (typePhone == 15)
                {
					try
					{
						apiType = phoneSimThue.Split(new char[] { '|' })[2];
					}
					catch 
					{ 
					}
                }
				if (phoneNumber == "" || phoneNumber == "0")
                {
					return "0|";
				} else
                {
					return requestID + "|" + phoneNumber + "|" + apiType;
                }

			} else
            {
				return "|";
			}
		}

		private string GetAllPhoneV2(string statusProxy = "", int indexRow = 0)
		{
			string phoneSimThue = "";
			string phoneNumber = "";
			string requestID = "";
			string apiType = "0";

			string nhamang = settings_common.GetValue("txtNhaMang");
			string dauso = settings_common.GetValue("txtDauSo");
			string serviceID = settings_common.GetValue("txtServiceID", "").Trim();

			// Kiểm tra xem có yêu cầu dừng thì kết thúc task
			if (token.IsCancellationRequested)
			{
				return "|";
			}

			if (typePhone == 0)
			{
				// Chothuesimcode
				nhamang = settings_common.GetValue("txtCTSCNhaMang");
				dauso = settings_common.GetValue("txtCTSCDauSo");
				bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
				phoneSimThue = this.GetPhoneChoThueSimCode(this.apiPhone, nhamang, dauso, indexRow, statusProxy, serviceID, isNewAPI, 200);
			}
			else if (typePhone == 1)
			{
				phoneSimThue = PhoneHelper.GetPhoneOtpSim(this.apiPhone, 60, nhamang, dauso);
			}
			else if (typePhone == 2)
			{
				// tempsms.co
				phoneSimThue = this.GetPhoneTempSMS(this.apiPhone, 60, indexRow, statusProxy, serviceID, 200);
			}
			else if (typePhone == 3)
			{
				// simfast.vn
				phoneSimThue = this.GetPhoneSimfast(this.apiPhone, 60);
			}
			else if (typePhone == 4)
			{
				// codesim.net
				phoneSimThue = this.GetPhoneCodeSim(this.apiPhone, 60, indexRow, statusProxy, 100);
			}
			else if (typePhone == 5)
			{
				// viotp
				phoneSimThue = this.GetPhoneViotp(this.apiPhone, 60, serviceID, indexRow, statusProxy, 200);
			}
			else if (typePhone == 6)
			{
				// 2ndline
				phoneSimThue = this.GetPhone2ndLine(this.apiPhone, 60, serviceID, indexRow, statusProxy, 100);
			}
			else if (typePhone == 7)
			{
				// sms-active
				phoneSimThue = this.GetPhoneSMSActivate(this.apiPhone, 60);
			}
			///////////////////////////////// ahasim
			else if (typePhone == 8)
			{
				// Ahasim
				phoneSimThue = GetPhoneAhasim(this.apiPhone, indexRow, statusProxy, serviceID, 200);
			}
			///////////////////////////////// thuesimxyz
			else if (typePhone == 9)
			{
				phoneSimThue = GetPhoneThuesimXyz(this.apiPhone);
			}
			else if (typePhone == 10)
			{
				phoneSimThue = GetPhoneJSNguyenLieu(this.apiPhone, indexRow, statusProxy, serviceID);
			}
			else if (typePhone == 11)
			{
				phoneSimThue = GetPhoneOtpMM(this.apiPhone);
			}
			else if (typePhone == 12)
			{
				phoneSimThue = GetPhoneCustomSimThue(this.apiPhone);
			}
			else if (typePhone == 13)
			{
				// Tempcocde.co
				phoneSimThue = GetPhoneTempCodeCo(this.apiPhone);
			}
			else if (typePhone == 14)
			{
				// codetext247
				phoneSimThue = GetPhoneCodeText247(this.apiPhone, indexRow, statusProxy, 100);
			}
			else if (typePhone == 15)
			{
				// List API
				List<string> listAPI = File.ReadAllLines(@"input\listapi.txt").ToList();
				listAPI = Common.RemoveEmptyItems(listAPI);
				if (listAPI.Count == 0)
				{
					return "|";
				}
				return GetPhoneByListAPI(listAPI, null, indexRow, statusProxy);

			}
			else if (typePhone == 16)
			{
				// Khách 01
				phoneSimThue = GetPhoneKhach01(this.apiPhone);
			}
			else if (typePhone == 17)
			{
				// Custom PHP API
				phoneSimThue = GetPhoneCustomAPIPhp(this.apiPhone, 60, indexRow, statusProxy);
			}
			else if (typePhone == 18)
			{
				// Khách 02
				phoneSimThue = GetPhoneKhach02(this.apiPhone, indexRow, statusProxy, 100);
			}

			if (phoneSimThue.Contains("|"))
			{
				requestID = phoneSimThue.Split(new char[] { '|' })[0];
				phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];

				// Lấy API type với List
				if (typePhone == 15)
				{
					try
					{
						apiType = phoneSimThue.Split(new char[] { '|' })[2];
					}
					catch
					{
					}
				}
				if (phoneNumber == "" || phoneNumber == "0")
				{
					return "0|";
				}
				else
				{
					return requestID + "|" + phoneNumber + "|" + apiType;
				}

			}
			else
			{
				return "|";
			}
		}

		#region Lay phone theo list

		public string GetOtpByListAPI(string apiType , string apikey, string requestID, int limitTimeOTP = 60, int indexRow = 0, string statusProxy = "")
		{
			string otpCode = "";

            try
            {
				if (apiType == "1")
                {
					// Chothuesimcode
					bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
					otpCode = this.GetOTPChoThueSimCode(apikey, requestID, limitTimeOTP, indexRow, statusProxy, isNewAPI);
				}
				else if (apiType == "2")
				{
					// Viotp
					otpCode = this.GetOTPViotp(apikey, requestID, limitTimeOTP, indexRow, statusProxy);
				} 
				else if (apiType == "3")
                {
					// Codesim.net
					otpCode = this.GetOTPCodeSim(apikey, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (apiType == "4")
                {
					// ahasim.com
					otpCode = GetOTPAhasim(apikey, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (apiType == "5")
                {
					// tempsms
					otpCode = this.GetOTPTempSMS(apikey, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (apiType == "6")
				{
					// tempsms
					otpCode = this.GetOtpCodeText247(apikey, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else
                {
					otpCode = "";
                }


			}
            catch (Exception ex)
            {
				otpCode = "";
				Common.ExportError(ex, "GetOtpByListAPI");
            }

			return otpCode;
		}

		private string GetPhoneByListAPI(List<string> listAPI, Device device = null, int indexRow = 0, string statusProxy = "")
		{
			string result = "|";
			string phoneSimThue = "";
			string phoneNumber = "";
			string requestID = "";
			string serviceID = settings_common.GetValue("txtServiceID", "").Trim();
			try
			{
				for (int i = 0; i < listAPI.Count; i++)
				{
					string apiType = listAPI[i].Split('|')[0].Trim();
					string apiPhone = listAPI[i].Split('|')[1].Trim();
					
					if (apiType == "1")
                    {
						// Chothuesimcode
						string nhamang = settings_common.GetValue("txtCTSCNhaMang");
						string dauso = settings_common.GetValue("txtCTSCDauSo");
						bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
						phoneSimThue = this.GetPhoneChoThueSimCode(apiPhone, nhamang, dauso, indexRow, statusProxy, serviceID, isNewAPI, 10);

						// Kiểm tra đã có số
						if (phoneSimThue != "|" && phoneSimThue != "")
						{
							requestID = phoneSimThue.Split(new char[] { '|' })[0];
							phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
							if (phoneNumber != "" && phoneNumber != "0")
							{
								// Kiểm tra có bị blacklist hay không
								if (BlackList.IsBlackList(phoneNumber))
                                {

                                } else
                                {
									result = requestID + "|" + phoneNumber + "|" + apiType + "|" + apiPhone;
									goto Xong;
								}
							}
						}
					} else if (apiType == "2")
                    {
						// Viotp
						phoneSimThue = this.GetPhoneViotp(apiPhone, 60, serviceID, indexRow, statusProxy, 10);

						// Kiểm tra đã có số
						if (phoneSimThue != "|" && phoneSimThue != "")
						{
							requestID = phoneSimThue.Split(new char[] { '|' })[0];
							phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
							if (phoneNumber != "" && phoneNumber != "0")
							{
								// Kiểm tra có bị blacklist hay không
								if (BlackList.IsBlackList(phoneNumber))
								{

								}
								else
								{
									result = requestID + "|" + phoneNumber +"|" + apiType + "|" + apiPhone;
									goto Xong;
								}
							}
						}
					}
					else if (apiType == "3")
                    {
						// Codesim.net
						phoneSimThue = this.GetPhoneCodeSim(apiPhone, 60, indexRow, statusProxy, 10);

						// Kiểm tra đã có số
						if (phoneSimThue != "|" && phoneSimThue != "")
						{
							requestID = phoneSimThue.Split(new char[] { '|' })[0];
							phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
							if (phoneNumber != "" && phoneNumber != "0")
							{
								// Kiểm tra có bị blacklist hay không
								if (BlackList.IsBlackList(phoneNumber))
								{

								}
								else
								{
									result = requestID + "|" + phoneNumber + "|" + apiType + "|" + apiPhone;
									goto Xong;
								}
							}
						}
					}
					else if (apiType == "4")
					{
						// ahasim.com
						phoneSimThue = GetPhoneAhasim(apiPhone, indexRow, statusProxy, serviceID, 10);

						// Kiểm tra đã có số
						if (phoneSimThue != "|" && phoneSimThue != "")
						{
							requestID = phoneSimThue.Split(new char[] { '|' })[0];
							phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
							if (phoneNumber != "" && phoneNumber != "0")
							{
								// Kiểm tra có bị blacklist hay không
								if (BlackList.IsBlackList(phoneNumber))
								{

								}
								else
								{
									result = requestID + "|" + phoneNumber + "|" + apiType + "|" + apiPhone;
									goto Xong;
								}
							}
						}
					} 
					else if (apiType == "5")
                    {
						// tempsms
						phoneSimThue = this.GetPhoneTempSMS(apiPhone, 60, indexRow, statusProxy, serviceID, 10);

						// Kiểm tra đã có số
						if (phoneSimThue != "|" && phoneSimThue != "")
						{
							requestID = phoneSimThue.Split(new char[] { '|' })[0];
							phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
							if (phoneNumber != "" && phoneNumber != "0")
							{
								// Kiểm tra có bị blacklist hay không
								if (BlackList.IsBlackList(phoneNumber))
								{

								}
								else
								{
									result = requestID + "|" + phoneNumber + "|" + apiType + "|" + apiPhone;
									goto Xong;
								}
							}
						}
					}
					else if (apiType == "6")
					{
						// tempsms
						phoneSimThue = this.GetPhoneCodeText247(apiPhone, indexRow, statusProxy, 10);

						// Kiểm tra đã có số
						if (phoneSimThue != "|" && phoneSimThue != "")
						{
							requestID = phoneSimThue.Split(new char[] { '|' })[0];
							phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
							if (phoneNumber != "" && phoneNumber != "0")
							{
								// Kiểm tra có bị blacklist hay không
								if (BlackList.IsBlackList(phoneNumber))
								{

								}
								else
								{
									result = requestID + "|" + phoneNumber + "|" + apiType + "|" + apiPhone;
									goto Xong;
								}
							}
						}
					}
					else
                    {
						// Không hỗ trợ
                    }
				}
			}
			catch (Exception ex)
			{
				return "|";
			}
			Xong:
			if (result == "")
            {
				return "|";
            } else
            {
				return result;
            }
		}


        #endregion

        private async Task<bool> NhanRegister(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Register", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int num = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Register", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (num != 7)
                        {
                            num++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanMaVung(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "US +1", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "US +1", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanSearch(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Search", deviceID);
                if (!telegramElement.isValid)
                {
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Search", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanVietNam(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Vietnam", deviceID);
                if (!telegramElement.isValid)
                {
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Vietnam", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanPhoneNumber(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Phone Number", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Phone Number", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanNext(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Next", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Next", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanConfirm(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn Confirm...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Confirm", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Confirm", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanXacNhanCaptcha(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn Verify Answers...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Next Challenge", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Next Challenge", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn Verify Answers!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanXacNhanCaptchaL2(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn Verify Answers...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Verify Answers", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Verify Answers", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn Verify Answers!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhapOtp(string deviceID = "", int indexRow = 0, string otp = "")
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("class", "android.widget.EditText", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("class", "android.widget.EditText", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
				Common.DelayTime(2);
                await DeviceV2.InputTextNormal(otp, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanVerify(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Verify", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Verify", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                Common.DelayTime(2);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanNextNhapTen(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Next", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Next", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                Common.DelayTime(2);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanVaNhapMatKhau(string deviceID = "", int indexRow = 0, string password = "")
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("NAF", "true", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("NAF", "true", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                Common.DelayTime(2);
                await DeviceV2.InputTextNormal(password, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }

        private async Task<bool> NhanVaNhapNgaySinh(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn register...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Date of birth", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
                    int soluong = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("content-desc", "Date of birth", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (soluong != 7)
                        {
                            soluong++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                Common.DelayTime(2);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "NhanTelegram()");
                return false;
            }
        }


        private async Task<bool> NhanTelegram(string deviceID = "", int indexRow = 0)
		{
            try
            {
				this.SetStatusAccount(indexRow, "Đang nhấn telegram...", null);
				UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Telegram", deviceID);
				if (!telegramElement.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi khi chọn Telegram để ramdom,dang thử lại", null);
					int num = 0;
					while (true)
					{
						telegramElement = await GetPointFromDumpWithKeyValue("text", "Telegram", deviceID);
						if (telegramElement.isValid)
						{
							break;
						}
						if (num != 7)
						{
							num++;
							continue;
						}
						return true;
					}
					await DeviceV2.TouchSreen(telegramElement, deviceID);
					return true;
				}
				this.SetStatusAccount(indexRow, "Đã nhấn telegram!", null);
				await DeviceV2.TouchSreen(telegramElement, deviceID);
				return true;
			}
            catch (Exception ex)
            {
				Common.ExportError(ex, "NhanTelegram()");
				return false;
            }
		}

        private async Task<bool> ClickRamdomAll(string deviceID = "", int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang nhấn random all...", null);
                UIElement telegramElement = await GetPointFromDumpWithKeyValue("text", "Random all", deviceID);
                if (!telegramElement.isValid)
                {
                    this.SetStatusAccount(indexRow, "Error clicking button ramdom, trying again", null);
                    int num = 0;
                    while (true)
                    {
                        telegramElement = await GetPointFromDumpWithKeyValue("text", "Random all", deviceID);
                        if (telegramElement.isValid)
                        {
                            break;
                        }
                        if (num != 7)
                        {
                            num++;
                            continue;
                        }
                        return true;
                    }
                    await DeviceV2.TouchSreen(telegramElement, deviceID);
                    return true;
                }
                this.SetStatusAccount(indexRow, "Đã nhấn Random All!", null);
                await DeviceV2.TouchSreen(telegramElement, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickRamdomAll()");
                return false;
            }
        }

        private async Task<string> ChayDangKy(Chrome chrome, Device device, int indexRow, string statusProxy, string proxy)
		{
			string output = "";
			string phoneSimThue = "";
			string phoneNumber = "";
			string password = "";
			string requestID = "";
			Random rd = new Random();
			string otpCode = "";
			UiElement uiElement = null;
			List<UiElement> uiElements = null;
			string apiListType = "0";
			string apiListPhone = "";


			// Scroll tới cuối
			try
			{
				this.dtgvAcc.Invoke(new MethodInvoker(() => {
					dtgvAcc.FirstDisplayedScrollingRowIndex = dtgvAcc.RowCount - 1;
				}));
			}
            catch { }

			try
			{
				// Cài đặt
				await DeviceV2.runCMD("settings put system accelerometer_rotation 0", device.DeviceId);
				await DeviceV2.runCMD("settings put system user_rotation 0", device.DeviceId);
				await DeviceV2.ClearData("org.telegram.messenger", device.DeviceId);

				// Đăng ký				
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cSTT", indexRow);
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cLD", "LDPLayer-" + device.IndexDevice);
				// device.ClearDataApp("org.telegram.messenger");
				device.LoadStatusLD(statusProxy + "Đang mở Telegram...");
				this.SetStatusAccount(indexRow, statusProxy + "Đang mở Telegram...", null);
				//device.InstallApp(FileHelper.GetPathToCurrentFolder() + "\\app\\Telegram.apk");
				Thread.Sleep(500);

				int iMaxRun = 0;
				// Lấy số phone
				phoneSimThue = GetAllPhone(device, indexRow, statusProxy);

				// Kiểm tra đã dừng
				if (this.isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					output = "0|Đã dừng";
					goto Xong;
				}

				if (phoneSimThue.Contains("|"))
				{
					requestID = phoneSimThue.Split(new char[] { '|' })[0];
					phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
					try
					{
						apiListType = phoneSimThue.Split(new char[] { '|' })[2];
					}
					catch { }
					try
					{
						apiListPhone = phoneSimThue.Split(new char[] { '|' })[3];
					}
					catch { }
				}
				else
				{
					if (phoneSimThue.Contains("tạm hết"))
					{
						return "0|Kho số cho ứng dụng đang tạm hết!";
					}
					else if (phoneSimThue.Contains("hết tiền"))
					{
						return "0|Tài khoản đã hết tiền!";
					}
					else
					{
						return "0|Không lấy được phone, kiểm tra API!";
					}

				}

				if (phoneNumber == "" || phoneNumber == "0")
				{
					return "0|Không lấy được phone, kiểm tra API!";
				} else
                {
					device.LoadStatusLD(statusProxy + "Phone = " + phoneNumber);
					this.SetStatusAccount(indexRow, statusProxy + "Phone = " + phoneNumber, null);
					phoneNumber = phoneNumber.Replace("+840", "+84");
					Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", phoneNumber);
				}

				device.LoadStatusLD(statusProxy + "Đang fake thiết bị...");
				this.SetStatusAccount(indexRow, statusProxy + "Đang fake thiết bị...", null);

				// Kiểm tra dừng
				if (this.isStop)
                {
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					output = "0|Đã dừng";
					goto Xong;
				}
				
				// 

				// Fake thiết bị
				// device.OpenApp("com.sollyu.xposed.hook.model");
				device.CloseApp("com.sollyu.xposed.hook.model");
				await DeviceV2.OpenApp("com.sollyu.xposed.hook.model", device.DeviceId);

				// Fake
				if (!(await NhanTelegram()))
				{
					this.SetStatusAccount(indexRow, statusProxy + "Lỗi fake thiết bị!", null);
					output = "0|Lỗi fake thiết bị";
					goto Xong;
				}

				// Mở quyền
				device.GrantPermission();

				// Mở app
				device.LoadStatusLD(statusProxy + "Open Telegram...");
				this.SetStatusAccount(indexRow, statusProxy + "Open Telegram...", null);
				device.OpenApp(TELEGRAM_PACKAGE);
				device.DelayTime(5);

				// Kiểm tra dừng
				if (this.isStop)
				{
					this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
					output = "0|Đã dừng";
					goto Xong;
				}


				if (device.TapByText("start messaging") == false)
                {
					// Không nhấn được
					uiElement = UiHelpers.FindElementByTextClassPackage(device, "Start Messaging", "android.widget.TextView", "org.telegram.messenger");
					if (uiElement != null)
					{
						device.TouchByUIElemnent(uiElement, true, 3);
					} else
                    {
						if (device.WaitForImageAppear("Telegram\\StartMessing", 10, true) == false)
                        {

                        } else
                        {
							output = "0|Không nhấn được Start Messaging!";
							return output;
						}
					}
				}
				// device.WaitForImageAppear("Telegram\\StartMessing", 10, true);
				device.DelayTime(1);

				ENTER_PHONE:

				// Lấy ra lastname
				//device.InputTextWithUnicode("Văn Phúc");
				//device.DelayTime(1);

				if (iMaxRun >= 3)
				{
					// Hủy số 
					bool isResultHuy = HuyPhone(typePhone, requestID, apiListType, apiListPhone);

					if (isResultHuy)
					{
						device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thành công");
						this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thành công", null);

					}
					else
					{
						device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thất bại");
						this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thất bại", null);
					}

					output = "0|Quá 2 lượt không có phone phù hợp!";
					return output;
				}

				// Xoá chữ và nhập
				device.InputKey(Device.KeyEvent.KEYCODE_MOVE_END);
				//device.RunCMD("input keyevent 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67");
                for (int i = 0; i < 5; i++)
                {
                    device.InputKeyBackspace();
                    Thread.Sleep((new Random()).Next(200, 800));
                    device.InputKeyBackspace();
                    Thread.Sleep((new Random()).Next(200, 800));
                    device.InputKeyBackspace();
                    Thread.Sleep((new Random()).Next(200, 800));
                }
                device.DelayTime(1);
				///// Nhập phone mới
				if (typePhone != 13)
                {
					uiElement = UiHelpers.FindElementByClassPackageContentDesc(device, "android.widget.EditText", "org.telegram.messenger", "Country code", false);
					device.SetEditTextByUIElement(uiElement, "84", true);

					// device.SendText("84");

					if (typePhone == 2)
					{
						// Tempsms
						phoneNumber = phoneNumber.Replace("+84", "");
					}
				} else
                {
					// Tempcode.co
                }

				//device.InputKeyTAB();

				// Chuẩn hóa số điện thoại
				phoneNumber = phoneNumber.Replace("+840", "+84");
				Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", phoneNumber);

				// Kiểm tra Blacklist
				if (BlackList.IsBlackList(phoneNumber))
				{
					device.LoadStatusLD(statusProxy + phoneNumber + " đã được đăng ký rồi, bỏ qua!");
					this.SetStatusAccount(indexRow, statusProxy + phoneNumber + " đã được đăng ký rồi, bỏ qua!", null);

					// Hủy số 
					bool isResultHuy = HuyPhone(typePhone, requestID, apiListType, apiListPhone);

					if (isResultHuy)
					{
						device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thành công");
						this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thành công", null);

					}
					else
					{
						device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thất bại");
						this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thất bại", null);
					}

					return "0|Số điện thoại bị Blacklist!";
				}

				// Thành công
				BlackList.Add(phoneNumber);
				device.LoadStatusLD(statusProxy + "Phone : " + phoneNumber);
				this.SetStatusAccount(indexRow, statusProxy + "Phone : " + phoneNumber, null);

				///Nhập Phone
				//device.SendText(phoneNumber);
				//Thread.Sleep(100);
				uiElement = UiHelpers.FindElementByClassPackageContentDesc(device, "android.widget.EditText", "org.telegram.messenger", "Phone number", false);
				device.SetEditTextByUIElement(uiElement, phoneNumber, true);

				// Lưu
				if (typePhone != 13)
                {
					Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", "+84" + phoneNumber);
				} else
                {
					// Tempcode.co
					Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", phoneNumber);
				}

				// Tiếp tục
				uiElement = UiHelpers.FindElementByClassPackageContentDesc(device, "android.widget.FrameLayout", "org.telegram.messenger", "Done", false);
				if (uiElement != null) device.TouchByUIElemnent(uiElement, true, 3);
				device.DelayRandom(0, 1);
				uiElement = UiHelpers.FindElementByIndexTextClassPackage(device, 3, "Yes", "android.widget.TextView", "org.telegram.messenger");
				if (uiElement != null) device.TouchByUIElemnent(uiElement, true, 3);
				device.DelayRandom(4,5);
                // device.WaitForImageAppear("Telegram\\TiepTuc", 10, true);
                // device.WaitForImageAppear("Telegram\\Yes", 10, true);

                // Kiểm tra trước ảnh SMS
                for (int j = 0; j < 5; j++)
                {
					//device.DeviceId = "emulator-5556";
					if (device.WaitForImageAppear("Telegram\\CheckTelegramSMS", 5, false))
					{
						uiElement = UiHelpers.FindElementByClassPackageContentDesc(device, "android.widget.ImageView", "org.telegram.messenger", "Back", false);
						device.TouchByUIElemnent(uiElement, true, 3);
						device.DelayTime(1);

						uiElement = UiHelpers.FindElementByTextClassPackage(device, "EDIT", "android.widget.TextView", "org.telegram.messenger");
						device.TouchByUIElemnent(uiElement, true, 3);
						device.DelayTime(1);

						// Hủy số 
						bool isResultHuy = HuyPhone(typePhone, requestID, apiListType, apiListPhone);

						if (isResultHuy)
						{
							device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thành công");
							this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thành công", null);

						}
						else
						{
							device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thất bại");
							this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thất bại", null);
						}

						// Số đã có pass rồi
						this.SetStatusAccount(indexRow, statusProxy + "Tài khoản đã đăng ký", null);
						output = "0|Tài khoản đã đăng ký!";
						return output;
					} else if  (device.WaitForImageAppear("Telegram\\LoiGiDo", 5, false))
                    {
						uiElement = UiHelpers.FindElementByTextClassPackage(device, "OK", "android.widget.TextView", "org.telegram.messenger");
						if (uiElement != null) device.TouchByUIElemnent(uiElement, true, 3);

						// Hủy số 
						bool isResultHuy = HuyPhone(typePhone, requestID, apiListType, apiListPhone);

						if (isResultHuy)
						{
							device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thành công");
							this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thành công", null);

						}
						else
						{
							device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thất bại");
							this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thất bại", null);
						}

						// Lấy lại phone
						phoneSimThue = GetAllPhone(device, indexRow, statusProxy);

						// Kiểm tra dừng
						if (this.isStop)
						{
							this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
							output = "0|Đã dừng";
							goto Xong;
						}

						if (phoneSimThue.Contains("|"))
						{
							requestID = phoneSimThue.Split(new char[] { '|' })[0];
							phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
							try
							{
								apiListType = phoneSimThue.Split(new char[] { '|' })[2];
							}
							catch { }
							try
							{
								apiListPhone = phoneSimThue.Split(new char[] { '|' })[3];
							}
							catch { }
						}
						else
						{
							if (phoneSimThue.Contains("tạm hết"))
							{
								return "0|Kho số cho ứng dụng đang tạm hết!";
							}
							else if (phoneSimThue.Contains("hết tiền"))
							{
								return "0|Tài khoản đã hết tiền!";
							}
							else
							{
								return "0|Không lấy được phone, kiểm tra API!";
							}

						}

						if (phoneNumber == "" || phoneNumber == "0")
						{
							return "0|Không lấy được phone, kiểm tra API!";
						}
						else
						{
							device.LoadStatusLD(statusProxy + "Phone = " + phoneNumber);
							this.SetStatusAccount(indexRow, statusProxy + "Phone = " + phoneNumber, null);
						}

						device.LoadStatusLD(statusProxy + "Đang fake thiết bị...");
						this.SetStatusAccount(indexRow, statusProxy + "Đang fake thiết bị...", null);

						// Kiểm tra dừng
						if (this.isStop)
						{
							this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
							output = "0|Đã dừng";
							goto Xong;
						}

						// Đổi phone
						iMaxRun++;
						goto ENTER_PHONE;
					}

					if (device.WaitForImageAppear("Telegram\\Code1", 5, false))
					{
						goto GET_OTP;
					}

				// Get otp
				GET_OTP:

					// Kiểm tra dừng
					if (this.isStop)
					{
						this.SetStatusAccount(indexRow, statusProxy + "Đã dừng!", null);
						output = "0|Đã dừng";
						goto Xong;
					}

					device.LoadStatusLD(statusProxy + "Getting OTP...");
					this.SetStatusAccount(indexRow, statusProxy + "Getting OTP...", null);
					int limitTimeOTP = settings_common.GetValueInt("nudTimeOTP", 100);

					// Lấy ra otp
					if (this.typePhone == 0)
					{
						// Cho thue sim code
						bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
						otpCode = this.GetOTPChoThueSimCode(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy, isNewAPI);
					}
					else if (this.typePhone == 1)
					{
						// Otpsim
						otpCode = this.GetOTPOtpSim(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 2)
					{
						// tempsms.co
						otpCode = this.GetOTPTempSMS(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 3)
					{
						// simfast.vn
						otpCode = this.GetOTPSimfast(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 4)
					{
						// codesim.net
						otpCode = this.GetOTPCodeSim(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 5)
					{
						// Viotp
						otpCode = this.GetOTPViotp(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 6)
					{
						// 2ndLine
						otpCode = this.GetOTP2ndLine(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 7)
					{
						// sms-activate
						otpCode = this.GetOTPSMSActivate(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 8)
					{
						// Ahasim
						otpCode = GetOTPAhasim(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 10)
					{
						// jsnguyenlieu
						otpCode = GetOTPJSNguyenLieu(this.apiPhone, requestID, limitTimeOTP);
					}
					else if (this.typePhone == 11)
					{
						// Otpmm.com
						otpCode = GetOtpOtpMM(this.apiPhone, phoneNumber, limitTimeOTP);
					}
					else if (this.typePhone == 12)
					{
						// custom simthue
						otpCode = GetOtpCustomSimThue(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 13)
					{
						// tempcode.co
						otpCode = GetOtpTempCodeCo(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 14)
					{
						// tempcode.co
						otpCode = GetOtpCodeText247(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 15)
					{
						// List api
						otpCode = GetOtpByListAPI(apiListType, apiListPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 16)
					{
						// Khách 01
						otpCode = GetOtpCodeKhach01(this.apiPhone, requestID, "", limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 17)
					{
						// Custom API Php
						otpCode = GetOtpCodeCustomAPIPhp(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}
					else if (this.typePhone == 18)
					{
						// Khachs 02
						otpCode = GetOTPKhach02(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
					}

					if ((otpCode == null ? true : otpCode == ""))
					{
						bool isResultHuy = HuyPhone(typePhone, requestID, apiListType, apiListPhone);

						if (isResultHuy)
						{
							device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thành công");
							this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thành công", null);

						}
						else
						{
							device.LoadStatusLD(statusProxy + "Hủy SĐT " + phoneNumber + " thất bại");
							this.SetStatusAccount(indexRow, statusProxy + "Hủy SĐT " + phoneNumber + " thất bại", null);
						}

						// Đổi phone
						this.SetStatusAccount(indexRow, statusProxy + "Không về OTP CODE!", null);
						output = "0|Không về OTP CODE!";
						return output;
					}
					else
					{
						// Ghi logs
						lock (this.lock_Output_Otp)
                        {
                            try
                            {
								// File.AppendAllText("log_otp.txt", string.Concat("Phone {0} : {1}", phoneNumber, otpCode));
								File.AppendAllText("log_otp.txt", phoneNumber + " | " + otpCode + " | " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "\r\n");
							}
                            catch { }
						}

						// Lấy code thành công
						device.LoadStatusLD(statusProxy + "Code : " + otpCode);
						this.SetStatusAccount(indexRow, statusProxy + "Code : " + otpCode, null);
						char[] charArr = otpCode.ToCharArray();
						device.SendText(charArr[0].ToString());
						Thread.Sleep((new Random()).Next(200, 800));
						device.SendText(charArr[1].ToString());
						Thread.Sleep((new Random()).Next(200, 800));
						device.SendText(charArr[2].ToString());
						Thread.Sleep((new Random()).Next(200, 800));
						device.SendText(charArr[3].ToString());
						Thread.Sleep((new Random()).Next(200, 800));
						device.SendText(charArr[4].ToString());
						Thread.Sleep((new Random()).Next(200, 800));
						device.DelayTime(3);
					}

					if (device.CheckExistText("your account is protected", "", 5))
					//if (device.Check("Telegram\\YourPass", 5, false))
					{
						if (output == "")
							output = "0|Số SĐT Đã Có Pass!";

						// Thêm logs
						lock (this.lock_Output_2FA)
						{
							try
							{
								File.AppendAllText("log_2fa.txt", phoneNumber + " | " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "\r\n");
							}
							catch { }
						}

						this.Them2FA();
						goto Xong;
					}
					string firtname = "";
					string lastname = "";

					device.LoadStatusLD(statusProxy + "Đang đăng ký...");
					this.SetStatusAccount(indexRow, statusProxy + "Đang đăng ký...", null);

					if (settings_common.GetValueInt("typeName") == 0)
					{
						firtname = RandomFirstName(1);
						lastname = RandomLastName(1);
					}
					else
					{
						firtname = RandomFirstName(2);
						lastname = RandomLastName(2);
					}

					// Bỏ dấu
					//firtname = Common.RemoveSign4VietnameseString(firtname);
					//lastname = Common.RemoveSign4VietnameseString(lastname);

					// Lấy ra lastname
					device.InputTextWithUnicode(firtname + " " + lastname);
					device.DelayTime(1);
					//device.InputKey(Device.KeyEvent.KEYCODE_TAB);
					//device.DelayTime(1);
					// Lấy ra firstname
					//device.SendText(firtname);
					//device.DelayTime(1);

					uiElement = UiHelpers.FindElementByClassPackageContentDesc(device, "android.widget.FrameLayout", "org.telegram.messenger", "Done", false);
					if (uiElement != null) device.TouchByUIElemnent(uiElement, true, 3);
					//device.TapByImage("Telegram\\TiepTuc", null, 10);
					//device.DelayTime(2);
					if (device.CheckExistText("your account is protected", "", 5))
					{
						if (output == "")
							output = "0|Số Đã Có Pass...........";

						// Thêm logs
						lock (this.lock_Output_2FA)
						{
							try
							{
								File.AppendAllText("log_2fa.txt", phoneNumber + "\r\n");
							}
							catch { }
						}

						this.Them2FA();
						goto Xong;
					}

					string phone = "";

					// Chuẩn hóa phone
					if (typePhone == 6)
					{
						// 2nd line
						phone = "+84" + phoneNumber.Substring(1);
					} else if (typePhone == 13)
                    {
						// tempcode.co
						phone = phoneNumber;
					}
					else
					{
						phone = "+84" + phoneNumber;
					}

					// Xóa số 0
					phone = phone.Replace("+840", "+84");
					Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPhone", phone);

					// Tạo session mới
					string hint = settings_common.GetValue("txtHint", "easysoftware");
                    password = "123456Ac";

                    if (settings_common.GetValueInt("typePass") == 0)
                    {
                        password = settings_common.GetValue("txtPassword");
                    }
                    else
                    {
                        password = Common.CreateRandomStringAndInt(8);
                    }
                    bool exportTdata = settings_common.GetValueBool("ckbExportTdata", false);
					bool imageAvatar = settings_common.GetValueBool("ckbAvatar", false);

					device.LoadStatusLD(statusProxy + "Đang tạo session...");
					this.SetStatusAccount(indexRow, statusProxy + "Đang tạo session...", null);

					output = await CreateTempSession(device, phone, password, hint, exportTdata, imageAvatar, indexRow, statusProxy, 3);

					// Hiển thị
					if (output.StartsWith("1|"))
                    {
                        // Thành công
                        lock (this.lock_Output_Success)
                        {
							string username = "";
							string api_id = "";
							string api_hash = "";
							// Lấy ra username
							try
                            {
                                string[] dataAcc = File.ReadAllText(Application.StartupPath + "\\Sessions\\" + phone + "\\info.txt").Split('|');
                                username = dataAcc[1].Trim();
								api_id = dataAcc[3].Trim();
								api_hash = dataAcc[4].Trim();
							}
                            catch { }

							// Kiểm tra lỗi
							if (api_id == "" || api_id == "2040")
                            {
								// goto TAO_SESSION_2;
								Console.WriteLine("Lỗi tạo session trùng!");
							}

                            Common.SetStatusDataGridView(dtgvAcc, indexRow, "cName", username);
                            Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPassword", password);
                            Common.SetStatusDataGridView(dtgvAcc, indexRow, "cAPIID", api_id);
                            Common.SetStatusDataGridView(dtgvAcc, indexRow, "cAPIHash", api_hash);
                            Common.SetStatusDataGridView(dtgvAcc, indexRow, "cNgay", DateTime.Now.ToString("yyyy'/'MM'/'dd'T'HH':'mm':'ss"));

                            //this.dtgvAcc01.Rows.Add("", username, phone, password, api_id, api_hash, DateTime.Now.ToString("yyyy'/'MM'/'dd'T'HH':'mm':'ss"), statusProxy + "[OK] Đăng ký thành công!");

                            // Ghi logs
                            this.WriteLog();

							device.LoadStatusLD(statusProxy + "[OK] Đăng ký thành công!");
							this.SetStatusAccount(indexRow, statusProxy + "[OK] Đăng ký thành công!", null);

							return "1|[OK] Đăng ký thành công!";
						}

						// Lần 2
						TAO_SESSION_2:
						device.LoadStatusLD(statusProxy + "Đang tạo session ....");
						this.SetStatusAccount(indexRow, statusProxy + "Đang tạo session ....", null);
						output = await CreateTempSession(device, phone, password, hint, exportTdata, imageAvatar, indexRow, statusProxy, 1);

						if (output.StartsWith("1|"))
						{
							// Thành công
							lock (this.lock_Output_Success)
							{
								string username = "";
								string api_id = "";
								string api_hash = "";
								// Lấy ra username
								try
								{
									string[] dataAcc = File.ReadAllText(Application.StartupPath + "\\Sessions\\" + phone + "\\info.txt").Split('|');
									username = dataAcc[1].Trim();
									api_id = dataAcc[3].Trim();
									api_hash = dataAcc[4].Trim();
								}
								catch { }

								// Kiểm tra lỗi
								if (api_id == "" || api_id == "2040")
								{
									Console.WriteLine("Lỗi lần 2");
								}

								Common.SetStatusDataGridView(dtgvAcc, indexRow, "cName", username);
								Common.SetStatusDataGridView(dtgvAcc, indexRow, "cPassword", password);
								Common.SetStatusDataGridView(dtgvAcc, indexRow, "cAPIID", api_id);
								Common.SetStatusDataGridView(dtgvAcc, indexRow, "cAPIHash", api_hash);
								Common.SetStatusDataGridView(dtgvAcc, indexRow, "cNgay", DateTime.Now.ToString("yyyy'/'MM'/'dd'T'HH':'mm':'ss"));

								//this.dtgvAcc01.Rows.Add("", username, phone, password, api_id, api_hash, DateTime.Now.ToString("yyyy'/'MM'/'dd'T'HH':'mm':'ss"), statusProxy + "[OK] Đăng ký thành công!");

								// Ghi logs
								this.WriteLog();
							}

							device.LoadStatusLD(statusProxy + "[OK] Đăng ký thành công!");
							this.SetStatusAccount(indexRow, statusProxy + "[OK] Đăng ký thành công!", null);

							return "1|[OK] Đăng ký thành công!";
						}
						else
						{
							// Thất bại
							lock (this.lock_Output_Fail)
							{
								output = "0|Tạo session lỗi : " + output.Split('|')[1].Trim();
								goto Xong;
							}
						}
						
                    } else
                    {
						// Thất bại
						lock (this.lock_Output_Fail)
						{
							output = "0|Tạo session lỗi : " + output.Split('|')[1].Trim();
							goto Xong;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "ChayDangKy()");
				return "0|Lỗi khác: " + ex.Message.ToString();
			}
		Xong:

			// Đóng LD
			device.ClearDataApp("org.telegram.messenger");
			if (output == "")
				output = "0|Đăng ký thất bại!";
			return output;
		}

		private string GetOtpCode(Chrome chrome, Device device, int indexRow = 0, string statusProxy = "", string phone = "")
		{
			string code = "";
			try
			{
				device.TapByImage("Telegram\\GetOtpCode", null, 5);
				device.DelayTime(1);

				device.TapByImage("Telegram\\GetLoginCodeCopy", null, 5);
				device.DelayTime(1);

				string clipboard = device.GetClipboard();

				code = Regex.Match(clipboard.Replace("\n", ""), "Login code: (.*?)\\.").Groups[1].Value.Trim();

				if (code != "")
				{
					goto Xong;
				}
				else
				{

				}


				//File.WriteAllText(Application.StartupPath + "\\log\\" + phone + ".txt", "");
				//using (var Input = new OcrInput(Application.StartupPath + "\\log\\" + phone + ".png"))
				//{
				//	OcrResult Result = null;
				//	try
				//	{
				//		for (int j = 0; j < 10; j++)
				//		{
				//			if (isConverting == false)
				//			{
				//				device.LoadStatusLD(statusProxy + "Đang convert ảnh...");
				//				this.SetStatusAccount(indexRow, statusProxy + "Đang convert ảnh...", null);

				//				isConverting = true;
				//				Result = ironTesseract.Read(Input);
				//				Thread.Sleep(1000);
				//				isConverting = false;
				//				code = Regex.Match(Result.Text, "Login code: (.*?)\\.").Groups[1].Value.Trim();
				//				if (code != "")
				//                            {
				//					goto Xong;
				//                            }
				//			}
				//			else
				//			{
				//				device.LoadStatusLD(statusProxy + "Đang chờ convert ảnh...");
				//				this.SetStatusAccount(indexRow, statusProxy + "Đang chờ convert ảnh...", null);

				//				Common.DelayTime(2);
				//			}
				//		}

				//	}
				//	catch (Exception ex)
				//	{
				//		Console.WriteLine("ERROR = " + ex.Message);
				//		Common.ExportError(ex, "GetLoginCode3()");
				//	}
				//}
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "GetLoginCode2()");
			}
		Xong:
			return code;


		}

		private string GetLoginCode(Chrome chrome, Device device, int indexRow, string statusProxy, string phone = "")
		{
			string result = "";
			string login_code = "";

			for (int i = 0; i < 2; i++)
			{
				// Login
				device.LoadStatusLD(statusProxy + "Đang tạo app_id...");
				this.SetStatusAccount(indexRow, statusProxy + "Đang tạo app_id...", null);


				// Mở đăng nhập
				chrome.GotoURL("https://my.telegram.org/auth/");
				chrome.DelayTime(2);

				chrome.SendKeys(3, "//input[@type='text']", phone);
				chrome.SendEnter(3, "//input[@type='text']");

				// Click vào tin nhắn
				if (chrome.chrome.PageSource.Contains("Confirmation code"))
				{
					// OK
				}
				else
				{
					chrome.Proxy = "";
					chrome.TypeProxy = 0;

					try
					{
						chrome.Close();
						chrome.DelayTime(1);
						chrome.Open();
						chrome.DelayTime(1);

						chrome.GotoURL("https://my.telegram.org/auth/");
						chrome.DelayTime(2);

						chrome.SendKeys(3, "//input[@type='text']", phone);
						chrome.SendEnter(3, "//input[@type='text']");
					}
					catch (Exception ex)
					{
						Common.ExportError(ex, "OpenAPI()");
					}
				}

				if (chrome.chrome.PageSource.Contains("Confirmation code"))
				{
					// OK
				}
				else
				{
					result = "0|Không truy cập chrome để lấy code!";
					goto Xong;
				}

				device.TapByImage("Telegram\\TelegramMesssage", null, 5);
				device.DelayTime(2);

				// Kiểm tra có ảnh
				// device.DeviceId = "emulator-5564";
				if (device.CheckExistImage("Telegram\\GetLoginCode", null, 5))
				{
					device.ScreenShoot("log", phone + "_login.png");
					device.DelayTime(1);
					goto LayLoginCode;
				}
				else
				{
					// Gửi lại
					chrome.ClearText(3, "//input[@type='text']");
					chrome.SendKeys(3, "//input[@type='text']", phone);
					chrome.SendEnter(3, "//input[@type='text']");

					if (device.CheckExistImage("Telegram\\GetLoginCode", null, 5))
					{
						device.ScreenShoot("log", phone + "_login.png");
						device.DelayTime(1);
						goto LayLoginCode;
					}
					else
					{
						result = "0|Gửi lấy login code không thành công -> Kiểm tra thiết bị!";
						goto Xong;
					}
				}

			LayLoginCode:

				// Kiểu lấy copy
				//device.DeviceId = "emulator-5556";
				string clipboard = "";

				for (int j = 0; j < 4; j++)
				{
					device.TapByImage("Telegram\\GetLoginCode", null, 5);
					device.DelayTime(1);

					device.TapByImage("Telegram\\GetLoginCodeCopy", null, 5);
					device.DelayTime(1);

					clipboard = device.GetClipboard();
					login_code = Regex.Match(clipboard.Replace("\n", ""), "login code:(.*?)Do").Groups[1].Value.Trim();
					if (login_code != "")
					{
						device.LoadStatusLD("Login Code : " + login_code);
						this.SetStatusAccount(indexRow, "Login Code : " + login_code, null);
						result = "1|" + login_code;
						goto Xong;
					}
				}

				if (login_code != "")
				{
					device.LoadStatusLD("Login Code : " + login_code);
					this.SetStatusAccount(indexRow, "Login Code : " + login_code, null);
					result = "1|" + login_code;
					goto Xong;

				}
				else
				{
					device.TapByImage("Telegram\\GetLoginCode", null, 5);
					device.DelayTime(1);

					device.TapByImage("Telegram\\GetLoginCodeCopy", null, 5);
					device.DelayTime(1);

					clipboard = device.GetClipboard();

					login_code = Regex.Match(clipboard.Replace("\n", ""), "login code:(.*?)Do").Groups[1].Value.Trim();

					if (login_code != "")
					{
						device.LoadStatusLD("Login Code : " + login_code);
						this.SetStatusAccount(indexRow, "Login Code : " + login_code, null);
						result = "1|" + login_code;
						goto Xong;

					}
					else
					{
						device.TapByImage("Telegram\\GetLoginCode", null, 5);
						device.DelayTime(1);

						device.TapByImage("Telegram\\GetLoginCodeCopy", null, 5);
						device.DelayTime(1);

						clipboard = device.GetClipboard();

						login_code = Regex.Match(clipboard.Replace("\n", ""), "login code:(.*?)Do").Groups[1].Value.Trim();

						if (login_code != "")
						{
							device.LoadStatusLD("Login Code : " + login_code);
							this.SetStatusAccount(indexRow, "Login Code : " + login_code, null);
							result = "1|" + login_code;
							goto Xong;

						}
						else
						{
							device.LoadStatusLD("Chưa lấy được login code, đang thử lại...");
							this.SetStatusAccount(indexRow, "Chưa lấy được login code, đang thử lại...", null);
							result = "0|Chưa lấy được login code";
						}
					}
				}

				//try
				//{
				//	using (OcrInput Input = new OcrInput(Application.StartupPath + "\\log\\" + phone + "_login.png"))
				//	{
				//		OcrResult Result = null;

				//		for (int j = 0; j < 10; j++)
				//		{
				//			try
				//			{
				//				if (isConverting == false)
				//				{
				//					device.LoadStatusLD(statusProxy + "Đang connect ảnh...");
				//					this.SetStatusAccount(indexRow, statusProxy + "Đang connect ảnh...", null);

				//					isConverting = true;
				//					Result = ironTesseract.Read(Input);
				//					Thread.Sleep(1000);
				//					isConverting = false;
				//					login_code = Regex.Match(Result.Text.Replace("\r\n", ""), "login code:(.*?)Do").Groups[1].Value.Trim();

				//					if (login_code != "")
				//					{
				//						device.LoadStatusLD("Login Code : " + login_code);
				//						this.SetStatusAccount(indexRow, "Login Code : " + login_code, null);
				//						result = "1|" + login_code;
				//						goto Xong;

				//					}
				//					else
				//					{
				//						device.LoadStatusLD("Chưa lấy được login code, đang thử lại...");
				//						this.SetStatusAccount(indexRow, "Chưa lấy được login code, đang thử lại...", null);
				//						Common.DelayTime(2);
				//					}
				//				}
				//				else
				//				{
				//					device.LoadStatusLD(statusProxy + "Đang chờ connect ảnh login code...");
				//					this.SetStatusAccount(indexRow, statusProxy + "Đang chờ connect ảnh login code...", null);

				//					Common.DelayTime(2);
				//				}
				//			}
				//			catch (Exception ex)
				//			{
				//				isConverting = true;
				//				Common.DelayTime(2);
				//				Common.ExportError(ex, "GetLoginCode3()");
				//			}
				//		}
				//	}
				//}
				//catch (Exception ex)
				//{
				//	result = "0|Lỗi khác : " + ex.Message.ToString();
				//	Common.ExportError(ex, "GetLoginCode()");
				//}
			}

		Xong:
			if (result == "")
			{
				result = "0|Không lấy được login code";
			}
			return result;
		}

		private bool ConnectProxy(Device device, string text)
		{
			device.TapByText("ok", "", 5);
			Thread.Sleep(500);

			bool flag = false;
			device.OpenApp("com.cell47.College_Proxy");
			for (int i = 0; i < 5 && !(device.GetActivity() == "com.cell47.College_Proxy/com.cell47.College_Proxy.ui.MainActivity"); i++)
			{
				device.DelayTime(1);
			}
			if (device.WaitForTextDisappear(30, new string[] { "please wait" }))
			{
				string html = device.GetHtml();
				if (device.CheckExistText("stop proxy service", html, 0))
				{
					device.TapByText("stop proxy service", html, 0);
					device.DelayTime(1);
					html = device.GetHtml();
				}
				string str = device.CheckIP();
				List<string> strs = new List<string>()
				{
					"260,188",
					"260,253",
					"260,325",
					"260,390"
				};
				for (int j = 0; j < 4; j++)
				{
					device.DoubleTap(Convert.ToInt32(strs[j].Split(new char[] { ',' })[0]), Convert.ToInt32(strs[j].Split(new char[] { ',' })[1]));
					device.DelayTime(1);
					device.InputText(text.Split(new char[] { ':' })[j].ToString());
					device.DelayTime(1);
				}
				device.TapByText("start proxy service", html, 0);
				device.DelayTime(1);
				device.TapByText("ok", "", 0);
				device.DelayTime(1);
				int indexDevice = device.IndexDevice * 2 + 5555;
				string str1 = string.Concat("127.0.0.1:", indexDevice.ToString());
				int num = 0;
				while (true)
				{
					if (num < 10)
					{
						html = device.GetHtml();
						int num1 = device.CheckExistTexts(html, 0, new string[] { "connection request", "stop proxy service" });
						if (num1 != 0)
						{
							if (num1 != 1)
							{
								break;
							}
							device.TapByText("ok", html, 0);
							device.DelayTime(1);
							break;
						}
						else
						{
							ADBHelper.ConnectDevice(str1);
							this.ReconnectPortLD();
							num++;
						}
					}
					else
					{
						break;
					}
				}
				ADBHelper.ConnectDevice(str1);
				this.ReconnectPortLD();
				string str2 = "";
				for (int k = 0; k < 10; k++)
				{
					str2 = device.CheckIP();
					if ((str2 == "" ? false : !str2.Contains("error")))
					{
						break;
					}
					Thread.Sleep(1000);
				}
				flag = (!(str != str2) || str2.Contains("error") ? false : str2 != "");
			}
			return flag;
		}

		private bool ConnectProxyV2(Device device, string text)
		{
			bool flag = false;
			device.OpenApp("com.cell47.College_Proxy");
			for (int i = 0; i < 5 && !(device.GetActivity() == "com.cell47.College_Proxy/com.cell47.College_Proxy.ui.MainActivity"); i++)
			{
				device.DelayTime(1);
			}
			if (device.WaitForTextDisappear(30, new string[] { "please wait" }))
			{
				string html = device.GetHtml();
				if (device.CheckExistText("stop proxy service", html, 0))
				{
					device.TapByText("stop proxy service", html, 0);
					device.DelayTime(1);
					html = device.GetHtml();
				}
				string str = device.CheckIP();
				List<string> strs = new List<string>()
				{
					"260,188",
					"260,253",
					"260,325",
					"260,390"
				};
				for (int j = 0; j < 2; j++)
				{
					device.DoubleTap(Convert.ToInt32(strs[j].Split(new char[] { ',' })[0]), Convert.ToInt32(strs[j].Split(new char[] { ',' })[1]));
					device.DelayTime(1);
					device.InputText(text.Split(new char[] { ':' })[j].ToString());
					device.DelayTime(1);
				}
				device.TapByText("start proxy service", html, 0);
				device.DelayTime(1);
				device.TapByText("ok", "", 0);
				device.DelayTime(1);
				int indexDevice = device.IndexDevice * 2 + 5555;
				string str1 = string.Concat("127.0.0.1:", indexDevice.ToString());
				int num = 0;
				while (true)
				{
					if (num < 10)
					{
						html = device.GetHtml();
						int num1 = device.CheckExistTexts(html, 0, new string[] { "connection request", "stop proxy service" });
						if (num1 != 0)
						{
							if (num1 != 1)
							{
								break;
							}
							device.TapByText("ok", html, 0);
							device.DelayTime(1);
							break;
						}
						else
						{
							ADBHelper.ConnectDevice(str1);
							this.ReconnectPortLD();
							num++;
						}
					}
					else
					{
						break;
					}
				}
				ADBHelper.ConnectDevice(str1);
				this.ReconnectPortLD();
				string str2 = "";
				for (int k = 0; k < 10; k++)
				{
					str2 = device.CheckIP();
					if ((str2 == "" ? false : !str2.Contains("error")))
					{
						break;
					}
					Thread.Sleep(1000);
				}
				flag = (!(str != str2) || str2.Contains("error") ? false : str2 != "");
			}
			return flag;
		}

		public void ReconnectPortLD()
		{
			for (int i = 0; i < this.lstDevice.Count; i++)
			{
				string deviceByIndex = ADBHelper.GetDeviceByIndex(this.lstDevice[i].IndexDevice);
				if (!string.IsNullOrEmpty(deviceByIndex))
				{
					this.lstDevice[i].DeviceId = deviceByIndex;
				}
			}
		}

		public void SetStatusAccount2(int indexRow, string value, Device device = null)
		{
			//DatagridviewHelper.SetStatusDataGridView(this.dtgvAcc01, indexRow, "cStatus01", value);
			//if (device != null)
			//{
			//	if (value.StartsWith("("))
			//	{
			//		value = value.Substring(value.IndexOf(")") + 1);
			//	}
			//	device.LoadHanhDongLD(value);
			//}
		}

		public void SetStatusAccount(int indexRow, string value, Device device = null)
		{
			DatagridviewHelper.SetStatusDataGridView(this.dtgvAcc, indexRow, "cStatus", value);
			//if (device != null)
			//{
			//	if (value.StartsWith("("))
			//	{
			//		value = value.Substring(value.IndexOf(")") + 1);
			//	}
			//	device.LoadHanhDongLD(value);
			//}
		}

		public void SetCellAccount(int indexRow, string column, object value, bool isAllowEmptyValue = true)
		{
			if ((isAllowEmptyValue ? true : value.ToString().Trim() != ""))
			{
				DatagridviewHelper.SetStatusDataGridView(this.dtgvAcc, indexRow, column, value);
			}
		}

		public void SetCellAccount(int indexRow, int column, object value)
		{
			DatagridviewHelper.SetStatusDataGridView(this.dtgvAcc, indexRow, column, value);
		}

		private void CloseFormViewLD()
		{
			Common.CloseForm("fViewLD");
		}

		public List<string> GetListKeyTinsoft()
		{
			List<string> strs = new List<string>();
			try
			{
				if (this.settings_common.GetValueInt("typeApiTinsoft", 0) != 0)
				{
					foreach (string valueList in this.settings_common.GetValueList("txtApiProxy", 0))
					{
						if (!TinsoftProxy.CheckApiProxy(valueList))
						{
							continue;
						}
						strs.Add(valueList);
					}
				}
				else
				{
					RequestHttp requestXNet = new RequestHttp("", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)", "");
					string str = requestXNet.RequestGet(string.Concat("http://proxy.tinsoftsv.com/api/getUserKeys.php?key=", this.settings_common.GetValue("txtApiUser", "")));
					foreach (JToken item in JObject.Parse(str)["data"])
					{
						if (!Convert.ToBoolean(item["success"].ToString()))
						{
							continue;
						}
						strs.Add(item["key"].ToString());
					}
				}
			}
			catch
			{
			}
			return strs;
		}

		private void OpenFormViewLD(bool isRunSwap)
		{
			if (!Common.CheckFormIsOpenning("fViewLD"))
			{
				(new fViewLD()
				{
					isRunSwap = false
				}).Show();
			}
		}

		private void cControl(string dt)
		{
			base.Invoke(new MethodInvoker(() => {
				try
				{
					if (dt == "start")
					{
						this.btnStart.Enabled = false;
						this.btnStop.Enabled = true;
					}
					else if (dt == "stop")
					{
						this.btnStop.Text = "STOP";
						this.btnStart.Enabled = true;
						this.btnStop.Enabled = false;
					}
				}
				catch (Exception exception)
				{
				}
			}));
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			try
			{
				this.isStop = true;
				this.btnStop.Enabled = false;
				this.btnStop.Text = "Đang dừng...";
			}
			catch
			{
			}
		}

		private void btnConfig_Click(object sender, EventArgs e)
		{
			Common.ShowForm(new frmConfig());
		}

		private void btnConfigProxy_Click(object sender, EventArgs e)
		{
			Common.ShowForm(new frmConfigProxy());
		}

		#region TempSMS.co

		private string GetPhoneTempSMS(string apikey, int timeOut = 60, int indexRow = 0, string statusProxy = "", string serviceID = "", int solan = 10)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";
			string result = "";

			if (serviceID == "")
			{
				serviceID = "65";

			}

			// Lấy reset ID
			for (int j = 0; j < solan; j++)
			{
				try
				{
					// Kiểm tra đã dừng
					if (this.isStop)
					{
						return "|";
					}

					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] Tempsms " + apikey + ":", j, solan), null);
					url = "https://api.tempsms.co/create?api_key=" + apikey + "&service_id=" + serviceID;

					string json = requestXNet.RequestGet(url);
					JObject jobject = JObject.Parse(json);

					if (json.Contains("Not enough money"))
					{
						result = "|";
						Thread.Sleep(3000);
					}
					else if (jobject["message"].ToString() == "Success")
					{
						request_id = jobject["request_id"].ToString();
						goto LayPhone;
					}
					else
					{
						Thread.Sleep(3000);
					}
				}
				catch (Exception ex)
				{
					result = "|";
					Thread.Sleep(6000);
				}
			}

			return result;

		LayPhone:
			for (int i = 0; i < 10; i++)
			{
				try
				{
					url = "https://api.tempsms.co/detail?api_key=" + apikey + "&request_id=" + request_id;

					string json = requestXNet.RequestGet(url);
					JObject jobject = JObject.Parse(json);

					try
					{
						phone = jobject["phone"].ToString();

						if (phone != "")
						{
							break;
						}

						Thread.Sleep(3000);
					}
					catch
					{

					}
				}
				catch (Exception ex)
				{
				}
			}
			return request_id + "|" + phone;
		}

		private string GetOTPTempSMS(string api, string idRequest, int timeOut = 40, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("https://api.tempsms.co/detail?api_key=" + api + "&request_id=" + idRequest);
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				try
				{
					JObject jobject = JObject.Parse(text);
					result = JObject.Parse(text)["otp"].ToString();
					if (result != "")
					{
						break;
					}
					Thread.Sleep(3000);
				}
				catch (Exception)
				{

				}
			}
			return result;
		}

		private bool HuyTempSMS(string api, string idRequest)
		{
			RequestHttp requestXNet = new RequestHttp("", "", "");
			try
			{
				string text = requestXNet.RequestGet("https://api.tempsms.co/cancel?api_key=" + api + "&request_id=" + idRequest);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}

		}


		#endregion

		#region simfast.vn

		private string GetPhoneSimfast(string apikey, int timeOut = 60)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";

			// Lấy reset ID
			try
			{
				url = "https://access.simfast.vn/api/ig/request?api_token=" + apikey + "&serviceId=2";

				string json = requestXNet.RequestGet(url);
				JObject jobject = JObject.Parse(json);

				if (jobject["success"].ToString() == "True")
				{
					request_id = jobject["data"]["session_id"].ToString();
				}
			}
			catch (Exception ex)
			{
				return "|";
			}

			for (int i = 0; i < 5; i++)
			{
				try
				{
					url = "https://access.simfast.vn/api/ig/code?api_token=" + apikey + "&sessionId=" + request_id;

					string json = requestXNet.RequestGet(url);
					JObject jobject = JObject.Parse(json);

					try
					{
						phone = jobject["data"]["phone"].ToString();

						if (phone != "")
						{
							break;
						}

						Thread.Sleep(3000);
					}
					catch
					{

					}
				}
				catch (Exception ex)
				{
				}
			}
			return request_id + "|" + phone;
		}

		private string GetOTPSimfast(string api, string idRequest, int soLuot = 2, int timeOut = 40, string proxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("https://access.simfast.vn/api/ig/code?api_token=" + api + "&sessionId=" + idRequest);
				try
				{
					JObject jobject = JObject.Parse(text);
					result = jobject["data"]["sms"].ToString();
					if (result != "")
					{
						result = Regex.Match(result, @"\d+").Value;
						break;
					}
					Thread.Sleep(3000);
				}
				catch (Exception)
				{

				}
			}
			return result;
		}

		#endregion

		#region codesim.net

		public string GetPhoneCodeSim(string apikey, int timeOut = 60, int indexRow = 0, string statusProxy = "", int solan = 10)
		{
			string phoneNumber = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string id_giaodich = "";
			for (int i = 0; i < solan; i++)
			{
				try
				{
					// Kiểm tra đã dừng
					if (this.isStop)
					{
						return "|";
					}

					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] Codesim.net " + apikey + ":", i, solan), null);
					string json = requestXNet.RequestGet("http://api.codesim.net/api/CodeSim/DangKy_GiaoDich?apikey=" + apikey + "&dichvu_id=3&so_sms_nhan=1");

					if (json.Contains("hiện đã hết số"))
                    {
						this.SetStatusAccount(indexRow, statusProxy + "Dịch vụ Telegram hiện đã hết số, vui lòng liên hệ admin để tiếp tục sử dụng dịch vụ", null);
					}

					JObject jobject = JObject.Parse(json);
					id_giaodich = jobject["data"]["id_giaodich"].ToString();
					if (id_giaodich != "" && id_giaodich != "0")
					{
						phoneNumber = jobject["data"]["phoneNumber"].ToString();
						phoneNumber = string.Format("84{0}", phoneNumber.Remove(0, 1));
						break;
					} else
                    {
						Thread.Sleep(1000);
					}
				}
				catch (Exception ex)
				{
					Common.ExportError(ex, "GetPhoneCodeSim()");
				}
			}
			return id_giaodich + "|" + phoneNumber;
		}

		public bool HuyCodeSim(string apikey, string id_order, int timeOut = 10)
		{
			bool result = false;
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			for (int j = 0; j < timeOut; j++)
			{
				Thread.Sleep(1000);
				string text = requestXNet.RequestGet("http://api.codesim.net/api/CodeSim/HuyGiaoDich?apikey=" + apikey + "&giaodich_id=" + id_order);
				if (text.Contains("thành công"))
				{
					return true;
				} else
                {

                }
			}
			return result;
		}

		public string GetOTPCodeSim(string apikey, string id_order, int timeOut = 120, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			for (int j = 0; j < timeOut; j++)
			{
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] Đang lấy OTP...", j, timeOut), null);
				Thread.Sleep(1000);
				string text = requestXNet.RequestGet("http://api.codesim.net/api/CodeSim/KiemTraGiaoDich?apikey=" + apikey + "&giaodich_id=" + id_order);
				if (text.Contains("smsContent"))
				{
					try
					{
						result = JObject.Parse(text)["data"]["listSms"][0]["number"].ToString();
						if (result != "")
						{
							break;
						}
						Thread.Sleep(1000);
					}
					catch
					{
					}
					Thread.Sleep(1000);
				}
			}
			return result;
		}

		#endregion

		#region viotp

		public string GetPhoneViotp(string apikey, int timeOut = 60, string serviceID = "", int indexRow = 0, string statusProxy = "", int solan = 10)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string requestID = "";
			if (serviceID == "")
			{
				serviceID = "19";
			}
			for (int i = 0; i < solan; i++)
			{
				try
				{
					// Kiểm tra đã dừng
					if (this.isStop)
					{
						return "|";
					}

					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] Viotp " + apikey + ":", i, solan), null);
					string json = requestXNet.RequestGet("https://api.viotp.com/request/get?token=" + apikey + "&serviceId=" + serviceID);

					if (json.Contains("không có sẵn số điện thoại phù hợp"))
                    {
						this.SetStatusAccount(indexRow, statusProxy + "Hiện không có sẵn số điện thoại phù hợp, vui lòng thử lại sau !", null);
						Thread.Sleep(1000);
					} else
                    {
						JObject jobject = JObject.Parse(json);
						requestID = jobject["data"]["request_id"].ToString();
						if (requestID != "")
						{
							phone = jobject["data"]["phone_number"].ToString();
							break;
						}
						Thread.Sleep(1000);
					}

					
				}
				catch
				{
					Thread.Sleep(1000);
				}
			}
			return requestID + "|" + phone;
		}

		public string GetOTPViotp(string apikey, string id_order, int timeOut = 120, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;

			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				string text = requestXNet.RequestGet("https://api.viotp.com/session/get?requestId=" + id_order + "&token=" + apikey);
				if (text.Contains("SmsContent"))
				{
					try
					{
						result = JObject.Parse(text)["data"]["Code"].ToString();
						if (result != "")
						{
							break;
						}
						Thread.Sleep(1000);
					}
					catch
					{
					}
				}
			}
			return result;
		}

		#endregion

		#region

		private string GetPhoneChoThueSimCode(string apiKey, string nha_mang, string dau_so, int indexRow, string statusProxy = "", string serviceID = "", bool isNewAPI = false, int solan = 10)
		{
			string result = "|";

			for (int i = 0; i < solan; i++)
			{
				try
				{
					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] CTSC " + apiKey + ":", i, solan), null);
					xNet.HttpRequest httpRequest = new xNet.HttpRequest();
					if (serviceID == "")
					{
						serviceID = "1006";
					}
					string url = "";
					if (!isNewAPI)
                    {
						url = "https://chothuesimcode.com/api?act=number&apik=" + apiKey + "&appId=" + serviceID;
					} else
                    {
						url = "http://yuenanka.com/api?act=number&apik=" + apiKey + "&appId=" + serviceID;
					}

					if (nha_mang != "")
					{
						url = url + "&carrier=" + nha_mang;
					}

					if (dau_so != "")
					{
						url = url + "&prefix=" + dau_so;
					}

					// Kiểm tra đã dừng
					if (this.isStop)
                    {
						return "|";
                    }

					string str1 = httpRequest.Get(url).ToString();
					string requestID = Regex.Match(str1, "\"Id\":(\\d+),").Groups[1].Value;
					string phoneNumber = Regex.Match(str1, "\"Number\":\"(\\d+)\",").Groups[1].Value;

					if (!(requestID != string.Empty) || !(phoneNumber != string.Empty))
					{
						result = "|";
						Common.DelayTime(5);
					}
					else
					{
						result = string.Concat(requestID, "|", phoneNumber);
						goto Xong;
					}
				}
				catch
				{
				}
			}

		Xong:
			return result;
		}

		public bool HuyChoThueSimCode(string api, string request_id, bool isNewAPI = false)
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			try
			{
				if (!isNewAPI)
				{
					requestXNet.RequestGet("https://chothuesimcode.com/api?act=expired&apik=" + api + "&id=" + request_id).ToString();
				}
				else
				{
					requestXNet.RequestGet("http://yuenanka.com/api?act=expired&apik=" + api + "&id=" + request_id).ToString();
				}

				if (result.Contains("OK"))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "HuyChoThueSimCode()");
			}
			return false;
		}

		public string GetOTPChoThueSimCode(string api, string request_id, int timeOut = 120, int indexRow = 0, string statusProxy = "", bool isNewAPI = false)
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);

				string text = "";
				if (!isNewAPI)
                {
					text = requestXNet.RequestGet(string.Concat("https://chothuesimcode.com/api?act=code&apik=", api, "&id=", request_id.Split('|')[0])).ToString();
				} else
                {
					text = requestXNet.RequestGet(string.Concat("http://yuenanka.com/api?act=code&apik=", api, "&id=", request_id.Split('|')[0])).ToString();
				}

				try
				{
					JObject jobject = JObject.Parse(text);
					result = jobject["Result"]["Code"].ToString();
					if (result != "")
					{
						break;
					}
					Thread.Sleep(3000);
				}
				catch (Exception)
				{

				}
			}
			return result;
		}

		#endregion

		#region 2nd line

		public string GetPhone2ndLine(string apikey, int timeOut = 60, string serviceID = "", int indexRow = 0, string statusProxy = "", int solan = 30)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";
			if (serviceID == "") serviceID = "269";
			// Lấy reset ID
			try
			{
				// Kiểm tra đã dừng
				if (this.isStop)
				{
					return "|";
				}

				url = "https://2ndline.io/apiv1/order?apikey=" + apikey + "&serviceId=" + serviceID + "&allowVoiceSms=false";

				string json = requestXNet.RequestGet(url);
				JObject jobject = JObject.Parse(json);

				if (jobject["status"].ToString() == "1")
				{
					request_id = jobject["id"].ToString();
				}
			}
			catch (Exception ex)
			{
				return "|";
			}

			for (int i = 0; i < solan; i++)
			{
				try
				{
					// Kiểm tra đã dừng
					if (this.isStop)
					{
						return "|";
					}

					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] 2ndline.io " + apikey + ":", i, solan), null);
					url = "https://2ndline.io/apiv1/ordercheck?apikey=" + apikey + "&id=" + request_id;

					string json = requestXNet.RequestGet(url);
					JObject jobject = JObject.Parse(json);

					try
					{
						phone = jobject["data"]["phone"].ToString();

						if (phone != "")
						{
							phone = string.Format("84{0}", phone.Remove(0, 1));
							break;
						}

						Thread.Sleep(1000);
					}
					catch
					{

					}
					Thread.Sleep(1000);
				}
				catch (Exception ex)
				{
				}
			}
			return request_id + "|" + phone;
		}

		public string GetOTP2ndLine(string apikey, string id_order, int timeOut = 120, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("https://2ndline.io/apiv1/ordercheck?apikey=" + apikey + "&id=" + id_order);
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				if (text.Contains("message"))
				{
					try
					{
						result = JObject.Parse(text)["data"]["code"].ToString();
						if (result != "")
						{
							break;
						}
						Thread.Sleep(1000);
					}
					catch
					{
					}
				}
			}
			return result;
		}

		#endregion

		#region SMS-Activate

		public string GetPhoneSMSActivate(string apikey, int timeOut = 60)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";

			// Lấy reset ID
			try
			{
				url = "https://sms-activate.org/stubs/handler_api.php?api_key=" + apikey + "&action=getNumber&service=tg&forward=0&country=10";

				string json = requestXNet.RequestGet(url);

				if (json.ToString().Contains("ACCESS_NUMBER"))
				{
					json = json.Replace("ACCESS_NUMBER:", "");
					request_id = json.Split(':')[0];
					phone = json.Split(':')[1];
				}
			}
			catch (Exception ex)
			{
				return "|";
			}

			return request_id + "|" + phone;
		}

		public string GetOTPSMSActivate(string apikey, string id_order, int timeOut = 120)
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("https://api.sms-activate.org/stubs/handler_api.php?api_key=" + apikey + "&action=getStatus&id=" + id_order);
				if (text.Contains("STATUS_OK"))
				{
					try
					{
						result = text.Replace("STATUS_OK:", "");
						if (result != "")
						{
							break;
						}
						Thread.Sleep(1000);
					}
					catch
					{
					}
				}
			}
			return result;
		}

		#endregion

		#region thuesimxyz

		public static string GetPhoneThuesimXyz(string apikey)
		{
			string str = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string str2 = "";
			for (int i = 0; i < 5; i++)
			{
				try
				{
					string json = requestXNet.RequestGet("https://thuesim.xyz/api/register-service/?serviceId=02.13&token=" + apikey);
					JObject jobject = JObject.Parse(json);
					str2 = jobject["RequestLog"]["ID"].ToString(); ///
					if (str2 != "")
					{
						str = jobject["RequestLog"]["Phone"].ToString();
						break;
					}
					Thread.Sleep(3000);
				}
				catch
				{
				}
			}
			return str2 + "|" + str;
		}

		#endregion

		#region JS Nguyen lieu

		public static string GetPhoneJSNguyenLieu(string apikey, int indexRow = 0, string statusProxy = "", string serviceID = "")
		{
			string phone = "";
			string order_id = "";
			for (int i = 0; i < 10; i++)
			{
				try
				{
					var client = new RestClient("https://jsnguyenlieu.com/api/sms/buy");
					client.Timeout = -1;
					var request = new RestRequest(Method.POST);
					request.AddHeader("Authorization", "Bearer " + apikey);
					var body = @"{
					" + "\n" +
										@"    ""package_id"" : 335
					" + "\n" +
					@"}";
					request.AddParameter("application/json", body, ParameterType.RequestBody);
					IRestResponse response = client.Execute(request);
					JObject jobject = JObject.Parse(response.Content);

					order_id = jobject["data"]["order_id"].ToString();
					if (order_id != "")
					{
						phone = jobject["data"]["phone"].ToString();
						break;
					}
					Thread.Sleep(6000);
				}
				catch
				{
				}
				Thread.Sleep(6000);
			}
			return order_id + "|" + phone;
		}

		public static string GetOTPJSNguyenLieu(string apikey, string request_id, int timeOut = 120)
		{
			string result = "";
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				var client = new RestClient("https://jsnguyenlieu.com/api/sms/order");
				client.Timeout = -1;
				var request = new RestRequest(Method.POST);
				request.AddHeader("Authorization", "Bearer " + apikey);
				request.AddHeader("Accept", "application/json");
				var body = new
				{
					order_id = request_id
				};
				request.AddJsonBody(body);
				IRestResponse response = client.Execute(request);

				try
				{
					JObject jobject = JObject.Parse(response.Content);
					result = jobject["data"]["code"].ToString();
					if (result != "")
					{
						break;
					}
					Thread.Sleep(3000);
				}
				catch (Exception)
				{

				}

			}
			return result;
		}

		#endregion

		#region Phone ahasim

		public string GetPhoneAhasim(string apikey, int indexRow = 0, string statusProxy = "", string serviceID = "", int solan = 10)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			if (serviceID == "") serviceID = "5";
			for (int i = 0; i < solan; i++)
			{
				try
				{
					// Kiểm tra đã dừng
					if (this.isStop)
					{
						return "|";
					}

					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] Ahasim " + apikey + ":", i, solan), null);
					string json = requestXNet.RequestGet("http://ahasim.com/api/phone/new-session?token=" + apikey + "&service=" + serviceID);
					JObject jobject = JObject.Parse(json);
					request_id = jobject["data"]["session"].ToString();
					if (request_id != "")
					{
						phone = jobject["data"]["phone_number"].ToString();
						this.SetStatusAccount(indexRow, statusProxy + "Phone : " + phone, null);
						break;
					}
					Thread.Sleep(1000);
				}
				catch
				{
				}
				Thread.Sleep(1000);
			}
			return request_id + "|" + phone;
		}
		public string GetOTPAhasim(string apikey, string request_id, int timeOut = 120, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet(string.Concat("http://ahasim.com/api/session/", request_id, "/get-otp?token=", apikey.Split('|')[0])).ToString();
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				try
				{
					JObject jobject = JObject.Parse(text);
					result = jobject["data"]["messages"][0]["otp"].ToString();
					if (result != "")
					{
						break;
					}
					Thread.Sleep(5000);
				}
				catch (Exception)
				{

				}
			}
			return result;
		}

		#endregion

		#region Otpmm.com

		public string GetPhoneOtpMM(string apikey, int timeOut = 60)
		{
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string result = "";

			// Lấy reset ID
			try
			{
				string url = "http://api.otpmm.com/?Accesskey=" + apikey + "&Method=GetNumber&App=telegram";

				string json = requestXNet.RequestGet(url);
				if (json != "Error")
				{
					result = "|" + json;
				}
			}
			catch (Exception ex)
			{
				result = "|";
			}
			return result;
		}

		public string GetOtpOtpMM(string apikey, string phone, int timeOut = 60)
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("http://api.otpmm.com/?Accesskey=" + apikey + "&Method=ResponseKey&App=telegram&Numberphone=" + phone);
				if (text != "Chưa có mã")
				{
					try
					{
						result = Regex.Match(text, "[0-9]{5}").Value;
						if (result != "")
						{
							break;
						}
						Thread.Sleep(1000);
					}
					catch
					{
					}
				}
			}
			return result;

		}

		#endregion

		#region http://103.142.139.33/simthue

		public string GetPhoneCustomSimThue(string apikey, int timeOut = 60)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";

			// Lấy reset ID
			try
			{
				url = "http://103.142.139.33:1337/V1/NewRequest?token=" + apikey + "&service_id=195";

				string json = requestXNet.RequestGet(url);
				JObject jobject = JObject.Parse(json);

				if (jobject["success"].ToString() == "True")
				{
					request_id = jobject["id"].ToString();
				}
			}
			catch (Exception ex)
			{
				return "|";
				Thread.Sleep(1000);
			}

			for (int i = 0; i < 5; i++)
			{
				try
				{
					url = "http://103.142.139.33:1337/V1/RequestCheck?token=" + apikey + "&request_id=" + request_id;

					string json = requestXNet.RequestGet(url);
					JObject jobject = JObject.Parse(json);

					try
					{
						phone = jobject["phoneNumber10No"].ToString();

						if (phone != "")
						{
							break;
						}

						Thread.Sleep(3000);
					}
					catch
					{

					}
				}
				catch (Exception ex)
				{
				}
			}
			return request_id + "|" + phone;
		}

		public string GetOtpCustomSimThue(string apikey, string request_id, int timeOut = 60, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("http://103.142.139.33:1337/V1/RequestCheck?token=" + apikey + "&request_id=" + request_id);
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				JObject jobject = JObject.Parse(text);
				try
				{
					result = jobject["otp"].ToString();
					if (result != "")
					{
						result = Regex.Match(result, "[0-9]{5}").Value;
						if (result != "")
						{
							break;
						}
					}
					Thread.Sleep(1000);
				}
				catch
				{
				}
			}
			return result;

		}

		#endregion

		#region Tempcode.co

		public string GetPhoneTempCodeCo(string apikey, int timeOut = 60)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";
			string result = "";

            // Lấy reset ID
            for (int j = 0; j < 10; j++)
            {
				try
				{
					url = "https://tempcode.co/api/orders.php";
					string data = "api_key=" + apikey + "&act=buy_number&service_id=telegram";
					string json = requestXNet.RequestPost(url, data);
					JObject jobject = JObject.Parse(json);

					if (jobject["success"].ToString() == "True")
					{
						request_id = jobject["data"]["order_id"].ToString();
						phone = jobject["data"]["phonenumber"].ToString();
						result = request_id + "|" + phone;
						goto Xong;
					} else
                    {
						result = "|";
						Thread.Sleep(1000);
                    }
				}
				catch (Exception ex)
				{
					result = "|";
					Thread.Sleep(1000);
				}
			}
			
			Xong:
			return result;
		}

		public string GetOtpTempCodeCo(string apikey, string request_id, int timeOut = 60, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string data = "api_key=" + apikey + "&act=read_message&order_id=" + request_id;
				string text = requestXNet.RequestPost("https://tempcode.co/api/orders.php", data);
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				JObject jobject = JObject.Parse(text);
				try
				{
					result = jobject["data"]["otp"].ToString();
					if (result != "")
					{
						result = Regex.Match(result, "[0-9]{5}").Value;
						if (result != "")
						{
							break;
						}
					}
					Thread.Sleep(1000);
				}
				catch
				{
				}
			}
			return result;

		}

		#endregion

		#region CodeText247

		public string GetPhoneCodeText247(string apikey, int indexRow = 0, string statusProxy = "", int solan = 10)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string result = "";

			// Lấy reset ID
			for (int j = 0; j < solan; j++)
			{
				try
				{
					// Kiểm tra đã dừng
					if (this.isStop)
					{
						return "|";
					}

					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] CodeText247 " + apikey + ":", j, solan), null);
					string json = requestXNet.RequestGet("https://administrator.codetext247.com/api/sim-request?token="+apikey+"&service=13");
					JObject jobject = JObject.Parse(json);

					if (jobject["phone"].ToString() != "")
					{
						phone = jobject["phone"].ToString();
						result = phone + "|" + phone;
						goto Xong;
					}
					else
					{
						result = "|";
						Thread.Sleep(1000);
					}
				}
				catch (Exception ex)
				{
					result = "|";
					Thread.Sleep(1000);
				}
			}

		Xong:
			return result;
		}

		public string GetOtpCodeText247(string apikey, string request_id, int timeOut = 60, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string data = "api_key=" + apikey + "&act=read_message&order_id=" + request_id;
				string text = requestXNet.RequestGet("https://administrator.codetext247.com/api/get-message?token="+apikey+"&phone=" + request_id);
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				JObject jobject = JObject.Parse(text);
				try
				{
					if (jobject["status"].ToString() == "COMPLETE")
					{
						result = Regex.Match(jobject["message"].ToString(), "[0-9]{5}").Value;
						if (result != "")
						{
							break;
						}
					}
					Thread.Sleep(1000);
				}
				catch
				{
				}
			}
			return result;

		}

		#endregion

		#region Khách 01

		public string GetPhoneKhach01(string apikey, int timeOut = 60)
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";
			string result = "";

			// Lấy reset ID
			for (int j = 0; j < 10; j++)
			{
				try
				{
					string json = requestXNet.RequestGet("http://171.226.249.225:2266/vicsim/NewRequest?token=" +apikey+"&service_id=1");
					JObject jobject = JObject.Parse(json);

					if (jobject["phoneNumber10No"].ToString() == "True")
					{
						request_id = jobject["id"].ToString();
						phone = jobject["phoneNumber10No"].ToString();
						result = request_id + "|" + phone;
						goto Xong;
					}
					else
					{
						result = "|";
					}
				}
				catch (Exception ex)
				{
					result = "|";
				}
			}

			Xong:
			return result;
		}

		#endregion

		#region Custom PHP API

		public string GetPhoneCustomAPIPhp(string apikey, int timeOut = 60, int indexRow = 0, string statusProxy = "")
		{
			string phone = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			string request_id = "";
			string url = "";
			string result = "";

			// Lấy reset ID
			for (int j = 0; j < 100; j++)
			{
				try
				{
					if (this.isStop)
                    {
						return "|";
                    }

					string json = requestXNet.RequestGet("http://localhost/smsapi/getphone.php?token=" + apikey);
					this.SetStatusAccount(indexRow, statusProxy + string.Format("[LẦN {0}/{1}] PHP Custom :" + apikey, j, 100), null);
					Console.WriteLine(json);
					Common.OutData(json);
					JObject jobject = JObject.Parse(json);

					if (jobject["phone"].ToString() != "")
					{
						request_id = jobject["request_id"].ToString();
						phone = jobject["phone"].ToString();
						result = request_id + "|" + phone;
						goto Xong;
					}
					else
					{
						result = "|";
						Thread.Sleep(1000);
					}
				}
				catch (Exception ex)
				{
					result = "|";
				}
			}

		Xong:
			return result;
		}

		public string GetOtpCodeCustomAPIPhp(string apikey, string request_id, int timeOut = 60, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("http://localhost/smsapi/getcode.php?token=" + apikey + "&request_id=" + request_id);
				Console.WriteLine(text);
				Common.OutData(text);
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);

				try
				{
					JObject jobject = JObject.Parse(text);
					if (jobject["otp"].ToString() != "")
					{
						result = Regex.Match(jobject["otp"].ToString(), "[0-9]{5}").Value;
						if (result != "")
						{
							break;
						}
					}
					Thread.Sleep(1000);
				}
				catch
				{
					Thread.Sleep(1000);
				}
			}
			return result;
		}


		#endregion

		#region Khách 02

		private string GetPhoneKhach02(string apikey, int indexRow = 0, string statusProxy = "", int solan = 10)
		{
			string phone = "";
			string request_id = "";
			string url = "";
			string service_id = "62f615ec1b953f97d5a69c8f";
			string result = "";

			for (int j = 0; j < solan; j++)
            {
				var client = new RestClient("https://bossotp.com/api/v1/rent/");
				client.Timeout = -1;
				var request = new RestRequest(Method.POST);
				request.AddHeader("accept", "application/json");
				request.AddHeader("authorization", apikey);
				request.AddHeader("Content-Type", "application/json");
				var body = @"{" + "\n" +
				@"  ""service_id"": ""62f615ec1b953f97d5a69c8f""," + "\n" +
				@"  ""re_sim_id"": """"," + "\n" +
				@"  ""network"": """"" + "\n" +
				@"}";
				request.AddParameter("application/json", body, ParameterType.RequestBody);
				IRestResponse response = client.Execute(request);
				Console.WriteLine(response.Content);

                try
                {
					JObject jobject = JObject.Parse(response.Content);

					if (jobject["rent_id"].ToString() != "")
					{
						request_id = jobject["rent_id"].ToString();
						goto LayPhone;
					}
					else
					{
						result = "|";
						Thread.Sleep(3000);
					}
				}
                catch (Exception ex)
                {
					result = "|";
					Thread.Sleep(3000);
				}
				
			}

			return result;

		LayPhone:
			for (int i = 0; i < 30; i++)
			{
				try
				{
					var client = new RestClient("https://bossotp.com/api/v1/rent/" + request_id);
					client.Timeout = -1;
					var request = new RestRequest(Method.GET);
					request.AddHeader("accept", "application/json");
					request.AddHeader("authorization", apikey);
					IRestResponse response = client.Execute(request);

					string json = response.Content;
					Console.WriteLine(response.Content);
					JObject jobject = JObject.Parse(json);

					try
					{
						phone = jobject["number"].ToString();

						if (phone != "")
						{
							break;
						}

						Thread.Sleep(3000);
					}
					catch
					{
						Thread.Sleep(3000);
					}
				}
				catch (Exception ex)
				{
					Thread.Sleep(3000);
				}
			}
			return request_id + "|" + phone;
		}

		private string GetOTPKhach02(string api, string idRequest, int timeOut = 40, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				var client = new RestClient("https://bossotp.com/api/v1/rent/" + idRequest);
				client.Timeout = -1;
				var request = new RestRequest(Method.GET);
				request.AddHeader("accept", "application/json");
				request.AddHeader("authorization", api);
				IRestResponse response = client.Execute(request);
				Console.WriteLine(response.Content);
				try
				{
					JObject jobject = JObject.Parse(response.Content);
					result = JObject.Parse(response.Content)["sms_content"].ToString();
					result = Regex.Match(result, "[0-9]{5}").Value;
					if (result != "")
					{
						break;
					}
					Thread.Sleep(3000);
				}
				catch (Exception)
				{
					Thread.Sleep(3000);
				}
			}
			return result;
		}


		#endregion

		public string GetOTPOtpSim(string apikey, string id_order, int timeOut = 120)
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("", "", "");
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet("http://otpsim.com/api/sessions/" + id_order + "?token=" + apikey);
				if (text.Contains("successful"))
				{
					try
					{
						JObject jobject = JObject.Parse(text);
						result = JObject.Parse(text)["data"]["messages"][0]["otp"].ToString();
						if (result != "")
						{
							break;
						}
						Thread.Sleep(3000);
					}
					catch
					{
					}
				}
			}
			return result;
		}

		private void button7_Click(object sender, EventArgs e)
		{

		}

		private void btnLoadHistory_Click(object sender, EventArgs e)
		{
			//try
			//{
			//	OpenFileDialog openFileDialog = new OpenFileDialog()
			//	{
			//		InitialDirectory = string.Concat(Directory.GetCurrentDirectory(), "\\Log Data"),
			//		FilterIndex = 0,
			//		RestoreDirectory = true
			//	};
			//	if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			//	{
			//		string[] logData = File.ReadAllText(openFileDialog.FileName).Trim().Split(new char[] { '\n' });

			//		this.dtgvAcc01.Rows.Clear();

			//		for (int i = 0; i < (int)logData.Length; i++)
			//		{
			//			int indexRow = this.dtgvAcc01.Rows.Add();
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cSTT01", indexRow + 1);
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cName01", logData[i].Split(new char[] { '|' })[0].Trim());
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cPhone01", logData[i].Split(new char[] { '|' })[1].Trim());
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cPassword01", logData[i].Split(new char[] { '|' })[2].Trim());
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cAPIID01", logData[i].Split(new char[] { '|' })[3].Trim());
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cAPIHash01", logData[i].Split(new char[] { '|' })[4].Trim());
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cNgay01", logData[i].Split(new char[] { '|' })[5].Trim());
			//			Common.SetStatusDataGridView(dtgvAcc01, i, "cStatus01", logData[i].Split(new char[] { '|' })[6].Trim());
			//			if (logData[i].Split(new char[] { '|' })[6].Trim().Contains("LỖI"))
			//			{
			//				this.dtgvAcc01.Rows[i].DefaultCellStyle.BackColor = Color.Red;
			//				this.dtgvAcc01.Rows[i].DefaultCellStyle.ForeColor = Color.White;
			//			}
			//			else
			//			{
			//				this.dtgvAcc01.Rows[i].DefaultCellStyle.BackColor = Color.Green;
			//				this.dtgvAcc01.Rows[i].DefaultCellStyle.ForeColor = Color.White;
			//			}
			//		}
			//	}
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show(ex.Message, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			//}
		}

		private void WriteLog()
		{
			//try
			//{
			//	List<string> strs = new List<string>();

			//	for (int i = 0; i < dtgvAcc01.RowCount; i++)
			//	{
			//		strs.Add(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", new object[]
			//		{
			//			dtgvAcc01.Rows[i].Cells["cName01"].Value.ToString(),
			//			dtgvAcc01.Rows[i].Cells["cPhone01"].Value.ToString(),
			//			dtgvAcc01.Rows[i].Cells["cPassword01"].Value.ToString(),
			//			dtgvAcc01.Rows[i].Cells["cAPIID01"].Value.ToString(),
			//			dtgvAcc01.Rows[i].Cells["cAPIHash01"].Value.ToString(),
			//			dtgvAcc01.Rows[i].Cells["cNgay01"].Value.ToString(),
			//			dtgvAcc01.Rows[i].Cells["cStatus01"].Value.ToString()
			//		}));
			//	}
			//	if (!Directory.Exists(string.Concat(Directory.GetCurrentDirectory(), "\\Log Data")))
			//	{
			//		Directory.CreateDirectory(string.Concat(Directory.GetCurrentDirectory(), "\\Log Data"));
			//	}
			//	string currentDirectory = Directory.GetCurrentDirectory();
			//	object[] month = new object[5];
			//	DateTime now = DateTime.Now;
			//	month[0] = now.Month;
			//	now = DateTime.Now;
			//	month[1] = now.Day;
			//	this.WriteFileFull(string.Concat(currentDirectory, "\\Log Data\\", string.Format("Log_{0}m_{1}d.txt", month)), string.Join("\n", strs));
			//}
			//catch (Exception ex)
			//{

			//}
		}

		public void WriteFileFull(string string_0, string string_1)
		{
			lock (new object())
			{
				using (FileStream fileStream = new FileStream(string_0, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
					{
						streamWriter.Write(string_1);
					}
				}
			}
		}

		private void LoadDataLog()
		{
			//try
			//{
			//	FileInfo[] files = (new DirectoryInfo(string.Concat(Directory.GetCurrentDirectory(), "\\Log Data"))).GetFiles();
			//	Array.Sort<FileInfo>(files, (FileInfo fileInfo_0, FileInfo fileInfo_1) => StringComparer.OrdinalIgnoreCase.Compare(fileInfo_0.CreationTime, fileInfo_1.CreationTime));
			//	string[] logData = File.ReadAllText(files[(int)files.Length - 1].FullName).Trim().Split(new char[] { '\n' });

			//	this.dtgvAcc01.Rows.Clear();

			//	for (int i = 0; i < (int)logData.Length; i++)
			//	{
			//		int indexRow = this.dtgvAcc01.Rows.Add();
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cSTT01", i + 1);
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cName01", logData[i].Split(new char[] { '|' })[0].Trim());
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cPhone01", logData[i].Split(new char[] { '|' })[1].Trim());
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cPassword01", logData[i].Split(new char[] { '|' })[2].Trim());
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cAPIID01", logData[i].Split(new char[] { '|' })[3].Trim());
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cAPIHash01", logData[i].Split(new char[] { '|' })[4].Trim());
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cNgay01", logData[i].Split(new char[] { '|' })[5].Trim());
			//		Common.SetStatusDataGridView(dtgvAcc01, i, "cStatus01", logData[i].Split(new char[] { '|' })[6].Trim());
			//		if (logData[i].Split(new char[] { '|' })[6].Trim().Contains("LỖI"))
			//		{
			//			this.dtgvAcc01.Rows[i].DefaultCellStyle.BackColor = Color.Red;
			//			this.dtgvAcc01.Rows[i].DefaultCellStyle.ForeColor = Color.White;
			//		}
			//		else
			//		{
			//			this.dtgvAcc01.Rows[i].DefaultCellStyle.BackColor = Color.Green;
			//			this.dtgvAcc01.Rows[i].DefaultCellStyle.ForeColor = Color.White;
			//		}
			//	}
			//}
			//catch (Exception exception)
			//{
			//	Common.ExportError(exception, "");
			//}
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			//this.LoadCheckVersion();
			//this.LoadCheckKey();
			this.LoadDataLog();

			Common.KillProcess("EasyRegTelegramUpdate");
			try
			{
                Process.Start(@"EasyRegTelegramUpdate.exe");
            }
			catch
			{
			}
			
			Thread.Sleep(4000);
		}

		private void LoadCheckVersion()
		{
			try
			{
				// Load version của máy
				IniFile ini_local = new IniFile("version.txt");
				string currentVersion = ini_local.Read("Version", "Infor");

				// Đổi tên
				this.Text = "EasyRegTelegram v" + currentVersion + " - Phần mềm tự động đăng ký Telegram LD - EasySoftware.VN";
				this.bunifuCustomLabel1.Text = "EasyRegTelegram v" + currentVersion + " - Phần mềm tự động đăng ký Telegram LD - EasySoftware.VN";

				string nameSoft = "easyregtelegram";
				string url = "http://app.jxteamsw.com/api/checkversion.php?easy_key=" + nameSoft;
				string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

				if (!string.IsNullOrEmpty(nameSoft))
				{
					var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
					var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
					{
						var result = streamReader.ReadToEnd();
						VersionDto versionData = JsonConvert.DeserializeObject<VersionDto>(result);

						if (versionData.version != currentVersion)
						{
							this.lblUpdate.Visible = true;
						}
						else
						{
							this.lblUpdate.Visible = false;
						}
					}
				}

			}
			catch (Exception ex)
			{
				return;
			}
		}

		private string deviceId = "";
		private LicenseDto licenseDto { get; set; }

		private void LoadCheckKey()
		{
			this.deviceId = ComputerInfo.GetComputerId();

			try
			{
				string key = "";
				key = Settings.Default.key;
				if (!string.IsNullOrEmpty(key))
				{
					LicenseHelper licenseHelper = new LicenseHelper();
					this.licenseDto = licenseHelper.CheckLicenseV2(this.deviceId, key);

					if (this.licenseDto != null)
					{
						if (this.licenseDto.error == 1)
						{
							// Hết hạn
							MessageBox.Show(this.licenseDto.thong_bao);
							base.Invoke(new MethodInvoker(() => base.Hide()));
							frm_Login _fActive = new frm_Login()
							{
								ShowInTaskbar = true
							};
							_fActive.ShowDialog();
						}
						else
						{
							// Thành công
							File.WriteAllText(Common.Base64Decode("QzpcXFByb2dyYW1EYXRhXFxFYXN5MjIuaW5p"), this.deviceId + "|" + key);

							this.lblHetHan.Text = Common.ConvertNgayHetHan(this.licenseDto.ngay_het_han);
							this.lblKey.Text = this.deviceId;
						}
					}
					else
					{
						base.Invoke(new MethodInvoker(() => base.Hide()));
						frm_Login _fActive = new frm_Login()
						{
							ShowInTaskbar = true
						};
						_fActive.ShowDialog();
					}
				}
				else
				{
					base.Invoke(new MethodInvoker(() => base.Hide()));
					frm_Login _fActive = new frm_Login()
					{
						ShowInTaskbar = true
					};
					_fActive.ShowDialog();
				}
			}
			catch
			{
				base.Invoke(new MethodInvoker(() => base.Hide()));
				frm_Login _fActive = new frm_Login()
				{
					ShowInTaskbar = true
				};
				_fActive.ShowDialog();
			}
		}

		private void chọnBôiĐenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyRowDatagrid("phone");
		}

		private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyRowDatagrid("pass");
		}

		private void CopyRowDatagrid(string modeCopy)
		{
			try
			{
				string textCopy = "";
				switch (modeCopy)
				{
					case "phone":
						textCopy = Common.GetStatusDataGridView(dtgvAcc, dtgvAcc.CurrentRow.Index, "cPhone");
						Clipboard.SetText(textCopy.TrimEnd('\r', '\n'));
						break;
					case "pass":
						textCopy = Common.GetStatusDataGridView(dtgvAcc, dtgvAcc.CurrentRow.Index, "cPassword");
						Clipboard.SetText(textCopy.TrimEnd('\r', '\n'));
						break;

				}
			}
			catch { }

		}

		private void btnOutput_Click(object sender, EventArgs e)
		{
			Process.Start(Application.StartupPath + "\\SessionTemp");
		}

		List<int> lstPossition = new List<int>();

		private TelegramClient telegramClient = null;
		private string hash = string.Empty;

		void LoadStatusDatagridView(int row, string status)
		{
			//Common.SetStatusDataGridView(dtgvAcc01, row, "ColMsg", status);
		}

		private void xóaToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			try
			{
				//foreach (DataGridViewRow row in dtgvAcc01.SelectedRows)
				//{
				//	dtgvAcc01.Rows.Remove(row);
				//}

				//this.dtgvAcc01.Refresh();


				//for (int i = 0; i < dtgvAcc01.SelectedRows.Count; i++)
				//{
				//	dtgvAcc01.Rows.RemoveAt(i);
				//	i--;
				//}

				this.WriteLog();
			}
			catch (Exception)
			{ }
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.LoadDataLog();
		}

		private void xongToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Common.SetStatusDataGridView(dtgvAcc, dtgvAcc.CurrentRow.Index, "cFinish", "OK");
		}

		private void restoreAccToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}

		CancellationTokenSource cts = new CancellationTokenSource();
		CancellationToken ct = new CancellationToken();

		private CancellationTokenSource tokenSource;
		private CancellationToken token;

		private async void SetAddress(string item)
        {
            while (true)
            {
				Console.WriteLine(item);
				Thread.Sleep(1000);
                if (ct.IsCancellationRequested)
                    break;
            }
        }

		public static string RandomString(int length)
		{
			Random rd = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[rd.Next(s.Length)]).ToArray());
		}

		static async Task RegisterOnThread_Old(DataGridView dtgv, string deviceID, int indexRow, ConsoleColor color, int seconds, CancellationToken token)
		{
			await Task.Run(() =>
			{
				//
				while (true)
				{
					lock (Console.Out)
					{
						Console.ForegroundColor = color;
						Console.WriteLine($"{deviceID,10}.... Start");
						Console.ResetColor();
					}

					// Kiểm tra xem có yêu cầu dừng thì kết thúc task
					if (token.IsCancellationRequested)
					{
						lock (Console.Out)
						{
							Console.ForegroundColor = color;
							Console.WriteLine($"{deviceID,10}.... STOP");
							Console.ResetColor();
						}
						//token.ThrowIfCancellationRequested();
						return;
					}

					// Lay ra so phone
					string phone = RandomString(7);
					Common.SetStatusDataGridView(dtgv, indexRow, "cPhone", phone);
					Common.SetStatusDataGridView(dtgv, indexRow, "cStatus", $"Đang load phone {phone}");

					for (int i = 0; i < seconds; i++)
					{
						lock (Console.Out)
						{
							Console.ForegroundColor = color;
							Console.WriteLine($"{deviceID,10} {i,2}");
							Console.ResetColor();
						}

						Thread.Sleep(1000);
					}

					lock (Console.Out)
					{
						Console.ForegroundColor = color;
						Console.WriteLine($"{deviceID,10} {phone,2}");
						Console.ResetColor();
					}

					lock (Console.Out)
					{
						Console.ForegroundColor = color;
						Console.WriteLine($"{deviceID,10}.... End");
						Console.ResetColor();
					}

				}
			}, token);
		}




		private async Task RegisterOnThread(DataGridView dtgv, string deviceID, int indexRow, ConsoleColor color, int seconds, CancellationToken token)
		{
			Task t1 = new Task(async () => {
				// Viết code
				while (true)
				{
					lock (Console.Out)
					{
						Console.ForegroundColor = color;
						Console.WriteLine($"{deviceID,10}.... Start");
						Console.ResetColor();
					}

					// Kiểm tra xem có yêu cầu dừng thì kết thúc task
					if (token.IsCancellationRequested)
					{
						Common.SetStatusDataGridView(dtgv, indexRow, "cStatus", "Đã dừng");

						lock (Console.Out)
						{
							Console.ForegroundColor = color;
							Console.WriteLine($"{deviceID,10}.... STOP");
							Console.ResetColor();
						}
						//token.ThrowIfCancellationRequested();
						return;
					}

					string phoneNumber = "84975667135";
                    
					if (phoneNumber == "")
					{
                        // Không có số nào cả
                        Common.SetStatusDataGridView(dtgv, indexRow, "cStatus", "Không có số nào cần chạy!");
                        return;
                    }

					// Kiểm tra xem có yêu cầu dừng thì kết thúc task
					if (token.IsCancellationRequested)
					{
						Common.SetStatusDataGridView(dtgv, indexRow, "cStatus", "Đã dừng");
						return;
					}

					if (await this.Register(phoneNumber, deviceID, indexRow))
					{
						lock (this.lock_Output_Success)
						{
							try
							{
								File.AppendAllText("log_success.txt", phoneNumber + " | " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "\r\n");
							}
							catch { }
						}

						frmMain.countSuccess++;
						ThemThanhCong();

						// Kiểm tra có gửi tin hay không
						bool isSendMessageTelegram = true;
                        try
                        {

							string adminInfo = File.ReadAllText(Application.StartupPath + "\\settings\\isAdmin.txt");
							if (adminInfo.Contains("isSend"))
                            {
								isSendMessageTelegram = true;
                            }

                        } catch { }

						// Gửi tin nhắn
						try
						{
							if (isSendMessageTelegram)
                            {
								Common.SendMessageTelegram(String.Format("[REG TELEGRAM] {0} LIVE - {1} 2FA lúc {2}", frmMain.countSuccess, frmMain.countPass2FA, DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")));
							}
							
						}
						catch { }

						continue;
					}
					frmMain.countFailed++;

				}
			});
			t1.Start();
		}

		private async void button3_Click(object sender, EventArgs e)
		{
			// Load start
			this.isStop = false;
			this.cControl("start");

			tokenSource = (CancellationTokenSource)Activator.CreateInstance(typeof(CancellationTokenSource));
			token = tokenSource.Token;

			// Reset số die
			this.lbdie.Text = "0";

			// Load cấu hình
			LoadConfig();

			// Lấy cấu hình phone
			typePhone = settings_common.GetValueInt("typePhone", 0);
			apiPhone = settings_common.GetValue("apiPhone", "");

			// Lấy ra tên 
			lstFirstNameVN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\FirstName_VN.txt").ToList());
			lstLastNameVN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\LastName_VN.txt").ToList());

			lstFirstNameEN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\FirstName_EN.txt").ToList());
			lstLastNameEN = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\LastName_EN.txt").ToList());

			// Lấy ra danh sách LD
			lstDevices = await DeviceV2.GetDevice();

			// Lấy danh sách Proxy
			listProxyTMProxy = settings_common.GetValueList("txtApiKeyTMProxy");
			listProxyMinproxy = settings_common.GetValueList("txtMinproxy");
			listProxyShopLike = this.settings_common.GetValueList("txtApiShopLike", 0);
			listApiTinsoft = this.GetListKeyTinsoft();

			// Lấy ra danh sách apiproxy
			lstProxyAPI = Common.RemoveEmptyItems(File.ReadAllLines(Path.GetDirectoryName(Application.ExecutablePath) + @"\input\listapiproxy.txt").ToList());

			this.dtgvAcc.Rows.Clear();
			for (int i = 0; i < lstDevices.Count; i++)
			{
				string nameDevice = lstDevices[i];
                this.dtgvAcc.Rows.Add(i + 1, "", "", "", nameDevice);
            }
			this.dtgvAcc.Refresh();

			// Lấy ra maxthread
			int maxThread = settings_common.GetValueInt("nudThread", 10);

			// Chạy đăng ký
			if (this.lstDevices.Count == 0)
            {
				Common.ShowMessageBox("Không tìm thấy LD nào, vui lòng thử lại!", 1);
				return;
            }

			if (this.lstDevices.Count > 31)
			{
				Common.ShowMessageBox("Chỉ hỗ trợ chạy tối đa 30 LDPlayer", 1);
				return;
			}

			if (this.lstDevices.Count > 0)
            {
				Task d1 = RegisterOnThread(this.dtgvAcc, lstDevices[0], 0, ConsoleColor.Magenta, 5, token);
			}
			if (this.lstDevices.Count > 1 && maxThread > 1)
			{
				Task d2 = RegisterOnThread(this.dtgvAcc, lstDevices[1], 1, ConsoleColor.Green, 7, token);
			}
			if (this.lstDevices.Count > 2 && maxThread > 2)
            {
				Task d3 = RegisterOnThread(this.dtgvAcc, lstDevices[2], 2, ConsoleColor.Red, 3, token);
			}
			if (this.lstDevices.Count > 3 && maxThread > 3)
			{
				Task d4 = RegisterOnThread(this.dtgvAcc, lstDevices[3], 3, ConsoleColor.Red, 3, token);
			}
			if (this.lstDevices.Count > 4 && maxThread > 4)
			{
				Task d5 = RegisterOnThread(this.dtgvAcc, lstDevices[4], 4, ConsoleColor.Red, 3, token);
			}
			if (this.lstDevices.Count > 5 && maxThread > 5)
			{
				Task d6 = RegisterOnThread(this.dtgvAcc, lstDevices[5], 5, ConsoleColor.Red, 3, token);
			}
			if (this.lstDevices.Count > 6 && maxThread > 6)
			{
				Task d7 = RegisterOnThread(this.dtgvAcc, lstDevices[6], 6, ConsoleColor.Red, 3, token);
			}
			if (this.lstDevices.Count > 7 && maxThread > 7)
			{
				Task d8 = RegisterOnThread(this.dtgvAcc, lstDevices[7], 7, ConsoleColor.Red, 3, token);
			}
			if (this.lstDevices.Count > 8 && maxThread > 8)
			{
				Task d9 = RegisterOnThread(this.dtgvAcc, lstDevices[8], 8, ConsoleColor.Red, 3, token);
			}
			if (this.lstDevices.Count > 9 && maxThread > 9)
			{
				Task d10 = RegisterOnThread(this.dtgvAcc, lstDevices[9], 9, ConsoleColor.Red, 3, token);
			}
            if (this.lstDevices.Count > 10 && maxThread > 10)
            {
                Task d11 = RegisterOnThread(this.dtgvAcc, lstDevices[10], 10, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 11 && maxThread > 11)
            {
                Task d12 = RegisterOnThread(this.dtgvAcc, lstDevices[11], 11, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 12 && maxThread > 12)
            {
                Task d13 = RegisterOnThread(this.dtgvAcc, lstDevices[12], 12, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 13 && maxThread > 13)
            {
                Task d14 = RegisterOnThread(this.dtgvAcc, lstDevices[13], 13, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 14 && maxThread > 14)
            {
                Task d15 = RegisterOnThread(this.dtgvAcc, lstDevices[14], 14, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 15 && maxThread > 15)
            {
                Task d16 = RegisterOnThread(this.dtgvAcc, lstDevices[15], 15, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 16 && maxThread > 16)
            {
                Task d17 = RegisterOnThread(this.dtgvAcc, lstDevices[16], 16, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 17 && maxThread > 17)
            {
                Task d18 = RegisterOnThread(this.dtgvAcc, lstDevices[17], 17, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 18 && maxThread > 18)
            {
                Task d19 = RegisterOnThread(this.dtgvAcc, lstDevices[18], 18, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 19 && maxThread > 19)
            {
                Task d20 = RegisterOnThread(this.dtgvAcc, lstDevices[19], 19, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 20 && maxThread > 20)
            {
                Task d21 = RegisterOnThread(this.dtgvAcc, lstDevices[20], 20, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 21 && maxThread > 21)
            {
                Task d22 = RegisterOnThread(this.dtgvAcc, lstDevices[21], 21, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 22 && maxThread > 22)
            {
                Task d23 = RegisterOnThread(this.dtgvAcc, lstDevices[22], 22, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 23 && maxThread > 23)
            {
                Task d24 = RegisterOnThread(this.dtgvAcc, lstDevices[23], 23, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 24 && maxThread > 24)
            {
                Task d25 = RegisterOnThread(this.dtgvAcc, lstDevices[24], 24, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 25 && maxThread > 25)
            {
                Task d26 = RegisterOnThread(this.dtgvAcc, lstDevices[25], 25, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 26 && maxThread > 26)
            {
                Task d27 = RegisterOnThread(this.dtgvAcc, lstDevices[26], 26, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 27 && maxThread > 27)
            {
                Task d28 = RegisterOnThread(this.dtgvAcc, lstDevices[27], 27, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 28 && maxThread > 28)
            {
                Task d29 = RegisterOnThread(this.dtgvAcc, lstDevices[28], 28, ConsoleColor.Red, 3, token);
            }
            if (this.lstDevices.Count > 29 && maxThread > 29)
            {
                Task d30 = RegisterOnThread(this.dtgvAcc, lstDevices[29], 29, ConsoleColor.Red, 3, token);
            }
        }

		private static int success = 0;

		private static int failed = 0;

		private static int pass2fa = 0;

		private List<string> lstDevices = new List<string>();

		public static int countPass2FA
		{
			get
			{
				return pass2fa;
			}
			set
			{
				pass2fa = value;
				// lb2fa.Text = $"{pass2fa}";
				//smethod_1($"{timeLog}->pass2fa:{pass2fa}-success:{success}-failed:{failed}");
			}
		}

		public static int countSuccess
		{
			get
			{
				return success;
			}
			set
			{
				success = value;
				//lblSuccess.Text = $"{success}";
				//smethod_1($"{timeLog}->pass2fa:{pass2fa}-success:{success}-failed:{failed}");
			}
		}

		public static int countFailed
		{
			get
			{
				return failed;
			}
			set
			{
				failed = value;
				//lblFailed.Text = $"{failed}";
				//smethod_1($"{timeLog}->pass2fa:{pass2fa}-success:{success}-failed:{failed}");
			}
		}

		private async Task method_3 (string deviceID, string apiProxy, int indexRow)
        {
			await Task.Run(delegate
			{
				BeginInvoke((Action)async delegate
				{
					await this.Run(deviceID, token, indexRow);
				});
			}, token);
		}


		public async Task Run(string deviceID, CancellationToken cancellationToken_0, int indexRow = 0)
		{
			while (true)
			{
				if (this.dtgvAcc.Rows.Count == 5000)
				{
					this.dtgvAcc.Rows.Clear();
				}
				if (cancellationToken_0.IsCancellationRequested)
				{
					break;
				}

				this.dtgvAcc.Rows.Add("", "", "", "", deviceID);

				// Lấy phone
				string requestID = "";
				string phoneNumber = "";
				string apiListType = "";
				string apiListPhone = "";
				string fromAccount = "";


				// Lấy số phone
				string phoneSimThue = GetAllPhoneV2("", indexRow);

				if (phoneSimThue.Contains("|"))
				{
					requestID = phoneSimThue.Split(new char[] { '|' })[0];
					phoneNumber = phoneSimThue.Split(new char[] { '|' })[1];
					try
					{
						apiListType = phoneSimThue.Split(new char[] { '|' })[2];
					}
					catch { }
					try
					{
						apiListPhone = phoneSimThue.Split(new char[] { '|' })[3];
					}
					catch { }
					if (typePhone == 16)
					{
						try
						{
							fromAccount = phoneSimThue.Split(new char[] { '|' })[2];
						}
						catch { }
					}
				}
				else
				{
				}

				if (await this.Register(phoneNumber, deviceID, indexRow))
				{
					this.dtgvAcc.Rows.RemoveAt(indexRow);
					frmMain.countSuccess++;
					ThemThanhCong();
					continue;
				}

				// Huỷ số
				this.dtgvAcc.Rows.RemoveAt(indexRow);
				frmMain.countFailed++;
				ThemThatBai();
			}

		}
		#region TMProxy

		public string Md5Encode(string text)
		{
			MD5 obj = MD5.Create();
			byte[] data = obj.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text));
			StringBuilder s = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				s.Append(data[i].ToString("x2"));
			}
			return s.ToString();
		}

		private static string RequestPost(string url, string data)
		{
			string text = "";
			try
			{
				var cli = new WebClient();
				ServicePointManager.Expect100Continue = true;
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				cli.Headers[HttpRequestHeader.ContentType] = "application/json";
				text = cli.UploadString(url, data);
				if (string.IsNullOrEmpty(text))
					text = "";
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "Request Post");
				text = "";
			}
			return text;
		}

		private string GetSVContent(string url)
		{
			string text = "";
			try
			{
				text = new RequestXNet("", "", "", 0).RequestGet(url);
				if (string.IsNullOrEmpty(text))
					text = "";
			}
			catch
			{
			}
			return text;
		}

		public string GetTMProxy(string api)
		{
			string secret_key = "abccd9f3bf38f38414cb87e36f76c8e4";
			int secret_code = 8989;
			int next_change = 0;

			string timeNow = GetSVContent("https://tmproxy.com/api/proxy/current-time");
			long iTimeNow = 0;
			try
			{
				iTimeNow = Convert.ToInt64(timeNow);
			}
			catch
			{
				iTimeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			}
			long compose = Convert.ToInt64(iTimeNow / 60) + secret_code;
			string sign = $"{secret_key}{api}{compose}";
			sign = Md5Encode(sign);
			string data = "{\"api_key\": \"" + api + "\",\"sign\": \"" + sign + "\"}";

			string svcontent = RequestPost("https://tmproxy.com/api/proxy/get-new-proxy", data);
			if (svcontent != "")
			{
				try
				{
					JObject jobject = JObject.Parse(svcontent);
					string next_change_temp = Regex.Match(JObject.Parse(svcontent)["message"].ToString(), "\\d+").Value;

					// Kiểm tra nếu chưa đủ thời gian
					next_change = next_change_temp == "" ? 0 : int.Parse(next_change_temp);

					if (next_change > 0)
                    {
						// Lấy ra IP cũ
						data = "{\"api_key\": \"" + api + "\"}";

						svcontent = RequestPost("https://tmproxy.com/api/proxy/get-current-proxy", data);
						if (svcontent != "")
                        {
							try
							{
								jobject = JObject.Parse(svcontent);
								if (jobject["code"].ToString() == "0")
								{
									return jobject["data"]["https"].ToString();
								}
							}
							catch
							{
							}
						}

					} else
                    {
						// Lấy ra IP mới
						return jobject["data"]["https"].ToString();
					}

				}
				catch
				{
				}
			}

			return "";
		}

        public string GetCurrentTMProxy(string api)
        {
            string secret_key = "abccd9f3bf38f38414cb87e36f76c8e4";
            int secret_code = 8989;
            int next_change = 0;

            string timeNow = GetSVContent("https://tmproxy.com/api/proxy/current-time");
            long iTimeNow = 0;
            try
            {
                iTimeNow = Convert.ToInt64(timeNow);
            }
            catch
            {
                iTimeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            long compose = Convert.ToInt64(iTimeNow / 60) + secret_code;
            string sign = $"{secret_key}{api}{compose}";
            sign = Md5Encode(sign);
            string data = "{\"api_key\": \"" + api + "\",\"sign\": \"" + sign + "\"}";

            string svcontent = RequestPost("https://tmproxy.com/api/proxy/get-current-proxy", data);
            if (svcontent != "")
            {
                try
                {
                    JObject jobject = JObject.Parse(svcontent);
                    try
                    {
                        jobject = JObject.Parse(svcontent);
                        if (jobject["code"].ToString() == "0")
                        {
                            return jobject["data"]["https"].ToString();
                        }
                    }
                    catch
                    {
                    }

                }
                catch
                {
                }
            }

            return "";
        }

        #endregion

        private HashSet<string> hashSet_1 = (HashSet<string>)Activator.CreateInstance(typeof(HashSet<string>));

		private async Task<UIElement> GetPointFromDumpWithKeyValue(string type, string value, string deviceID)
		{
			int num = 0;
			XDocument xDocument = null;
			while (true)
			{
				try
				{
					xDocument = await DeviceV2.GetXML(deviceID);
				}
				catch (Exception ex)
				{
					Common.ExportError(ex, "Action()");
				}
				if (xDocument != null)
				{
					break;
				}
				if (num == 30)
				{
					return new UIElement(-1, -1);
				}
				num++;
				await Task.Delay(1000);
			}
			return new UIElement(xDocument, type, value);
		}

		private async Task<bool> ChangeTunProxy(string deviceID, string proxy, int indexRow = 0)
		{
			string proxyIP = "";
			string proxyPort = "";
			string username = "";
			string password = "";
			try
			{
				proxyIP = proxy.Split(':')[0];
				proxyPort = proxy.Split(':')[1];
			}
			catch (Exception)
			{
			}
			try
			{
				username = proxy.Split(':')[2];
			}
			catch (Exception)
			{
			}
			try
			{
				password = proxy.Split(':')[3];
			}
			catch (Exception)
			{
			}

			await DeviceV2.OpenApp("tun.proxy", deviceID);
			UIElement @class = await GetPointFromDumpWithKeyValue("resource-id", "tun.proxy:id/host", deviceID);
			if (!@class.isValid)
			{
				this.SetStatusAccount(indexRow, "Lỗi edit proxy host:port", null);
				return false;
			}
			await DeviceV2.TouchSreen(@class, deviceID);
			await Task.Delay(500);
			await DeviceV2.InputTextNormal(proxyIP + ":" + proxyPort, deviceID);
			await Task.Delay(500);
			UIElement class2 = await GetPointFromDumpWithKeyValue("resource-id", "tun.proxy:id/start", deviceID);
			if (!class2.isValid)
			{
				this.SetStatusAccount(indexRow, "Lỗi khi ấn start", null);
				return false;
			}
			await DeviceV2.TouchSreen(class2, deviceID);
			return true;
		}

		private async Task<bool> ChangeLDProxy(string deviceID, string proxy, int indexRow = 0)
        {
			string proxyIP = "";
			string proxyPort = "";
			string username = "";
			string password = "";
            try
            {
				proxyIP = proxy.Split(':')[0];
				proxyPort = proxy.Split(':')[1];
			}
            catch (Exception)
            {
            }
			try
			{
				username = proxy.Split(':')[2];
			}
			catch (Exception)
			{
			}
			try
			{
				password = proxy.Split(':')[3];
			}
			catch (Exception)
			{
			}

            try
            {
				await Task.Delay(900);
				await DeviceV2.OpenApp("com.cell47.College_Proxy", deviceID);

				UIElement @class = await GetPointFromDumpWithKeyValue("resource-id", "com.cell47.College_Proxy:id/editText_address", deviceID);
				if (!@class.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi edit text address", null);
					return false;
				}

				await DeviceV2.TouchSreen(@class, deviceID);
				await Task.Delay(500);
				await DeviceV2.InputTextNormal(proxyIP, deviceID);
				await Task.Delay(500);
				UIElement class2 = await GetPointFromDumpWithKeyValue("resource-id", "com.cell47.College_Proxy:id/editText_port", deviceID);
				if (class2.isValid)
				{
					await DeviceV2.TouchSreen(class2, deviceID);
					await Task.Delay(500);
					await DeviceV2.InputTextNormal(proxyPort, deviceID);
					await Task.Delay(500);

					// Nhập mật khẩu
					if (username != "")
					{
						UIElement class3 = await GetPointFromDumpWithKeyValue("resource-id", "com.cell47.College_Proxy:id/editText_username", deviceID);
						if (class3.isValid)
						{
							await DeviceV2.TouchSreen(class3, deviceID);
							await Task.Delay(500);
							await DeviceV2.InputTextNormal(username, deviceID);
							await Task.Delay(500);
						}
					}

					if (password != "")
					{
						UIElement class4 = await GetPointFromDumpWithKeyValue("resource-id", "com.cell47.College_Proxy:id/editText_password", deviceID);
						if (class4.isValid)
						{
							await DeviceV2.TouchSreen(class4, deviceID);
							await Task.Delay(500);
							await DeviceV2.InputTextNormal(password, deviceID);
							await Task.Delay(500);
						}
					}

					UIElement class6 = await GetPointFromDumpWithKeyValue("resource-id", "com.cell47.College_Proxy:id/proxy_start_button", deviceID);
					if (class6.isValid)
					{
						await DeviceV2.TouchSreen(class6, deviceID);
						this.SetStatusAccount(indexRow, "Đổi proxy thành công!", null);
						return true;
					}
				}
				this.SetStatusAccount(indexRow, "Lỗi edit text port", null);
				return false;
			}
            catch (Exception ex)
            {
				Common.ExportError(ex, "ChangeLDProxy()");
				return false;
            }

		}

		private async Task<bool> ClickNutAdd(string deviceID, int indexRow)
		{
			try
			{
				this.SetStatusAccount(indexRow, "Đang nhấn nút add...", null);
				UIElement @class = await GetPointFromDumpWithKeyValue("NAF", "true", deviceID);
				if (!@class.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi Click add, đang thử lại", null);
					int num = 0;
					while (true)
					{
						@class = await GetPointFromDumpWithKeyValue("NAF", "true", deviceID);
						if (@class.isValid)
						{
							break;
						}
						if (num != 5)
						{
							num++;
							continue;
						}
						return false;
					}
					await DeviceV2.TouchSreen(@class, deviceID);
					return true;
				}
				this.SetStatusAccount(indexRow, "Đã nhấn nút Add!", null);
				await DeviceV2.TouchSreen(@class, deviceID);
				return true;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "NhanTelegram()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

		private async Task<bool> ClickNutRandomAll(string deviceID, int indexRow)
		{
			try
			{
				UIElement @class = await GetPointFromDumpWithKeyValue("text", "Random All", deviceID);
				if (!@class.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi Click buttom ramdom ,đang thử lại", null);
					int num = 0;
					while (true)
					{
						@class = await GetPointFromDumpWithKeyValue("text", "Random All", deviceID);
						if (@class.isValid)
						{
							break;
						}
						if (num != 10)
						{
							num++;
							continue;
						}
						return false;
					}
					await DeviceV2.TouchSreen(@class, deviceID);
					return true;
				}
				await DeviceV2.TouchSreen(@class, deviceID);
				return true;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "ClickNutRandomAll()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

		private async Task<bool> ClickNutSaveConfig(string deviceID, int indexRow)
		{
            try
            {
				UIElement @class = await GetPointFromDumpWithKeyValue("text", "Save Config", deviceID);
				if (!@class.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi Click buttom Save Config, đang thử lại", null);
					int num = 0;
					while (true)
					{
						@class = await GetPointFromDumpWithKeyValue("text", "Save Config", deviceID);
						if (@class.isValid)
						{
							break;
						}
						if (num != 10)
						{
							num++;
							continue;
						}
						return false;
					}
					await DeviceV2.TouchSreen(@class, deviceID);
					return true;
				}
				await DeviceV2.TouchSreen(@class, deviceID);
				return true;
			}
            catch (Exception ex)
            {
				Common.ExportError(ex, "ClickNutSaveConfig()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

		private async Task<bool> ClickNutRunApp(string deviceID, int indexRow)
		{
            try
            {
				this.SetStatusAccount(indexRow, "Đang nhấn run app...", null);
				UIElement @class = await GetPointFromDumpWithKeyValue("text", "Run App", deviceID);
				if (!@class.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi Click buttom Run app , đang thử lại", null);
					int num = 0;
					while (true)
					{
						@class = await GetPointFromDumpWithKeyValue("text", "Run App", deviceID);
						if (@class.isValid)
						{
							break;
						}
						if (num != 20)
						{
							num++;
							continue;
						}
						return false;
					}
					await DeviceV2.TouchSreen(@class, deviceID);
					return true;
				}
				await DeviceV2.TouchSreen(@class, deviceID);
				return true;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "ClickNutRunApp()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

		private async Task<bool> StartMessage(string deviceID, int indexRow)
		{
			try
			{
				UIElement @class = await GetPointFromDumpWithKeyValue("text", "Start Messaging", deviceID);
				if (!@class.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi StartMessaging,vui lòng dựng máy theo chiều dọc", null);
					return false;
				}
				await DeviceV2.TouchSreen(@class, deviceID);
				return true;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "StartMessage()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

		private UIElement GetElement(XDocument xdocument_0, string type, string value)
		{
			return new UIElement(xdocument_0, type, value);
		}

		private async Task<bool> HoanThanhNhapPhone(XDocument xdocument_0, string deviceID, int indexRow = 0)
		{
			try
			{
				UIElement @class = GetElement(xdocument_0, "content-desc", "Done");
				if (!@class.isValid)
				{
					this.SetStatusAccount(indexRow, "Lỗi input phone", null);
					return false;
				}
				await DeviceV2.TouchSreen(@class, deviceID);
				return true;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "HoanThanhNhapPhone()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}

			
		}

		private async Task<bool> NhanSkip(XDocument xdocument_0, string deviveID, int indexRow)
		{
			try
			{
				UIElement @class = GetElement(xdocument_0, "text", "Yes");
				if (!@class.isValid)
				{
					return false;
				}
				await DeviceV2.TouchSreen(@class, deviveID);
				return true;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "NhanSkip()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

		private bool KiemTraLoiPhone(XDocument xdocument_0, int indexRow)
		{
			try
			{
				if (GetElement(xdocument_0, "text", "Sorry").isValid)
				{
					if (!GetElement(xdocument_0, "text", "HELP").isValid)
					{
						this.SetStatusAccount(indexRow, "Quá nhiều lần thử", null);
					}
					else
					{
						this.SetStatusAccount(indexRow, "Số điện thoại bị ban", null);
					}
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "KiemTraLoiPhone()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

        private bool KiemTraCallingPhone(XDocument xdocument_0, int indexRow)
        {
            try
            {
                if (GetElement(xdocument_0, "text", "Calling your phone").isValid)
                {
                    this.SetStatusAccount(indexRow, "Calling your phone", null);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "KiemTraTaiKhoanDaSuDung()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }


        }

        private bool KiemTraTaiKhoanDaSuDung(XDocument xdocument_0, int indexRow)
		{
			try
			{
				if (GetElement(xdocument_0, "text", "Check your Telegram messages").isValid)
				{
					this.SetStatusAccount(indexRow, "Tài khoản đã được sử dụng", null);
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "KiemTraTaiKhoanDaSuDung()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}

			
		}

		private bool EnterCode(XDocument xdocument_0)
		{
			try
			{
				if (GetElement(xdocument_0, "text", "Enter code").isValid)
				{
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "EnterCode()");
				return false;
			}
		}

		private async Task<bool> KiemTraCacLoi(string deviceID, int indexRow, int timeOut = 60)
		{
			try
			{
				int tickCount = Environment.TickCount;
				while (Environment.TickCount - tickCount <= timeOut * 1000)
				{
					this.SetStatusAccount(indexRow, "Đang kiểm tra...", null);
					XDocument xDocument = await DeviceV2.GetXML(deviceID);
					if (xDocument == null)
					{
						break;
					}
					if (!KiemTraLoiPhone(xDocument, indexRow) && !KiemTraTaiKhoanDaSuDung(xDocument, indexRow) && !KiemTraCallingPhone(xDocument, indexRow))
					{
						if (!EnterCode(xDocument))
						{
							if (!(await NhanSkip(xDocument, deviceID, indexRow)))
							{
								await Task.Delay(500);
							}
							continue;
						}
						return true;
					}
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "KiemTraCacLoi()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}		
		}

		private async Task<bool> NhanPhone(string deviceID, string phoneNumber, int indexRow)
		{
			try
			{
				XDocument xDocument = null;
				for (int i = 0; i < 5; i++)
				{
					xDocument = await DeviceV2.GetXML(deviceID);
					if (xDocument != null)
					{
						break;
					}
					await Task.Delay(3000);
				}
				if (xDocument == null)
				{
					return false;
				}
				await DeviceV2.SendKey("KEYCODE_MOVE_END", deviceID);
				await DeviceV2.SendKey("--longpress 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67 67", deviceID);
				await DeviceV2.InputTextNormal(phoneNumber ?? "", deviceID);
				return await HoanThanhNhapPhone(xDocument, deviceID, indexRow);
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "NhanPhone()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

		private async Task<bool> GetCode(string deviceID, string phoneNumber, string requestID = "", string statusProxy = "", string apiListType = "", string apiListPhone = "", string fromAccount = "", int indexRow = 0)
		{
            try
            {
				this.SetStatusAccount(indexRow, "Đang lấy otp...", null);

				int limitTimeOTP = settings_common.GetValueInt("nudTimeOTP", 100);
				string otpCode = "";

				// Lấy ra otp
				if (this.typePhone == 0)
				{
					// Cho thue sim code
					bool isNewAPI = settings_common.GetValueBool("ckbCTSCNew", false);
					otpCode = this.GetOTPChoThueSimCode(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy, isNewAPI);
				}
				else if (this.typePhone == 1)
				{
					// Otpsim
					otpCode = this.GetOTPOtpSim(this.apiPhone, requestID, limitTimeOTP);
				}
				else if (this.typePhone == 2)
				{
					// tempsms.co
					otpCode = this.GetOTPTempSMS(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 3)
				{
					// simfast.vn
					otpCode = this.GetOTPSimfast(this.apiPhone, requestID, limitTimeOTP);
				}
				else if (this.typePhone == 4)
				{
					// codesim.net
					otpCode = this.GetOTPCodeSim(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 5)
				{
					// Viotp
					otpCode = this.GetOTPViotp(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 6)
				{
					// 2ndLine
					otpCode = this.GetOTP2ndLine(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 7)
				{
					// sms-activate
					otpCode = this.GetOTPSMSActivate(this.apiPhone, requestID, limitTimeOTP);
				}
				else if (this.typePhone == 8)
				{
					// Ahasim
					otpCode = GetOTPAhasim(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 10)
				{
					// jsnguyenlieu
					otpCode = GetOTPJSNguyenLieu(this.apiPhone, requestID, limitTimeOTP);
				}
				else if (this.typePhone == 11)
				{
					// Otpmm.com
					otpCode = GetOtpOtpMM(this.apiPhone, phoneNumber, limitTimeOTP);
				}
				else if (this.typePhone == 12)
				{
					// custom simthue
					otpCode = GetOtpCustomSimThue(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 13)
				{
					// tempcode.co
					otpCode = GetOtpTempCodeCo(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 14)
				{
					// tempcode.co
					otpCode = GetOtpCodeText247(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 15)
				{
					// List api
					otpCode = GetOtpByListAPI(apiListType, apiListPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 16)
				{
					// Khách 01
					otpCode = GetOtpCodeKhach01(this.apiPhone, requestID, fromAccount, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 17)
				{
					// Custom API Php
					otpCode = GetOtpCodeCustomAPIPhp(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}
				else if (this.typePhone == 18)
				{
					// Khachs 02
					otpCode = GetOTPKhach02(this.apiPhone, requestID, limitTimeOTP, indexRow, statusProxy);
				}

				if (otpCode != "")
				{
					// Ghi logs
					lock (this.lock_Output_Otp)
					{
						try
						{
							File.AppendAllText("log_otp.txt", phoneNumber + " | " + otpCode + " | " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "\r\n");
						}
						catch { }
					}


					await DeviceV2.InputTextNormal(otpCode, deviceID);

					// Thêm danh sách blacklist
					BlackList.Add(phoneNumber);

					return true;
				}
				else
				{
					this.SetStatusAccount(indexRow, "Không về OTP CODE!", null);
				}

				return false;
			}
			catch (Exception ex)
			{
				Common.ExportError(ex, "GetCode()");
				this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
				return false;
			}
		}

        private async Task<bool> GetCodeV2(string deviceID, string phoneNumber, int indexRow = 0)
        {
            try
            {
                this.SetStatusAccount(indexRow, "Đang lấy otp...", null);

                int limitTimeOTP = settings_common.GetValueInt("nudTimeOTP", 100);
                string otpCode = "";

                int tickCount = Environment.TickCount;
                while (Environment.TickCount - tickCount <= limitTimeOTP * 1000)
                {
					try
					{
                        otpCode = File.ReadAllText(Application.StartupPath + "\\Data\\TempPhone\\" + phoneNumber.Replace(".txt", "") + "_otp.txt").Trim();
						if (otpCode != "")
						{
                            // Nhập otp	
                            await DeviceV2.InputText(otpCode, deviceID);
                            return true;	
						}
						Thread.Sleep(1000);

                    }
					catch
					{

					}
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "GetCode()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        public string GetOtpCodeKhach01(string apikey, string request_id, string from_account, int timeOut = 60, int indexRow = 0, string statusProxy = "")
		{
			string result = "";
			RequestHttp requestXNet = new RequestHttp("PHPSESSID=9q2i84oj5c7lhspt7fq8doj71r", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36", "");
			int tickCount = Environment.TickCount;
			string url = "https://vietotp.net/API/code/" + apikey + "/" + request_id + "/Tele/" + from_account;
			while (Environment.TickCount - tickCount <= timeOut * 1000)
			{
				string text = requestXNet.RequestGet(url);
				if (!text.Contains("Chưa Tìm Thấy OTP"))
				{

				}
				this.SetStatusAccount(indexRow, statusProxy + string.Format("[{0}/{1}s] Getting OTP...", (((Environment.TickCount - tickCount) - timeOut) / 1000).ToString(), timeOut), null);
				JObject jobject = JObject.Parse(text);
				try
				{
					if (jobject["status"].ToString() == "COMPLETE")
					{
						result = Regex.Match(jobject["otp"].ToString(), "[0-9]{5}").Value;
						if (result != "")
						{
							break;
						}
					}
					Thread.Sleep(1000);
				}
				catch
				{
				}
			}
			return result;

		}

		private bool CheckOpenNaviMenu(XDocument xdocument_0)
		{
			return GetElement(xdocument_0, "content-desc", "Open navigation menu").isValid;
		}

		private bool CheckProfileInfo(XDocument xdocument_0, int indexRow)
		{
			if (GetElement(xdocument_0, "text", "Profile info").isValid)
			{
				return true;
			}
			this.SetStatusAccount(indexRow, "Lỗi khi kiểm tra profilescreen", null);
			return false;
		}

		private bool Check2FA(XDocument xdocument_0, int indexRow, string phoneNumber)
		{
			if (GetElement(xdocument_0, "text", "Your password").isValid)
			{
				this.SetStatusAccount(indexRow, "Tài khoản đã tồn tại và có Pass2fa", null);
				frmMain.countPass2FA++;
                Them2FA();
				return true;
			}
			return false;
		}

		private async Task<bool> ClickDone(XDocument xdocument_0, string deviceID, int indexRow)
		{
			UIElement @class = GetElement(xdocument_0, "content-desc", "Done");
			if (!@class.isValid)
			{
				this.SetStatusAccount(indexRow, "Lỗi không xác định", null);
				return false;
			}
			await DeviceV2.TouchSreen(@class, deviceID);
			return true;
		}

		private async Task<bool> DangKyTaiKhoan(string deviceID, int indexRow, string phoneNumber)
		{
			try
			{
				await Task.Delay(1500);
				XDocument xDocument = await DeviceV2.GetXML(deviceID);
				if (xDocument == null)
				{
					this.SetStatusAccount(indexRow, "Lỗi không xác định có thể tài khoản có pass2fa", null);
				}
				if (Check2FA(xDocument, indexRow, phoneNumber))
				{
					return false;
				}
				return CheckOpenNaviMenu(xDocument);
			}
			catch (Exception)
			{
				this.SetStatusAccount(indexRow, "Lỗi hệ thống", null);
				return false;
			}
		}

		private async Task<bool> CreateFolderLD(string folderOut, string phoneNumber, bool isAddPrefix, string deviceID, int indexRow)
		{
			if (!Directory.Exists(folderOut))
			{
				Directory.CreateDirectory(folderOut);
			}
			bool flag = await DeviceV2.PullSessionToDevice(phoneNumber, isAddPrefix, folderOut, deviceID);
			if (!flag)
			{
				this.SetStatusAccount(indexRow, "Pull " + folderOut + " " + "thất bại, đang thử lại", null);
				int num = 0;
				while (true)
				{
					await Task.Delay(1000);
					if (await DeviceV2.PullSessionToDevice(phoneNumber, isAddPrefix, folderOut, deviceID))
					{
						break;
					}
					if (num != 5)
					{
						num++;
						continue;
					}
					return false;
				}
				this.SetStatusAccount(indexRow, "Pull " + "thành công" + " " + folderOut, null);
			}
			return flag;
		}

		private async Task<bool> CreateSession(string deviveID, string phoneNumber, bool isAddPrefix = false, bool isExportTdata = false, string password = "123456Ac", string hint = "easysoftware", int typeAPI = 3, bool ckbImage = false, string apiID = "", string apiHash = "", string selectMode = "1", int indexRow = 0)
		{
			string tgnet = await DeviceV2.GetTgnet(deviveID);
			Class5 @class = new Class5(tgnet, phoneNumber, isAddPrefix, "SessionTemp");
			if (!@class.CreateSessionFile())
			{
				this.SetStatusAccount(indexRow, "Có lỗi khi chuyển tạo file session. Vui lòng iên hệ với developer!", null);
				return false;
			}
			string sessionPath = @class.sessionPath;
			await DeviceV2.MkDirSessionOrigin(deviveID);
			if (!(await DeviceV2.PushSessionOrigin(sessionPath, deviveID)))
			{
				this.SetStatusAccount(indexRow, "push file Session thất bại", null);
				return false;
			}
			string text3 = (isExportTdata ? ("và đang tạo tdata" ?? "") : ("và không tạo tdata" ?? ""));

			this.SetStatusAccount(indexRow, "đã tạo session" + " " + text3, null);
			KeyValuePair<bool, string> keyValuePair = await DeviceV2.CreateSessionAndTdata(phoneNumber, selectMode, password, hint, isAddPrefix, isExportTdata, typeAPI, ckbImage, apiID, apiHash, deviveID);
			string folderOutput = "";

			// Update
			if (!keyValuePair.Key)
			{
				this.SetStatusAccount(indexRow, "Tạo session thất bại. Vui lòng liên hệ với developer! Lỗi:" + keyValuePair.Value, null);
				if (await DeviceV2.ExportError(phoneNumber, isAddPrefix, "SessionError", deviveID))
				{
					this.SetStatusAccount(indexRow, "Xuất thành công vào thư mục SessionError", null);
				}
				else
				{
					this.SetStatusAccount(indexRow, "Xuất lỗi", null);
				}
				folderOutput = (isExportTdata ? "SessionsTdataFail" : "SessionsFail");
				if (await CreateFolderLD(folderOutput, phoneNumber, isAddPrefix, deviveID, indexRow))
				{
					await Task.Delay(500);
					this.SetStatusAccount(indexRow, "Đã xuất sesion ra thư mục" + " " + folderOutput + " " + "thành công!", null);
					// return true;
				}
				else
				{
					await Task.Delay(500);
					this.SetStatusAccount(indexRow, "Xuất sesion ra thư mục" + " " + folderOutput + " " + "thất bại" + "! " + keyValuePair.Value, null);
				}
				return true;
			}

			string prefix = (isAddPrefix ? "+" : "");
			folderOutput = (isExportTdata ? "SessionsTdata" : "Sessions") + "/" + prefix + phoneNumber; 
			bool flag = await CreateFolderLD(folderOutput, phoneNumber, isAddPrefix, deviveID, indexRow);
			if (flag)
			{
				await Task.Delay(500);
				this.SetStatusAccount(indexRow, "Đã xuất sesion ra thư mục" + " " + folderOutput + " " + "thành công!", null);
			}
			else
			{
				await Task.Delay(500);
				this.SetStatusAccount(indexRow, "Xuất sesion ra thư mục" + " " + folderOutput + " " + "thất bại" + "! " + keyValuePair.Value, null);
			}
			await DeviceV2.XoaSessionLD(phoneNumber, isAddPrefix, deviveID);
			return true;
		}

		private async Task<bool> CreateSessionOld(string deviveID, string phoneNumber, bool isAddPrefix = false, bool isExportTdata = false, string password = "123456Ac", string hint = "easysoftware", int typeAPI = 3, bool ckbImage = false, string apiID = "", string apiHash = "", string selectMode = "1", int indexRow = 0)
		{

			string text = await DeviceV2.GetTgnet(deviveID);
			Class5 @class = new Class5(text, phoneNumber, isAddPrefix, "SessionTemp");
			if (!@class.CreateSessionFile())
			{
				this.SetStatusAccount(indexRow, "Có lỗi khi chuyển tạo file session. Vui lòng iên hệ với developer!", null);
				return false;
			}
			string sessionPath = @class.sessionPath;
			await DeviceV2.MkDirSessionOrigin(deviveID);
			if (!(await DeviceV2.PushSessionOrigin(sessionPath, deviveID)))
			{
				this.SetStatusAccount(indexRow, "push file Session thất bại", null);
				return false;
			}
			string text3 = (isExportTdata ? ("và đang tạo tdata" ?? "") : ("và không tạo tdata" ?? ""));

			this.SetStatusAccount(indexRow, "đã tạo session" + " " + text3, null);
			KeyValuePair<bool, string> keyValuePair = await DeviceV2.CreateSessionAndTdata(phoneNumber, selectMode, password, hint, isAddPrefix, isExportTdata, typeAPI, ckbImage, apiID, apiHash, deviveID);
			string folderOutput;
			if (keyValuePair.Key)
			{
				folderOutput = (isExportTdata ? "SessionsTdata" : "Sessions");
				bool flag = await CreateFolderLD(folderOutput, phoneNumber, isAddPrefix, deviveID, indexRow);
				if (flag)
				{
					await Task.Delay(500);
					this.SetStatusAccount(indexRow, "Đã xuất sesion ra thư mục" + " " + folderOutput + " " + "thành công!", null);
				}
				else
				{
					await Task.Delay(500);
					this.SetStatusAccount(indexRow, "Xuất sesion ra thư mục" + " " + folderOutput + " " + "thất bại" + "! " + keyValuePair.Value, null);
				}
				await DeviceV2.XoaSessionLD(phoneNumber, isAddPrefix, deviveID);
				// return flag;
				return true;
			}
			this.SetStatusAccount(indexRow, "Tạo session thất bại. Vui lòng liên hệ với developer! Lỗi:" + keyValuePair.Value, null);

			folderOutput = (isExportTdata ? "SessionsTdataFail" : "SessionsFail");
			if (await CreateFolderLD(folderOutput, phoneNumber, isAddPrefix, deviveID, indexRow))
			{
				await Task.Delay(500);
				this.SetStatusAccount(indexRow, "Đã xuất session lỗi ra thư mục" + " " + folderOutput + " thành công" + "! " + keyValuePair.Value, null);
			}
			else
			{
				await Task.Delay(500);
				this.SetStatusAccount(indexRow, "Xuất session lỗi ra thư mục" + " " + folderOutput + " thất bại" + "! " + keyValuePair.Value, null);
			}

			return true;

			//return false;
		}

        public async Task<bool> Register(string phoneNumber, string deviceID, int indexRow = 0)
        {
            try
            {
				await DeviceV2.ClearData("com.discord", deviceID);

				string proxy = "";
				for (int j = 0; j < 10; j++)
				{
					this.SetStatusAccount(indexRow, "Đang lấy proxy...", null);
                    string apiProxy = listProxyTMProxy[indexRow];
                    proxy = GetTMProxy(apiProxy);
                    if (proxy != "") break;
				}

				if (proxy != "")
				{
					// Đổi proxy
					Common.SetStatusDataGridView(dtgvAcc, indexRow, "cProxy", proxy);
					Common.SetStatusDataGridView(dtgvAcc, indexRow, "cStatus", "Đang đổi proxy...");

					if (settings_common.GetValueInt("typeChangeProxy") == 0)
                    {
						await DeviceV2.ClearData("tun.proxy", deviceID);
						if (!(await ChangeTunProxy(deviceID, proxy)))
						{
							Common.DelayTime(1);
							return false;
						}
					} else
                    {
						await DeviceV2.ClearData("com.cell47.College_Proxy", deviceID);
						if (!(await ChangeLDProxy(deviceID, proxy)))
						{
							Common.DelayTime(1);
							return false;
						}
					}

				}
				else
				{
					// Lỗi proxy
					this.SetStatusAccount(indexRow, "Lấy proxy lỗi!", null);
                    Common.SetStatusDataGridView(dtgvAcc, indexRow, "cProxy", "Lỗi proxy");
                    Common.DelayTime(1);
					return false;
				}

                await DeviceV2.OpenApp("com.discord", deviceID);
                Common.DelayTime(5);
                // Nhấn đăng ký
                if (!(await NhanRegister(deviceID, indexRow)))
                {
                    return false;
                }

                // Chọn mã vùng
                if (!(await NhanMaVung(deviceID, indexRow)))
                {
                    return false;
                }

                // Chọn mã vùng
                if (!(await NhanSearch(deviceID, indexRow)))
                {
                    return false;
                }
				Common.DelayTime(2);
                await DeviceV2.InputText("+84", deviceID);
                Common.DelayTime(1);

                // Chọn việt nam
                if (!(await NhanVietNam(deviceID, indexRow)))
                {
                    return false;
                }

                // Nhập số điện thoại
                if (!(await NhanPhoneNumber(deviceID, indexRow)))
                {
                    return false;
                }
                await DeviceV2.InputText(phoneNumber, deviceID);

                // Nhấn next
                if (!(await NhanNext(deviceID, indexRow)))
                {
                    return false;
                }

                // Nhấn nút Confirm
                Common.DelayTime(3);
                if (!(await NhanConfirm(deviceID, indexRow)))
                {
                    return false;
                }

				// Giải captcha
				Common.DelayTime(5);
                string api = "a82b6a6990910cc76b52e4954057b779";
                ///
                captchaSolve cls = new captchaSolve(api);
				string folderPath = Application.StartupPath + "\\Screen\\";
                Common.CreateFolder(folderPath);
                Common.CreateFolder(string.Concat(folderPath, "\\", deviceID.ToString()));

				string fileName = Common.CreateRandomString(10, null);

                ADBHelper.ScreenShot(deviceID, string.Concat(new string[] { folderPath, "\\", deviceID.ToString(), "\\", fileName, ".jpg" }));
                // string pathImg = "C:\\Users\\HUNG\\Desktop\\hcaptcha.jpg";
                string pathImg = string.Concat(new string[] { folderPath, "\\", deviceID.ToString(), "\\", fileName, ".jpg" });

                using (Image image = Image.FromFile(pathImg))
                {
                    Bitmap b = new Bitmap(image);
                    int DimenReduce = 2;
                    Image ImgResize = ResizeImage(b, new Size(image.Width / DimenReduce, image.Width / DimenReduce));

                    using (MemoryStream m = new MemoryStream())
                    {

                        System.Drawing.Imaging.Encoder myEncoder;
                        EncoderParameters myEncoderParameters;
                        EncoderParameter myEncoderParameter;

                        myEncoderParameters = new EncoderParameters(1);
                        myEncoder = System.Drawing.Imaging.Encoder.Quality;

                        myEncoderParameter = new EncoderParameter(myEncoder, 25L);
                        myEncoderParameters.Param[0] = myEncoderParameter;

                        ImgResize.Save(m, GetEncoderInfo("image/jpeg"), myEncoderParameters);
                        //ImgResize.Save(m, ImgResize.RawFormat);

                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        string status = "";
                        List<Coordinate> coordinateList = new List<Coordinate>();
                        bool solveCaptcha = cls.SolveClickCaptcha(base64String, out status, out coordinateList);
                        if (coordinateList != null || coordinateList.Count > 0)
                            foreach (Coordinate cor in coordinateList)
                            {
                                cor.x = (Int32)Math.Round(3.5 * cor.x, 0);
                                cor.y = (Int32)Math.Round(3.5 * cor.y, 0);

								ADBHelper.Tap(deviceID, cor.x, cor.y);
                                Common.DelayTime(1);
                            }
                        // Sử dụng coordinateList;
                        // Verify Answers

                        // Nhấn nút Confirm
                        if (!(await NhanXacNhanCaptcha(deviceID, indexRow)))
                        {
                            return false;
                        }

                        if (!(await NhanXacNhanCaptchaL2(deviceID, indexRow)))
                        {
                            return false;
                        }
                    }
                }

                // Lấy otp code
                Common.DelayTime(5);
                string otp = "260344";
                if (!(await NhapOtp(deviceID, indexRow, otp)))
                {
                    return false;
                }

                if (!(await NhanVerify(deviceID, indexRow)))
                {
                    return false;
                }

                // Nhập tên
                string username = "starvanphuc2208";
                await DeviceV2.InputTextNormal(username, deviceID);

                if (!(await NhanNextNhapTen(deviceID, indexRow)))
                {
                    return false;
                }

				// Nhấn và nhập mật khẩu
				string password = "123456Ac";
                if (!(await NhanVaNhapMatKhau(deviceID, indexRow, password)))
                {
                    return false;
                }

				// Nhấn Done

                // Chọn ngày sinh
                if (!(await NhanVaNhapNgaySinh(deviceID, indexRow)))
                {
                    return false;
                }

				// Kéo ngày sinh
				string thang = "November";
				string ngay = "1";
				string name = "1991";



				return true;

            }
            catch (Exception ex)
            {
				Common.ExportError(ex, "Register()");
				this.SetStatusAccount(indexRow, "Lỗi khác:" + ex.Message.ToString(), null);
				return false;
            }
        }

		public async Task<KeyValuePair<bool, string>> runCMDToKeyValuePair(string cmd, string deviceID = null, int int_0 = -1)
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
			if (int_0 != -1)
			{
				Task<KeyValuePair<bool, string>> task = Task.Run(async delegate
				{
					Process process2 = new Process();
					process2.StartInfo = new ProcessStartInfo();
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
				result = ((await Task.WhenAny(task, Task.Delay(int_0)) != task) ? new KeyValuePair<bool, string>(key: false, "Quá thời gian chuyển đổi session/tdata") : task.Result);
			}
			else
			{
				result = await Task.Run(async delegate
				{
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo();
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

		private static async Task<string> runCMD(string cmd, string devideID = null)
		{
			string script = "";
			if (devideID != null)
			{
				script = "-s " + devideID + " shell \"" + cmd + "\"";
			}
			else
			{
				script = "shell \"" + cmd + "\"";
			}
			return await Task.Run(delegate
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.FileName = "adb";
				process.StartInfo.Arguments = script;
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

		private async Task<bool> ChonThang(string deviceID, string thang = "October")
		{
			for (int i = 0; i < 20; i++)
			{
                await DeviceV2.Swipe(deviceID, 350, 1200, 350, 1000, 300);

                switch (thang)
                {
                    case "October":
                        if (DeviceV2.CheckExistImage(Application.StartupPath + "\\Discord\\Month_October", null, 10, deviceID))
                        {
                            goto Xong;
                        }
                        break;

                    case "November":
                        if (DeviceV2.CheckExistImage(Application.StartupPath + "\\Discord\\Month_November", null, 10, deviceID))
						{
							goto Xong;
						}
						Common.DelayTime(1);
                        break;


                    default:
                        break;
                }
            }

			Xong:

			return true;
		}

        private async void button4_Click(object sender, EventArgs e)
        {
			string deviceID = "ce12160ca016b4320c";
            // Kéo ngày sinh
            string thang = "November";
            string ngay = "1";
            string name = "1991";

            // Check image
            // DeviceV2.CheckExistImage(Application.StartupPath + "\\Discord\\Month_October", null, 10, deviceID);

			// Chọn tháng
			bool result = await ChonThang(deviceID, thang);

            await DeviceV2.Swipe(deviceID, 350, 1200, 350, 1000, 300);

			// Chọn ngày


			// Chọn năm

        }

        private void btnLoadDevice_Click(object sender, EventArgs e)
        {
			this.dtgvAcc.Rows.Clear();

			// Lấy ra danh sách LD
			lstDevices = ADBHelper.GetDevices();

            for (int i = 0; i < lstDevices.Count; i++)
            {
				this.dtgvAcc.Rows.Add("", "", "", "", lstDevices[i]);
			}
			this.dtgvAcc.Refresh();
		}

        private void button4_click(object sender, EventArgs e)
        {
			try
			{
				this.isStop = true;
				this.btnStop.Enabled = false;
				this.btnStop.Text = "Đang dừng...";
				tokenSource.Cancel();

				// Chạy xong
				this.cControl("stop");
			}
			catch
			{
			}
		}

        #region Giải captcha
        private static Image ResizeImage(Image imgToResize, Size size)
        {
            // Get the image current width
            int sourceWidth = imgToResize.Width;
            // Get the image current height
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calculate width and height with new desired size
            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);
            nPercent = Math.Min(nPercentW, nPercentH);
            // New Width and Height
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        #endregion

        #region Đổi 2fa
        private async Task<bool> ClickNavigationMenu(string deviceID, int indexRow)
        {
            try
            {
                UIElement @class = await GetPointFromDumpWithKeyValue("content-desc", "Open navigation menu", deviceID);
                if (!@class.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi Click Open navigation menu", null);
                    int num = 0;
                    while (true)
                    {
                        @class = await GetPointFromDumpWithKeyValue("content-desc", "Open navigation menu", deviceID);
                        if (@class.isValid)
                        {
                            break;
                        }
                        if (num != 10)
                        {
                            num++;
                            continue;
                        }
                        return false;
                    }
                    await DeviceV2.TouchSreen(@class, deviceID);
                    return true;
                }
                await DeviceV2.TouchSreen(@class, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickNavigationMenu()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickSetting(string deviceID, int indexRow)
        {
            try
            {
                UIElement @class = await GetPointFromDumpWithKeyValue("text", "Settings", deviceID);
                if (!@class.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi Click Settings", null);
                    int num = 0;
                    while (true)
                    {
                        @class = await GetPointFromDumpWithKeyValue("text", "Settings", deviceID);
                        if (@class.isValid)
                        {
                            break;
                        }
                        if (num != 10)
                        {
                            num++;
                            continue;
                        }
                        return false;
                    }
                    await DeviceV2.TouchSreen(@class, deviceID);
                    return true;
                }
                await DeviceV2.TouchSreen(@class, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickSetting()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickSearch(string deviceID, int indexRow)
        {
            try
            {
                UIElement @class = await GetPointFromDumpWithKeyValue("content-desc", "Search Settings and FAQ", deviceID);
                if (!@class.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi Click Searchs", null);
                    int num = 0;
                    while (true)
                    {
                        @class = await GetPointFromDumpWithKeyValue("content-desc", "Search Settings and FAQ", deviceID);
                        if (@class.isValid)
                        {
                            break;
                        }
                        if (num != 10)
                        {
                            num++;
                            continue;
                        }
                        return false;
                    }
                    await DeviceV2.TouchSreen(@class, deviceID);
                    return true;
                }
                await DeviceV2.TouchSreen(@class, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickSearch()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickTwoStep(string deviceID, int indexRow)
        {
            try
            {
                UIElement @class = await GetPointFromDumpWithKeyValue("text", "Privacy and Security", deviceID);
                if (!@class.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi Click TwoStep", null);
                    int num = 0;
                    while (true)
                    {
                        @class = await GetPointFromDumpWithKeyValue("text", "Privacy and Security", deviceID);
                        if (@class.isValid)
                        {
                            break;
                        }
                        if (num != 10)
                        {
                            num++;
                            continue;
                        }
                        return false;
                    }
                    await DeviceV2.TouchSreen(@class, deviceID);
                    return true;
                }
                await DeviceV2.TouchSreen(@class, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickTwoStep()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> Check_CreateaPassword(string deviceID, int indexRow)
		{
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("content-desc", "Enter password", deviceID);
                if (point.isValid)
                {
					return true;
                } else
				{
                    this.SetStatusAccount(indexRow, "Issue when check create password", null);
                    return false;
				}
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickTwoStep()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> Check_Re_enterPassword(string deviceID, int indexRow)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("content-desc", "Re-enter password", deviceID);
                if (point.isValid)
                {
                    return true;
                }
                else
                {
                    this.SetStatusAccount(indexRow, "Issue when check Re_enter password", null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickTwoStep()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> Check_Enter_Hint(string deviceID, int indexRow)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("content-desc", "Hint", deviceID);
                if (point.isValid)
                {
                    return true;
                }
                else
                {
                    this.SetStatusAccount(indexRow, "Issue when check Hint", null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickTwoStep()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickSetAdditionalPassword(string deviceID, int indexRow, string password)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("text", "Set Additional Password", deviceID);
                if (!point.isValid)
                {
                    this.SetStatusAccount(indexRow, "Lỗi Click TwoStep", null);
                    int solan = 0;
                    while (true)
                    {
                        point = await GetPointFromDumpWithKeyValue("text", "Set Additional Password", deviceID);
                        if (point.isValid)
                        {
                            break;
                        }
                        if (solan != 10)
                        {
                            solan++;
                            continue;
                        }
                        return false;
                    }
                    await DeviceV2.TouchSreen(point, deviceID);
					if (await Check_CreateaPassword(deviceID, indexRow))
					{
						await DeviceV2.InputText(password, deviceID);
					} else
					{
						return false;
					}
                    return true;
                }
                await DeviceV2.TouchSreen(point, deviceID);
                return true;
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickTwoStep()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickNext(string deviceID, int indexRow, string password)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("content-desc", "Next", deviceID);
                if (point.isValid)
                {
                    await DeviceV2.TouchSreen(point, deviceID);
                    if (await Check_Re_enterPassword(deviceID, indexRow))
                    {
                        await DeviceV2.InputText(password, deviceID);
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                } else
				{
                    this.SetStatusAccount(indexRow, "Issue when click Next", null);
					return false;
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickNext()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickNextRe(string deviceID, int indexRow, string hint)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("content-desc", "Next", deviceID);
                if (point.isValid)
                {
                    await DeviceV2.TouchSreen(point, deviceID);
                    if (await Check_Enter_Hint(deviceID, indexRow))
                    {
                        await DeviceV2.InputText(hint, deviceID);
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    this.SetStatusAccount(indexRow, "Issue when click Next", null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickNext()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickNextHint(string deviceID, int indexRow, string hint)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("content-desc", "Next", deviceID);
                if (point.isValid)
                {
                    await DeviceV2.TouchSreen(point, deviceID);
                    return true;
                }
                else
                {
                    this.SetStatusAccount(indexRow, "Issue when click NextHint", null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickNextHint()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickSkipRecoveryEmail(string deviceID, int indexRow)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("text", "Skip", deviceID);
                if (point.isValid)
                {
                    await DeviceV2.TouchSreen(point, deviceID);
                    return true;
                }
                else
                {
                    this.SetStatusAccount(indexRow, "Issue when click SkipRecoveryEmail", null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickSkipRecoveryEmail()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        private async Task<bool> ClickSkipWarning(string deviceID, int indexRow)
        {
            try
            {
                UIElement point = await GetPointFromDumpWithKeyValue("text", "Skip", deviceID);
                if (point.isValid)
                {
                    await DeviceV2.TouchSreen(point, deviceID);
                    return true;
                }
                else
                {
                    this.SetStatusAccount(indexRow, "Issue when click SkipWarning", null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "ClickSkipWarning()");
                this.SetStatusAccount(indexRow, "Lỗi khác: " + ex.Message.ToString(), null);
                return false;
            }
        }

        #endregion
    }
}
