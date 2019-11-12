"use strict";
require('angular2-universal-polyfills');
require('angular2-universal-patch');
require('zone.js');
var aspnet_prerendering_1 = require('aspnet-prerendering');
var core_1 = require('@angular/core');
var angular2_universal_1 = require('angular2-universal');
var app_module_1 = require('./app/app.module');
core_1.enableProdMode();
var platform = angular2_universal_1.platformNodeDynamic();
Object.defineProperty(exports, "__esModule", { value: true });
exports.default = aspnet_prerendering_1.createServerRenderer(function (params) {
    return new Promise(function (resolve, reject) {
        var requestZone = Zone.current.fork({
            name: 'angular-universal request',
            properties: {
                baseUrl: '/',
                requestUrl: params.url,
                originUrl: params.origin,
                preboot: false,
                document: '<app></app>'
            },
            onHandleError: function (parentZone, currentZone, targetZone, error) {
                // If any error occurs while rendering the module, reject the whole operation
                reject(error);
                return true;
            }
        });
        return requestZone.run(function () { return platform.serializeModule(app_module_1.AppModule); }).then(function (html) {
            resolve({ html: html });
        }, reject);
    });
});
//# sourceMappingURL=boot-server.js.map