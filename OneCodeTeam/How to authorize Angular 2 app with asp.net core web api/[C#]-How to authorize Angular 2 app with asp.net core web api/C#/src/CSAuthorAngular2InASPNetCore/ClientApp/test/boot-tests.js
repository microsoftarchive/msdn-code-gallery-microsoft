"use strict";
// Load required polyfills and testing libraries
require('angular2-universal-polyfills');
require('zone.js/dist/long-stack-trace-zone');
require('zone.js/dist/proxy.js');
require('zone.js/dist/sync-test');
require('zone.js/dist/jasmine-patch');
require('zone.js/dist/async-test');
require('zone.js/dist/fake-async-test');
var testing = require('@angular/core/testing');
var testingBrowser = require('@angular/platform-browser-dynamic/testing');
// Prevent Karma from running prematurely
__karma__.loaded = function () { };
// First, initialize the Angular testing environment
testing.getTestBed().initTestEnvironment(testingBrowser.BrowserDynamicTestingModule, testingBrowser.platformBrowserDynamicTesting());
// Then we find all the tests
var context = require.context('../', true, /\.spec\.ts$/);
// And load the modules
context.keys().map(context);
// Finally, start Karma to run the tests
__karma__.start();
//# sourceMappingURL=boot-tests.js.map