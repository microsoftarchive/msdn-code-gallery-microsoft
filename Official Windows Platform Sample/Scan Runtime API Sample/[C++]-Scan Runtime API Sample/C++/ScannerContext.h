//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample
{
    namespace Common
    {
        namespace WDE = Windows::Devices::Enumeration;
        
        /// <summary>
        /// Class for storing and binding scanner device information
        /// </summary
        [Windows::UI::Xaml::Data::Bindable]
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class ScannerDataItem sealed  
        {
        public:
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="DeviceInterface">The device interface for this entry</param>
            ScannerDataItem(WDE::DeviceInformation^ deviceInfo)
            {
                scannerDeviceInfo = deviceInfo;
                IsMatched = false;
            };

            /// <summary>
            /// The scanner name 
            /// </summary>
            property Platform::String^ Name
            {
                Platform::String^ get() 
                {
                    return scannerDeviceInfo->Name;
                };
            }

            // <summary>
            /// The scanner device id
            /// </summary>
            property Platform::String^ Id
            {
                Platform::String^ get() 
                {
                    return scannerDeviceInfo->Id;
                };
            }

            /// <summary>
            /// Has the device been found during the current enumeration
            /// </summary>
            property bool IsMatched;
        private:
            /// <summary>
            /// The DeviceInformation object (device interface) for this Scanner device
            /// </summary>
            WDE::DeviceInformation^ scannerDeviceInfo;
        };

        /// <summary>
        /// Class for maintaining list of scanners that are attached and current scanner that is selected
        /// </summary
        [Windows::UI::Xaml::Data::Bindable]
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class ScannerContext sealed : Windows::UI::Xaml::DependencyObject, Windows::UI::Xaml::Data::INotifyPropertyChanged
        {
        public:    
            
            ScannerContext();
            virtual ~ScannerContext();
            void StartScannerWatcher();
            void StopScannerWatcher();
            
            /// <summary>
            /// List of scanner devices currently attached to the system.
            /// </summary>
            property Windows::Foundation::Collections::IObservableVector<ScannerDataItem^>^ ListOfScanners 
            {
                 Windows::Foundation::Collections::IObservableVector<ScannerDataItem^>^ get() 
                 { 
                     return scannerInfoList; 
                 };
            }

            /// <summary>
            /// Collections of scanner devices currently attached to the system.
            /// </summary>
            property unsigned int ScannerListSize
            {
                unsigned int get() { return scannerInfoList->Size; };
            }

            /// <summary>
            /// Specifies if watcher for the scanner devices has been started or not
            /// </summary>
            property bool WatcherStarted 
            {
                bool get() 
                { 
                    return watcherStarted; 
                };
            };

            /// <summary>
            /// Id of the scanner that is selected
            /// </summary>
            property Platform::String^ CurrentScannerDeviceId
            {
                Platform::String^ get() 
                { 
                    return currentScannerDeviceId; 
                };				
                void set(Platform::String^ ScannerId)
                {
                    currentScannerDeviceId = ScannerId;
                    OnPropertyChanged("CurrentScannerDeviceId");
                };
            }

            /// <summary>
            /// Even Handler for property change event
            /// </summary>
            void OnPropertyChanged(_In_ Platform::String^ propertyName)
            {
                PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
            }
            
            virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

        private:
            Platform::Collections::Vector<ScannerDataItem^>^ scannerInfoList;  
            bool watcherSuspended;
            bool watcherStarted;
            Platform::String^ currentScannerDeviceId;
            WDE::DeviceWatcher^ watcher; 

            void OnScannerAdded(_In_ WDE::DeviceWatcher^ sender, _In_ WDE::DeviceInformation^ deviceInfo);
            void OnScannerRemoved(_In_ WDE::DeviceWatcher^ sender, _In_ WDE::DeviceInformationUpdate^ deviceInfo);
            void OnScannerEnumerationComplete(_In_ WDE::DeviceWatcher^ sender, _In_ Platform::Object^ args);
            void SuspendWatcher(_In_ Platform::Object^ Sender, _In_ Windows::ApplicationModel::SuspendingEventArgs^ Args);
            void ResumeWatcher(_In_ Platform::Object^ Sender, _In_ Platform::Object^ Args);

            void InitScannerWatcher();
            ScannerDataItem^ ScannerContext::FindDevice(_In_ Platform::String^ Id);            
            void AppendToList(_In_ WDE::DeviceInformation^ deviceInfo);
            void RemoveFromListAtEnd();
        };

        /// <summary>
        /// Class for storing and binding file name
        /// </summary>
        [Windows::UI::Xaml::Data::Bindable]
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class FileItem sealed  
        {
        public:
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="file">Storage file for which the name need to be stored</param>
            FileItem(Windows::Storage::StorageFile^ file)
            {
                fileName = file->Name;				
            };

            /// <summary>
            /// The file name 
            /// </summary>
            property Platform::String^ Name
            {
                Platform::String^ get() 
                {
                    return fileName;
                };
            }
            
        private:
            /// <summary>
            /// Name of the file that is to be displayed
            /// </summary>
            Platform::String^ fileName;
            
        };

        /// <summary>
        /// Class for maintaining the data context that is used by all the scenarios
        /// </summary>
        [Windows::UI::Xaml::Data::Bindable]
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class ModelDataContext sealed : Windows::UI::Xaml::DependencyObject, Windows::UI::Xaml::Data::INotifyPropertyChanged
        {
        public:
            /// <summary>
            /// Constructor
            /// </summary>
            ModelDataContext()
            {
                if (currentScanContext == nullptr)
                {
                    currentScanContext = ref new ScannerContext();
                }
                propertyToken = currentScanContext->PropertyChanged += ref new Windows::UI::Xaml::Data::PropertyChangedEventHandler(this, &ModelDataContext::ScanContextPropertyEventHandler);

                scannedFileList = ref new Platform::Collections::Vector<FileItem^>();
                scenarioRunning = false;	
                    
            }

            // <summary>
            /// Event Handler for notification of property changes from scanner context
            /// </summary>
            /// <param name="sender">Object sending the notification</param>
            /// <param name="e">Event data</param>
            void ModelDataContext::ScanContextPropertyEventHandler(_In_ Platform::Object^ sender, Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e)
            {  
                if (e->PropertyName->Equals("CurrentScannerDeviceId"))
                {
                    OnPropertyChanged("IsScanningAllowed");
                }
            }
           
            /// <summary>
            /// Current scanner context maintaining the list of scanners and current scannder that is selected
            /// </summary>
            property ScannerContext^ ScannerDataContext
            {
                ScannerContext^ get()
                    {
                        return currentScanContext;
                    };
            };

            /// <summary>
            /// Destination folder where all the scanned images are kept
            /// </summary>
            property Windows::Storage::StorageFolder^ DestinationFolder
            {
                Windows::Storage::StorageFolder^ get()
                {
                    return destinationFolder;
                };
            }

            /// <summary>
            /// List of scanned files
            /// </summary>
            property Windows::Foundation::Collections::IObservableVector<FileItem^>^ FileList 
            {
                 Windows::Foundation::Collections::IObservableVector<FileItem^>^ get() 
                 { 
                     return scannedFileList; 
                 };
            }
            
            /// <summary>
            /// Size of scanned files list
            /// </summary>
            property unsigned int FileListSize 
            {
                 unsigned int get() 
                 { 
                     return scannedFileList->Size; 
                 };
            }

            /// <summary>
            /// Poperty for specifying if the scenario is running or not
            /// </summary>
            property bool ScenarioRunning
            {
                bool get()
                {
                    return scenarioRunning;
                };
                void set(bool value)
                {
                    scenarioRunning = value;
                     OnPropertyChanged("ScenarioRunning");
                     // Raise property change event of IsScanningAllowed property, as IsScanningAllowed property is dependent on ScenarioRunning property
                     OnPropertyChanged("IsScanningAllowed");
                };
            }

            /// <summary>
            /// Adds the name of the given storage file name to the FileItem list
            /// </summary>
            /// <param name="file">Storage file for which the name need to be stored</param>
            void AddToFileList(Windows::Storage::StorageFile ^file)
            {
                scannedFileList->Append(ref new FileItem(file));
                OnPropertyChanged("FileListSize");
            }

            /// <summary>
            /// Clears items in the File list
            /// </summary>
            void ClearFileList()
            {
                scannedFileList->Clear();
                OnPropertyChanged("FileListSize");
            }
            
            /// <summary>
            ///  Property for specifying if the start scenario button is to be enabled or not
            /// </summary>
            property bool IsScanningAllowed
            {
                bool get()
                {
                    return ((currentScanContext->CurrentScannerDeviceId != nullptr) && !scenarioRunning);
                };
            }

            /// <summary>
            /// Even Handler for property change event
            /// </summary>
            void OnPropertyChanged(_In_ Platform::String^ propertyName)
            {
                PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
            }
            
            virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;
        private:
            ~ModelDataContext()
            {
                currentScanContext->PropertyChanged -= propertyToken;
            }
            
            static ScannerContext^ currentScanContext;			
            static Windows::Storage::StorageFolder^ destinationFolder;
            Platform::Collections::Vector<FileItem^>^ scannedFileList;			
            bool scenarioRunning;
            Windows::Foundation::EventRegistrationToken propertyToken;
        };
    }
}