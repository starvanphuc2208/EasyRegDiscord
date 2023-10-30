using maxcare;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace MCommon
{
	internal class CommonSQL
	{
		public CommonSQL()
		{
		}

		public static bool AddColumnsIntoTable(string table, string columnName, int typeColumnData)
		{
			bool flag = false;
			try
			{
				Connector instance = Connector.Instance;
				string[] strArrays = new string[] { "ALTER TABLE ", table, " ADD COLUMN '", columnName, "' ", null, null };
				strArrays[5] = (typeColumnData == 0 ? "INT" : "TEXT");
				strArrays[6] = ";";
				if (instance.ExecuteNonQuery(string.Concat(strArrays)) > 0)
				{
					flag = true;
				}
			}
			catch
			{
			}
			return flag;
		}

		public static bool CheckColumnIsExistInTable(string table, string column)
		{
			return Connector.Instance.ExecuteScalar(string.Concat(new string[] { "SELECT COUNT(*) AS count FROM pragma_table_info('", table, "') WHERE name='", column, "'" })) > 0;
		}

		public static bool CheckExistTable(string table)
		{
			return Connector.Instance.ExecuteScalar(string.Concat("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='", table, "';")) > 0;
		}

		public static bool CheckExitsFile(string name)
		{
			return Connector.Instance.ExecuteScalar(string.Concat("SELECT COUNT(*) FROM files WHERE name='", name, "' AND active=1;")) > 0;
		}

		public static List<string> ConvertToSqlInsertAccount(List<string> lstSqlStatement)
		{
			List<string> strs = new List<string>();
			try
			{
				int num = 100;
				int num1 = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((double)lstSqlStatement.Count * 1 / 100f)));
				string str = "";
				for (int i = 0; i < num1; i++)
				{
					str = string.Concat("INSERT INTO accounts(uid, pass,token,cookie1,email,name,friends,groups,birthday,gender,info,fa2,idfile,passmail,useragent,proxy,dateImport,active, device) VALUES ", string.Join(",", lstSqlStatement.GetRange(num * i, (num * i + num <= lstSqlStatement.Count ? num : lstSqlStatement.Count % num))));
					strs.Add(str);
				}
			}
			catch
			{
			}
			return strs;
		}

		public static string ConvertToSqlInsertAccount(string uid, string pass, string token, string cookie, string email, string name, string friends, string groups, string birthday, string gender, string info, string fa2, string idFile, string passMail, string useragent, string proxy, string ldIndex)
		{
			string str = "";
			try
			{
				str = "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}',1,'{17}')";
				object[] objArray = new object[] { uid, pass.Replace("'", "''"), token, cookie, email, name.Replace("'", "''"), friends, groups, birthday, gender, info, fa2, idFile, passMail, useragent, proxy, null, null };
				objArray[16] = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
				objArray[17] = ldIndex;
				str = string.Format(str, objArray);
			}
			catch
			{
			}
			return str;
		}

		public static bool DeleteAccountByIdFile(string idFile)
		{
			bool flag = true;
			try
			{
				flag = (Connector.Instance.ExecuteNonQuery(string.Concat("UPDATE accounts SET active=0, dateDelete='", DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), "' where idfile=", idFile)) <= 0 ? false : true);
			}
			catch
			{
			}
			return flag;
		}

		public static bool DeleteAccountToDatabase(string id)
		{
			bool flag;
			try
			{
				Connector instance = Connector.Instance;
				DateTime now = DateTime.Now;
				flag = instance.ExecuteNonQuery(string.Concat("UPDATE accounts SET active=0, dateDelete='", now.ToString("HH:mm:ss dd/MM/yyyy"), "' where id=", id)) > 0;
				return flag;
			}
			catch
			{
			}
			flag = false;
			return flag;
		}

		public static bool DeleteAccountToDatabase(List<string> lstId, bool isReallyDelete = false)
		{
			DateTime now;
			if (isReallyDelete)
			{
				List<string> strs = new List<string>();
				DataTable accFromId = CommonSQL.GetAccFromId(lstId);
				for (int i = 0; i < accFromId.Rows.Count; i++)
				{
					string str = "";
					for (int j = 0; j < accFromId.Columns.Count; j++)
					{
						str = string.Concat(str, accFromId.Rows[i][j].ToString(), "|");
					}
					str = str.Substring(0, str.Length - 1);
					strs.Add(str);
				}
				now = DateTime.Now;
				File.AppendAllText("bin.txt", string.Concat("======", now.ToString("HH:mm:ss dd/MM/yyyy"), "======\r\n"));
				File.AppendAllLines("bin.txt", strs);
			}
			bool flag = true;
			try
			{
				int num = 100;
				int num1 = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((double)lstId.Count * 1 / 100f)));
				List<string> strs1 = new List<string>();
				string str1 = "";
				for (int k = 0; k < num1; k++)
				{
					if (!isReallyDelete)
					{
						string[] strArrays = new string[] { "UPDATE accounts SET active=0, dateDelete='", null, null, null, null };
						now = DateTime.Now;
						strArrays[1] = now.ToString("HH:mm:ss dd/MM/yyyy");
						strArrays[2] = "' where id IN (";
						strArrays[3] = string.Join(",", lstId.GetRange(num * k, (num * k + num <= lstId.Count ? num : lstId.Count % num)));
						strArrays[4] = ")";
						str1 = string.Concat(strArrays);
					}
					else
					{
						str1 = string.Concat("delete from accounts where id IN (", string.Join(",", lstId.GetRange(num * k, (num * k + num <= lstId.Count ? num : lstId.Count % num))), ")");
					}
					strs1.Add(str1);
				}
				for (int l = 0; l < strs1.Count; l++)
				{
					flag = Connector.Instance.ExecuteNonQuery(strs1[l]) > 0;
				}
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, "DeleteAccountToDatabase");
			}
			return flag;
		}

		public static bool DeleteFileToDatabase(string idFile)
		{
			bool flag = false;
			try
			{
				if (Connector.Instance.ExecuteScalar(string.Concat("SELECT COUNT(idfile) FROM accounts WHERE idfile=", idFile)) == 0)
				{
					flag = Connector.Instance.ExecuteNonQuery(string.Concat("delete from files where id=", idFile)) > 0;
				}
				else if (Connector.Instance.ExecuteNonQuery(string.Concat("UPDATE files SET active=0 where id=", idFile)) > 0)
				{
					flag = CommonSQL.DeleteAccountByIdFile(idFile);
				}
			}
			catch
			{
			}
			return flag;
		}

		public static bool DeleteFileToDatabaseIfEmptyAccount()
		{
			bool flag = false;
			try
			{
				flag = Connector.Instance.ExecuteNonQuery("delete from files where id NOT IN (SELECT DISTINCT idfile FROM accounts)") > 0;
			}
			catch
			{
			}
			return flag;
		}

		public static DataTable GetAccFromFile(List<string> lstIdFile = null, string info = "", bool isGetActive = true)
		{
			DataTable dataTable = new DataTable();
			try
			{
				string str = "WHERE ";
				string str1 = (lstIdFile == null || lstIdFile.Count <= 0 ? "" : string.Concat("t1.idFile IN (", string.Join(",", lstIdFile), ")"));
				if (str1 != "")
				{
					str = string.Concat(str, str1, " AND ");
				}
				string str2 = (info != "" ? string.Concat("t1.info = '", info, "'") : "");
				if (str2 != "")
				{
					str = string.Concat(str, str2, " AND ");
				}
				string str3 = string.Format("t1.active = '{0}'", (isGetActive ? 1 : 0));
				str = string.Concat(str, str3);
				string str4 = string.Concat("SELECT t1.*, t2.name AS nameFile FROM accounts t1 JOIN files t2 ON t1.idfile=t2.id ", str, " ORDER BY t1.idfile");
				dataTable = Connector.Instance.ExecuteQuery(str4);
			}
			catch
			{
			}
			return dataTable;
		}

		public static DataTable GetAccFromId(List<string> lstId)
		{
			DataTable dataTable = new DataTable();
			try
			{
				int num = 100;
				int num1 = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((double)lstId.Count * 1 / 100f)));
				List<string> strs = new List<string>();
				string str = "";
				for (int i = 0; i < num1; i++)
				{
					str = string.Concat("SELECT uid, pass, token, cookie1,email, passmail, fa2 FROM accounts WHERE id IN ('", string.Join("','", lstId.GetRange(num * i, (num * i + num <= lstId.Count ? num : lstId.Count % num))), "')");
					strs.Add(str);
				}
				dataTable = Connector.Instance.ExecuteQuery(strs);
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, "GetAccFromFile");
			}
			return dataTable;
		}

		public static DataTable GetAccFromUid(List<string> lstUid)
		{
			DataTable dataTable = new DataTable();
			try
			{
				int num = 100;
				int num1 = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((double)lstUid.Count * 1 / 100f)));
				List<string> strs = new List<string>();
				string str = "";
				for (int i = 0; i < num1; i++)
				{
					str = string.Concat("SELECT t1.*, t2.name AS nameFile FROM accounts t1 JOIN files t2 ON t1.idfile=t2.id WHERE t1.uid IN ('", string.Join("','", lstUid.GetRange(num * i, (num * i + num <= lstUid.Count ? num : lstUid.Count % num))), "') and t1.active=1 ORDER BY t1.uid");
					strs.Add(str);
				}
				dataTable = Connector.Instance.ExecuteQuery(strs);
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, "GetAccFromFile");
			}
			return dataTable;
		}

		public static DataTable GetAllAccountFromDatabase(string field)
		{
			DataTable dataTable = new DataTable();
			try
			{
				string str = string.Concat("select ", field, " from accounts where active = 1");
				dataTable = Connector.Instance.ExecuteQuery(str);
			}
			catch
			{
			}
			return dataTable;
		}

		public static DataTable GetAllFilesFromDatabase(bool isShowAll = false)
		{
			DataTable dataTable = new DataTable();
			try
			{
				string str = "";
				str = (isShowAll ? string.Concat(new string[] { "select id, name from files where active=1 UNION SELECT -1 AS id, '", "[Tất cả thư mục]", "' AS name UNION SELECT 999999 AS id, '", "[Chọn nhiều thư mục]", "' AS name ORDER BY id ASC" }) : "select id, name from files where active=1");
				dataTable = Connector.Instance.ExecuteQuery(str);
			}
			catch
			{
			}
			return dataTable;
		}

		public static DataTable GetAllFilesFromDatabaseForBin(bool isShowAll = false)
		{
			DataTable dataTable = new DataTable();
			try
			{
				string str = "";
				str = (isShowAll ? string.Concat(new string[] { "select id, name from files WHERE id IN (SELECT DISTINCT idfile FROM accounts WHERE active=0) UNION SELECT -1 AS id, '", "[Tất cả thư mục]", "' AS name UNION SELECT 999999 AS id, '", "[Chọn nhiều thư mục]", "' AS name ORDER BY id ASC" }) : "select id, name from files WHERE id IN (SELECT DISTINCT idfile FROM accounts WHERE active=0)");
				dataTable = Connector.Instance.ExecuteQuery(str);
			}
			catch
			{
			}
			return dataTable;
		}

		public static DataTable GetAllInfoFromAccount(List<string> lstIdFile, bool isGetActive = true)
		{
			int num;
			DataTable dataTable = new DataTable();
			try
			{
				string str = "";
				if ((lstIdFile == null ? false : lstIdFile.Count != 0))
				{
					string str1 = string.Join(",", lstIdFile);
					num = (isGetActive ? 1 : 0);
					str = string.Concat("where idfile IN (", str1, ") AND active=", num.ToString());
				}
				else
				{
					num = (isGetActive ? 1 : 0);
					str = string.Concat("where active=", num.ToString());
				}
				string str2 = string.Concat(new string[] { "SELECT '-1' as id, '", "[Tất cả tình trạng]", "' AS name UNION select DISTINCT '0' as id,info from accounts ", str, " ORDER BY id ASC" });
				dataTable = Connector.Instance.ExecuteQuery(str2);
			}
			catch
			{
			}
			return dataTable;
		}

		public static string GetIdFileFromIdAccount(string id)
		{
			string str;
			try
			{
				int num = Connector.Instance.ExecuteScalar(string.Concat("SELECT idFile FROM accounts WHERE id='", id, "'"));
				str = num.ToString();
				return str;
			}
			catch
			{
			}
			str = "";
			return str;
		}

		public static DataTable GetIdStatus()
		{
			DataTable dataTable = new DataTable();
			try
			{
				dataTable = Connector.Instance.ExecuteQuery("SELECT id, status FROM accounts");
			}
			catch
			{
			}
			return dataTable;
		}

		public static bool InsertAccountToDatabase(string uid, string pass, string token, string cookie, string email, string phone, string name, string friends, string groups, string birthday, string gender, string info, string backup, string fa2, string idFile, string emaiRecovery = "", string passMail = "", string useragent = "", string proxy = "")
		{
			bool flag = true;
			try
			{
				string str = "INSERT INTO accounts(uid, pass,token,cookie1,email,name,friends,groups,birthday,gender,info,fa2,backup,idfile,passmail,useragent,proxy,dateImport,active) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}',1)";
				object[] objArray = new object[] { uid, pass.Replace("'", "''"), token, cookie, email, name, friends, groups, birthday, gender, info, fa2, backup, idFile, passMail, useragent, proxy, null };
				objArray[17] = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
				str = string.Format(str, objArray);
				Connector.Instance.ExecuteQuery(str);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static bool InsertFileToDatabase(string namefile)
		{
			bool flag = true;
			try
			{
				string[] str = new string[] { "insert into files values(null,'", namefile, "','", null, null };
				str[3] = DateTime.Now.ToString();
				str[4] = "',1)";
				string str1 = string.Concat(str);
				Connector.Instance.ExecuteQuery(str1);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static bool InsertInteractToDatabase(string uid, string hanhDong, string cauHinh)
		{
			bool flag = true;
			try
			{
				string[] str = new string[] { "INSERT INTO interacts(uid, timeInteract,hanhDong,cauHinh) VALUES ('", uid, "','", null, null, null, null, null, null };
				str[3] = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
				str[4] = "','";
				str[5] = hanhDong;
				str[6] = "','";
				str[7] = cauHinh;
				str[8] = "')";
				string str1 = string.Concat(str);
				Connector.Instance.ExecuteQuery(str1);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static bool UpdateAccountByUid(string account)
		{
			bool flag;
			string[] strArrays = account.Split(new char[] { '|' });
			string str = "";
			string str1 = "";
			string str2 = "";
			string str3 = "";
			string str4 = "";
			string str5 = "";
			string str6 = "";
			string str7 = "";
			string str8 = "";
			str = strArrays[0];
			if (str.Trim() != "")
			{
				str1 = strArrays[1];
				str2 = strArrays[2];
				str3 = strArrays[3];
				str4 = strArrays[4];
				str5 = strArrays[5];
				str6 = strArrays[6];
				str7 = strArrays[7];
				str8 = strArrays[8];
				List<string> strs = new List<string>()
				{
					(str1 != "" ? string.Concat("pass|", str1) : ""),
					(str2 != "" ? string.Concat("token|", str2) : ""),
					(str3 != "" ? string.Concat("cookie1|", str3) : ""),
					(str4 != "" ? string.Concat("email|", str4) : ""),
					(str5 != "" ? string.Concat("passmail|", str5) : ""),
					(str6 != "" ? string.Concat("fa2|", str6) : ""),
					(str7 != "" ? string.Concat("useragent|", str7) : ""),
					(str8 != "" ? string.Concat("proxy|", str8) : "")
				};
				string str9 = "update accounts set";
				foreach (string str10 in strs)
				{
					if (str10 == "")
					{
						continue;
					}
					string[] strArrays1 = new string[] { str9, " ", null, null, null, null };
					strArrays1[2] = str10.Split(new char[] { '|' })[0];
					strArrays1[3] = "='";
					strArrays1[4] = str10.Split(new char[] { '|' })[1];
					strArrays1[5] = "',";
					str9 = string.Concat(strArrays1);
					if (str10.Split(new char[] { '|' })[0] != "pass")
					{
						continue;
					}
					str9 = string.Concat(str9, "pass_old=pass,");
				}
				str9 = str9.TrimEnd(new char[] { ',' });
				str9 = string.Concat(str9, " where uid='", str, "'");
				flag = Connector.Instance.ExecuteNonQuery(str9) > 0;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool UpdateFieldToAccount(string id, string fieldName, string fieldValue)
		{
			bool flag = false;
			try
			{
				string str = "";
				if (fieldName == "pass")
				{
					str = ", pass_old=pass";
				}
				string str1 = string.Concat(new string[] { "update accounts set ", fieldName, " = '", fieldValue.Replace("'", "''"), "'", str, " where id=", id });
				flag = (Connector.Instance.ExecuteNonQuery(str1) <= 0 ? false : true);
			}
			catch
			{
			}
			return flag;
		}

		public static bool UpdateFieldToAccount(List<string> lstId, string fieldName, string fieldValue)
		{
			bool flag = false;
			try
			{
				int num = 100;
				int num1 = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((double)lstId.Count * 1 / 100f)));
				List<string> strs = new List<string>();
				string str = "";
				if (fieldName == "pass")
				{
					str = ", pass_old=pass";
				}
				for (int i = 0; i < num1; i++)
				{
					string[] strArrays = new string[] { "update accounts set ", fieldName, " = '", fieldValue.Replace("'", "''"), "'", str, " where id IN (", null, null };
					strArrays[7] = string.Join(",", lstId.GetRange(num * i, (num * i + num <= lstId.Count ? num : lstId.Count % num)));
					strArrays[8] = ")";
					strs.Add(string.Concat(strArrays));
				}
				flag = (Connector.Instance.ExecuteNonQuery(strs) <= 0 ? false : true);
			}
			catch
			{
			}
			return flag;
		}

		public static bool UpdateFieldToFile(string idFile, string fieldName, string fieldValue)
		{
			bool flag = false;
			try
			{
				string str = string.Concat(new string[] { "update files set ", fieldName, " = '", fieldValue.Replace("'", "''"), "' where id=", idFile });
				flag = (Connector.Instance.ExecuteNonQuery(str) <= 0 ? false : true);
			}
			catch
			{
			}
			return flag;
		}

		public static bool UpdateFieldToFile(List<string> lstId, string fieldName, string fieldValue)
		{
			bool flag = true;
			try
			{
				string str = string.Concat(new string[] { "update files set ", fieldName, " = '", fieldValue, "' where id IN (", string.Join(",", lstId), ")" });
				flag = (Connector.Instance.ExecuteNonQuery(str) <= 0 ? false : true);
			}
			catch
			{
			}
			return flag;
		}

		public static bool UpdateFileNameToDatabase(string idFile, string nameFile)
		{
			bool flag;
			try
			{
				string str = string.Concat("UPDATE files SET name='", nameFile, "' where id=", idFile);
				flag = Connector.Instance.ExecuteNonQuery(str) > 0;
				return flag;
			}
			catch
			{
			}
			flag = false;
			return flag;
		}

		public static bool UpdateMultiField(string field, List<string> lstId_FieldValue, string table = "accounts")
		{
			List<string> strs = new List<string>();
			string str = "";
			string str1 = "";
			string str2 = "";
			for (int i = 0; i < lstId_FieldValue.Count; i++)
			{
				str = lstId_FieldValue[i].Split(new char[] { '|' })[0];
				str1 = lstId_FieldValue[i].Split(new char[] { '|' })[1];
				if (!string.IsNullOrEmpty(str))
				{
					strs.Add(str);
					str2 = string.Concat(new string[] { str2, "WHEN '", str, "' THEN '", str1, "' " });
				}
			}
			string str3 = string.Concat(new string[] { "UPDATE ", table, " SET ", field, " = CASE id ", str2, "END WHERE id IN('", string.Join("','", strs), "'); " });
			return Connector.Instance.ExecuteNonQuery(str3) > 0;
		}

		public static bool UpdateMultiFieldToAccount(string id, string lstFieldName, string lstFieldValue, bool isAllowEmptyValue = true)
		{
			bool flag = false;
			try
			{
				if ((int)lstFieldName.Split(new char[] { '|' }).Length == (int)lstFieldValue.Split(new char[] { '|' }).Length)
				{
					int length = (int)lstFieldName.Split(new char[] { '|' }).Length;
					string str = "";
					for (int i = 0; i < length; i++)
					{
						if ((isAllowEmptyValue ? true : lstFieldValue.Split(new char[] { '|' })[i].Trim() != ""))
						{
							string[] strArrays = new string[] { str, null, null, null, null };
							strArrays[1] = lstFieldName.Split(new char[] { '|' })[i];
							strArrays[2] = "='";
							strArrays[3] = lstFieldValue.Split(new char[] { '|' })[i].Replace("'", "''");
							strArrays[4] = "',";
							str = string.Concat(strArrays);
						}
					}
					str = str.TrimEnd(new char[] { ',' });
					string str1 = string.Concat("update accounts set ", str, " where id=", id);
					flag = Connector.Instance.ExecuteNonQuery(str1) > 0;
				}
			}
			catch
			{
			}
			return flag;
		}

		public static bool UpdateMultiFieldToAccount(List<string> lstId, string lstFieldName, string lstFieldValue)
		{
			bool flag = false;
			try
			{
				if ((int)lstFieldName.Split(new char[] { '|' }).Length == (int)lstFieldValue.Split(new char[] { '|' }).Length)
				{
					int length = (int)lstFieldName.Split(new char[] { '|' }).Length;
					string str = "";
					for (int i = 0; i < length; i++)
					{
						string[] strArrays = new string[] { str, null, null, null, null };
						strArrays[1] = lstFieldName.Split(new char[] { '|' })[i];
						strArrays[2] = "='";
						strArrays[3] = lstFieldValue.Split(new char[] { '|' })[i].Replace("'", "''");
						strArrays[4] = "',";
						str = string.Concat(strArrays);
					}
					str = str.TrimEnd(new char[] { ',' });
					int num = 100;
					int num1 = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((double)lstId.Count * 1 / 100f)));
					List<string> strs = new List<string>();
					for (int j = 0; j < num1; j++)
					{
						string[] strArrays1 = new string[] { "update accounts set ", str, " where id IN (", null, null };
						strArrays1[3] = string.Join(",", lstId.GetRange(num * j, (num * j + num <= lstId.Count ? num : lstId.Count % num)));
						strArrays1[4] = ")";
						strs.Add(string.Concat(strArrays1));
					}
					flag = (Connector.Instance.ExecuteNonQuery(strs) <= 0 ? false : true);
				}
			}
			catch
			{
			}
			return flag;
		}
	}
}