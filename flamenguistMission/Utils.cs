using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flamenguistMission
{
    class Utils
    {
        private static Utils _shared; 
        private Utils() { }

        public static Utils GetInstance()
        {
            if (_shared == null)
            {
                _shared = new Utils();
            }
            return _shared;
        }

    }
}
