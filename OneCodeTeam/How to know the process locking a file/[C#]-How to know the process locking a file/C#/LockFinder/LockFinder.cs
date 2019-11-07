using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace LockFinder
{
    /// <summary>
    /// This class contains all the functions to find out which are the processes
    /// are locking a file.
    /// </summary>
    public partial class LockFinderForm : Form
    {
        // A system restart is not required.
        private const int RmRebootReasonNone = 0;
        // maximum character count of application friendly name.
        private const int CCH_RM_MAX_APP_NAME = 255;
        // maximum character count of service short name.
        private const int CCH_RM_MAX_SVC_NAME = 63;
        private delegate void AddTreeNode(TreeNode node);
        private List<Process> LastProcessList;

        /// <summary>
        /// Uniquely identifies a process by its PID and the time the process began. 
        /// An array of RM_UNIQUE_PROCESS structures can be passed
        /// to the RmRegisterResources function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct RM_UNIQUE_PROCESS
        {
            // The product identifier (PID).
            public int dwProcessId;
            // The creation time of the process.
            public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
        }

        /// <summary>
        /// Describes an application that is to be registered with the Restart Manager.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct RM_PROCESS_INFO
        {
            // Contains an RM_UNIQUE_PROCESS structure that uniquely identifies the
            // application by its PID and the time the process began.
            public RM_UNIQUE_PROCESS Process;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            // If the process is a service, this parameter returns the 
            // long name for the service.
            public string strAppName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            // If the process is a service, this is the short name for the service.
            public string strServiceShortName;
            // Contains an RM_APP_TYPE enumeration value.
            public RM_APP_TYPE ApplicationType;
            // Contains a bit mask that describes the current status of the application.
            public uint AppStatus;
            // Contains the Terminal Services session ID of the process.
            public uint TSSessionId;
            // TRUE if the application can be restarted by the 
            // Restart Manager; otherwise, FALSE.
            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }

        /// <summary>
        /// Specifies the type of application that is described by
        /// the RM_PROCESS_INFO structure.
        /// </summary>
        enum RM_APP_TYPE
        {
            // The application cannot be classified as any other type.
            RmUnknownApp = 0,
            // A Windows application run as a stand-alone process that
            // displays a top-level window.
            RmMainWindow = 1,
            // A Windows application that does not run as a stand-alone
            // process and does not display a top-level window.
            RmOtherWindow = 2,
            // The application is a Windows service.
            RmService = 3,
            // The application is Windows Explorer.
            RmExplorer = 4,
            // The application is a stand-alone console application.
            RmConsole = 5,
            // A system restart is required to complete the installation because
            // a process cannot be shut down.
            RmCritical = 1000
        }

        /// <summary>
        /// Registers resources to a Restart Manager session. The Restart Manager uses 
        /// the list of resources registered with the session to determine which 
        /// applications and services must be shut down and restarted. Resources can be 
        /// identified by filenames, service short names, or RM_UNIQUE_PROCESS structures
        /// that describe running applications.
        /// </summary>
        /// <param name="pSessionHandle">
        /// A handle to an existing Restart Manager session.
        /// </param>
        /// <param name="nFiles">The number of files being registered</param>
        /// <param name="rgsFilenames">
        /// An array of null-terminated strings of full filename paths.
        /// </param>
        /// <param name="nApplications">The number of processes being registered</param>
        /// <param name="rgApplications">An array of RM_UNIQUE_PROCESS structures</param>
        /// <param name="nServices">The number of services to be registered</param>
        /// <param name="rgsServiceNames">
        /// An array of null-terminated strings of service short names.
        /// </param>
        /// <returns>The function can return one of the system error codes that 
        /// are defined in Winerror.h
        /// </returns>
        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int RmRegisterResources(uint pSessionHandle,
                                            UInt32 nFiles, string[] rgsFilenames,
                                            UInt32 nApplications, 
                                            [In] RM_UNIQUE_PROCESS[] rgApplications,
                                            UInt32 nServices, string[] rgsServiceNames);

        /// <summary>
        /// Starts a new Restart Manager session. A maximum of 64 Restart Manager 
        /// sessions per user session can be open on the system at the same time. 
        /// When this function starts a session, it returns a session handle and 
        /// session key that can be used in subsequent calls to the Restart Manager API.
        /// </summary>
        /// <param name="pSessionHandle">
        /// A pointer to the handle of a Restart Manager session.
        /// </param>
        /// <param name="dwSessionFlags">Reserved. This parameter should be 0.</param>
        /// <param name="strSessionKey">
        /// A null-terminated string that contains the session key to the new session.
        /// </param>
        /// <returns></returns>
        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags,
                                        string strSessionKey);

        /// <summary>
        /// Ends the Restart Manager session. This function should be called by the 
        /// primary installer that has previously started the session by calling the 
        /// RmStartSession function. The RmEndSession function can be called by a 
        /// secondary installer that is joined to the session once no more resources 
        /// need to be registered by the secondary installer.
        /// </summary>
        /// <param name="pSessionHandle">
        /// A handle to an existing Restart Manager session.
        /// </param>
        /// <returns>
        /// The function can return one of the system error codes
        /// that are defined in Winerror.h.
        /// </returns>
        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int RmEndSession(uint pSessionHandle);

        /// <summary>
        /// Gets a list of all applications and services that are currently using 
        /// resources that have been registered with the Restart Manager session.
        /// </summary>
        /// <param name="dwSessionHandle">
        /// A handle to an existing Restart Manager session.
        /// </param>
        /// <param name="pnProcInfoNeeded">A pointer to an array size necessary to 
        /// receive RM_PROCESS_INFO structures required to return information for 
        /// all affected applications and services.
        /// </param>
        /// <param name="pnProcInfo">
        /// A pointer to the total number of RM_PROCESS_INFO structures in an array
        /// and number of structures filled.
        /// </param>
        /// <param name="rgAffectedApps">
        /// An array of RM_PROCESS_INFO structures that list the applications and 
        /// services using resources that have been registered with the session.
        /// </param>
        /// <param name="lpdwRebootReasons">
        /// Pointer to location that receives a value of the RM_REBOOT_REASON
        /// enumeration that describes the reason a system restart is needed.
        /// </param>
        /// <returns></returns>
        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded,
                                    ref uint pnProcInfo, 
                                    [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
                                    ref uint lpdwRebootReasons);

        public LockFinderForm()
        {
            InitializeComponent();
        }

        void TimerTick(object sender, EventArgs e)
        {
            List<Process> CurrentProcessList = new List<Process>();
            List<int> LastProcessListId = new List<int>();
            List<int> CurrentProcessListId = new List<int>();

            if (tbFilePath.Text != "")
            {
                CurrentProcessList = LockFinder(tbFilePath.Text);
                CurrentProcessListId = CurrentProcessList.OrderBy(process => process.Id)
                                                        .Select(pid => pid.Id).ToList();
                LastProcessListId = LastProcessList.OrderBy(process => process.Id)
                                                        .Select(pid => pid.Id).ToList();
                bool check = CurrentProcessListId.SequenceEqual(LastProcessListId);

                if (!check)
                {
                    tvwShowLockingProcess.Nodes.Clear();
                    RefresData(CurrentProcessList);
                    LastProcessList = CurrentProcessList;
                }
            }
        }

        /// <summary>
        /// Refreshes the TreeView control with new set of processes.
        /// </summary>
        /// <param name="list"></param>
        void RefresData(List<Process> list)
        {
            AddTreeNode addTreeNode = new AddTreeNode(OnAddTreeNode);

            foreach (Process item in list)
            {
                TreeNode ipNode = new TreeNode();
                ipNode.Text = item.ProcessName;
                // Gets the base priority of the associated process
                ipNode.Nodes.Add("BasePriority: " + item.BasePriority);
                // Gets the native handle of the associated process.
                ipNode.Nodes.Add("Handle: " + item.Handle);
                // Gets the number of handles opened by the process.
                ipNode.Nodes.Add("HandleCount: " + item.HandleCount);
                // Gets the unique identifier for the associated process.
                ipNode.Nodes.Add("Id: " + item.Id);
                // Gets the window handle of the main window of the associated process.
                ipNode.Nodes.Add("MainWindowHandle: " + item.MainWindowHandle);
                // Gets the caption of the main window of the process.
                ipNode.Nodes.Add("MainWindowTitle: " + item.MainWindowTitle);
                // Gets or sets the maximum allowable working set size for
                // the associated process.
                ipNode.Nodes.Add("MaxWorkingSet: " + item.MaxWorkingSet);
                // Gets or sets the minimum allowable working set size for
                // the associated process.
                ipNode.Nodes.Add("MinWorkingSet: " + item.MinWorkingSet);
                // Gets the nonpaged system memory size allocated to this process.
                ipNode.Nodes.Add("NonpagedSystemMemorySize64: " + 
                                            item.NonpagedSystemMemorySize64);
                // Gets the amount of paged memory allocated for the associated process.
                ipNode.Nodes.Add("PagedMemorySize64: " + item.PagedMemorySize64);
                // Gets the amount of pageable system memory allocated for the 
                // associated process.
                ipNode.Nodes.Add("PagedSystemMemorySize64: " + 
                                                item.PagedSystemMemorySize64);
                // Gets the maximum amount of memory in the virtual memory paging 
                // file used by the associated process.
                ipNode.Nodes.Add("PeakPagedMemorySize64: " + item.PeakPagedMemorySize64);
                // Gets the maximum amount of physical memory used by the
                // associated process.
                ipNode.Nodes.Add("PeakWorkingSet64: " + item.PeakWorkingSet64);
                // Gets the maximum amount of virtual memory used by 
                // the associated process.
                ipNode.Nodes.Add("PeakVirtualMemorySize64: " + 
                                                    item.PeakVirtualMemorySize64);
                // Gets or sets a value indicating whether the associated process 
                // priority should temporarily be boosted by the operating system 
                // when the main window has the focus.
                ipNode.Nodes.Add("PriorityBoostEnabled: " + item.PriorityBoostEnabled);
                // Gets or sets the overall priority category for the associated process.
                ipNode.Nodes.Add("PriorityClass: " + item.PriorityClass);
                // Gets the amount of private memory allocated for 
                // the associated process.
                ipNode.Nodes.Add("PrivateMemorySize64: " + item.PrivateMemorySize64);
                // Gets the privileged processor time for this process.
                ipNode.Nodes.Add("PrivilegedProcessorTime: " + 
                                                        item.PrivilegedProcessorTime);
                // Gets the name of the process.
                ipNode.Nodes.Add("ProcessName: " + item.ProcessName);
                // Gets or sets the processors on which the threads in this process
                // can be scheduled to run.
                ipNode.Nodes.Add("ProcessorAffinity: " + item.ProcessorAffinity);
                // Gets a value indicating whether the user interface of the 
                // process is responding.
                ipNode.Nodes.Add("Responding: " + item.Responding);
                // Gets the Terminal Services session identifier for 
                // the associated process.
                ipNode.Nodes.Add("SessionId: " + item.SessionId);
                // Gets the time that the associated process was started.
                ipNode.Nodes.Add("StartTime: " + item.StartTime);
                // Gets the total processor time for this process.
                ipNode.Nodes.Add("TotalProcessorTime: " + item.TotalProcessorTime);
                // Gets the user processor time for this process.
                ipNode.Nodes.Add("UserProcessorTime: " + item.UserProcessorTime);
                // Gets the amount of the virtual memory allocated for 
                // the associated process.
                ipNode.Nodes.Add("VirtualMemorySize64: " + item.VirtualMemorySize64);
                // Gets the amount of physical memory allocated for 
                // the associated process.
                ipNode.Nodes.Add("WorkingSet64: " + item.WorkingSet64);
                // Executes the specified delegate, on the thread that owns the control's
                // underlying window handle, with the specified list of arguments.
                tvwShowLockingProcess.Invoke(addTreeNode, new object[] { ipNode });
            }
        }

        private void LockFinderForm_Load(object sender, EventArgs e)
        {
            tmrRefreshData.Interval = 1;
            tmrRefreshData.Tick += new EventHandler(TimerTick);
            tmrRefreshData.Start();
        }

        private void OnAddTreeNode(TreeNode node)
        {
            tvwShowLockingProcess.Nodes.Add(node);
        }

        /// <summary>
        /// This function finds out what process(es) have a lock on the specified file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <returns>Processes locking the file</returns>
        static public List<Process> LockFinder(string path)
        {
            uint handle;
            string key = Guid.NewGuid().ToString();
            List<Process> processes = new List<Process>();

            int res = RmStartSession(out handle, 0, key);
            if (res != 0)
            {
                throw new Exception("Could not begin restart session. " +
                                    "Unable to determine file locker.");
            }

            try
            {
                const int ERROR_MORE_DATA = 234;
                uint pnProcInfoNeeded = 0, pnProcInfo = 0,
                    lpdwRebootReasons = RmRebootReasonNone;
                string[] resources = new string[] { path };

                res = RmRegisterResources(handle, (uint)resources.Length,
                                            resources, 0, null, 0, null);
                if (res != 0)
                {
                    throw new Exception("Could not register resource.");
                }
                // There's a race around condition here. The first call to RmGetList()
                // returns the total number of process. However, when we call RmGetList()
                // again to get the actual processes this number may have increased.
                res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null,
                                ref lpdwRebootReasons);
                if (res == ERROR_MORE_DATA)
                {
                    // Create an array to store the process results.
                    RM_PROCESS_INFO[] processInfo =
                        new RM_PROCESS_INFO[pnProcInfoNeeded];
                    pnProcInfo = pnProcInfoNeeded;
                    // Get the list.
                    res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo,
                        processInfo, ref lpdwRebootReasons);
                    if (res == 0)
                    {
                        processes = new List<Process>((int)pnProcInfo);
                        // Enumerate all of the results and add them to the 
                        // list to be returned.
                        for (int i = 0; i < pnProcInfo; i++)
                        {
                            try
                            {
                                processes.Add(Process.GetProcessById(processInfo[i].
                                    Process.dwProcessId));
                            }
                            // Catch the error in case the process is no longer running.
                            catch (ArgumentException) { }
                        }
                    }
                    else
                    {
                        throw new Exception("Could not list processes locking resource");
                    }
                }
                else if (res != 0)
                {
                    throw new Exception("Could not list processes locking resource." +
                                        "Failed to get size of result.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message,"Lock Finder",MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                RmEndSession(handle);
            }

            return processes;
        }

        private void btnBrowseFile_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.Cursor = Cursors.Hand;
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog chooseFileDialog = new OpenFileDialog();
            chooseFileDialog.ShowDialog();
            if (chooseFileDialog.FileName != "")
            {
                tbFilePath.Text = chooseFileDialog.FileName;
            }
        }

        private void tbFilePath_TextChanged(object sender, EventArgs e)
        {
            if (tbFilePath.Text != "")
            {
                tvwShowLockingProcess.Nodes.Clear();
                LastProcessList = LockFinder(tbFilePath.Text);
                RefresData(LastProcessList);
            }
        }
    }
}