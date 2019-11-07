using System.Diagnostics;

namespace Shared_Library
{
    public class Logger
    {
        public static void Log(string type, string message)
        {
            Debug.WriteLine(string.Format("{0} -- {1}", type, message));
        }
    }
}
