//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var sdkSample = {};

(function () {
    function initialize() {
        // Add sdk sample header
        var header = document.getElementById("header");
        if (Boolean(header)) {
            var logo = document.createElement("img");
            var title = document.createElement("span");
            logo.src = "images/windows-sdk.png";
            logo.alt = "Windows Logo";
            title.innerText = "Windows SDK Samples";
            
            header.appendChild(logo);
            header.appendChild(title);
        }

        // Add sdk sample footer
        var footer = document.getElementById("footer");
        if (Boolean(footer)) {
            var footerLogo = document.createElement("img");
            var footerText = document.createElement("div");
            var links = document.createElement("div");
            var company = document.createElement("div");
            var companyText = document.createElement("span");
            var terms = document.createElement("a");
            var pipe = document.createElement("span");
            var trademarks = document.createElement("a");
            var privacy = document.createElement("a");

            footerLogo.src = "images/microsoft-sdk.png";
            footerLogo.alt = "Microsoft";
            company.className = "company";
            companyText.innerText = "© Microsoft Corporation. All rights reserved.";
            terms.innerText = "Terms of use";
            terms.href = "http://www.microsoft.com/About/Legal/EN/US/IntellectualProperty/Copyright/default.aspx";
            trademarks.innerText = "Trademarks";
            trademarks.href = "http://www.microsoft.com/About/Legal/EN/US/IntellectualProperty/Trademarks/EN-US.aspx";
            privacy.innerText = "Privacy Statement";
            privacy.href = "http://privacy.microsoft.com";
            links.className = "links";
            pipe.className = "pipe";

            links.appendChild(terms);
            links.appendChild(pipe);
            links.appendChild(trademarks);
            links.appendChild(pipe.cloneNode(true));
            links.appendChild(privacy);

            company.appendChild(companyText);
            footerText.appendChild(company);
            footerText.appendChild(links);

            footer.appendChild(footerLogo);
            footer.appendChild(footerText);

            footer.style.left = (window.innerWidth - footer.offsetWidth - 20) + "px";
//            footer.style.top = window.innerHeight - footer.offsetHeight;
        }
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();


