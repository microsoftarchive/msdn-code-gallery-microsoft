//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/XSLTTransformation.html", {
        ready: function (element, options) {
            document.getElementById("scenario5XsltTransform").addEventListener("click", scenario5XsltTransform, false);

            xsltInitialize();
        }
    });

    function scenario5XsltTransform() {
        var xml = document.getElementById("xml");
        var xslt = document.getElementById("xslt");
        var result = document.getElementById("result");

        // get xml content
        xml.style.color = "black";
        var doc = new Windows.Data.Xml.Dom.XmlDocument;
        try
        {
            if (xml.value === null || xml.value.trim() === "") {
                result.value = "Source XML can't be empty";
                result.style.color = "red";
                return;
            }
            doc.loadXml(xml.value);
        } catch (error) {
            xml.style.color = "red";
            result.value = error.description;
            result.style.color = "red";
            return;
        }

        // get xsl content
        xslt.style.color = "black";
        var xslDoc = new Windows.Data.Xml.Dom.XmlDocument;
        try {
            if (xslt.value === null || xslt.value.trim() === "") {
                result.value = "XSL content can't be empty";
                result.style.color = "red";
                return;
            }
            xslDoc.loadXml(xslt.value);
        }catch(error){
            xslt.style.color = "red";
            result.value = error.description;
            result.style.color = "red";
            return;
        }

        // Transform xml according to the style sheet declaration specified in xslt filem
        try {
            var xsltProcessor = new Windows.Data.Xml.Xsl.XsltProcessor(xslDoc);
            result.value = xsltProcessor.transformToString(doc);
            result.style.color = "black";
        } catch (error) {
            result.value = error.description;
            result.style.color = "red";
            return;
        }
    }

    function xsltInitialize() {
        var xml = document.getElementById("xml");
        var xslt = document.getElementById("xslt");
        var result = document.getElementById("result");

        xml.value = "";
        xslt.value = "";
        xml.style.color = "black";
        xslt.style.color = "black";

        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("xsltTransform").then(function (xsltFolder) {
            // load xml file
            xsltFolder.getFileAsync("xmlContent.xml").done(function (file) {
                Windows.Data.Xml.Dom.XmlDocument.loadFromFileAsync(file).then(function (doc) {
                    xml.value = doc.getXml();
                });
            });

            // load xslt file
            xsltFolder.getFileAsync("xslContent.xml").then(function (file) {
                Windows.Data.Xml.Dom.XmlDocument.loadFromFileAsync(file).done(function (doc) {
                    xslt.value = doc.getXml();
                });
            });
        });

        result.value = "";
        result.style.color = "black";
    }
})();
