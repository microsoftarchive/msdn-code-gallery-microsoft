//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Media;

namespace Accessibility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
        {
            this.InitializeComponent();
            MediaElementContainer me = new MediaElementContainer(Scenario3Output);
            AutomationProperties.SetName(me, "CustomMediaElement");
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #region MediaElementContainer example code
        public class MediaElementContainer : ContentControl
        {
            const string mediaUri = @"Media/video.wmv";
            MediaElement mediaElement;
            public MediaElementContainer(Panel parent)
            {
                parent.Children.Add(this);
                mediaElement = new MediaElement() { Width = 200 };

                this.Content = mediaElement;
                mediaElement.Source = new Uri(parent.BaseUri, mediaUri);
                this.IsTabStop = true;
                AutomationProperties.SetAutomationId(mediaElement, "MediaElement1");
                AutomationProperties.SetName(mediaElement, "MediaElement");
            }


            protected override AutomationPeer OnCreateAutomationPeer()
            {
                return new MediaContainerAP(this, mediaElement);
            }
        }

        public class MediaContainerAP : FrameworkElementAutomationPeer, IRangeValueProvider, IToggleProvider
        {
            MediaElement _mediaElement;
            FrameworkElement _labeledBy;

            public MediaContainerAP(MediaElementContainer owner, MediaElement mediaElement)
                : base(owner)
            {
                _mediaElement = mediaElement;
            }

            public MediaContainerAP(MediaElementContainer owner, MediaElement mediaElement, FrameworkElement labeledBy)
                : this(owner, mediaElement)
            {
                _labeledBy = labeledBy;
            }

            protected override object GetPatternCore(PatternInterface patternInterface)
            {
                if (patternInterface == PatternInterface.RangeValue)
                {
                    return this;
                }
                else if (patternInterface == PatternInterface.Toggle)
                {
                    return this;
                }
                return null;
            }


            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Group;
            }

            protected override string GetLocalizedControlTypeCore()
            {
                return "Video";
            }

            protected override string GetClassNameCore()
            {
                return "MediaElementContainer";
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public double LargeChange
            {
                get { return _mediaElement.NaturalDuration.TimeSpan.TotalSeconds / 10; }
            }

            public double Maximum
            {
                get { return _mediaElement.NaturalDuration.TimeSpan.TotalSeconds; }
            }

            public double Minimum
            {
                get { return 0d; }
            }

            public void SetValue(double value)
            {
                if (value > Maximum || value < 0)
                {
                    throw new ArgumentException("Seeking to a point that is out of range", "value");
                }
                _mediaElement.Position = TimeSpan.FromSeconds(value);
            }

            public double SmallChange
            {
                get { return _mediaElement.NaturalDuration.TimeSpan.TotalSeconds / 50; }
            }

            public double Value
            {
                get { return _mediaElement.Position.TotalSeconds; }
            }

            public void Toggle()
            {
                if (_mediaElement.CurrentState == MediaElementState.Playing)
                {
                    if (_mediaElement.CanPause)
                    {
                        _mediaElement.Pause();
                    }
                    else
                    {
                        _mediaElement.Stop();
                    }
                }
                else if (_mediaElement.CurrentState == MediaElementState.Paused || _mediaElement.CurrentState == MediaElementState.Stopped)
                {
                    _mediaElement.Play();
                }
            }

            public Windows.UI.Xaml.Automation.ToggleState ToggleState
            {
                get
                {
                    if (_mediaElement.CurrentState == MediaElementState.Playing)
                    {
                        return Windows.UI.Xaml.Automation.ToggleState.On;
                    }
                    else if (_mediaElement.CurrentState == MediaElementState.Stopped || _mediaElement.CurrentState == MediaElementState.Paused)
                    {
                        return Windows.UI.Xaml.Automation.ToggleState.Off;
                    }
                    else
                    {
                        return Windows.UI.Xaml.Automation.ToggleState.Indeterminate;
                    }
                }
            }

        }
        #endregion

    }
}
