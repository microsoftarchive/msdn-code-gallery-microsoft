//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var flyout;

    var page = WinJS.UI.Pages.define("/html/scenario10.html", {
        ready: function (element, options) {
            flyout = document.getElementById('settingsDiv').winControl;
            document.getElementById("scenario10Show").addEventListener("click", show, false);
            flyout.addEventListener('aftershow', aftershow, false);
            showLanguages();
        },

        // primaryLanguageOverride is intended to persist between app sessions,
        // so this wouldn't be done in a real app.
        unload: function () {
            Windows.Globalization.ApplicationLanguages.primaryLanguageOverride = "";
        }
    });

    function show() {
        flyout.show();
    }

    function aftershow() {
        showLanguages();
        document.getElementById("settingsDiv").querySelector('.languageOverride').addEventListener("change", function () {
            showLanguages();
        });
    }

    function showLanguages() {
        var applicationLanguages = Windows.Globalization.ApplicationLanguages.languages;
        WinJS.log && WinJS.log("The application language(s) are: " + applicationLanguages, "sample", "status");
    }
})();

(function () {

    var appLanguages = Windows.Globalization.ApplicationLanguages;
    var languages = appLanguages.languages;
    var manifestLanguages = appLanguages.manifestLanguages;
    var override = appLanguages.primaryLanguageOverride;

    // A custom WinJS control for letting users override language
    // Default Option is the text displayed by default in the drop down (defaults to English text)
    var DisplayLanguageOverridePicker = WinJS.Class.define(function (element, options) {
        this.element = element;
        element.winControl = this;

        // Set defaults and then options
        this.defaultOption = 'Use my default language preferences';
        WinJS.UI.setOptions(this, options);

        // First show the default setting
        this._addOption(this.defaultOption, "");

        var that = this;

        // If there are app languages that the user speaks, show them next

        // Note: the first (non-override) language, if set as the primary language override
        // would give the same result as not having any primary language override. There's
        // still a difference, though: If the user changes their language preferences, the 
        // default setting (no override) would mean that the actual primary app language
        // could change. But if it's set as an override, then it will remain the primary
        // app language after the user changes their language preferences.

        languages.forEach(function (langTag, index) {
            that.addLanguage(langTag);
        });


        // Finally, add a divider followed by all the app manifest languages
        
        // NOTE: If an app is deployed using a bundle with resource packages, the following
        // addition to the list may not be useful: The set of languages returned by 
        // ApplicationLanguages.manifestLanguages will consist of only those manifest 
        // languages in the main app package or in the resource packages that are installed 
        // and registered for the current user. Language resource packages get deployed for 
        // the user if the language is in the user's profile. Therefore, the only difference 
        // from the set returned by ApplicationLanguages.languages above would depend on 
        // which languages are included in the main app package.

        this._addOption("——————", "", true);

        var list = new WinJS.Binding.List(manifestLanguages);
        list.sort(function (a, b) {
            var langA = new Windows.Globalization.Language(a).displayName;
            var langB = new Windows.Globalization.Language(b).displayName;
            return langA - langB;
        });
        list.forEach(function (langTag) {
            that.addLanguage(langTag);
        });

        this.element.addEventListener("change", this._change, false);

    }, {

        addLanguage: function (langTag) {
            var lang = new Windows.Globalization.Language(langTag);
            var text = (lang.nativeName === lang.displayName) ? lang.displayName : lang.displayName + " - " + lang.nativeName;
            this._addOption(text, langTag, false, (langTag && langTag === override));
        },

        _addOption: function (text, value, disabled, selected) {
            var option = document.createElement('option');
            option.value = value;
            option.selected = selected || false;
            option.disabled = disabled || false;
            option.textContent = text;
            this.element.appendChild(option);
        },

        // Sets the primary langauge override so resources, the host and other things attempt to load
        // according to it first.
        _change: function () {
            appLanguages.primaryLanguageOverride = this.value;
        }

    });


    WinJS.Namespace.define("Sample", {
        DisplayLanguageOverridePicker: DisplayLanguageOverridePicker
    });
})();
