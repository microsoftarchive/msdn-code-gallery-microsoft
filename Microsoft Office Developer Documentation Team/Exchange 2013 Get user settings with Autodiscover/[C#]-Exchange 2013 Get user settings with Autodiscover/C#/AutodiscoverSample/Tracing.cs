using System;
using System.IO;

namespace Microsoft.Exchange.Samples.Autodiscover
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure 
    // that the code meets the coding requirements of your organization. 
    class Tracing
    {
        private static TextWriter logFileWriter = null;

        // InitalizeLog
        //   This function initalizes the logfile.
        //
        // Parameters:
        //   filename: The path to the file.
        //
        // Returns:
        //   None.
        //
        public static void InitalizeLog(string filename)
        {
            logFileWriter = new StreamWriter(filename);
        }

        // Write
        //   This function writes to the console and to the
        //   log file without writing a line terminator.
        //
        // Parameters:
        //   format: The composite format string to write. See the
        //           documentation for Console.Write for more information.
        //   args: An array of objects to write using the composite format string.
        //
        // Returns:
        //   None.
        //
        public static void Write(string format, params object[] args)
        {
            Console.Write(format, args);
            if (logFileWriter != null)
                logFileWriter.Write(format, args);
        }

        // WriteLine
        //   This function writes to the console and to the
        //   log file followed by a line terminator.
        //
        // Parameters:
        //   format: The composite format string to write. See the
        //           documentation for Console.WriteLine for more information.
        //   args: An array of objects to write using the composite format string.
        //
        // Returns:
        //   None.
        //
        public static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            if (logFileWriter != null)
                logFileWriter.WriteLine(format, args);
        }

        // FinalizeLog
        //   This function finalizes the log file.
        //
        // Parameters:
        //   None.
        //
        // Returns:
        //   None.
        //
        public static void FinalizeLog()
        {
            logFileWriter.Flush();
            logFileWriter.Close();
        }
    }
}
