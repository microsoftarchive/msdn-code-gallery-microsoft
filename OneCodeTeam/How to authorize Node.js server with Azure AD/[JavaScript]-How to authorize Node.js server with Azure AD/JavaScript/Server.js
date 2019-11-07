'use strict';

var express = require('express');
var cookieParser = require('cookie-parser');
var cookieSession = require('cookie-session');

var authObj = require("./Auth.js").Create({
    tenant:"<your tenant name, e.g xxxx.onmicrosoft.com>",
    clientId:"<you appliction id in Azure AD>",
    secret:"<app key you copyied>",
    redirectUri:"http://localhost:3000/getAToken"
});

var app = express();
app.use(cookieParser('a deep secret'));
app.use(cookieSession({name: 'session',keys: [""]}));

app.get('/', function(req, res) {
    res.end('\
        <head>\
        <title>test</title>\
        </head>\
        <body>\
        <a href="./auth">Login</a>\
        </body>\
    ');
});

app.get('/auth', function(req, res) {
    authObj.loginIfNotAuth(req,res,function(){
        res.send("authed");
    });
});

app.get('/getAToken', function(req, res) {
    authObj.receiveToken(req,res,function(){
        res.redirect('/AuthInfo');
    });
});

app.get('/AuthInfo', function(req, res) {
    var sessionValue = req.session.authInfo;
    var authString = JSON.stringify(sessionValue);
    var userID = sessionValue.userId;
    var familyName = sessionValue.familyName;
    var givenName = sessionValue.givenName;

    res.end(`\
        <h1>UserID: ${userID}</h1>
        <h2>familyName: ${familyName}</h2>
        <h2>givenName: ${givenName}</h2>
        <h2>full data:</h2>
        <p>${authString}</p>
    `);
});

app.listen(3000);
console.log("listen 3000");