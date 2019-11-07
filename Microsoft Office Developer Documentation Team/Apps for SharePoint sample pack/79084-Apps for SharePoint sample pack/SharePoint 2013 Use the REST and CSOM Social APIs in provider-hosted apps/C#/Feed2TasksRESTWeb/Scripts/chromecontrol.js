var FeedTasks = window.FeedTasks || {};

FeedTasks.ChromeControl = function () {

    render = function () {
        var options = {
            "appIconUrl": "../Images/AppIcon.png",
            "appTitle": "Create Tasks from Feed REST",
        };

        var nav = new SP.UI.Controls.Navigation(
                                "chrome_ctrl_placeholder",
                                options
                          );
        nav.setVisible(true);
    };

    return {
        render: render
    }
}();

$(document).ready(function () {
    FeedTasks.ChromeControl.render();
});