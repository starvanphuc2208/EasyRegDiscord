using MCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy
{
	internal class UpdateStatus
	{
		private static Dictionary<string, string> dicIdStatus;

		public static bool isSaveSettings;

		static UpdateStatus()
		{
			easy.UpdateStatus.dicIdStatus = new Dictionary<string, string>();
			easy.UpdateStatus.isSaveSettings = false;
		}

		public UpdateStatus()
		{
		}

		public static string GetStatusById(string id)
		{
			string str;
			if (easy.UpdateStatus.isSaveSettings)
			{
				string item = "";
				if (easy.UpdateStatus.dicIdStatus.ContainsKey(id))
				{
					item = easy.UpdateStatus.dicIdStatus[id];
				}
				str = item;
			}
			else
			{
				str = "";
			}
			return str;
		}

		public static void GetValueFromDatabase()
		{
			if (easy.UpdateStatus.isSaveSettings)
			{
				easy.UpdateStatus.dicIdStatus = CommonSQL.GetIdStatus().AsEnumerable().ToDictionary<DataRow, string, string>((DataRow row) => row[0].ToString(), (DataRow row) => row[1].ToString());
			}
		}

		public static void SetStatusById(string id, string status)
		{
			if (easy.UpdateStatus.isSaveSettings)
			{
				if (!easy.UpdateStatus.dicIdStatus.ContainsKey(id))
				{
					easy.UpdateStatus.dicIdStatus.Add(id, status);
				}
				else
				{
					easy.UpdateStatus.dicIdStatus[id] = status;
				}
			}
		}

		public static void SetValueFromDatabase()
		{
			if (easy.UpdateStatus.isSaveSettings)
			{
				List<string> list = (
					from x in easy.UpdateStatus.dicIdStatus
					where x.Value.Trim() != ""
					select string.Concat(x.Key, "|", x.Value)).ToList<string>();
				CommonSQL.UpdateMultiField("status", list, "accounts");
			}
		}
	}
}
