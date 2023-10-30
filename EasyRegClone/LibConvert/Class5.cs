using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy
{
    public class Class5
    {
		private string string_0;

		private string phone;

		private string folder;

		private bool isAddPrefix;

		public string sessionPath = "";

		private Dictionary<uint, string> dictionary_0;

		public Class5(string string_4, string string_5, bool bool_1, string string_6)
		{
			object obj = Activator.CreateInstance(typeof(Dictionary<uint, string>));
			((Dictionary<uint, string>)obj).Add(1u, "149.154.175.53");
			((Dictionary<uint, string>)obj).Add(2u, "149.154.167.51");
			((Dictionary<uint, string>)obj).Add(3u, "149.154.175.100");
			((Dictionary<uint, string>)obj).Add(4u, "149.154.167.91");
			((Dictionary<uint, string>)obj).Add(5u, "91.108.56.130");
			dictionary_0 = (Dictionary<uint, string>)obj;
			string_0 = string_4;
			phone = string_5;
			folder = string_6;
			isAddPrefix = bool_1;
		}

		public bool CreateSessionFile()
		{
            try
            {
				Class8 @class = new Class8(string_0);
				KeyValuePair<uint, string> keyValuePair_0 = @class.method_0();
				if (keyValuePair_0.Value != null)
				{
					byte[] value = (from int_0 in Enumerable.Range(0, keyValuePair_0.Value.Length)
									where int_0 % 2 == 0
									select Convert.ToByte(keyValuePair_0.Value.Substring(int_0, 2), 16)).ToArray();
					string prefix = (isAddPrefix ? "+" : "");
					string text2 = folder + "\\" + prefix + phone; // SessionTemp\\+84886642929
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
					}
					sessionPath = text2 + "\\" + prefix + phone + ".session";
					File.Copy("RegHelper.dll", sessionPath, overwrite: true);
					SQLiteConnection sQLiteConnection = new SQLiteConnection("Data Source=" + sessionPath + ";Version=3;New=False;Compress=True;");
					SQLiteCommand sQLiteCommand = new SQLiteCommand("INSERT INTO sessions (dc_id, server_address,port,auth_key,takeout_id) VALUES (@dc_id, @server_address,@port,@auth_key,@takeout_id)", sQLiteConnection);
					sQLiteCommand.Parameters.Add(new SQLiteParameter("@dc_id", keyValuePair_0.Key));
					sQLiteCommand.Parameters.Add(new SQLiteParameter("@server_address", dictionary_0[keyValuePair_0.Key]));
					sQLiteCommand.Parameters.Add(new SQLiteParameter("@port", 443));
					sQLiteCommand.Parameters.Add(new SQLiteParameter("@auth_key", value));
					sQLiteCommand.Parameters.Add(new SQLiteParameter("@takeout_id", null));
					try
					{
						sQLiteConnection.Open();
						sQLiteCommand.ExecuteNonQuery();
						sQLiteConnection.Close();
						return true;
					}
					catch
					{
						return false;
					}
				}
				return false;
			}
            catch (Exception ex)
            {
				return false;
			}
			
		}
	}
}
