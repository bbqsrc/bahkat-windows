using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divvun.PkgMgr
{
    public class Util
    {
        public static String BytesToString(UInt64 bytes)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (bytes == 0)
            {
                return "0 " + suf[0];
            }
            Int32 place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            Double num = Math.Round(bytes / Math.Pow(1024, place), 2);
            return num.ToString() + " " + suf[place];
        }
    }
}
