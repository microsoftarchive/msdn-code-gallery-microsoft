using System;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using System.Threading;

namespace CSWinFormSplashScreen
{
    //Solution 1: Customized SplashScreen with "fade in" and "fade out" effect.
    //Solution 2: Using VB.NET Framework without "fade in" and "fade out" effect.
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // If we use Solution 2, we need to comment the following.
            #region Solution1
            Application.Run(new SplashScreen1());
            // After splash form closed, start the main form.
            Application.Run(new MainForm());
            #endregion

            // If we use Solution 2, uncomment the following
            //#region Solution2
            //new SplashScreenUsingVBFramework().Run(args);
            //#endregion
        }
    }

    #region Solution2

    //We need to add Microsoft.VisualBasic reference for type 
    //WindowsFormsApplicationBase.The following code is not 
    //necessary if we use Solution1.

    class SplashScreenUsingVBFramework : WindowsFormsApplicationBase
    {
        protected override void OnCreateSplashScreen()
        {
            base.OnCreateSplashScreen();
            // You can replace the Splash2 screen to yours.
            this.SplashScreen = new CSWinFormSplashScreen.SplashScreen2();
        }
        protected override void OnCreateMainForm()
        {
            base.OnCreateMainForm();
            //Here you can specify the MainForm you want to start.
            this.MainForm = new MainForm();
        }
    }
    #endregion
}
