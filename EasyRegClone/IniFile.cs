using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace update
{
	public class IniFile
	{
		private string Path;

		private string EXE = Assembly.GetExecutingAssembly().GetName().Name;

		public IniFile(string IniPath = null)
		{
			this.Path = (new FileInfo(IniPath ?? string.Concat(this.EXE, ".ini"))).FullName.ToString();
		}

		public void DeleteKey(string Key, string Section = null)
		{
			this.Write(Key, null, Section ?? this.EXE);
		}

		public void DeleteSection(string Section = null)
		{
			this.Write(null, null, Section ?? this.EXE);
		}

		[DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = false)]
		private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW", ExactSpelling = false)]
		private static extern uint GetPrivateProfileStringByByteArray(string lpAppName, string lpKeyName, string lpDefault, byte[] lpReturnedString, uint nSize, string lpFileName);

		public bool KeyExists(string Key, string Section = null)
		{
			return this.Read(Key, Section).Length > 0;
		}

		public string Read(string Key, string Section = null)
		{
			StringBuilder RetVal = new StringBuilder(255);
			IniFile.GetPrivateProfileString(Section ?? this.EXE, Key, "", RetVal, 255, this.Path);
			return RetVal.ToString();
		}

		public string ReadUnicode(string Key, string Section = null)
		{
			byte[] byteAr = new byte[1024];
			uint resultSize = IniFile.GetPrivateProfileStringByByteArray(Section ?? this.EXE, Key, "", byteAr, (uint)byteAr.Length, this.Path);
			string strall = Encoding.Unicode.GetString(byteAr, 0, (int)(resultSize * 2));
			return strall;
		}

		public void Write(string Key, string Value, string Section = null)
		{
			IniFile.WritePrivateProfileString(Section ?? this.EXE, Key, Value, this.Path);
		}

		[DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = false)]
		private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
	}
}