"use strict";
require('angular2-universal-polyfills/browser');
var core_1 = require('@angular/core');
var angular2_universal_1 = require('angular2-universal');
var app_module_1 = require('./app/app.module');
require('bootstrap');
var rootElemTagName = 'app'; // Update this if you change your root component selector
// Enable either Hot Module Reloading or production mode
if (module['hot']) {
    module['hot'].accept();
    module['hot'].dispose(function () {
        // Before restarting the app, we create a new root element and dispose the old one
        var oldRootElem = document.querySelector(rootElemTagName);
        var newRootElem = document.createElement(rootElemTagName);
        oldRootElem.parentNode.insertBefore(newRootElem, oldRootElem);
        platform.destroy();
    });
}
else {
    core_1.enableProdMode();
}
// Boot the application, either now or when the DOM content is loaded
var platform = angular2_universal_1.platformUniversalDynamic();
var bootApplication = function () { platform.bootstrapModule(app_module_1.AppModule); };
if (document.readyState === 'complete') {
    bootApplication();
}
else {
    document.addEventListener('DOMContentLoaded', bootApplication);
}
//# sourceMappingURL=boot-client.js.map