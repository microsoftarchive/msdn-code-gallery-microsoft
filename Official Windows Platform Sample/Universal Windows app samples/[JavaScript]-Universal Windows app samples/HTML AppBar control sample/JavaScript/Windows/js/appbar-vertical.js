//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appBar;

    var page = WinJS.UI.Pages.define("/html/appbar-vertical.html", {
        ready: function (element, options) {
            document.getElementById("scenarioShowFullscreen").addEventListener("click", doShowFullscreen, false);
            document.getElementById("scenarioHideFullscreen").addEventListener("click", doHideFullscreen, false);

            appBar = document.getElementById("verticalAppBar").winControl;
            appBar.getCommandById("cmdAdd").addEventListener("click", doClickAdd, false);
            appBar.getCommandById("cmdRemove").addEventListener("click", doClickRemove, false);
            appBar.getCommandById("cmdDelete").addEventListener("click", doClickDelete, false);
            appBar.getCommandById("cmdCamera").addEventListener("click", doClickCamera, false);

            fillContent(); // generate fake content

         }
    });

    // Command button functions
    function doClickAdd() {
        WinJS.log && WinJS.log("Add button pressed", "sample", "status");
    }

    function doClickRemove() {
        WinJS.log && WinJS.log("Remove button pressed", "sample", "status");
    }

    function doClickDelete() {
        WinJS.log && WinJS.log("Delete button pressed", "sample", "status");
    }

    function doClickCamera() {
        WinJS.log && WinJS.log("Camera button pressed", "sample", "status");
    }

    function doShowFullscreen() {
        document.getElementById("verticalFullscreen").style.visibility = 'visible';
        // Show the AppBar in full screen mode
        appBar.disabled = false;
        appBar.sticky = true;
        appBar.show();
    }

    function doHideFullscreen() {
        document.getElementById("verticalFullscreen").style.visibility = 'hidden';
        // Hide the AppBar when not in full screen mode
        appBar.hide();
        appBar.sticky = false;
        appBar.disabled = true;
    }

    function fillContent() {
        document.getElementById("fakeContent").innerHTML = " \
            Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna. \
            Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus. \
            <br \><br \> \
            Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci. \
            Aenean nec lorem. In porttitor. Donec laoreet nonummy augue. \
            <br \><br \> \
            Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy. \
            Fusce aliquet pede non pede. Suspendisse dapibus lorem pellentesque magna. Integer nulla. \
            <br \><br \> \
            Donec blandit feugiat ligula. Donec hendrerit, felis et imperdiet euismod, purus ipsum pretium metus, in lacinia nulla nisl eget sapien. Donec ut est in lectus consequat consequat. \
            Etiam eget dui. Aliquam erat volutpat. Sed at lorem in nunc porta tristique. \
            <br \><br \> \
            Proin nec augue. Quisque aliquam tempor magna. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. \
            Nunc ac magna. Maecenas odio dolor, vulputate vel, auctor ac, accumsan id, felis. Pellentesque cursus sagittis felis. \
            Pellentesque porttitor, velit lacinia egestas auctor, diam eros tempus arcu, nec vulputate augue magna vel risus. Cras non magna vel ante adipiscing rhoncus. Vivamus a mi. \
            Morbi neque. Aliquam erat volutpat. Integer ultrices lobortis eros. \
            <br \><br \> \
            Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin semper, ante vitae sollicitudin posuere, metus quam iaculis nibh, vitae scelerisque nunc massa eget pede. Sed velit urna, interdum vel, ultricies vel, faucibus at, quam. \
            Donec elit est, consectetuer eget, consequat quis, tempus quis, wisi. In in nunc. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos. \
            Donec ullamcorper fringilla eros. Fusce in sapien eu purus dapibus commodo. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. \
            Cras faucibus condimentum odio. Sed ac ligula. Aliquam at eros. \
            <br \><br \> \
            Etiam at ligula et tellus ullamcorper ultrices. In fermentum, lorem non cursus porttitor, diam urna accumsan lacus, sed interdum wisi nibh nec nisl. Ut tincidunt volutpat urna. \
            Mauris eleifend nulla eget mauris. Sed cursus quam id felis. Curabitur posuere quam vel nibh. \
            <br \><br \> \
            Cras dapibus dapibus nisl. Vestibulum quis dolor a felis congue vehicula. Maecenas pede purus, tristique ac, tempus eget, egestas quis, mauris. \
            Curabitur non eros. Nullam hendrerit bibendum justo. Fusce iaculis, est quis lacinia pretium, pede metus molestie lacus, at gravida wisi ante at libero. \
            Quisque ornare placerat risus. Ut molestie magna at mi. Integer aliquet mauris et nibh. \
            <br \><br \> \
            Ut mattis ligula posuere velit. Nunc sagittis. Curabitur varius fringilla nisl. \
            <br \><br \> \
            Duis pretium mi euismod erat. Maecenas id augue. Nam vulputate. \
            <br \><br \> \
            Duis a quam non neque lobortis malesuada. Praesent euismod. Donec nulla augue, venenatis scelerisque, dapibus a, consequat at, leo. \
            <br \><br \> \
            Pellentesque libero lectus, tristique ac, consectetuer sit amet, imperdiet ut, justo. Sed aliquam odio vitae tortor. Proin hendrerit tempus arcu. \
            <br \><br \> \
            In hac habitasse platea dictumst. Suspendisse potenti. Vivamus vitae massa adipiscing est lacinia sodales. \
            <br \><br \> \
            Donec metus massa, mollis vel, tempus placerat, vestibulum condimentum, ligula. Nunc lacus metus, posuere eget, lacinia eu, varius quis, libero. Aliquam nonummy adipiscing augue. \
            <br \><br \> \
            Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna. \
            <br \><br \> \
            Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus. \
            <br \><br \> \
            Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci. \
            <br \><br \> \
            Aenean nec lorem. In porttitor. Donec laoreet nonummy augue. \
            <br \><br \> \
            Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.";
    }
})();
