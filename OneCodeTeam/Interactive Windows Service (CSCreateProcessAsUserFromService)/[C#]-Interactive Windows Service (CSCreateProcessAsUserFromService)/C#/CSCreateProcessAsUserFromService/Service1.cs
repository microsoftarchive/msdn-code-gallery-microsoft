using System;
using System.ServiceProcess;
using System.Runtime.InteropServices;

namespace CSCreateProcessAsUserFromService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // As creating a child process might be a time consuming operation,
            // its better to do that in a separate thread than blocking the main thread.
            System.Threading.Thread ProcessCreationThread = new System.Threading.Thread(MyThreadFunc);
            ProcessCreationThread.Start();
        }

        protected override void OnStop()
        {
        }

        // This thread function would launch a child process 
        // in the interactive session of the logged-on user.
        public static void MyThreadFunc()
        {
            CreateProcessAsUserWrapper.LaunchChildProcess("C:\\Windows\\notepad.exe");
        }
    }
}
