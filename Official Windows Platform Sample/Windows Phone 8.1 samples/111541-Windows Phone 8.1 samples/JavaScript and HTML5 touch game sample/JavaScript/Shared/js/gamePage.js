//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function ready(element, options) {

        // Stop previous loop if it is running already
        if (GameManager.gameId !== null) {
            stopGameLoop();
        }

        WinJS.UI.processAll(element)
            .done(function () {
                gamePage.enableAppBarGameButtons();

                if (GameManager.gameId === null) {
                    // Set up game area
                    gameCanvas.width = window.innerWidth;
                    gameCanvas.height = window.innerHeight;

                    // Initialize update loop
                    if (GameManager.state.config.frameRate > 0) {
                        updateTimer.reset(updateLoop, GameManager.state.config.frameRate);
                    }

                    // Initialize draw loop
                    GameManager.gameId = window.requestAnimationFrame(gamePage.renderLoop);

                    // Set up touch panel
                    GameManager.touchPanel.initialize(touchCanvas, GameManager.game.doTouch);

                    // Prepare game for first-time showing
                    GameManager.game.showFirst();
                }
            });
    }

    function unload(e) {
        gamePage.disableAppBarGameButtons();

        // Stop previous loop if it is running
        if (GameManager.gameId !== null) {
            stopGameLoop();
        }
    }

    // Handle showing and hiding game buttons from the app bar
    function enableAppBarGameButtons() {
        WinJS.Utilities.removeClass(document.getElementById("newgame"), "game-button");
        WinJS.Utilities.removeClass(document.getElementById("pause"), "game-button");
    }

    function disableAppBarGameButtons() {
        WinJS.Utilities.addClass(document.getElementById("newgame"), "game-button");
        WinJS.Utilities.addClass(document.getElementById("pause"), "game-button");
    }


    // Stop drawing loop for the game
    function stopGameLoop() {
        window.cancelAnimationFrame(GameManager.gameId);
        GameManager.gameId = null;
    }

    var updateTimer = new FrameTimer();

    function renderLoop() {
        if (typeof gameCanvas !== "undefined") {
            GameManager.game.draw();
            window.requestAnimationFrame(renderLoop);
        }
    }

    function updateLoop() {
        if (typeof gameCanvas !== "undefined") {
            GameManager.game.update();
        }
    }

    WinJS.UI.Pages.define("/html/gamePage.html", {
        ready: ready,
        unload: unload
    });

    WinJS.Namespace.define("gamePage", {
        renderLoop: renderLoop,
        updateLoop: updateLoop,
        updateTimer: updateTimer,
        enableAppBarGameButtons: enableAppBarGameButtons,
        disableAppBarGameButtons: disableAppBarGameButtons
    });

})();
