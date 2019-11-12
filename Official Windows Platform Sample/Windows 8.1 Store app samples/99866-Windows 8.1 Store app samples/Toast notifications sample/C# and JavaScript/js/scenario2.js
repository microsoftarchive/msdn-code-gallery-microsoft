//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            var buttons = document.querySelectorAll("button.toastButton");
            for (var i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayPackageImageToast, false);
            }

            buttons = document.querySelectorAll("button.toastStringButton");
            for (i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayPackageImageToastWithStringManipulation, false);
            }
        }
    });

    var Notifications = Windows.UI.Notifications;
    var ToastContent = NotificationsExtensions.ToastContent;
    var imageSrc = "images/toastImageAndText.png";
    var altText = "Placeholder image";

    function displayPackageImageToast(e) {
        var targetButton = e.currentTarget;
        var templateName = targetButton.id;

        // Get the toast manager for the current app.
        var notificationManager = Notifications.ToastNotificationManager;

        // Create the toast content using the notifications content library.
        var content;

        if (templateName === "toastImageAndText01") {
            content = ToastContent.ToastContentFactory.createToastImageAndText01();
            content.textBodyWrap.text = "Body text that wraps over three lines";
        } else if (templateName === "toastImageAndText02") {
            content = ToastContent.ToastContentFactory.createToastImageAndText02();
            content.textHeading.text = "Heading text";
            content.textBodyWrap.text = "Body text that wraps over two lines";
        } else if (templateName === "toastImageAndText03") {
            content = ToastContent.ToastContentFactory.createToastImageAndText03();
            content.textHeadingWrap.text = "Heading text that wraps over two lines";
            content.textBody.text = "Body text";
        } else if (templateName === "toastImageAndText04") {
            content = ToastContent.ToastContentFactory.createToastImageAndText04();
            content.textHeading.text = "Heading text";
            content.textBody1.text = "First body text";
            content.textBody2.text = "Second body text";
        }

        content.image.src = imageSrc;
        content.image.alt = altText;

        // Display the XML of the toast.
        WinJS.log && WinJS.log(content.getContent(), "sample", "status");

        // Create a toast, then create a ToastNotifier object
        // to send the toast.
        var toast = content.createNotification();

        notificationManager.createToastNotifier().show(toast);
    }

    function displayPackageImageToastWithStringManipulation(e) {
        var targetButton = e.currentTarget;
        var templateName = targetButton.id.substring(0, 19);

        // Get the toast manager for the current app.
        var notificationManager = Notifications.ToastNotificationManager;

        var toastXmlString;

        if (templateName === "toastImageAndText01") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='toastImageAndText01'>"
                           + "<text id='1'>Body text that wraps over three lines</text>"
                           + "<image id='1' src='" + imageSrc + "' alt='" + altText + "'/>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
        } else if (templateName === "toastImageAndText02") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='toastImageAndText02'>"
                           + "<text id='1'>Heading text</text>"
                           + "<text id='2'>Body text that wraps over two lines</text>"
                           + "<image id='1' src='" + imageSrc + "' alt='" + altText + "'/>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
        } else if (templateName === "toastImageAndText03") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='toastImageAndText03'>"
                           + "<text id='1'>Heading text that wraps over two lines</text>"
                           + "<text id='2'>Body text</text>"
                           + "<image id='1' src='" + imageSrc + "' alt='" + altText + "'/>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
        } else if (templateName === "toastImageAndText04") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='toastImageAndText04'>"
                           + "<text id='1'>Heading text</text>"
                           + "<text id='2'>First body text</text>"
                           + "<text id='3'>Second body text</text>"
                           + "<image id='1' src='" + imageSrc + "' alt='" + altText + "'/>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
        }

        // Create a toast, then create a ToastNotifier object
        // to send the toast.
        var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
        toastDOM.loadXml(toastXmlString);
        var toast = new Notifications.ToastNotification(toastDOM);

        // Display the XML of the toast.
        WinJS.log && WinJS.log(toastDOM.getXml(), "sample", "status");

        notificationManager.createToastNotifier().show(toast);
    }
})();
