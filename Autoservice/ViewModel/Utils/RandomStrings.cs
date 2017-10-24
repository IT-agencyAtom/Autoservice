using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.ViewModel.Utils
{
    public static class RandomStrings
    {
        public static int OrderNumberLength = 10;
        private static Random _rnd;

        static RandomStrings()
        {
            _rnd = new Random(DateTime.Now.Millisecond);
        }

        public static string GetRandomString(int length)
        {
            string res = "";
            for (int i = 0; i < OrderNumberLength; i++)
            {
                res += (char) _rnd.Next(50, 256);
            }
            return res;
        }
    }
}
