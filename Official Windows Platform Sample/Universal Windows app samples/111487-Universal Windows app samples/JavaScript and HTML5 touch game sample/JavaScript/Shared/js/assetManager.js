//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var AssetType = { image: 1, audio: 2, other: 3 };

var AssetManager = WinJS.Class.define(
    null,
{
    assets: null,
    loadedCount: 0,
    toLoadCount: 0,
    loadCompleteHandler: null,

    load: function (assetsToLoad, loadCompleteHandler) {
        this.assets = assetsToLoad;
        this.toLoadCount = Object.keys(assetsToLoad).length;
        this.loadCompleteHandler = loadCompleteHandler;
        var that = this;

        Object.keys(that.assets).forEach(function (asset, i) {
            switch (that.assets[asset].fileType) {
                case AssetType.image:
                    that.assets[asset].object = new Image();
                    that.assets[asset].object.addEventListener("load", function (okEvent) { that.loadCompleted(okEvent, i); }, false);
                    that.assets[asset].object.addEventListener("error", function (error) { that.loadFailed(error, i); }, false);
                    that.assets[asset].object.src = that.assets[Object.keys(that.assets)[i]].fileName;
                    break;
                case AssetType.audio:
                    that.assets[asset].object = new Audio(that.assets[Object.keys(that.assets)[i]].fileName);
                    if (that.assets[asset].object === null || !!!that.assets[asset].object.canPlayType) {
                        that.assets[asset].object = null;
                        that.loadFailed(null, i);
                        return;
                    }

                    that.assets[asset].object.addEventListener("canplaythrough", function (okEvent) { that.soundLoadCompleted(okEvent, i); }, false);
                    that.assets[asset].object.addEventListener("error", function (error) { that.loadFailed(error, i); }, false);

                    if (!!that.assets[asset].object.canPlayType("audio/mpeg") &&
                        !!that.assets[asset].object.canPlayType("audio/ogg") &&
                        !!that.assets[asset].object.canPlayType("audio/wav")) {
                        that.assets[asset].object = null;
                    }

                    if (that.assets[asset].object === null) {
                        that.loadFailed(null, i);
                    }

                    break;
            }
        });

    },

    loadCompleted: function (e, i) {
        this.loadedCount++;
        if (this.loadedCount === this.toLoadCount) {
            this.loadCompleteHandler.call();
        }
    },

    loadFailed: function (e, indexOfFailed) {
        // Sound could not be loaded, so set variable to null, so we don't try to play it
        this.assets[Object.keys(this.assets)[indexOfFailed]].object = null;
        this.loadedCount++;
        if (this.loadedCount === this.toLoadCount) {
            this.loadCompleteHandler.call();
        }
    },

    soundLoadCompleted: function (okEvent, indexOfCompleted) {
        if (this.assets[Object.keys(this.assets)[indexOfCompleted]].loop) {
            this.assets[Object.keys(this.assets)[indexOfCompleted]].object.addEventListener("ended", function () {
                this.assets[Object.keys(this.assets)[indexOfCompleted]].object.currentTime = 0;
                this.assets[Object.keys(this.assets)[indexOfCompleted]].object.play();
            }, false);
        };
        this.loadedCount++;

        if (this.loadedCount === this.toLoadCount) {
            this.loadCompleteHandler.call();
        }
    },

    playSound: function (sound) {
        try {
            // Reset the sound if it is currently playing
            sound.object.volume = GameManager.state.external.soundVolume / 100;
            if (!sound.object.ended) {
                sound.object.pause();
                sound.object.currentTime = 0;
            }
            sound.object.play();
            return true;
        }
        catch (e) {
            return false;
        }
    }

});
