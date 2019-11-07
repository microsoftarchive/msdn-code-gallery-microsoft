//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.ApplicationModel;
using SDKTemplate;
using System.ComponentModel;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.Foundation;

namespace SDKTemplate.Common
{
    /// <summary>
    /// Class for storing and binding file name
    /// </summary>
    public class FileItem
    {
        String name;
        /// <summary>
        /// The file name 
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">Storage file for which name is to be stored</param>
        public FileItem(StorageFile file)
        {
            name = file.Name;
        }
    }

    /// <summary>
    /// Class for maintaining the data context that is used by all the scenarios
    /// </summary>
    public class ModelDataContext : INotifyPropertyChanged
    {
        public ModelDataContext()
        {
            propertyChangedEventHandler = new PropertyChangedEventHandler(ScanContextPropertyEventHandler);
            currentScannerContext.PropertyChanged += propertyChangedEventHandler;
        }

        public void UnLoad()
        {
            currentScannerContext.PropertyChanged -= propertyChangedEventHandler;
        }

        // <summary>
        /// Event Handler for notification of property changes from scanner context
        /// </summary>
        /// <param name="sender">Object sending the notification</param>
        /// <param name="e">Event data</param>
        public void ScanContextPropertyEventHandler(Object sender,PropertyChangedEventArgs e)
        {  
            if (e.PropertyName.Equals("CurrentScannerDeviceId"))
            {
                    OnPropertyChanged("IsScanningAllowed");
            }
        }

        /// <summary>
        /// Even Handler for property change event
        /// </summary>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Current scanner context maintaining the list of scanners and current scannder that is selected
        /// </summary>
        public ScannerContext ScannerDataContext
        {
            get
            {
                return currentScannerContext;
            }
        }

        /// <summary>
        /// Destination folder where all the scanned images are kept
        /// </summary>
        public StorageFolder DestinationFolder
        {
            get
            {
                return destinationFolder;
            }
        }

        /// <summary>
        /// Poperty for specifying if the scenario is running or not
        /// </summary
        public bool ScenarioRunning
        {
            get
            {
                return scenarioRunning;
            }
            set
            {
                scenarioRunning = value;
                OnPropertyChanged("ScenarioRunning");
                // Raise property change event of IsScanningAllowed property, as IsScanningAllowed property is dependent on ScenarioRunning property
                OnPropertyChanged("IsScanningAllowed");
            }
        }

        /// <summary>
        ///  Property for specifying if the start scenario button is to be enabled or not
        /// </summary>
        public bool IsScanningAllowed
        {
            get
            {
                return ((currentScannerContext.CurrentScannerDeviceId != null) && !scenarioRunning);
            }
        }
        /// <summary>
        /// List of scanned files
        /// </summary>
        public ObservableCollection<FileItem> FileList 
        {
            get
            { 
                return scannedFileList; 
            }
        }
            
        /// <summary>
        /// Size of scanned files list
        /// </summary>
        public int FileListSize 
        {
            get
            { 
                return scannedFileList.Count;
            }
        }

        /// <summary>
        /// Add the name of the give storage item name to the File lsit
        /// </summary>
        public void AddToFileList(StorageFile file)
        {
            scannedFileList.Add(new FileItem(file));
            OnPropertyChanged("FileListSize");
        }

        /// <summary>
        /// Clears items in the File lsit
        /// </summary>
        public void ClearFileList()
        {
            scannedFileList.Clear();
            OnPropertyChanged("FileListSize");
        }
    
        static ScannerContext currentScannerContext = new ScannerContext();
        static StorageFolder destinationFolder = KnownFolders.PicturesLibrary;
        ObservableCollection<FileItem> scannedFileList = new ObservableCollection<FileItem>();
        bool scenarioRunning = false;

        public event PropertyChangedEventHandler PropertyChanged;
        private PropertyChangedEventHandler propertyChangedEventHandler;
        
    }

    /// <summary>
    /// Class for storing and binding scanner device information
    /// </summary
    public class ScannerDataItem
    {
        /// <summary>
        /// The DeviceInformation object (device interface) for this Scanner device
        /// </summary>
        private  DeviceInformation scannerDeviceInfo;

        /// <summary>
        /// The scanner device id
        /// </summary>
        public String Id 
        {
            get 
            {
                return scannerDeviceInfo.Id; 
            } 
        }

        /// <summary>
        /// The scanner name 
        /// </summary>
        public String Name
        {
            get
            {
                return scannerDeviceInfo.Name;
            }
        }

        /// <summary>
        /// Has the device been found during the current enumeration
        /// </summary>
        public bool Matched = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DeviceInterface">The device interface for this entry</param>
        public ScannerDataItem(DeviceInformation deviceInfo)
        {
            scannerDeviceInfo = deviceInfo;
        }
    }

    /// <summary>
    /// Class for maintaining list of scanners that are attached and current scanner that is selected
    /// </summary
    public class ScannerContext : INotifyPropertyChanged
    {
        
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Constructor for ScannerContext class
        /// </summary>
        public ScannerContext()
        {
            InitDeviceWatcher();
            // Register for app suspend/resume handlers
            App.Current.Suspending += new SuspendingEventHandler(this.SuspendDeviceWatcher);
            App.Current.Resuming += this.ResumeDeviceWatcher;
        }

        /// <summary>
        /// List of scanner devices currently attached to the system.  This
        /// list can be updated by the PNP watcher.
        /// </summary>
        public ObservableCollection<ScannerDataItem> ListOfScanners 
        { 
            get 
            { 
                return scannerInfoList; 
            } 
        }

        /// <summary>
        /// Id of the scanner that is selected
        /// </summary>
        public String CurrentScannerDeviceId
        {
            get
            {
                return currentScannerDeviceId;
            }
            set
            {
                currentScannerDeviceId = value;
                OnPropertyChanged("CurrentScannerDeviceId");
            }
        }

        /// <summary>
        /// Collections of scanner devices currently attached to the system.
        /// </summary>
        public int ScannerListSize
        {
            get
            { 
                return scannerInfoList.Count; 
            }
        }

        /// <summary>
        /// Start Watcher for scanner devices
        /// </summary>
        public void StartScannerWatcher()
        {
            MainPage.Current.NotifyUser("Starting scanner watcher", NotifyType.StatusMessage);
            foreach (var entry in scannerInfoList)
            {
                entry.Matched = false;
            }

            WatcherStarted = true;
            scannerWatcher.Start();

            return;
        }

        /// <summary>
        /// Stop Watcher for scanner devices
        /// </summary>
        public void StopScannerWatcher()
        {
            MainPage.Current.NotifyUser("Stopping scanner watcher", NotifyType.StatusMessage);
            scannerInfoList.Clear();
            scannerWatcher.Stop();
            WatcherStarted = false;
        }

        /// <summary>
        /// Initializes the watcher which is used for enumerating scanners
        /// </summary>
        void InitDeviceWatcher()
        {
            // Create a Device Watcher class for type Image Scanner for enumerating scanners
            scannerWatcher = DeviceInformation.CreateWatcher(DeviceClass.ImageScanner);

            scannerWatcher.Added += OnScannerAdded;
            scannerWatcher.Removed += OnScannerRemoved;
            scannerWatcher.EnumerationCompleted += OnScannerEnumerationComplete;
        }

        /// <summary>
        /// Event handler a scanner being added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInfo">The device info for the device which was added</param>
        private async void OnScannerAdded(DeviceWatcher sender,  DeviceInformation deviceInfo)
        {
            await
            MainPage.Current.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    MainPage.Current.NotifyUser(String.Format("Scanner with device id {0} has been added", deviceInfo.Id), NotifyType.StatusMessage);

                    // search the device list for a device with a matching device id
                    ScannerDataItem match = FindInList(deviceInfo.Id);

                    // If we found a match then mark it as verified and return
                    if (match != null)
                    {
                        match.Matched = true;
                        return;
                    }

                    // Add the new element to the end of the list of devices
                    AppendToList(deviceInfo);
                }
            );
        }

        /// <summary>
        /// Event handler for the removal of a scanner device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInfo">The device info for the device which was added</param>
        private async void OnScannerRemoved(DeviceWatcher sender,  DeviceInformationUpdate deviceInfo)
        {
            await
            MainPage.Current.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    var deviceId = deviceInfo.Id;
                    MainPage.Current.NotifyUser(String.Format("Scanner with device id {0} has been removed", deviceId), NotifyType.StatusMessage);
                    // Search the list of devices for one with a matching device id
                    var match = FindInList(deviceId);
                    if (match != null)
                    {
                        // Remove the matched item
                        MainPage.Current.NotifyUser(String.Format("Scanner with device id {0} has been removed from list", deviceId), NotifyType.StatusMessage);
                        scannerInfoList.Remove(match);
                    }
                }
            );
        }

        /// <summary>
        /// Event handler for the end of an enumeration/reenumeration started
        /// by calling Start on the device watcher.  This culls out any entries
        /// in the list which are no longer matched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnScannerEnumerationComplete(DeviceWatcher sender, object args)
        {
            await
            MainPage.Current.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    MainPage.Current.NotifyUser("Enumeration of scanners is complete", NotifyType.StatusMessage);

                    ScannerDataItem removedScannerInfo;

                    while ((removedScannerInfo = scannerInfoList.FirstOrDefault(e => e.Matched == false)) != null)
                    {
                        MainPage.Current.NotifyUser("Enumeration of scanners is complete: Removing missing device " + removedScannerInfo.Id, NotifyType.StatusMessage);
                        RemoveFromList(removedScannerInfo);
                        
                    }
                }
            );
        }

        /// <summary>
        /// Appends a SannerDataItem to the scanner Info list with the given device information
        /// </summary>
        /// <param name="deviceInfo">device information of the scanner that is to be added</param>
        private void AppendToList(DeviceInformation deviceInfo)
        {
            scannerInfoList.Add(new ScannerDataItem(deviceInfo));
            OnPropertyChanged("ScannerListSize");
            // Set current Scanner if it was not previously set 
            if (currentScannerDeviceId == null)
            {
                ChangeCurrentlySelectedScanner(deviceInfo.Id);
            }
        }

        /// <summary>
        /// Removes a given SannerDataItem from the scanner information list 
        /// </summary>
        /// <param name="removedScannerInfo">ScannerDataItem that is to be removed from the list </param>
        private  void  RemoveFromList(ScannerDataItem removedScannerInfo)
        {

            // If currently selected scanner is being removed, make sure
            // to select a different one
            if (currentScannerDeviceId == removedScannerInfo.Id) 
            {
                String scannerIdToSelect = (scannerInfoList.Count == 0) ? 
                                             null :
                                             scannerInfoList.ElementAt(0).Id;

                ChangeCurrentlySelectedScanner(scannerIdToSelect);
            }
            scannerInfoList.Remove(removedScannerInfo);
            OnPropertyChanged("ScannerListSize");
        }

        /// <summary>
        /// Changes the currently selected scanner 
        /// </summary>
        private void ChangeCurrentlySelectedScanner(String id)
        {
            currentScannerDeviceId = id;    
            OnPropertyChanged("CurrentScannerDeviceId");           
        }

        /// <summary>
        /// Finds the ScannerDataItem element for a given device id
        /// </summary>
        /// <param name="id">device id of the scanner that is to be found</param>
        private ScannerDataItem FindInList(string id)
        {
            return scannerInfoList.FirstOrDefault(entry => entry.Id == id);
        }

        /// <summary>
        /// Suspends the scanner watcher
        /// </summary>
        private void SuspendDeviceWatcher(object sender, SuspendingEventArgs e)
        {
            if (WatcherStarted)
            {
                watcherSuspended = WatcherStarted;
                StopScannerWatcher();
            }
        }

        /// <summary>
        /// Resumes the scanner watcher
        /// </summary>
        private void ResumeDeviceWatcher(object sender, object e)
        {
            if (watcherSuspended)
            {
                StartScannerWatcher();
                watcherSuspended = false;
            }
        }


        private String currentScannerDeviceId;
        private bool watcherSuspended = false;

        /// <summary>
        /// The device watcher that we setup to look for scanner devices
        /// </summary>
         DeviceWatcher scannerWatcher = null;

        /// <summary>
        /// Internal list of scanner device information.
        /// </summary>
        ObservableCollection<ScannerDataItem> scannerInfoList = new ObservableCollection<ScannerDataItem>();

        /// <summary>
        /// Flag to keep track of whether the device watcher has been started.
        /// </summary>
        public bool WatcherStarted = false;
        public event PropertyChangedEventHandler PropertyChanged;

    }

}
