using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy.MCommon
{
    internal class DataManage
    {
        public static bool isStop { get; set; }

        public static int NumberOfAccounts { get; set; }

        public static string AccountPassword { get; set; }

        public static string APIRental { get; set; }

        public static string ProviderRental { get; set; }

        public static int PeriodRental { get; set; }

        public static string ProxyAddress { get; set; }

        public static string ProxyPort { get; set; }

        public static int NumberOfThreads { get; set; }
    }
}
