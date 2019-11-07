/*

    This file has a bunch of utility and helper functions that didn't fit nicely into the other files.

*/

(function () {

    "use strict";

    // hooks up an appbar item to the navigation model.
    function setupNavItem(loc) {
        var id = '#' + loc;
        function nav() { ConferenceApp.Navigation.navigate(loc); }
        $(id).click(nav);
    }

    // creates the appbar using the WinJS AppBar control.
    function createAppBar(items) {
        var appItems = [];
        for (var id in items) {
            appItems.push({id:id, icon:' ', label: items[id], section: 'selection'});
        }
        appItems.push({ id: 'save', label: 'Save Report', icon: ' ', tooltip:'Save Trip Report', section: 'global' });
        var template = $("<div>").attr('id', 'appbar')[0];
        return new WinJS.UI.AppBar(template, { commands: appItems });
    }

    // Turns an OData Date literal into a JavaScript Date object.
    function parseODataDate(date) {
        var dateRegex = /^\/Date\((-?\d+)(\+|-)?(\d+)?\)\/$/;
        var result = date;
        var matches = dateRegex.exec(date);
        if(matches) {
            result = new Date(parseInt(matches[1], 10));
            if(matches[2]) {
                var offset = parseInt(matches[3], 10);
                if(matches[2] === "-") {
                    offset = -offset;
                }
                result.setUTCMinutes(result.getUTCMinutes() - offset);
            }
        }
        return result;
    }

    // Turns a JSON string back into JS Objects, also reviving Date literals.
    function parseJSON(string) {
        function dateReviver(key, value) {
            var a;
            if (typeof value === 'string') {
                a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/.exec(value);
                if (a) {
                    return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4],
                                    +a[5], +a[6]));
                }
            }
            return value;
        }

        return JSON.parse(string, dateReviver);
    }

    // Trim the whitespace around a string.
    function trim(string) {
        string = string || "";
        return string.replace(/^(\s|\u00A0)+|(\s|\u00A0)+$/g,'');
    }

    // Return the string as a URL (prefix it with http:// if it's not already)
    function ensureUrl(url) {
        if ((url.indexOf('http://') === 0) || (url.indexOf('https://') === 0)) {
            return url;
        }
        else if (url.length > 0) {
            return 'http://' + url;
        }
        else {
            return url;
        }
    }

    // True if the app is currently snapped.
    function isSnapped() {
        return Windows.UI.ViewManagement.ApplicationView.value === Windows.UI.ViewManagement.ApplicationViewState.snapped;
    }

    // True if the app has internet access.
    function isConnected() {
        var profile = Windows.Networking.Connectivity.NetworkInformation.getInternetConnectionProfile();
        if (profile) {
            return (profile.getNetworkConnectivityLevel() !== Windows.Networking.Connectivity.NetworkConnectivityLevel.none);
        }
        else {
            return false;
        }
    }

    // register a callback as both a click handler and a keydown of enter handler for the selected elements.
    function clickAndEnter(selector, callback) {
        $(selector).click(callback).listen('keydown', function (evt) {
            if (evt.keyCode === 13 || evt.keyCode === 32) {
                callback.call(this);
            }
        });
    }

    // Turns a room name into a map id for searching through the map SVG file
    function makeMapId(str) {
        return str.replace(/\s/g, '').toLowerCase();
    }

    // Push a tile update for a popular session
    function updateTile(title, abstract) {
        var Notifications = Windows.UI.Notifications;
        var tileXml = Notifications.TileUpdateManager.getTemplateContent(Notifications.TileTemplateType.tileWideText09);

        var tileTextAttributes = tileXml.getElementsByTagName("text");
        tileTextAttributes[0].appendChild(tileXml.createTextNode("Popular Session"));
        tileTextAttributes[1].appendChild(tileXml.createTextNode(title + " - " + abstract));

        var tileNotification = new Notifications.TileNotification(tileXml);
        Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);
    }

    // Launch the given URL in the default browser.
    function launchUrl(url) {
        url = ensureUrl(url);
        try {
            Windows.System.Launcher.launchUriAsync(new Windows.Foundation.Uri(url)).then();
        }
        catch(e) {
        }
    }

    // Update the "loading message" during initial activation.
    function updateLoadingMessage(msg) {
        $('#loadingMessage').text(msg);
    }

    // Makes a DOM element give visual feedback for touch.
    function enablePressFeedback(selector) {
        $(selector).forEach(function (element) {
            element.addEventListener("MSPointerDown", function () {
                WinJS.UI.Animation.pointerDown(element);
            }, false);
            element.addEventListener("MSPointerOver", function () {
                WinJS.UI.Animation.pointerDown(element);
            }, false);
            element.addEventListener("MSPointerOut", function () {
                WinJS.UI.Animation.pointerUp(element);
            }, false);
            element.addEventListener("MSPointerCancel", function () {
                WinJS.UI.Animation.pointerUp(element);
            }, false);
            element.addEventListener("MSPointerUp", function () {
                WinJS.UI.Animation.pointerUp(element);
            }, false);
            
        });
    }

    WinJS.Namespace.define("ConferenceApp.Util", {
        createAppBar: createAppBar,
        parseODataDate : parseODataDate,
        parseJSON: parseJSON,
        setupNavItem: setupNavItem,
        trim: trim,
        ensureUrl: ensureUrl,
        launchUrl: launchUrl,
        isSnapped: isSnapped,
        isConnected: isConnected,
        clickAndEnter: clickAndEnter,
        makeMapId: makeMapId,
        updateTile: updateTile,
        updateLoadingMessage: updateLoadingMessage,
        enablePressFeedback: enablePressFeedback
    });

})();
