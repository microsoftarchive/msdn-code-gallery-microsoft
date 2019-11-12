//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var Notifications = Windows.UI.Notifications;
    var ToastContent = NotificationsExtensions.ToastContent;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            var buttons = document.querySelectorAll("button.toastButton");
            for (var i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayTextToast, false);
            }

            buttons = document.querySelectorAll("button.toastStringButton");
            for (i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayTextToastWithStringManipulation, false);
            }

            buttons = document.querySelectorAll("button.toastXmlButton");
            for (i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayToastUsingXmlManipulation, false);
            }
        }
    });

    function displayTextToast(e) {
        // Get some data from the button the user clicked on.
        var targetButton = e.currentTarget;
        var toastTemplateName = targetButton.id;

        // Get the toast manager.
        var notificationManager = Notifications.ToastNotificationManager;

        // Create the toast content using the notifications content library.
        // Alternatively, you could use getTemplateContent from the notification
        // manager and manipulate Xml using the XmlDocument.  See the function
        // displayToastUsingXmlManipulation for an example.
        // Another option is to use strings directly. See the fuction
        // displayToastUsingStringManipulation for an example.
        var content;

        if (toastTemplateName === "toastText01") {
            content = ToastContent.ToastContentFactory.createToastText01();
            content.textBodyWrap.text = "Body text that wraps over three lines";
        } else if (toastTemplateName === "toastText02") {
            content = ToastContent.ToastContentFactory.createToastText02();
            content.textHeading.text = "Heading text";
            content.textBodyWrap.text = "Body text that wraps over two lines";
        } else if (toastTemplateName === "toastText03") {
            content = ToastContent.ToastContentFactory.createToastText03();
            content.textHeadingWrap.text = "Heading text that is very long and wraps over two lines";
            content.textBody.text = "Body text";
        } else if (toastTemplateName === "toastText04") {
            content = ToastContent.ToastContentFactory.createToastText04();
            content.textHeading.text = "Heading text";
            content.textBody1.text = "First body text";
            content.textBody2.text = "Second body text";
        }

        // Display the XML of the toast.
        WinJS.log && WinJS.log(content.getContent(), "sample", "status");

        // Create a toast, then create a ToastNotifier object
        // to send the toast.
        var toast = content.createNotification();

        notificationManager.createToastNotifier().show(toast);
    }

    function displayTextToastWithStringManipulation(e) {
        // Get some data from the button the user clicked on.
        var targetButton = e.currentTarget;
        var toastTemplateName = targetButton.id.substring(0, 11);

        // Get the toast manager.
        var notificationManager = Notifications.ToastNotificationManager;
        // Scheduled toasts use the same toast templates as all other kinds of toasts.
        var toastXmlString;

        if (toastTemplateName === "toastText01") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText01'>"
                           + "<text id='1'>Body text that wraps over three lines</text>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
        } else if (toastTemplateName === "toastText02") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText02'>"
                           + "<text id='1'>Heading text</text>"
                           + "<text id='2'>Body text that wraps over two lines</text>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
        } else if (toastTemplateName === "toastText03") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText03'>"
                           + "<text id='1'>Heading text that is very long and wraps over two lines</text>"
                           + "<text id='2'>Body text</text>"
                           + "</binding>"
                           + "</visual>"
                           + "</toast>";
        } else if (toastTemplateName === "toastText04") {
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

        // Create a toast, then create a ToastNotifier object
        // to send the toast.
        var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
        toastDOM.loadXml(toastXmlString);
        var toast = new Notifications.ToastNotification(toastDOM);

        // Display the XML of the toast.
        WinJS.log && WinJS.log(toastDOM.getXml(), "sample", "status");

        notificationManager.createToastNotifier().show(toast);
    }

    function displayToastUsingXmlManipulation(e) {
        // Get some data from the button the user clicked on.
        var targetButton = e.currentTarget;
        var toastTemplateName = targetButton.id.substring(0, 11);

        // Get the toast manager.
        var notificationManager = Notifications.ToastNotificationManager;

        // getTemplateContent returns a Windows.Data.Xml.Dom.XmlDocument object
        // containing the toast XML.
        var toastXml = notificationManager.getTemplateContent(Notifications.ToastTemplateType[toastTemplateName]);

        // You can use the methods from the XML document to specify
        // all the required parameters for the toast.
        var textNodes = toastXml.getElementsByTagName("text");
        textNodes.forEach(function (value, index) {
            var textNumber = index + 1;
            var text = "";
            for (var j = 0; j < 10; j++) {
                text += "Text input " + /*@static_cast(String)*/textNumber + " ";
            }
            value.appendChild(toastXml.createTextNode(text));
        });

        // Display the XML of the toast.
        WinJS.log && WinJS.log(toastXml.getXml(), "sample", "status");

        // Create a toast from the XML, then create a ToastNotifier object
        // to send the toast.
        var toast = new Notifications.ToastNotification(toastXml);

        notificationManager.createToastNotifier().show(toast);
    }

})();
