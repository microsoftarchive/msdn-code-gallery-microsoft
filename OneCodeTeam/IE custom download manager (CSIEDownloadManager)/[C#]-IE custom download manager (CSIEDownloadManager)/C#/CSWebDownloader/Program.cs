using System;
using System.Linq;
using System.Windows.Forms;

namespace CSWebDownloader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm mainForm = new MainForm();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Uri url = null;
                bool result = Uri.TryCreate(args.Last(), UriKind.Absolute, out url);
                if (result)
                {
                    mainForm.FileToDownload = url.ToString();
                }
            }
            Application.Run(mainForm);
        }
    }
}
