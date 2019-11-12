// These functions ensure OAuth data is accessible to our app pages

// This function manages querystring parameters
function getQueryStringParams() {
    var vars = [], hash;
    if (window.location.href.indexOf('?') >= 0) {
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
    }
    return vars;
}

// This function manages querystring parameters
function getQueryStringParam(name) {
    var params = getQueryStringParams();
    if (params.length > 0) {
        return params[name];
    }
    else {
        return "";
    }
}

// This function is used to get OAuth cookie
function getCookie(NameOfCookie) {

    // First we check to see if there is a cookie stored.
    // Otherwise the length of document.cookie would be zero.
    if (document.cookie.length > 0) {

        // Second we check to see if the cookie's name is stored in the
        // "document.cookie" object for the page.
        // Since more than one cookie can be set on a
        // single page it is possible that our cookie
        // is not present, even though the "document.cookie" object
        // is not just empty text.
        // If our cookie name is not present the value -1 is stored
        // in the variable called "begin".
        begin = document.cookie.indexOf(NameOfCookie + "=");
        if (begin != -1) // Note: != means "is not equal to"
        {

            // Our cookie was set. 
            // The value stored in the cookie is returned from the function.

            begin += NameOfCookie.length + 1;
            end = document.cookie.indexOf(";", begin);
            if (end == -1) end = document.cookie.length;
            return unescape(document.cookie.substring(begin, end));
        }
    }
    // Our cookie was not set. 
    // An empty string is returned from the function.
    return "";
}

// This function is used to set our OAuth cookie
function setCookie(NameOfCookie, value, expiredays) {

    // Three variables are used to set the new cookie. 
    // The name of the cookie, the value to be stored,
    // and finally the number of days until the cookie expires.
    // The first lines in the function convert 
    // the number of days to a valid date.
    var ExpireDate = new Date();
    ExpireDate.setTime(ExpireDate.getTime() + (expiredays * 24 * 3600 * 1000));

    // The next line stores the cookie, simply by assigning 
    // the values to the "document.cookie" object.
    // Note the date is converted to Greenwich Mean time using
    // the "toGMTstring()" function.
    document.cookie = NameOfCookie + "=" + escape(value) +
    ((expiredays == null) ? "" : "; expires=" + ExpireDate.toGMTString());
}

// This function is used to expire our OAuth cookie
function delCookie(NameOfCookie) {
    // The function simply checks to see if the cookie is set.
    // If so, the expiration date is set to Jan. 1st 1970.
    if (getCookie(NameOfCookie)) {
        document.cookie = NameOfCookie + "=" +
        "; expires=Thu, 01-Jan-70 00:00:01 GMT";
    }
}
