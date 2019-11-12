/*
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
*/

// The snippet view is a custom control for displaying code examples. 

(function () {

    var snippetViewer = WinJS.Class.define(
        function (element, options) {
            var self = this;
            this._element = element || document.createElement("div");
            this._element.winControl = this;

            var currentContent = this._element.innerHTML;
            this._element.innerHTML = "";
            WinJS.Utilities.addClass(this._element, "codeSnippet");

            var buttonDiv = document.createElement("div");
            WinJS.Utilities.addClass(buttonDiv, "buttonDiv");
            this._element.appendChild(buttonDiv);

            options = options || {};

            if (!options.heading) {

                options.heading = "HTML"

            }
            var codeHeading = document.createElement("span");
            codeHeading.innerText = options.heading;
            buttonDiv.appendChild(codeHeading);

            this._copyButton = document.createElement("button");
            this._copyButton.innerText = WinJS.Resources.getString("CopyButtonText").value;
            buttonDiv.appendChild(this._copyButton);

            this._codeDisplay = document.createElement("pre");
            this._element.appendChild(this._codeDisplay);
            this._codeDisplay.innerHTML = currentContent;

            this._copyButton.addEventListener("click", function (eventInfo) {

                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.requestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.copy;
                dataPackage.setRtf(self._codeDisplay.outerText);
                dataPackage.setText(self._codeDisplay.outerText);
                dataPackage.setHtmlFormat(Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.createHtmlFormat(self._codeDisplay.outerHTML));
                Windows.ApplicationModel.DataTransfer.Clipboard.setContent(dataPackage);

            });

        },
    {

        refreshLayout: function () {

        },
        element: {
            get: function () { return this._element; }
        },


    });

    WinJS.Namespace.define("SampleUtilities", {
        SnippetViewer: snippetViewer
    });

})();