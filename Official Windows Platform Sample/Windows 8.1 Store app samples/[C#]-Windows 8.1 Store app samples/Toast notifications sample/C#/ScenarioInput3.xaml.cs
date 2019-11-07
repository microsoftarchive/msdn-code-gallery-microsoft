// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using NotificationsExtensions.ToastContent;
using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace ToastsSampleCS
{
    public sealed partial class ScenarioInput3 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        const String ALT_TEXT = "Web image";

        public ScenarioInput3()
        {
            InitializeComponent();
            Scenario3DisplayToastImage01.Click += (sender, e) => { DisplayWebImageToast(ToastTemplateType.ToastImageAndText01); };
            Scenario3DisplayToastImage02.Click += (sender, e) => { DisplayWebImageToast(ToastTemplateType.ToastImageAndText02); };
            Scenario3DisplayToastImage03.Click += (sender, e) => { DisplayWebImageToast(ToastTemplateType.ToastImageAndText03); };
            Scenario3DisplayToastImage04.Click += (sender, e) => { DisplayWebImageToast(ToastTemplateType.ToastImageAndText04); };

            Scenario3DisplayToastImage01String.Click += (sender, e) => { DisplayWebImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText01); };
            Scenario3DisplayToastImage02String.Click += (sender, e) => { DisplayWebImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText02); };
            Scenario3DisplayToastImage03String.Click += (sender, e) => { DisplayWebImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText03); };
            Scenario3DisplayToastImage04String.Click += (sender, e) => { DisplayWebImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText04); };
        }

        void DisplayWebImageToast(ToastTemplateType templateType)
        {
            IToastNotificationContent toastContent = null;
            string toastImageSrc = Scenario3ImageUrl.Text;

            if (templateType == ToastTemplateType.ToastImageAndText01)
            {
                IToastImageAndText01 templateContent = ToastContentFactory.CreateToastImageAndText01();
                templateContent.TextBodyWrap.Text = "Body text that wraps";
                templateContent.Image.Src = toastImageSrc;
                templateContent.Image.Alt = ALT_TEXT;
                toastContent = templateContent;
            }
            else if (templateType == ToastTemplateType.ToastImageAndText02)
            {
                IToastImageAndText02 templateContent = ToastContentFactory.CreateToastImageAndText02();
                templateContent.TextHeading.Text = "Heading text";
                templateContent.TextBodyWrap.Text = "Body text that wraps.";
                templateContent.Image.Src = toastImageSrc;
                templateContent.Image.Alt = ALT_TEXT;
                toastContent = templateContent;
            }
            else if (templateType == ToastTemplateType.ToastImageAndText03)
            {
                IToastImageAndText03 templateContent = ToastContentFactory.CreateToastImageAndText03();
                templateContent.TextHeadingWrap.Text = "Heading text that wraps";
                templateContent.TextBody.Text = "Body text";
                templateContent.Image.Src = toastImageSrc;
                templateContent.Image.Alt = ALT_TEXT;
                toastContent = templateContent;
            }
            else if (templateType == ToastTemplateType.ToastImageAndText04)
            {
                IToastImageAndText04 templateContent = ToastContentFactory.CreateToastImageAndText04();
                templateContent.TextHeading.Text = "Heading text";
                templateContent.TextBody1.Text = "Body text";
                templateContent.TextBody2.Text = "Another body text";
                templateContent.Image.Src = toastImageSrc;
                templateContent.Image.Alt = ALT_TEXT;
                toastContent = templateContent;
            }

            rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage);

            // Create a toast from the Xml, then create a ToastNotifier object to show
            // the toast
            ToastNotification toast = toastContent.CreateNotification();

            // If you have other applications in your package, you can specify the AppId of
            // the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        void DisplayWebImageToastWithStringManipulation(ToastTemplateType templateType)
        {
            string toastXmlString = String.Empty;
            string toastImageSrc = Scenario3ImageUrl.Text;

            if (templateType == ToastTemplateType.ToastImageAndText01)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='toastImageAndText01'>"
                               + "<text id='1'>Body text that wraps over three lines</text>"
                               + "<image id='1' src='" + toastImageSrc + "' alt='" + ALT_TEXT + "'/>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }
            else if (templateType == ToastTemplateType.ToastImageAndText02)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='toastImageAndText02'>"
                               + "<text id='1'>Heading text</text>"
                               + "<text id='2'>Body text that wraps over two lines</text>"
                               + "<image id='1' src='" + toastImageSrc + "' alt='" + ALT_TEXT + "'/>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }
            else if (templateType == ToastTemplateType.ToastImageAndText03)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='toastImageAndText03'>"
                               + "<text id='1'>Heading text that wraps over two lines</text>"
                               + "<text id='2'>Body text</text>"
                               + "<image id='1' src='" + toastImageSrc + "' alt='" + ALT_TEXT + "'/>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }
            else if (templateType == ToastTemplateType.ToastImageAndText04)
            {
                toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='toastImageAndText04'>"
                               + "<text id='1'>Heading text</text>"
                               + "<text id='2'>First body text</text>"
                               + "<text id='3'>Second body text</text>"
                               + "<image id='1' src='" + toastImageSrc + "' alt='" + ALT_TEXT + "'/>"
                               + "</binding>"
                               + "</visual>"
                               + "</toast>";
            }


            Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                toastDOM.LoadXml(toastXmlString);

                rootPage.NotifyUser(toastDOM.GetXml(), NotifyType.StatusMessage);

                // Create a toast, then create a ToastNotifier object to show
                // the toast
                ToastNotification toast = new ToastNotification(toastDOM);

                // If you have other applications in your package, you can specify the AppId of
                // the app to create a ToastNotifier for that application
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch (Exception)
            {
                rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage);
            }
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
