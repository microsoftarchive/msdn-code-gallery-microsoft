"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
require("rxjs/add/operator/toPromise");
var AuthService = (function () {
    function AuthService(http) {
        this.http = http;
        this.tokeyKey = "token";
    }
    AuthService.prototype.login = function (userName, password) {
        return this.http.post("/api/TokenAuth", { Username: userName, Password: password }).toPromise()
            .then(function (response) {
            var result = response.json();
            if (result.State == 1) {
                var json = result.Data;
                sessionStorage.setItem("token", json.accessToken);
            }
            return result;
        })
            .catch(this.handleError);
    };
    AuthService.prototype.logout = function () { };
    AuthService.prototype.checkLogin = function () {
        var token = sessionStorage.getItem(this.tokeyKey);
        return token != null;
    };
    AuthService.prototype.getUserInfo = function () {
        return this.authGet("/api/TokenAuth");
    };
    AuthService.prototype.authPost = function (url, body) {
        var headers = this.initAuthHeaders();
        return this.http.post(url, body, { headers: headers }).toPromise()
            .then(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    AuthService.prototype.authGet = function (url) {
        var headers = this.initAuthHeaders();
        return this.http.get(url, { headers: headers }).toPromise()
            .then(function (response) { return response.json(); })
            .catch(this.handleError);
    };
    AuthService.prototype.getLocalToken = function () {
        if (!this.token) {
            this.token = sessionStorage.getItem(this.tokeyKey);
        }
        return this.token;
    };
    AuthService.prototype.initAuthHeaders = function () {
        var token = this.getLocalToken();
        if (token == null)
            throw "No token";
        var headers = new http_1.Headers();
        headers.append("Authorization", "Bearer " + token);
        return headers;
    };
    AuthService.prototype.handleError = function (error) {
        console.error('An error occurred', error);
        return Promise.reject(error.message || error);
    };
    AuthService = __decorate([
        core_1.Injectable(), 
        __metadata('design:paramtypes', [http_1.Http])
    ], AuthService);
    return AuthService;
}());
exports.AuthService = AuthService;
//# sourceMappingURL=auth.service.js.map