//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var Game = WinJS.Class.define(
    null,
{
    // Colors currently in use by the game
    circleColor: null,
    scoreAndIndicatorColor: null,
    bouncesRemainingColor: null,

    // Convenience variables
    gameContext: null,
    stateHelper: null,
    state: null,
    settings: null,

    // Called when Game is first loaded
    initialize: function (state) {
        if (GameManager.gameId === null) {
            this.stateHelper = state;
            this.state = state.internal;
            this.settings = state.external;

            // Request that we be notified when the user changes their high-contrast settings via the 'contextchanged'
            // event. See: http://msdn.microsoft.com/en-us/library/windows/apps/hh465248.aspx
            var me = this;
            WinJS.Resources.addEventListener("contextchanged", function (e) {
                if (e.detail.qualifier === "Contrast") {
                    me.updateGameColors();
                }
            }, false);

            // Retrieve the colors we should be using
            this.updateGameColors();
        }
    },

    // Called to update the colors that are in use by the game to account for high-contrast themes
    updateGameColors: function () {
        // Get the colors we should be using from our string resources
        this.circleColor = WinJS.Resources.getString("colors/circle").value;
        this.scoreAndIndicatorColor = WinJS.Resources.getString("colors/scoreAndIndicator").value;
        this.bouncesRemainingColor = WinJS.Resources.getString("colors/bouncesRemaining").value;
    },

    // Called to get list of assets to pre-load
    getAssets: function () {
        // To add asset to a list of loading assets follow the the examples below
        var assets = {
            sndBounce: { object: null, fileName: "/sounds/bounce.wav", fileType: AssetType.audio, loop: false },
            //// backgroundImage: { object: null, fileName: "/images/background.jpg", fileType: AssetType.image },
            //// sndMusic: { object: null, fileName: "/sounds/music", fileType: AssetType.audio, loop: true }
        };
        return assets;
    },
    
    // Called the first time the game is shown
    showFirst: function () {
        // If game was previously running, pause it.
        if (this.state.gamePhase === "started") {
            this.pause();
        }

        // Note: gameCanvas is the name of the <canvas> in default.html
        this.gameContext = gameCanvas.getContext("2d");
    },

    // Called each time the game is shown
    show: function () {
    },

    // Called each time the game is hidden
    hide: function () {
        this.pause();
    },

    // Called to reset the game
    newGame: function () {
        GameManager.game.ready();
    },

    // Called when the game is being prepared to start
    ready: function () {
        // TODO: Replace with your own new game initialization logic
        this.state.gamePaused = false;

        this.state.gamePhase = "ready";
        switch (this.settings.skillLevel) {
            case 0:
                this.state.speed.x = 5;
                this.state.speed.y = 5;
                this.state.bounceLimit = 10;
                break;
            case 1:
                this.state.speed.x = 10;
                this.state.speed.y = 4;
                this.state.bounceLimit = 8;
                break;
            case 2:
                this.state.speed.x = 8;
                this.state.speed.y = 12;
                this.state.bounceLimit = 5;
                break;
        }
        this.state.position.x = 100;
        this.state.position.y = 100;
        this.state.score = 0;
        this.state.bounce = 0;
    },

    // Called when the game is started
    start: function () {
        this.state.gamePhase = "started";
    },
    
    // Called when the game is over
    end: function () {
        this.state.gamePhase = "ended";
        var newRank = GameManager.scoreHelper.newScore({ player: this.settings.playerName, score: this.state.score, skill: this.settings.skillLevel });
    },

    // Called when the game is paused
    pause: function () {
        this.state.gamePaused = true;
    },

    // Called when the game is un-paused
    unpause: function () {
        this.state.gamePaused = false;
    },

    // Called to toggle the pause state
    togglePause: function () {
        if (GameManager.game.state.gamePaused) {
            GameManager.game.unpause();
        } else {
            GameManager.game.pause();
        }
    },

    // Touch events... All touch events come in here before being passed onward based on type
    doTouch: function (touchType, e) {
        switch (touchType) {
            case "start": GameManager.game.touchStart(e); break;
            case "end": GameManager.game.touchEnd(e); break;
            case "move": GameManager.game.touchMove(e); break;
            case "cancel": GameManager.game.touchCancel(e); break;
        }
    },

    touchStart: function (e) {
        // TODO: Replace game logic
        // Touch screen to move from ready phase to start the game
        if (this.state.gamePhase === "ready") {
            this.start();
        }
    },

    touchEnd: function (e) {
        // TODO: Replace game logic.
        if (this.state.gamePhase === "started" && !this.state.gamePaused) {
            if (Math.sqrt(((e.x - this.state.position.x) * (e.x - this.state.position.x)) +
            ((e.y - this.state.position.y) * (e.y - this.state.position.y))) < 50) {
                this.state.score++;
            }
        }
    },
    
    touchMove: function (e) {
        // TODO: Add game logic
    },

    touchCancel: function (e) {
        // TODO: Add game logic
    },

    // Called before preferences panel or app bar is shown
    showExternalUI: function (e) {
        if (e.srcElement.id === "settingsDiv") {
            this.pause();
        }
    },

    // Called after preferences panel or app bar is hidden
    hideExternalUI: function (e) {
        if (e.srcElement.id === "settingsDiv") {
            this.unpause();
        }
    },

    // Called by settings panel to populate the current values of the settings
    getSettings: function () {
        // Note: The left side of these assignment operators refers to the setting controls in default.html
        // TODO: Update to match any changes in settings panel
        settingPlayerName.value = this.settings.playerName;
        settingSoundVolume.value = this.settings.soundVolume;
        for (var i = 0; i < settingSkillLevel.length; i++) {
            if (settingSkillLevel[i].value === "" + this.settings.skillLevel) {
                settingSkillLevel[i].checked = true;
            }
        }
    },

    // Called when changes are made on the settings panel
    setSettings: function () {
        // Note: The right side of these assignment operators refers to the controls in default.html
        // TODO: Update to match any changes in settings panel
        this.settings.playerName = settingPlayerName.value;
        this.settings.soundVolume = settingSoundVolume.value;
        // Changing the skill level re-starts the game
        var skill = 0;
        for (var i = 0; i < settingSkillLevel.length; i++) {
            if (settingSkillLevel[i].checked) {
                skill = parseInt(settingSkillLevel[i].value);
            }
        }
        if (this.settings.skillLevel !== skill) {
            // Update the skill level
            this.settings.skillLevel = skill;

            // Start a new game so high scores represent entire games at a given skill level only.
            this.ready();

            // Save state so that persisted skill-derived values match the skill selected
            this.stateHelper.save("internal");
        }

        // Save settings out
        this.stateHelper.save("external");
    },

    // Called when the app is suspended
    suspend: function () {
        this.pause();
        this.stateHelper.save();
    },

    // Called when the app is resumed
    resume: function () {
    },

    // Main game update loop
    update: function () {
        // TODO: Sample game logic to be replaced
        if (!this.state.gamePaused && this.state.gamePhase === "started") {
            if (this.state.position.x < 0 || this.state.position.x > gameCanvas.width) {
                this.state.speed.x = -this.state.speed.x;
                this.state.bounce++;

                // Play bounce sound
                GameManager.assetManager.playSound(GameManager.assetManager.assets.sndBounce);
            }
            if (this.state.position.y < 0 || this.state.position.y > gameCanvas.height) {
                this.state.speed.y = -this.state.speed.y;
                this.state.bounce++;

                // Play bounce sound
                GameManager.assetManager.playSound(GameManager.assetManager.assets.sndBounce);
            }
            this.state.position.x += this.state.speed.x;
            this.state.position.y += this.state.speed.y;
            
            // Check if game is over
            if (this.state.bounce >= this.state.bounceLimit) {
                this.end();
            }
        }
    },

    // Main game render loop
    draw: function () {
        this.gameContext.clearRect(0, 0, gameCanvas.width, gameCanvas.height);

        // TODO: Sample game rendering to be replaced

        // Draws on canvas a circle of radius 20 at the coordinates defined by position attribute
        this.gameContext.beginPath();
        this.gameContext.fillStyle = this.circleColor;
        this.gameContext.arc(this.state.position.x, this.state.position.y, 20, 0, Math.PI * 2, true);
        this.gameContext.closePath();
        this.gameContext.fill();

        // Draw the current score
        this.gameContext.fillStyle = this.scoreAndIndicatorColor;
        this.gameContext.font = "bold 48px Segoe UI";
        this.gameContext.textBaseline = "middle";
        this.gameContext.textAlign = "right";
        this.gameContext.fillText(this.state.score, gameCanvas.width - 5, 20);

        // Draw a ready or game over or paused indicator
        if (this.state.gamePhase === "ready") {
            this.gameContext.textAlign = "center";
            this.gameContext.fillText("READY", gameCanvas.width / 2, gameCanvas.height / 2);
        } else if (this.state.gamePhase === "ended") {
            this.gameContext.textAlign = "center";
            this.gameContext.fillText("GAME OVER", gameCanvas.width / 2, gameCanvas.height / 2);
        } else if (this.state.gamePaused) {
            this.gameContext.textAlign = "center";
            this.gameContext.fillText("PAUSED", gameCanvas.width / 2, gameCanvas.height / 2);
        }

        // Draw the number of bounces remaining
        this.gameContext.fillStyle = this.bouncesRemainingColor;
        this.gameContext.textAlign = "left";
        this.gameContext.fillText(Math.max(this.state.bounceLimit - this.state.bounce, 0), 5, 20);
    }
});
