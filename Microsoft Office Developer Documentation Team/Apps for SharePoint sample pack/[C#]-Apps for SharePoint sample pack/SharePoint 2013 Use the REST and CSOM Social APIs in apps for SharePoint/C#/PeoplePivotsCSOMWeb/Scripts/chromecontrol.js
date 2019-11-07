var PeoplePivot = window.PeoplePivot || {};

PeoplePivot.ChromeControl = function () {

     render = function () {
        var options = {
            "appIconUrl": "../Images/AppIcon.png",
            "appTitle": "People Pivots CSOM",
            };

        var nav = new SP.UI.Controls.Navigation(
                                "chrome_ctrl_placeholder",
                                options
                          );
        nav.setVisible(true);
    },

    getQueryStringParameter = function (p) {
        var params =
            document.URL.split("?")[1].split("&");
        var strParams = "";
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split("=");
            if (singleParam[0] == p)
                return singleParam[1];
        }
    }

    return {
        render: render
    }
}();

$(document).ready(function () {
    PeoplePivot.ChromeControl.render();
});