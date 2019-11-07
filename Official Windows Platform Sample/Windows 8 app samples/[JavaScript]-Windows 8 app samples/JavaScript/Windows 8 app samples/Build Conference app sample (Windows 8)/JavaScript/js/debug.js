(function() {

    "use strict";

    function log(msg) {
        /// <summary>Logs the given message to the console.</summary>
        /// <disable>JS2043.RemoveDebugCode</disable>
        console && console.log(msg);
    }

    WinJS.Namespace.define("ConferenceApp.Debug", {
        log: log
    });

})();
