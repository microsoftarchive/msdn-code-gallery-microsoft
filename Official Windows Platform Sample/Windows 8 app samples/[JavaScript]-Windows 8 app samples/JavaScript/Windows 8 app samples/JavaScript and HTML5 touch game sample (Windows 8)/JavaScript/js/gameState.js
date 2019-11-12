//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var GameState = WinJS.Class.define(
    null,
{
    config: {
    // TODO: Adjust these values to configure the template itself
        frameRate: 20, // Set to 0 to have no update loop at all
        currentPage: "/html/homePage.html",
        gameName: "SDK Game Sample", // Used by share contract on scores page
    },

    // TODO: Replace these public settings exposed on the settings panel
    external: {
        playerName: "Player",
        soundVolume: 100,
        skillLevel: 0,
    },

    // TODO: Replace these values with state variables relevant for your game
    internal: {
        gamePaused: false,
        gamePhase: "ready",
        position: { x: 100, y: 100 },
        speed: { x: 5, y: 5 },
        score: 0,
        bounce: 0,
        bounceLimit: 10,
    },

    load: function (flag) {
        var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;

        if (flag === undefined || flag === "config") {
            var configString = roamingSettings.values["config"];
            if (configString) {
                this.config = JSON.parse(configString);
            } else {
                this.save("config"); // Save the defaults
            }
        }

        if (flag === undefined || flag === "external") {
            var externalString = roamingSettings.values["external"];
            if (externalString) {
                this.external = JSON.parse(externalString);
            } else {
                this.save("external"); // Save the defaults
            }
        }

        if (flag === undefined || flag === "internal") {
            var internalString = roamingSettings.values["internal"];
            if (internalString) {
                this.internal = JSON.parse(internalString);
            } else {
                this.save("internal"); // Save the defaults
            }
        }
    },

    save: function (flag) {
        var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;
        if (flag === undefined || flag === "config") {
            roamingSettings.values["config"] = JSON.stringify(this.config);
        }
        if (flag === undefined || flag === "external") {
            roamingSettings.values["external"] = JSON.stringify(this.external);
        }
        if (flag === undefined || flag === "internal") {
            roamingSettings.values["internal"] = JSON.stringify(this.internal);
        }
    }

});
