/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Navigation;
using PhoneVoIPApp.BackEnd;

namespace PhoneVoIPApp.UI
{
    public class BaseViewModel : INotifyPropertyChanged, ICallControllerStatusListener
    {
        public BaseViewModel()
        {
            this.callStatus = CallStatus.None;
        }

        public string ApplicationTitle
        {
            get
            {
                return "VOIP CHATTERBOX";
            }
        }

        /// <summary>
        /// The status of a call, if any
        /// </summary>
        public CallStatus CallStatus
        {
            get
            {
                return this.callStatus;
            }

            private set
            {
                this.callStatus = value;

                // Raise the change notification so that the view can update itself
                this.OnCachedCallStatusUpdated();
            }
        }

        /// <summary>
        /// The status of a call, if any
        /// </summary>
        public CameraLocation CameraLocation
        {
            get
            {
                return this.cameraLocation;
            }

            private set
            {
                this.cameraLocation = value;
                this.OnCachedCameraLocationUpdated();
            }
        }

        /// <summary>
        /// The page corresponding to this view model
        /// </summary>
        public BasePage Page
        {
            get;
            set;
        }

        /// <summary>
        /// Initiate a navigation to the call status page
        /// </summary>
        public void NavigateToCallStatusPage()
        {
            this.Page.NavigationService.Navigate(new Uri("/CallStatusPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The page corresponding to this view model has been navigated to
        /// </summary>
        public virtual void OnNavigatedTo(NavigationEventArgs nee)
        {
            // Register to receive callbacks from the CallController
            BackgroundProcessController.Instance.CallController.SetStatusCallback(this);

            // Then, get and cache the call status
            this.CallStatus = BackgroundProcessController.Instance.CallController.CallStatus;
            this.CameraLocation = BackgroundProcessController.Instance.CallController.CameraLocation;
        }

        /// <summary>
        /// The page corresponding to this view model has been navigated to
        /// </summary>
        public virtual void OnNavigatedFrom(NavigationEventArgs nee)
        {
            // Unregister from receiving callbacks from the CallController
            BackgroundProcessController.Instance.CallController.SetStatusCallback(null);
        }

        /// <summary>
        /// This method is called by this class if the cached call status changes
        /// </summary>
        public virtual void OnCachedCallStatusUpdated()
        {
            // Nothing to do here in the base class
        }

        /// <summary>
        /// This method is called by this class if the cached camera location changes
        /// </summary>
        public virtual void OnCachedCameraLocationUpdated()
        {
            // Nothing to do here in the base class
        }

        /// <summary>
        /// This method is called by this class when a new call starts
        /// </summary>
        public virtual void OnNewCallStarted()
        {
            // By default, if a new call has started, navigate to the call status page
            this.NavigateToCallStatusPage();
        }

        /// <summary>
        /// The call status has changed
        /// </summary>
        /// <param name="newStatus"></param>
        public virtual void OnCallStatusChanged(CallStatus newStatus)
        {
            // Note, this call is called on some IPC thread - dispatch to the UI thread before doing anything else
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                BackEnd.CallStatus oldStatus = this.CallStatus;

                if (newStatus != oldStatus)
                {
                    // The call status has changed
                    this.CallStatus = newStatus;

                    // Check if a call has just started
                    if ((oldStatus == BackEnd.CallStatus.None) && (oldStatus != newStatus))
                    {
                        this.OnNewCallStarted();
                    }
                }
            });
        }

        /// <summary>
        /// The call audio output device has changed
        /// </summary>
        public virtual void OnCallAudioRouteChanged(CallAudioRoute newRoute)
        {
            // Note, this call is called on some IPC thread - dispatch to the UI thread before doing anything else
            // For now, nothing to be done here
        }

        /// <summary>
        /// Video/audio capture/render has started/stopped
        /// </summary>
        public virtual void OnMediaOperationsChanged(MediaOperations newOperations)
        {
            // Note, this call is called on some IPC thread - dispatch to the UI thread before doing anything else
            Debug.WriteLine("[BaseViewModel] OnMediaOperationsChanged. New operations = {0}", newOperations);
        }

        /// <summary>
        /// Camera location has changed
        /// </summary>
        public virtual void OnCameraLocationChanged(CameraLocation newCameraLocation)
        {
            // Note, this call is called on some IPC thread - dispatch to the UI thread before doing anything else
            Debug.WriteLine("[BaseViewModel] OnCameraLocationChanged. New location = {0}", newCameraLocation);
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                // Update the base cached copy 
                this.cameraLocation = newCameraLocation;
            });
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Private

        private CallStatus callStatus;

        private CameraLocation cameraLocation;

        #endregion
    }
}
