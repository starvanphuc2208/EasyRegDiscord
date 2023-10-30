using MCommon;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace easy.Helper
{
	internal class DatagridviewHelper
	{
		public DatagridviewHelper()
		{
		}

		public static string GetStatusDataGridView(DataGridView dgv, int row, int col)
		{
			string str = "";
			try
			{
				if (dgv.Rows[row].Cells[col].Value != null)
				{
					try
					{
						str = dgv.Rows[row].Cells[col].Value.ToString();
					}
					catch
					{
						dgv.Invoke(new MethodInvoker(() => str = dgv.Rows[row].Cells[col].Value.ToString()));
					}
				}
			}
			catch
			{
			}
			return str;
		}

		public static string GetStatusDataGridView(DataGridView dgv, int row, string colName)
		{
			string str = "";
			try
			{
				if (dgv.Rows[row].Cells[colName].Value != null)
				{
					try
					{
						str = dgv.Rows[row].Cells[colName].Value.ToString();
					}
					catch
					{
						dgv.Invoke(new MethodInvoker(() => str = dgv.Rows[row].Cells[colName].Value.ToString()));
					}
				}
			}
			catch
			{
			}
			return str;
		}

		public static void LoadDtgvAccFromDatatable(DataGridView dgv, DataTable tableAccount)
		{
			for (int i = 0; i < tableAccount.Rows.Count; i++)
			{
				DataRow item = tableAccount.Rows[i];
				dgv.Rows.Add(new object[] { false, dgv.RowCount + 1, item["id"], item["uid"], item["token"], item["cookie1"], item["email"], item["phone"], item["name"], item["follow"], item["friends"], item["groups"], item["birthday"], item["gender"], item["pass"], "", item["passmail"], item["backup"], item["fa2"], item["useragent"], item["proxy"], item["dateCreateAcc"], item["avatar"], item["profile"], item["nameFile"], item["interactEnd"], item["device"], item["info"], item["ghiChu"], easy.UpdateStatus.GetStatusById(item["id"].ToString()) });
			}
		}

		public static void SetStatusDataGridView(DataGridView dgv, int row, int col, object status)
		{
			try
			{
				try
				{
					dgv.Invoke(new MethodInvoker(() => dgv.Rows[row].Cells[col].Value = status));
				}
				catch
				{
					dgv.Rows[row].Cells[col].Value = status;
				}
			}
			catch
			{
			}
		}

		public static void SetStatusDataGridView(DataGridView dgv, int row, string colName, object status)
		{
			try
			{
				//if ((!easy.UpdateStatus.isSaveSettings ? false : colName == "cStatus"))
				//{
				//	string statusDataGridView = DatagridviewHelper.GetStatusDataGridView(dgv, row, "cId");
				//	easy.UpdateStatus.SetStatusById(statusDataGridView, status.ToString());
				//}
				try
				{
					dgv.Invoke(new MethodInvoker(() => dgv.Rows[row].Cells[colName].Value = status));
				}
				catch
				{
					dgv.Rows[row].Cells[colName].Value = status;
				}
			}
			catch
			{
			}
		}

		public static void SetStatusDataGridViewWithWait(DataGridView dgv, int row, string colName, int timeWait = 0, string status = "Đợi {time} giây...")
		{
			try
			{
				int tickCount = Environment.TickCount;
				while ((Environment.TickCount - tickCount) / 1000 - timeWait < 0)
				{
					dgv.Invoke(new MethodInvoker(() => dgv.Rows[row].Cells[colName].Value = status.Replace("{time}", (timeWait - (Environment.TickCount - tickCount) / 1000).ToString())));
					Common.DelayTime(0.5);
				}
			}
			catch
			{
			}
		}

		public static void SetStatusDataGridViewWithWait(DataGridView dgv, int row, string colName, int timeWait = 0, int timeStart = 0, string status = "Đợi {time} giây...")
		{
			try
			{
				int tickCount = Environment.TickCount;
				while ((Environment.TickCount - tickCount) / 1000 - timeWait < 0)
				{
					dgv.Invoke(new MethodInvoker(() => dgv.Rows[row].Cells[colName].Value = status.Replace("{time}", (timeStart - (Environment.TickCount - tickCount) / 1000).ToString())));
					Common.DelayTime(0.5);
				}
			}
			catch
			{
			}
		}
	}
}