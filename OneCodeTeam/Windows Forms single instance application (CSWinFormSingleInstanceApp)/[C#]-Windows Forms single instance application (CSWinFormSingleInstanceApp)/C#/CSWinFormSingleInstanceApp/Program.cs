using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace CSWinFormSingleInstanceApp
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
            // Start the message loop and pass in the main form reference.
            SingleInstanceAppStarter.Start(new MainForm(), StartNewInstance);
        }

        // The handler when attempting to start another instance of this application
        // We can customize the logic here for which form to activate in different 
        // conditions. Like in this sample, we will be selectively activate the LoginForm
        // or MainForm based on the login state of the user.
        static void StartNewInstance(object sender, StartupNextInstanceEventArgs e)
        {
            FormCollection forms = Application.OpenForms;
            if (forms["LoginForm"] != null && forms["LoginForm"].WindowState== FormWindowState.Minimized)
            {
                forms["LoginForm"].WindowState = FormWindowState.Normal;
                forms["LoginForm"].Activate();
            }
            else if (forms["LoginForm"] == null && GlobleData.IsUserLoggedIn == false)
            {
                LoginForm f = new LoginForm();
                f.ShowDialog();
            }
        }
    }
}