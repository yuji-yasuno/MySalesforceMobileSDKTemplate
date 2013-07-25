using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    class MySFSharedData
    {
        public MySFOAuthStoredData storedData { get; set; }
        private static MySFSharedData _instance;
        private MySFSharedData() { }
        public static MySFSharedData getInstance() 
        {
            if (_instance == null) _instance = new MySFSharedData();
            return _instance;
        }
    }
}
