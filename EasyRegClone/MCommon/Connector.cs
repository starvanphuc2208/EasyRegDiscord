using maxcare;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace MCommon
{
	public class Connector
	{
		private static Connector instance;

		private string connectionSTR = string.Concat("Data Source=", Base.pathDataBase, "\\database\\db_maxcare.sqlite;Version=3;");

		private SQLiteConnection connection = null;

		public static Connector Instance
		{
			get
			{
				if (Connector.instance == null)
				{
					Connector.instance = new Connector();
				}
				return Connector.instance;
			}
			private set
			{
				Connector.instance = value;
			}
		}

		private Connector()
		{
		}

		private void CheckConnectServer()
		{
			try
			{
				if (this.connection == null)
				{
					this.connection = new SQLiteConnection(this.connectionSTR);
				}
				if (this.connection.State == ConnectionState.Closed)
				{
					this.connection.Open();
				}
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, "CheckConnectServer");
			}
		}

		public int ExecuteNonQuery(List<string> lstQuery)
		{
			int num = 0;
			try
			{
				this.CheckConnectServer();
				for (int i = 0; i < lstQuery.Count; i++)
				{
					string item = lstQuery[i];
					num = (new SQLiteCommand(item, this.connection)).ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, "ExecuteNonQuery");
			}
			return num;
		}

		public int ExecuteNonQuery(string query)
		{
			int num = 0;
			try
			{
				this.CheckConnectServer();
				num = (new SQLiteCommand(query, this.connection)).ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, string.Concat("ExecuteNonQuery: ", query));
			}
			return num;
		}

		public DataTable ExecuteQuery(string query)
		{
			DataTable dataTable = new DataTable();
			try
			{
				this.CheckConnectServer();
				SQLiteCommand sQLiteCommand = new SQLiteCommand(query, this.connection);
				(new SQLiteDataAdapter(sQLiteCommand)).Fill(dataTable);
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, "ExecuteQuery");
			}
			return dataTable;
		}

		public DataTable ExecuteQuery(List<string> lstQuery)
		{
			DataTable dataTable = new DataTable();
			try
			{
				this.CheckConnectServer();
				for (int i = 0; i < lstQuery.Count; i++)
				{
					SQLiteCommand sQLiteCommand = new SQLiteCommand(lstQuery[i], this.connection);
					(new SQLiteDataAdapter(sQLiteCommand)).Fill(dataTable);
				}
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, "ExecuteQuery");
			}
			return dataTable;
		}

		public int ExecuteScalar(string query)
		{
			int num = 0;
			try
			{
				this.CheckConnectServer();
				SQLiteCommand sQLiteCommand = new SQLiteCommand(query, this.connection);
				num = (int)((long)sQLiteCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				Common.ExportError(exception, string.Concat("ExecuteScalar: ", query));
			}
			return num;
		}
	}
}