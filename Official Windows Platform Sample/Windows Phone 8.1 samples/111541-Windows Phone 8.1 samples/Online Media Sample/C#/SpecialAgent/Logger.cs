using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpecialAgent
{
    public class Logger
    {
        public static void Log(string type, string message)
        {
            Debug.WriteLine(string.Format("{0} -- {1}", type, message));
        }
    }

}
