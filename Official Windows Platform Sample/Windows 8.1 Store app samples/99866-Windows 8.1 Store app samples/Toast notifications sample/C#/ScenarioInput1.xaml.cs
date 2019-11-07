// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using NotificationsExtensions.ToastContent;
using System;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace ToastsSampleCS
{
    public sealed partial class ScenarioInput1 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public ScenarioInput1()
        {
            InitializeComponent();

            Scenario1DisplayToastText01.Click += (sender, e) => { DisplayTextToast(ToastTemplateType.ToastText01); };
            Scenario1DisplayToastText02.Click += (sender, e) => { DisplayTextToast(ToastTemplateType.ToastText02); };
            Scenario1DisplayToastText03.Click += (sender, e) => { DisplayTextToast(ToastTemplateType.ToastText03); };
            Scenario1DisplayToastText04.Click += (sender, e) => { DisplayTextToast(ToastTemplateType.ToastText04); };

            Scenario1DisplayToastText01String.Click += (sender, e) => { DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText01); };
            Scenario1DisplayToastText02String.Click += (sender, e) => { DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText02); };
            Scenario1DisplayToastText03String.Click += (sender, e) => { DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText03); };
            Scenario1DisplayToastText04String.Click += (sender, e) => { DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText04); };
        }

        void DisplayTextToast(ToastTemplateType templateType)
        {
            // Creates a toast using the notification object model, which is another project
            // in this solution.  For an example using Xml manipulation, see the function
            // DisplayToastUsingXmlManipulation below.
            IToastNotificationContent toastContent = null;

            if (templateType == ToastTemplateType.ToastText01)
            {
                IToastText01 templateContent = ToastContentFactory.CreateToastText01();
                templateContent.TextBodyWrap.Text = "Body text that wraps over three lines";
                toastContent = templateContent;
            }
            else if (templateType == ToastTemplateType.ToastText02)
            {
                IToastText02 templateContent = ToastContentFactory.CreateToastText02();
                templateContent.TextHeading.Text = "Heading text";
                templateContent.TextBodyWrap.Text = "Body text that wraps over two lines";
                toastContent = templateContent;
            }
            else if (templateType == ToastTemplateType.ToastText03)
            {
                IToastText03 templateContent = ToastContentFactory.CreateToastText03();
                templateContent.TextHeadingWrap.Text = "Heading text that is very long and wraps over two lines";
                templateContent.TextBody.Text = "Body text";
                toastContent = templateContent;
            }
            else if (templateType == ToastTemplateType.ToastText04)
            {
                IToastText04 templateContent = ToastContentFactory.CreateToastText04();
                templateContent.TextHeading.Text = "Heading text";
                templateContent.TextBody1.Text = "First body text";
                templateContent.TextBody2.Text = "Second body text";
                toastContent = templateContent;
            }

            rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage);

            // Create a toast, then create a ToastNotifier object to show
            // the toast
            ToastNotification toast = toastContent.CreateNotification();

            // If you have other applications in your package, you can specify the AppId of
            // the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        void DisplayTextToastWithStringManipulation(ToastTemplateType templateType)
        {
            string toastXmlString = String.Empty;
            if (templateType == ToastTemplateType.ToastText01)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='ToastText01'>"
                               + "<text id='1'>Body text that wraps over three lines</text>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }
            else if (templateType == ToastTemplateType.ToastText02)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='ToastText02'>"
                               + "<text id='1'>Heading text</text>"
                               + "<text id='2'>Body text that wraps over two lines</text>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }
            else if (templateType == ToastTemplateType.ToastText03)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='ToastText03'>"
                               + "<text id='1'>Heading text that is very long and wraps over two lines</text>"
                               + "<text id='2'>Body text</text>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }
            else if (templateType == ToastTemplateType.ToastText04)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='ToastText04'>"
                               + "<text id='1'>Heading text</text>"
                               + "<text id='2'>First body text</text>"
                               + "<text id='3'>Second body text</text>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }

            Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
            toastDOM.LoadXml(toastXmlString);

            rootPage.NotifyUser(toastDOM.GetXml(), NotifyType.StatusMessage);

            // Create a toast, then create a ToastNotifier object to show
            // the toast
            ToastNotification toast = new ToastNotification(toastDOM);

            // If you have other applications in your package, you can specify the AppId of
            // the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;
        }
        #endregion
    }
}
