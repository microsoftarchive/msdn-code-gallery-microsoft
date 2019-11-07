'use strict';

var crypto = require('crypto');
var AuthenticationContext = require('adal-node').AuthenticationContext;

module.exports = {
    Create:function(params){
        var authObj = {
            tenant:params.tenant,
            clientId:params.clientId,
            secret:params.secret,
            redirectUri:params.redirectUri
        };

        authObj.authorityHostUrl = "https://login.windows.net";
        authObj.authorityUrl = authObj.authorityHostUrl + "/" + authObj.tenant;
        authObj.resource = "00000002-0000-0000-c000-000000000000";
        authObj.templateAuthzUrl = 'https://login.windows.net/' + authObj.tenant + '/oauth2/authorize?response_type=code&client_id=<client_id>&redirect_uri=<redirect_uri>&state=<state>&resource=<resource>';

        authObj.loginIfNotAuth = function(req,res,action){
            if(isAuthored(req))
            {
                if(isExpire(req))
                {
                    authObj.refreshToken(req,res,action);
                }
                else{
                    action();
                }
            }
            else
            {
                authWithAzureAD(res);
            }
        };

        authObj.receiveToken = function(req,res,action){
            if (req.cookies.authstate !== req.query.state) {
                res.send('error: state does not match');
                return;
            }

            var authenticationContext = new AuthenticationContext(authObj.authorityUrl);
            authenticationContext.acquireTokenWithAuthorizationCode(req.query.code, authObj.redirectUri, authObj.resource, authObj.clientId, authObj.secret, function(err, response) {
                var message = '';
                if (err) {
                    message = 'error: ' + err.message;
                    res.send(message);
                    return;
                }
                response.requestOn = Date.now();
                //set token to session
                req.session.authInfo = response;
                //do the action
                if(action){
                    action();
                }
            });
        };

        authObj.refreshToken = function(req,res,action) {
            var authenticationContext = new AuthenticationContext(authObj.authorityUrl);
            authenticationContext.acquireTokenWithRefreshToken(req.session.authInfo.refreshToken, authObj.clientId, authObj.secret, authObj.resource, function(refreshErr, refreshResponse) {
                if (refreshErr) {
                    var message = 'refreshError: ' + refreshErr.message;
                    res.send(message); 
                    return;
                }
                refreshResponse.requestOn = Date.now();
                //set token to session
                req.session.authInfo = refreshResponse;
                //do the action
                if(action){
                    action();
                }
            }); 
        };

        function authWithAzureAD(res){
            crypto.randomBytes(48, function(ex, buf) {
                var token = buf.toString('base64').replace(/\//g,'_').replace(/\+/g,'-');

                res.cookie('authstate', token);
                var authorizationUrl = createAuthorizationUrl(token);

                res.redirect(authorizationUrl);
            });
        }

        function isAuthored(req){
            return req.session.authInfo;
        }

        function isExpire(req){
            var now = Date.now();
            var requestOn = req.session.authInfo.requestOn;
            var expiresIn = req.session.authInfo.expiresIn * 1000;
            return requestOn + expiresIn >= Date.now();
        }

        function createAuthorizationUrl(state) {
            var authorizationUrl = authObj.templateAuthzUrl.replace('<client_id>', authObj.clientId)
                .replace('<redirect_uri>',authObj.redirectUri)
                .replace('<state>', state)
                .replace('<resource>', authObj.resource);
            return authorizationUrl;
        }

        return authObj;
    }
};