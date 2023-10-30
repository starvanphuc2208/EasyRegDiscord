using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace easy
{
    public static class ComputerInfo
    {
        // Token: 0x06000080 RID: 128 RVA: 0x00007824 File Offset: 0x00005A24
        public static string GetComputerId()
        {
            return "REGTREGTELEGRAM-" + ComputerInfo.GetHash(string.Concat(new string[]
            {
                "CPU >> ",
                ComputerInfo.CpuId(),
                "\nBIOS >> ",
                ComputerInfo.BiosId(),
                "\nBASE >> ",
                ComputerInfo.MacId(),
                "\nDISK >> ",
                ComputerInfo.DiskId(),
            }));
        }

        // Token: 0x06000081 RID: 129 RVA: 0x0000787C File Offset: 0x00005A7C
        private static string GetHash(string s)
        {
            HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
            byte[] bytes = new ASCIIEncoding().GetBytes(s);
            return ComputerInfo.GetHexString(hashAlgorithm.ComputeHash(bytes));
        }

        // Token: 0x06000082 RID: 130 RVA: 0x000078A8 File Offset: 0x00005AA8
        private static string GetHexString(byte[] bt)
        {
            string text = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int num = (int)(b & 15);
                int num2 = b >> 4 & 15;
                text = ((num2 > 9) ? (text + ((char)(num2 - 10 + 65)).ToString()) : (text + num2.ToString()));
                text = ((num > 9) ? (text + ((char)(num - 10 + 65)).ToString()) : (text + num.ToString()));
                if (i + 1 != bt.Length)
                {
                    if ((i + 1) % 2 == 0)
                    {
                        text += "-";
                    }
                }
            }
            return text;
        }

        // Token: 0x06000083 RID: 131 RVA: 0x00007954 File Offset: 0x00005B54
        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string text = "";
            foreach (ManagementBaseObject managementBaseObject in new ManagementClass(wmiClass).GetInstances())
            {
                ManagementObject managementObject = (ManagementObject)managementBaseObject;
                if (managementObject[wmiMustBeTrue].ToString() == "True" && text == "")
                {
                    try
                    {
                        text = managementObject[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return text;
        }

        // Token: 0x06000084 RID: 132 RVA: 0x000079F0 File Offset: 0x00005BF0
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string text = "";
            foreach (ManagementBaseObject managementBaseObject in new ManagementClass(wmiClass).GetInstances())
            {
                ManagementObject managementObject = (ManagementObject)managementBaseObject;
                if (text == "")
                {
                    try
                    {
                        text = managementObject[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return text;
        }

        // Token: 0x06000085 RID: 133 RVA: 0x00007A74 File Offset: 0x00005C74
        private static string CpuId()
        {
            string text = ComputerInfo.identifier("Win32_Processor", "UniqueId");
            if (text == "")
            {
                text = ComputerInfo.identifier("Win32_Processor", "ProcessorId");
                if (text == "")
                {
                    text = ComputerInfo.identifier("Win32_Processor", "Name");
                    if (text == "")
                    {
                        text = ComputerInfo.identifier("Win32_Processor", "Manufacturer");
                    }
                    text += ComputerInfo.identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return text;
        }

        // Token: 0x06000086 RID: 134 RVA: 0x00007B00 File Offset: 0x00005D00
        private static string BiosId()
        {
            return string.Concat(new string[]
            {
                ComputerInfo.identifier("Win32_BIOS", "Manufacturer"),
                ComputerInfo.identifier("Win32_BIOS", "SMBIOSBIOSVersion"),
                ComputerInfo.identifier("Win32_BIOS", "IdentificationCode"),
                ComputerInfo.identifier("Win32_BIOS", "SerialNumber"),
                ComputerInfo.identifier("Win32_BIOS", "ReleaseDate"),
                ComputerInfo.identifier("Win32_BIOS", "Version")
            });
        }

        // Token: 0x06000087 RID: 135 RVA: 0x00007B84 File Offset: 0x00005D84
        private static string DiskId()
        {
            return ComputerInfo.identifier("Win32_DiskDrive", "Model") + ComputerInfo.identifier("Win32_DiskDrive", "Manufacturer") + ComputerInfo.identifier("Win32_DiskDrive", "Signature") + ComputerInfo.identifier("Win32_DiskDrive", "TotalHeads");
        }

        // Token: 0x06000088 RID: 136 RVA: 0x00007BD4 File Offset: 0x00005DD4
        private static string BaseId()
        {
            return ComputerInfo.identifier("Win32_BaseBoard", "Model") + ComputerInfo.identifier("Win32_BaseBoard", "Manufacturer") + ComputerInfo.identifier("Win32_BaseBoard", "Name") + ComputerInfo.identifier("Win32_BaseBoard", "SerialNumber");
        }

        // Token: 0x06000089 RID: 137 RVA: 0x00007C24 File Offset: 0x00005E24
        private static string VideoId()
        {
            return ComputerInfo.identifier("Win32_VideoController", "DriverVersion") + ComputerInfo.identifier("Win32_VideoController", "Name");
        }

        // Token: 0x0600008A RID: 138 RVA: 0x00007C54 File Offset: 0x00005E54
        private static string MacId()
        {
            return ComputerInfo.identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }
    }
}
