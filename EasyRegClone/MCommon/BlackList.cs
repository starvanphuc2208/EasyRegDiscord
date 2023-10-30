namespace easy.MCommon
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class BlackList
    {
        static BlackList()
        {
            if (File.Exists(BlackList.fileName))
            {
                File.ReadAllLines(BlackList.fileName).ToList<string>().ForEach(delegate (string x)
                {
                    BlackList.data.Add(x);
                });
            }
        }

        private static void save()
        {
            File.WriteAllLines(BlackList.fileName, BlackList.data);
        }

        public static bool IsBlackList(string item)
        {
            object @lock = BlackList._lock;
            bool result;
            lock (@lock)
            {
                result = BlackList.data.Contains(item);
            }
            return result;
        }

        public static void Add(string item)
        {
            object @lock = BlackList._lock;
            lock (@lock)
            {
                BlackList.data.Add(item);
                BlackList.save();
            }
        }

        private static HashSet<string> data = new HashSet<string>();

        private static object _lock = new object();

        private static string fileName = "blacklist.txt";
    }
}
