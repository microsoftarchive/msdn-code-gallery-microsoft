/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PhoneVoIPApp.UI
{
    /// <summary>
    /// The view model for the Call Status page
    /// </summary>
    public class CallStatusViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CallStatusViewModel()
            : base()
        {
            this.PageTitle = string.Empty;
            this.CallerName = string.Empty;
            this.CallerNumber = string.Empty;
            this.CallStartTime = string.Empty;
            this.CameraToggleButtonText = string.Empty;
            this.IsCameraToggleButtonEnabled = false;
            this.IsHoldButtonEnabled = false;
            this.IsHoldButtonChecked = false;
            this.HoldButtonText = string.Empty;
            this.IsHangUpButtonEnabled = false;
            this.IsEarpieceButtonEnabled = false;
            this.IsSpeakerButtonEnabled = false;
            this.IsBluetoothButtonEnabled = false;
            this.EarpieceButtonBorder = null;
            this.SpeakerButtonBorder = null;
            this.BluetoothButtonBorder = null;
            this.BigHeadPreviewUri = null;
            this.BigHeadVisibility = Visibility.Collapsed;
            this.LittleHeadPreviewUri = null;
            this.LittleHeadVisibility = Visibility.Collapsed;
        }

        private string pageTitle;
        public string PageTitle
        {
            get
            {
                return this.pageTitle;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.pageTitle = value;
                this.OnPropertyChanged("PageTitle");
            }
        }

        private string callerName;
        public string CallerName
        {
            get
            {
                return this.callerName;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.callerName = value;
                this.OnPropertyChanged("CallerName");
            }
        }

        private string callerNumber;
        public string CallerNumber
        {
            get
            {
                return this.callerNumber;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.callerNumber = value;
                this.OnPropertyChanged("CallerNumber");
            }
        }

        private string callStartTime;
        public string CallStartTime
        {
            get
            {
                return this.callStartTime;
            }

            set
            {
                this.callStartTime = value;
                this.OnPropertyChanged("CallStartTime");
            }
        }

        private bool isCameraToggleButtonEnabled;
        public bool IsCameraToggleButtonEnabled
        {
            get
            {
                return this.isCameraToggleButtonEnabled;
            }

            set
            {
                if (this.isCameraToggleButtonEnabled != value)
                {
                    this.isCameraToggleButtonEnabled = value;
                    this.OnPropertyChanged("IsCameraToggleButtonEnabled");
                }
            }
        }

        private string cameraToggleButtonText;
        public string CameraToggleButtonText
        {
            get
            {
                return this.cameraToggleButtonText;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.cameraToggleButtonText = value;
                this.OnPropertyChanged("CameraToggleButtonText");
            }
        }

        private bool isHoldButtonEnabled;
        public bool IsHoldButtonEnabled
        {
            get
            {
                return this.isHoldButtonEnabled;
            }

            set
            {
                if (this.isHoldButtonEnabled != value)
                {
                    this.isHoldButtonEnabled = value;
                    this.OnPropertyChanged("IsHoldButtonEnabled");
                }
            }
        }

        private bool isHoldButtonChecked;
        public bool IsHoldButtonChecked
        {
            get
            {
                return this.isHoldButtonChecked;
            }

            set
            {
                if (this.isHoldButtonChecked != value)
                {
                    this.isHoldButtonChecked = value;
                    this.OnPropertyChanged("IsHoldButtonChecked");
                }
            }
        }

        private string holdButtonText;
        public string HoldButtonText
        {
            get
            {
                return this.holdButtonText;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.holdButtonText = value;
                this.OnPropertyChanged("HoldButtonText");
            }
        }

        private bool isHangUpButtonEnabled;
        public bool IsHangUpButtonEnabled
        {
            get
            {
                return this.isHangUpButtonEnabled;
            }

            set
            {
                if (this.isHangUpButtonEnabled != value)
                {
                    this.isHangUpButtonEnabled = value;
                    this.OnPropertyChanged("IsHangUpButtonEnabled");
                }
            }
        }

        private bool isEarpieceButtonEnabled;
        public bool IsEarpieceButtonEnabled
        {
            get
            {
                return this.isEarpieceButtonEnabled;
            }

            set
            {
                if (this.isEarpieceButtonEnabled != value)
                {
                    this.isEarpieceButtonEnabled = value;
                    this.OnPropertyChanged("IsEarpieceButtonEnabled");
                }
            }
        }

        private bool isSpeakerButtonEnabled;
        public bool IsSpeakerButtonEnabled
        {
            get
            {
                return this.isSpeakerButtonEnabled;
            }

            set
            {
                if (this.isSpeakerButtonEnabled != value)
                {
                    this.isSpeakerButtonEnabled = value;
                    this.OnPropertyChanged("IsSpeakerButtonEnabled");
                }
            }
        }

        private bool isBluetoothButtonEnabled;
        public bool IsBluetoothButtonEnabled
        {
            get
            {
                return this.isBluetoothButtonEnabled;
            }

            set
            {
                if (this.isBluetoothButtonEnabled != value)
                {
                    this.isBluetoothButtonEnabled = value;
                    this.OnPropertyChanged("IsBluetoothButtonEnabled");
                }
            }
        }

        private Brush earpieceButtonBorder;
        public Brush EarpieceButtonBorder
        {
            get
            {
                return this.earpieceButtonBorder;
            }

            set
            {
                if (this.earpieceButtonBorder != value)
                {
                    this.earpieceButtonBorder = value;
                    this.OnPropertyChanged("EarpieceButtonBorder");
                }
            }
        }

        private Brush speakerButtonBorder;
        public Brush SpeakerButtonBorder
        {
            get
            {
                return this.speakerButtonBorder;
            }

            set
            {
                if (this.speakerButtonBorder != value)
                {
                    this.speakerButtonBorder = value;
                    this.OnPropertyChanged("SpeakerButtonBorder");
                }
            }
        }

        private Brush bluetoothButtonBorder;
        public Brush BluetoothButtonBorder
        {
            get
            {
                return this.bluetoothButtonBorder;
            }

            set
            {
                if (this.bluetoothButtonBorder != value)
                {
                    this.bluetoothButtonBorder = value;
                    this.OnPropertyChanged("BluetoothButtonBorder");
                }
            }
        }

        private Uri littleHeadPreviewUri;
        public Uri LittleHeadPreviewUri
        {
            get
            {
                return this.littleHeadPreviewUri;
            }

            set
            {
                if (this.littleHeadPreviewUri != value)
                {
                    this.littleHeadPreviewUri = value;
                    this.OnPropertyChanged("LittleHeadPreviewUri");
                }
            }
        }

        private Visibility littleHeadVisibility;
        public Visibility LittleHeadVisibility
        {
            get
            {
                return this.littleHeadVisibility;
            }

            set
            {
                if (this.littleHeadVisibility != value)
                {
                    this.littleHeadVisibility = value;
                    this.OnPropertyChanged("LittleHeadVisibility");
                }
            }
        }

        private Uri bigHeadPreviewUri;
        public Uri BigHeadPreviewUri
        {
            get
            {
                return this.bigHeadPreviewUri;
            }

            set
            {
                if (this.bigHeadPreviewUri != value)
                {
                    this.bigHeadPreviewUri = value;
                    this.OnPropertyChanged("BigHeadPreviewUri");
                }
            }
        }

        private Visibility bigHeadVisibility;
        public Visibility BigHeadVisibility
        {
            get
            {
                return this.bigHeadVisibility;
            }

            set
            {
                if (this.bigHeadVisibility != value)
                {
                    this.bigHeadVisibility = value;
                    this.OnPropertyChanged("BigHeadVisibility");
                }
            }
        }

        private int cameraRotation;
        public int CameraRotation
        {
            get
            {
                return this.cameraRotation;
            }

            set
            {
                if (this.cameraRotation != value)
                {
                    this.cameraRotation = value;
                    this.OnPropertyChanged("CameraRotation");
                }
            }
        }

        /// <summary>
        /// Toggle the held/resumed state of the call
        /// </summary>
        public void ToggleHold()
        {
            // We must have a call at this point
            Debug.Assert(base.CallStatus != BackEnd.CallStatus.None);

            if (base.CallStatus == BackEnd.CallStatus.Held)
            {
                // The call is held - resume it
                BackgroundProcessController.Instance.CallController.ResumeCall();
            }
            else
            {
                // The call is not held - put it on hold
                BackgroundProcessController.Instance.CallController.HoldCall();
            }

            // Change the hold button text to show that an action is in progress
            this.HoldButtonText = "Please wait";

            // Disable all call status buttons, so the user doesn't press them again and again.
            // The buttons will get re-enabled if required when the call status changes.
            this.DisableAllCallStatusButtons();
        }

        /// <summary>
        /// Toggle the front/rear camera during a call
        /// </summary>
        public void ToggleCamera()
        {
            Debug.Assert(base.CallStatus == BackEnd.CallStatus.InProgress);

            this.CameraToggleButtonText = "Please wait";
            
            BackgroundProcessController.Instance.CallController.ToggleCamera();

            this.DisableAllCallStatusButtons();
        }

        /// <summary>
        /// End the call
        /// </summary>
        public void HangUp()
        {
            // We must have a call at this point
            Debug.Assert(base.CallStatus != BackEnd.CallStatus.None);

            // End it
            BackgroundProcessController.Instance.CallController.EndCall();

            // Disable all call status buttons, so the user doesn't press them again and again.
            // The buttons will get re-enabled if required when the call status changes.
            this.DisableAllCallStatusButtons();
        }

        /// <summary>
        /// Change the audio route for this call
        /// </summary>
        public void SetAudioRoute(BackEnd.CallAudioRoute newRoute)
        {
            // We must have a call at this point
            Debug.Assert(base.CallStatus != BackEnd.CallStatus.None);

            // Change the audio route, if it has changed
            if (newRoute != BackgroundProcessController.Instance.CallController.AudioRoute)
            {
                BackgroundProcessController.Instance.CallController.AudioRoute = newRoute;

                // Disable all audio route buttons, so the user doesn't press them again and again.
                // The buttons will get re-enabled if required when the audio route changes.
                this.DisableAllAudioRouteButtons();
            }
        }

        /// <summary>
        /// The call status has changed
        /// </summary>
        public override void OnCachedCallStatusUpdated()
        {
            base.OnCachedCallStatusUpdated();

            // Load the latest call information
            this.LoadCallInformation();
        }

        /// <summary>
        /// The cached camera location has changed
        /// </summary>
        public override void OnCachedCameraLocationUpdated()
        {
            base.OnCachedCameraLocationUpdated();

            if(base.CameraLocation == BackEnd.CameraLocation.Front)
            {
                this.CameraToggleButtonText = CallStatusViewModel.CameraToggleBack;
                LittleHeadPreviewUri = frontFacingCameraStreamUri;
                CameraRotation = -90;
            }
            else
            {
                this.CameraToggleButtonText = CallStatusViewModel.CameraToggleFront;
                LittleHeadPreviewUri = rearFacingCameraStreamUri;
                CameraRotation = 90;
            }
        }

        /// <summary>
        /// The camera location value has changed
        /// </summary>


        /// <summary>
        /// The audio route or the available audio devices has changed
        /// </summary>
        /// <param name="newRoute"></param>
        public override void OnCallAudioRouteChanged(BackEnd.CallAudioRoute newRoute)
        {
            // Note, this call is called on some IPC thread - dispatch to the UI thread before doing anything else
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                // Call the base class method first
                base.OnCallAudioRouteChanged(newRoute);

                // Now, update the states of the various audio route buttons
                this.UpdateAudioRouteButtonStates(newRoute);
            });
        }

        public override void OnMediaOperationsChanged(BackEnd.MediaOperations newOperations)
        {
            // Note, this call is called on some IPC thread - dispatch to the UI thread before doing anything else
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                // Call the base class method first
                base.OnMediaOperationsChanged(newOperations);

                // Now, update the states of the various media elements

                // Are we rendering video?
                if ((newOperations & BackEnd.MediaOperations.VideoRender) != BackEnd.MediaOperations.None)
                {
                    // Yes, we are rendering video
                    Debug.WriteLine("[CallStatusViewModel.OnMediaOperationsChanged] => Showing BigHead");
                    this.BigHeadPreviewUri = CallStatusViewModel.renderStreamUri;
                    this.BigHeadVisibility = Visibility.Visible;
                }
                else
                {
                    // No, we are not rendering video
                    Debug.WriteLine("[CallStatusViewModel.OnMediaOperationsChanged] => Hiding BigHead");
                    this.BigHeadPreviewUri = null;
                    this.BigHeadVisibility = Visibility.Collapsed;
                }

                // Are we capturing video?
                if ((newOperations & BackEnd.MediaOperations.VideoCapture) != BackEnd.MediaOperations.None)
                {
                    // Yes, we are capturing video
                    Debug.WriteLine("[CallStatusViewModel.OnMediaOperationsChanged] => Showing LittleHead");
                    if (base.CameraLocation == BackEnd.CameraLocation.Front)
                    {
                        this.LittleHeadPreviewUri = CallStatusViewModel.frontFacingCameraStreamUri;
                    }
                    else
                    {
                        this.LittleHeadPreviewUri = CallStatusViewModel.rearFacingCameraStreamUri;
                    }
                    this.LittleHeadVisibility = Visibility.Visible;
                    this.IsCameraToggleButtonEnabled = true;
                }
                else
                {
                    // No, we are not capturing video
                    Debug.WriteLine("[CallStatusViewModel.OnMediaOperationsChanged] => Hiding LittleHead");
                    this.LittleHeadPreviewUri = null;
                    this.LittleHeadVisibility = Visibility.Collapsed;
                    this.IsCameraToggleButtonEnabled = false;
                }
            });
        }

        public override void OnCameraLocationChanged(BackEnd.CameraLocation newCameraLocation)
        {
            // Note, this call is called on some IPC thread - dispatch to the UI thread before doing anything else
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                base.OnCameraLocationChanged(newCameraLocation);
                if (newCameraLocation == BackEnd.CameraLocation.Front)
                {
                    this.CameraToggleButtonText = CallStatusViewModel.CameraToggleBack;
                    this.LittleHeadPreviewUri = frontFacingCameraStreamUri;
                    CameraRotation = -90;
                }
                else if (newCameraLocation == BackEnd.CameraLocation.Back)
                {
                    // we only have two supported camera locations
                    this.CameraToggleButtonText = CallStatusViewModel.CameraToggleFront;
                    this.LittleHeadPreviewUri = rearFacingCameraStreamUri;
                    CameraRotation = 90;
                }

                this.IsCameraToggleButtonEnabled = true;
                this.IsHangUpButtonEnabled = true;
                this.IsHoldButtonEnabled = true;
            });
        }

        /// <summary>
        /// A new call has started
        /// </summary>
        public override void OnNewCallStarted()
        {
            // Calling the base class navigate method will navigate to another instance of this page.
            // We don't want that - we'll just reload call information when the call status is updated.
            // So, there's nothing to do here.
        }

        /// <summary>
        /// This page has been navigated to
        /// </summary>
        public override void OnNavigatedTo(NavigationEventArgs nee)
        {
            // Call the base class method first
            base.OnNavigatedTo(nee);

            // Let the call controller know that we are showing video
            BackgroundProcessController.Instance.CallController.IsShowingVideo = true;
            BackgroundProcessController.Instance.CallController.IsRenderingVideo = true;
            if (CameraLocation == BackEnd.CameraLocation.Back)
            {
                CameraRotation = 90;
            }
            else
            {
                CameraRotation = -90;
            }
        }

        /// <summary>
        /// This page has been navigated away from
        /// </summary>
        public override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            // Call the base class method first
            base.OnNavigatedFrom(nee);

            // Let the call controller know that we are no longer showing video
            BackgroundProcessController.Instance.CallController.IsShowingVideo = false;
            BackgroundProcessController.Instance.CallController.IsRenderingVideo = false;
        }

        /// <summary>
        /// Update the information displayed on this page
        /// </summary>
        private void LoadCallInformation()
        {
            // Get the caller information from the back end
            this.CallerName = BackgroundProcessController.Instance.CallController.OtherPartyName;
            this.CallerNumber = BackgroundProcessController.Instance.CallController.OtherPartyNumber;

            // Update the UI state based on call state
            if (base.CallStatus == BackEnd.CallStatus.None)
            {
                this.PageTitle = string.IsNullOrEmpty(this.CallerName) ? "no call" : "call ended";
                this.IsHoldButtonEnabled = false;
                this.IsHoldButtonChecked = false;
                this.HoldButtonText = CallStatusViewModel.HoldOff;
                this.IsHangUpButtonEnabled = false;
                this.IsCameraToggleButtonEnabled = false;
            }
            else if (base.CallStatus == BackEnd.CallStatus.Held)
            {
                this.PageTitle = "call held";
                this.IsHoldButtonEnabled = true;
                this.IsHoldButtonChecked = true;
                this.HoldButtonText = CallStatusViewModel.HoldOn;
                this.IsHangUpButtonEnabled = true;
                this.IsCameraToggleButtonEnabled = false;
            }
            else if (base.CallStatus == BackEnd.CallStatus.InProgress)
            {
                this.PageTitle = "call in progress";
                this.IsHoldButtonEnabled = true;
                this.IsHoldButtonChecked = false;
                this.HoldButtonText = CallStatusViewModel.HoldOff;
                this.IsHangUpButtonEnabled = true;
            }
            else
            {
                Debug.Assert(false, string.Format("Unknown call state {0}", base.CallStatus));
            }

            if (base.CallStatus == BackEnd.CallStatus.None)
                this.CallStartTime = string.Empty;
            else
                this.CallStartTime = string.Format("(started {0:T})", BackgroundProcessController.Instance.CallController.CallStartTime);

            // Update the state of the various audio route buttons
            this.UpdateAudioRouteButtonStates(BackgroundProcessController.Instance.CallController.AudioRoute);
        }

        /// <summary>
        /// Disable all call status buttons
        /// </summary>
        private void DisableAllCallStatusButtons()
        {
            this.IsHoldButtonEnabled = false;
            this.IsHangUpButtonEnabled = false;
            this.IsCameraToggleButtonEnabled = false;
        }

        /// <summary>
        /// Update the state of the various audio route buttons
        /// </summary>
        private void UpdateAudioRouteButtonStates(BackEnd.CallAudioRoute audioRoute)
        {
            if (base.CallStatus == BackEnd.CallStatus.InProgress)
            {
                // There is a call in progress - update the audio routing button states

                // First, get the available audio devices
                BackEnd.CallAudioRoute availableDevices = BackgroundProcessController.Instance.CallController.AvailableAudioRoutes;

                // Enable/disable buttons based on availability
                this.IsEarpieceButtonEnabled = ((availableDevices & BackEnd.CallAudioRoute.Earpiece) != BackEnd.CallAudioRoute.None);
                this.IsSpeakerButtonEnabled = ((availableDevices & BackEnd.CallAudioRoute.Speakerphone) != BackEnd.CallAudioRoute.None);
                this.IsBluetoothButtonEnabled = ((availableDevices & BackEnd.CallAudioRoute.Bluetooth) != BackEnd.CallAudioRoute.None);

                // Set the border color of the currently selected audio route button
                Brush accentBrush = (Brush)App.Current.Resources["PhoneAccentBrush"];
                Brush borderBrush = (Brush)App.Current.Resources["PhoneForegroundBrush"];
                this.EarpieceButtonBorder = (audioRoute == BackEnd.CallAudioRoute.Earpiece) ? accentBrush : borderBrush;
                this.SpeakerButtonBorder = (audioRoute == BackEnd.CallAudioRoute.Speakerphone) ? accentBrush : borderBrush;
                this.BluetoothButtonBorder = (audioRoute == BackEnd.CallAudioRoute.Bluetooth) ? accentBrush : borderBrush;
            }
            else
            {
                // There is no call in progress - disable audio routing buttons
                this.DisableAllAudioRouteButtons();
            }
        }

        public void RootFrame_Obscured(object sender, ObscuredEventArgs e)
        {
            Debug.WriteLine("[App] Obscured");
            BackgroundProcessController.Instance.CallController.IsRenderingVideo = false;
        }

        public void RootFrame_Unobscured(object sender, EventArgs e)
        {
            Debug.WriteLine("[App] Unobscured");
            BackgroundProcessController.Instance.CallController.IsRenderingVideo = true;
        }

        /// <summary>
        /// Disable all audio route buttons
        /// </summary>
        private void DisableAllAudioRouteButtons()
        {
            this.IsEarpieceButtonEnabled = false;
            this.IsSpeakerButtonEnabled = false;
            this.IsBluetoothButtonEnabled = false;
        }

        // Hold button text
        private static readonly string HoldOn = "resume";
        private static readonly string HoldOff = "hold";

        private static readonly string CameraToggleFront = "front camera";
        private static readonly string CameraToggleBack = "back camera";

        // Camera stream URIs
        private static Uri frontFacingCameraStreamUri = new Uri("ms-media-stream-id:camera-FrontFacing");
        private static Uri rearFacingCameraStreamUri = new Uri("ms-media-stream-id:camera-RearFacing");

        // Render stream URI
        private static Uri renderStreamUri = new System.Uri("ms-media-stream-id:MediaStreamer-123");
    }
}
