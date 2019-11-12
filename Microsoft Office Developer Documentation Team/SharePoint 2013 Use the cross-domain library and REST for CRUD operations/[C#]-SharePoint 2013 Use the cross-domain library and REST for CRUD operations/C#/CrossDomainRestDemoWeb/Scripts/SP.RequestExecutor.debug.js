if (typeof window["SP"] == "undefined") {
    window["SP"] = new Object();
    window["SP"].__namespace = true;
}
if (window.Type && window.Type.registerNamespace)
    Type.registerNamespace('SP');
SP.PostMessageRequestInfo = function SP_PostMessageRequestInfo() {
};
SP.PostMessageResponseInfo = function SP_PostMessageResponseInfo() {
};
SP.RequestInfo = function SP_RequestInfo() {
};
SP.AppWebProxyRequestInfo = function SP_AppWebProxyRequestInfo() {
};
SP.ResponseInfo = function SP_ResponseInfo() {
};
SP.RequestExecutorMessageProcessorFormDigestInfo = function SP_RequestExecutorMessageProcessorFormDigestInfo() {
};
SP.RequestExecutorMessageProcessorInitInfo = function SP_RequestExecutorMessageProcessorInitInfo() {
};
SP.PostMessageCommands = function SP_PostMessageCommands() {
};
SP.ProxyWebRequestExecutorInternal = function SP_ProxyWebRequestExecutorInternal() {
};
SP.ProxyWebRequestExecutorInternal.processSuccessCallback = function SP_ProxyWebRequestExecutorInternal$processSuccessCallback(executor, responseInfo) {
    executor.$1c_1(responseInfo);
};
SP.ProxyWebRequestExecutorInternal.processErrorCallback = function SP_ProxyWebRequestExecutorInternal$processErrorCallback(executor, responseInfo, errorCode, errorMessage) {
    executor.$1b_1(responseInfo, errorCode, errorMessage);
};
SP.ProxyWebRequestExecutor = function SP_ProxyWebRequestExecutor($p0, $p1) {
    SP.ProxyWebRequestExecutor.initializeBase(this);
    this.$A_1 = $p0;
    this.$S_1 = $p1;
};
SP.ProxyWebRequestExecutor.prototype = {
    $A_1: null,
    $S_1: null,
    $1D_1: false,
    $1G_1: false,
    $1F_1: false,
    $8_1: null,
    get_aborted: function SP_ProxyWebRequestExecutor$get_aborted() {
        return this.$1D_1;
    },
    get_responseAvailable: function SP_ProxyWebRequestExecutor$get_responseAvailable() {
        return !!this.$8_1 && this.$8_1.responseAvailable;
    },
    get_responseData: function SP_ProxyWebRequestExecutor$get_responseData() {
        if (!this.$8_1) {
            return null;
        }
        return this.$8_1.body;
    },
    get_started: function SP_ProxyWebRequestExecutor$get_started() {
        return this.$1F_1;
    },
    get_statusCode: function SP_ProxyWebRequestExecutor$get_statusCode() {
        if (!this.$8_1) {
            return 0;
        }
        return this.$8_1.statusCode;
    },
    get_statusText: function SP_ProxyWebRequestExecutor$get_statusText() {
        if (!this.$8_1) {
            return null;
        }
        return this.$8_1.statusText;
    },
    get_timedOut: function SP_ProxyWebRequestExecutor$get_timedOut() {
        return this.$1G_1;
    },
    get_xml: function SP_ProxyWebRequestExecutor$get_xml() {
        return null;
    },
    abort: function SP_ProxyWebRequestExecutor$abort() {
        this.$1D_1 = true;
    },
    executeRequest: function SP_ProxyWebRequestExecutor$executeRequest() {
        var $v_0 = this.get_webRequest();
        var $v_1 = new SP.RequestExecutor(this.$A_1, this.$S_1);
        var $v_2 = new SP.RequestInfo();

        $v_2.body = $v_0.get_body();
        $v_2.headers = {};
        if ($v_0.get_headers()) {
            var $$dict_3 = $v_0.get_headers();

            for (var $$key_4 in $$dict_3) {
                var $v_3 = {
                    key: $$key_4,
                    value: $$dict_3[$$key_4]
                };
                var $v_4 = ($v_0.get_headers())[$v_3.key];

                if (typeof $v_4 !== 'function') {
                    $v_2.headers[$v_3.key] = ($v_0.get_headers())[$v_3.key];
                }
            }
        }
        $v_2.method = $v_0.get_httpVerb();
        $v_2.url = $v_0.get_url();
        $v_2.timeout = $v_0.get_timeout();
        $v_2.success = SP.RequestExecutorNative.getProxyWebRequestExecutorSuccessCallback(this);
        $v_2.error = SP.RequestExecutorNative.getProxyWebRequestExecutorErrorCallback(this);
        this.$1F_1 = true;
        $v_1.executeAsync($v_2);
    },
    $1c_1: function SP_ProxyWebRequestExecutor$$1c_1($p0) {
        this.$8_1 = $p0;
        (this.get_webRequest()).completed(Sys.EventArgs.Empty);
    },
    $1b_1: function SP_ProxyWebRequestExecutor$$1b_1($p0, $p1, $p2) {
        if ($p1 === SP.RequestExecutorErrors.requestAbortedOrTimedout) {
            this.$1G_1 = true;
        }
        this.$8_1 = $p0;
        this._SPError_ = $p2;
        (this.get_webRequest()).completed(Sys.EventArgs.Empty);
    },
    getAllResponseHeaders: function SP_ProxyWebRequestExecutor$getAllResponseHeaders() {
        if (!this.$8_1) {
            return null;
        }
        return this.$8_1.allResponseHeaders;
    },
    getResponseHeader: function SP_ProxyWebRequestExecutor$getResponseHeader($p0) {
        if (!this.$8_1 || !this.$8_1.headers) {
            return null;
        }
        if (!$p0) {
            return null;
        }
        return this.$8_1.headers[$p0.toUpperCase()];
    }
};
SP.ProxyWebRequestExecutorFactory = function SP_ProxyWebRequestExecutorFactory(url, viaUrl) {
    SP.RequestExecutorUtility.$5(url, 'url', 'string', true);
    SP.RequestExecutorUtility.$5(viaUrl, 'viaUrl', 'string', false);
    SP.RequestExecutorUtility.$a(url, 'url');
    var $v_0 = url.indexOf('?');

    if ($v_0 >= 0) {
        throw SP.RequestExecutorUtility.$G('url');
    }
    this.$A_0 = url;
    this.$S_0 = viaUrl;
};
SP.ProxyWebRequestExecutorFactory.prototype = {
    $A_0: null,
    $S_0: null,
    createWebRequestExecutor: function SP_ProxyWebRequestExecutorFactory$createWebRequestExecutor() {
        return new SP.ProxyWebRequestExecutor(this.$A_0, this.$S_0);
    }
};
SP.RequestExecutor = function SP_RequestExecutor(url, viaUrl) {
    SP.RequestExecutorUtility.$5(url, 'url', 'string', true);
    SP.RequestExecutorUtility.$5(viaUrl, 'viaUrl', 'string', false);
    if (SP.RequestExecutorUtility.$1(viaUrl)) {
        if (SP.RequestExecutorUtility.$1(url)) {
            throw SP.RequestExecutorUtility.$G('url');
        }
        var $v_0 = url.indexOf('?');

        if ($v_0 >= 0) {
            throw SP.RequestExecutorUtility.$G('url');
        }
        $v_0 = url.indexOf('#');
        if ($v_0 >= 0) {
            throw SP.RequestExecutorUtility.$G('url');
        }
        if (url.charAt(0) === '/') {
            url = SP.RequestExecutor.$b(window.location.href, url);
        }
        SP.RequestExecutorUtility.$a(url, 'url');
        if (url.charCodeAt(url.length - 1) === '/') {
            url = url.substr(0, url.length - 1);
        }
        url = url.toLowerCase();
        this.$A_0 = url;
        if (SP.RequestExecutorInternalSharedUtility.$O(url) === SP.RequestExecutorInternalSharedUtility.$O(window.location.href)) {
            this.$R_0 = 1;
        }
        else {
            this.$R_0 = 2;
            if (url.charAt(url.length - 1) !== '/') {
                url = url + '/';
            }
            this.$1E_0 = url;
        }
    }
    else {
        SP.RequestExecutorUtility.$a(url, 'url');
        this.$R_0 = 3;
        this.$A_0 = url;
        this.$S_0 = viaUrl;
    }
    this.formDigestHandlingEnabled = true;
    this.iFrameSourceUrl = '_layouts/15/AppWebProxy.aspx';
};
SP.RequestExecutor.$b = function SP_RequestExecutor$$b($p0, $p1) {
    SP.RequestExecutorUtility.$a($p0, 'baseUrl');
    if (($p1.substr(0, 8)).toLowerCase() === 'https://' || ($p1.substr(0, 7)).toLowerCase() === 'http://') {
        return $p1;
    }
    else if ($p1.charAt(0) === '/') {
        var $v_0 = $p0.indexOf('://');

        $v_0 = $p0.indexOf('/', $v_0 + 3);
        if ($v_0 > 0) {
            $p0 = $p0.substr(0, $v_0);
        }
        return $p0 + $p1;
    }
    else {
        if ($p0.charAt($p0.length - 1) !== '/') {
            $p0 = $p0 + '/';
        }
        return $p0 + $p1;
    }
};
SP.RequestExecutor.$O = function SP_RequestExecutor$$O($p0) {
    SP.RequestExecutorUtility.$a($p0, 'url');
    return SP.RequestExecutorInternalSharedUtility.$O($p0);
};
SP.RequestExecutor.$i = function SP_RequestExecutor$$i($p0) {
    return document.getElementById($p0);
};
SP.RequestExecutor.$1O = function SP_RequestExecutor$$1O($p0) {
    var $v_0;

    $v_0 = document.createElement('IFRAME');
    $v_0.src = $p0;
    $v_0.id = $p0;
    $v_0.style.display = 'none';
    var $v_1 = SP.RequestExecutorNative.getIFrameOnloadCallback($p0);

    if ($v_0.addEventListener) {
        $v_0.addEventListener('load', $v_1, false);
    }
    else if ($v_0.attachEvent) {
        $v_0.attachEvent('onload', $v_1);
    }
    else {
        throw SP.RequestExecutorUtility.$q();
    }
    document.body.appendChild($v_0);
    var $v_2 = window.setTimeout(function() {
        delete SP.RequestExecutor.$9[$p0];
        SP.RequestExecutor.$1J($p0);
    }, 8000);

    SP.RequestExecutor.$9[$p0] = $v_2;
    return $v_0;
};
SP.RequestExecutor.$1B = function SP_RequestExecutor$$1B($p0, $p1) {
    var $v_0 = SP.RequestExecutorUtility.$c($p0);

    $v_0 = SP.RequestExecutorUtility.$h($v_0);
    $v_0 = $v_0 + 'MS.SP.url' + '=' + encodeURIComponent($p1);
    return $v_0;
};
SP.RequestExecutor.$16 = function SP_RequestExecutor$$16($p0, $p1, $p2, $p3, $p4) {
    if (!SP.RequestExecutor.$X) {
        SP.RequestExecutor.$X = {};
    }
    if (!SP.RequestExecutor.$f) {
        SP.RequestExecutor.$f = {};
    }
    $p0 = $p0.toLowerCase();
    var $v_0 = new Date();

    if (!$p2 || $p3.method.toUpperCase() === 'GET' || !SP.RequestExecutorUtility.$0(SP.RequestExecutor.$X[$p0]) && SP.RequestExecutor.$X[$p0] > $v_0.getTime()) {
        $p4(SP.RequestExecutor.$f[$p0], $p3);
    }
    else {
        var $v_1 = SP.RequestExecutor.$j();

        $v_1.open('POST', $p1);
        $v_1.setRequestHeader('accept', 'application/json;odata=verbose');
        var $v_2 = 0;

        if ($p3.timeout > 0) {
            $v_2 = window.setTimeout(function() {
                $v_1.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
                try {
                    $v_1.abort();
                }
                catch ($$e_8) { }
                SP.RequestExecutor.$W($p3, SP.RequestExecutorErrors.requestAbortedOrTimedout, SP.RequestExecutorResources.getString('RE_RequestAbortedOrTimedout'));
            }, $p3.timeout);
        }
        $v_1.onreadystatechange = function() {
            if ($v_1.readyState === 4) {
                if ($v_2) {
                    window.clearTimeout($v_2);
                }
                $v_1.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
                if ($v_1.status !== 200 || !$v_1.getResponseHeader('content-type') || (($v_1.getResponseHeader('content-type')).toLowerCase()).indexOf('application/json') < 0) {
                    var $v_3 = SP.RequestExecutorResources.getString('RE_RequestUnexpectedResponseWithContentTypeAndStatus');

                    $v_3 = SP.RequestExecutorUtility.$N($v_3, $v_1.getResponseHeader('content-type'), $v_1.status.toString());
                    SP.RequestExecutor.$W($p3, SP.RequestExecutorErrors.unexpectedResponse, $v_3);
                }
                else {
                    var $v_4 = JSON.parse($v_1.responseText);

                    if (SP.RequestExecutorUtility.$0($v_4)) {
                        SP.RequestExecutor.$W($p3, SP.RequestExecutorErrors.unexpectedResponse, SP.RequestExecutorResources.getString('RE_RequestUnexpectedResponse'));
                    }
                    else {
                        var $v_5 = $v_4.d.GetContextWebInformation.FormDigestValue;
                        var $v_6 = $v_4.d.GetContextWebInformation.FormDigestTimeoutSeconds;
                        var $v_7 = (new Date()).getTime() + $v_6 * 750;

                        SP.RequestExecutor.$f[$p0] = $v_5;
                        SP.RequestExecutor.$X[$p0] = $v_7;
                        $p4($v_5, $p3);
                    }
                }
            }
        };
        $v_1.send('');
    }
};
SP.RequestExecutor.$W = function SP_RequestExecutor$$W($p0, $p1, $p2) {
    if ($p0.error) {
        var $v_0 = new SP.ResponseInfo();

        $v_0.state = $p0.state;
        $v_0.body = '';
        $p0.error($v_0, $p1, $p2);
    }
};
SP.RequestExecutor.$n = function SP_RequestExecutor$$n() {
    var $v_0 = window.postMessage;

    if (SP.RequestExecutorUtility.$7($v_0)) {
        throw SP.RequestExecutorUtility.$q();
    }
    $v_0 = window.JSON;
    if (SP.RequestExecutorUtility.$7($v_0) || SP.RequestExecutorUtility.$7($v_0.stringify) || SP.RequestExecutorUtility.$7($v_0.parse)) {
        throw SP.RequestExecutorUtility.$q();
    }
};
SP.RequestExecutor.$1N = function SP_RequestExecutor$$1N() {
    var $v_0 = window.ArrayBuffer;

    if (SP.RequestExecutorUtility.$7($v_0)) {
        throw SP.RequestExecutorUtility.$s();
    }
    $v_0 = window.Uint8Array;
    if (SP.RequestExecutorUtility.$7($v_0)) {
        throw SP.RequestExecutorUtility.$s();
    }
    if (SP.RequestExecutorUtility.$7(window.BlobBuilder) && SP.RequestExecutorUtility.$7(window.MozBlobBuilder) && SP.RequestExecutorUtility.$7(window.Blob)) {
        throw SP.RequestExecutorUtility.$s();
    }
};
SP.RequestExecutor.$B = function SP_RequestExecutor$$B($p0) {
    SP.RequestExecutorUtility.$5($p0.postMessageId, 'requestInfo.postMessageId', 'string', true);
    SP.RequestExecutorUtility.$5($p0.appWebProxyUrl, 'requestInfo.appWebProxyUrl', 'string', true);
    var $v_0 = SP.RequestExecutor.$i($p0.appWebProxyUrl);

    if (!$v_0) {
        throw SP.RequestExecutorUtility.$r();
    }
    if (!SP.RequestExecutor.$3) {
        SP.RequestExecutor.$3 = {};
    }
    SP.RequestExecutor.$3[$p0.postMessageId] = $p0;
    var $v_1 = new SP.PostMessageRequestInfo();

    $v_1.command = $p0.command;
    $v_1.url = $p0.requestInfo.url;
    $v_1.method = $p0.requestInfo.method;
    $v_1.body = $p0.requestInfo.body;
    $v_1.headers = $p0.requestInfo.headers;
    $v_1.postMessageId = $p0.postMessageId;
    $v_1.timeout = $p0.requestInfo.timeout;
    $v_1.binaryStringRequestBody = $p0.requestInfo.binaryStringRequestBody;
    $v_1.binaryStringResponseBody = $p0.requestInfo.binaryStringResponseBody;
    var $v_2 = JSON.stringify($v_1);
    var $v_3 = $p0.appWebProxyUrl;

    $v_0.contentWindow.postMessage($v_2, $v_3);
};
SP.RequestExecutor.internalProcessIFrameOnload = function SP_RequestExecutor$internalProcessIFrameOnload(appWebProxyUrl) {
    SP.RequestExecutor.$P[appWebProxyUrl] = true;
    if (!SP.RequestExecutorUtility.$0(SP.RequestExecutor.$9[appWebProxyUrl])) {
        window.clearTimeout(SP.RequestExecutor.$9[appWebProxyUrl]);
        delete SP.RequestExecutor.$9[appWebProxyUrl];
    }
    if (!SP.RequestExecutorUtility.$0(SP.RequestExecutor.$L[appWebProxyUrl])) {
        window.clearTimeout(SP.RequestExecutor.$L[appWebProxyUrl]);
        delete SP.RequestExecutor.$L[appWebProxyUrl];
    }
    SP.RequestExecutor.$g++;
    var $v_0 = new SP.AppWebProxyRequestInfo();

    $v_0.command = 'Ping';
    $v_0.postMessageId = SP.RequestExecutorInternalSharedUtility.$e + SP.RequestExecutor.$g.toString();
    $v_0.appWebProxyUrl = appWebProxyUrl;
    var $v_1 = window.setTimeout(function() {
        delete SP.RequestExecutor.$9[appWebProxyUrl];
        SP.RequestExecutor.$1f($v_0.postMessageId, appWebProxyUrl);
    }, 1000);

    $v_0.timeoutId = $v_1;
    $v_0.requestInfo = new SP.RequestInfo();
    SP.RequestExecutor.$3[$v_0.postMessageId] = $v_0;
    SP.RequestExecutor.$L[appWebProxyUrl] = $v_1;
    SP.RequestExecutor.$B($v_0);
};
SP.RequestExecutor.$1f = function SP_RequestExecutor$$1f($p0, $p1) {
    var $v_0 = SP.RequestExecutor.$3[$p0];

    if (SP.RequestExecutorUtility.$0($v_0)) {
        return;
    }
    if (SP.RequestExecutor.$K[$p1]) {
        return;
    }
    SP.RequestExecutor.$3[$p0] = null;
    delete SP.RequestExecutor.$3[$p0];
    SP.RequestExecutor.$1J($p1);
};
SP.RequestExecutor.$1J = function SP_RequestExecutor$$1J($p0) {
    var $v_0 = SP.RequestExecutor.$J[$p0];

    if (!SP.RequestExecutorUtility.$0($v_0) && !$v_0.closed) {
        SP.RequestExecutor.$o($p0);
    }
    else {
        if (SP.RequestExecutorUtility.$0(SP.RequestExecutor.$C[$p0]) || SP.RequestExecutor.$C[$p0] < 1) {
            SP.RequestExecutorNotificationPanel.$1k($p0);
        }
        else {
            SP.RequestExecutor.$w($p0, 'RE_CannotAccessSite');
        }
    }
};
SP.RequestExecutor.$1d = function SP_RequestExecutor$$1d($p0) {
    var $v_0 = $p0;

    $v_0 = SP.RequestExecutorUtility.$h($v_0);
    $v_0 = $v_0 + 'SP.CloseWindow=true';
    var $v_1 = window.open($v_0);

    if (SP.RequestExecutorUtility.$0($v_1)) {
        SP.RequestExecutor.$w($p0, 'RE_CannotAccessSiteOpenWindowFailed');
    }
    else {
        SP.RequestExecutor.$J[$p0] = $v_1;
        if (SP.RequestExecutorUtility.$0(SP.RequestExecutor.$C[$p0])) {
            SP.RequestExecutor.$C[$p0] = 1;
        }
        else {
            SP.RequestExecutor.$C[$p0] = SP.RequestExecutor.$C[$p0] + 1;
        }
        SP.RequestExecutor.$o($p0);
    }
};
SP.RequestExecutor.$w = function SP_RequestExecutor$$w($p0, $p1) {
    SP.RequestExecutor.$V[$p0] = true;
    var $v_0 = SP.RequestExecutor.$D[$p0];

    SP.RequestExecutor.$D[$p0] = [];
    if (!SP.RequestExecutorUtility.$0(SP.RequestExecutor.$9[$p0])) {
        window.clearTimeout(SP.RequestExecutor.$9[$p0]);
        delete SP.RequestExecutor.$9[$p0];
    }
    SP.RequestExecutor.$1h($p0);
    if ($v_0) {
        var $v_1 = SP.RequestExecutor.$1U($p0, $p1);

        for (var $v_2 = 0; $v_2 < $v_0.length; $v_2++) {
            var $v_3 = $v_0[$v_2];
            var $v_4 = $v_3.postMessageId;
            var $v_5 = SP.RequestExecutor.$3[$v_4];

            if (!SP.RequestExecutorUtility.$0($v_5)) {
                SP.RequestExecutor.$3[$v_4] = null;
                delete SP.RequestExecutor.$3[$v_4];
                if (!SP.RequestExecutorUtility.$0($v_3.timeoutId)) {
                    window.clearTimeout($v_3.timeoutId);
                }
                SP.RequestExecutor.$W($v_3.requestInfo, SP.RequestExecutorErrors.iFrameLoadError, $v_1);
            }
        }
    }
};
SP.RequestExecutor.$1h = function SP_RequestExecutor$$1h($p0) {
    var $v_0 = SP.RequestExecutor.$i($p0);

    if ($v_0 && $v_0.parentNode) {
        $v_0.parentNode.removeChild($v_0);
    }
    if (SP.RequestExecutor.$D) {
        delete SP.RequestExecutor.$D[$p0];
    }
    if (SP.RequestExecutor.$9) {
        delete SP.RequestExecutor.$9[$p0];
    }
    if (SP.RequestExecutor.$P) {
        delete SP.RequestExecutor.$P[$p0];
    }
    if (SP.RequestExecutor.$K) {
        delete SP.RequestExecutor.$K[$p0];
    }
    if (SP.RequestExecutor.$L) {
        delete SP.RequestExecutor.$L[$p0];
    }
    if (SP.RequestExecutor.$V) {
        delete SP.RequestExecutor.$V[$p0];
    }
    if (SP.RequestExecutor.$J) {
        delete SP.RequestExecutor.$J[$p0];
    }
    if (SP.RequestExecutor.$C) {
        delete SP.RequestExecutor.$C[$p0];
    }
};
SP.RequestExecutor.$o = function SP_RequestExecutor$$o($p0) {
    var $v_0 = SP.RequestExecutor.$J[$p0];

    if ($v_0) {
        if ($v_0.closed) {
            delete SP.RequestExecutor.$J[$p0];
            var $v_1 = SP.RequestExecutor.$i($p0);

            SP.RequestExecutor.$K[$p0] = false;
            SP.RequestExecutor.$P[$p0] = false;
            SP.RequestExecutor.$V[$p0] = false;
            $p0 = SP.RequestExecutorUtility.$h($p0);
            $v_1.src = $p0 + 'ts=' + (Math.random()).toString();
        }
        else {
            window.setTimeout(function() {
                SP.RequestExecutor.$o($p0);
            }, 500);
        }
    }
};
SP.RequestExecutor.$1e = function SP_RequestExecutor$$1e($p0) {
    SP.RequestExecutor.$K[$p0] = true;
    SP.RequestExecutorNotificationPanel.$1g($p0);
    var $v_0 = SP.RequestExecutor.$D[$p0];

    if ($v_0) {
        SP.RequestExecutor.$D[$p0] = [];
        for (var $v_1 = 0; $v_1 < $v_0.length; $v_1++) {
            var $v_2 = $v_0[$v_1];

            if (SP.RequestExecutor.$3[$v_2.postMessageId]) {
                if ($v_2.requestInfo.$p_0) {
                    SP.RequestExecutor.$12($v_2);
                }
                else {
                    SP.RequestExecutor.$B($v_2);
                }
            }
        }
    }
};
SP.RequestExecutor.$12 = function SP_RequestExecutor$$12($p0) {
    if (!SP.RequestExecutorUtility.$0($p0.timeoutId)) {
        window.clearTimeout($p0.timeoutId);
    }
    var $v_0 = new SP.ResponseInfo();

    $v_0.responseAvailable = false;
    $v_0.body = '';
    $v_0.state = $p0.requestInfo.state;
    if ($p0.requestInfo.success) {
        $p0.requestInfo.success($v_0);
    }
};
SP.RequestExecutor.internalProcessIFrameRequestTimeoutCallback = function SP_RequestExecutor$internalProcessIFrameRequestTimeoutCallback(postMessageId) {
    if (!SP.RequestExecutor.$3) {
        return;
    }
    var $v_0 = SP.RequestExecutor.$3[postMessageId];

    if (SP.RequestExecutorUtility.$0($v_0)) {
        return;
    }
    SP.RequestExecutor.$3[postMessageId] = null;
    delete SP.RequestExecutor.$3[postMessageId];
    SP.RequestExecutor.$W($v_0.requestInfo, SP.RequestExecutorErrors.requestAbortedOrTimedout, SP.RequestExecutorResources.getString('RE_RequestAbortedOrTimedout'));
};
SP.RequestExecutor.internalProcessXMLHttpRequestTimeoutCallback = function SP_RequestExecutor$internalProcessXMLHttpRequestTimeoutCallback(xhr, requestInfo) {
    xhr.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
    try {
        xhr.abort();
    }
    catch ($$e_2) { }
    SP.RequestExecutor.$W(requestInfo, SP.RequestExecutorErrors.requestAbortedOrTimedout, SP.RequestExecutorResources.getString('RE_RequestAbortedOrTimedout'));
};
SP.RequestExecutor.internalProcessXMLHttpRequestOnreadystatechange = function SP_RequestExecutor$internalProcessXMLHttpRequestOnreadystatechange(xhr, requestInfo, timeoutId) {
    if (xhr.readyState === 4) {
        if (timeoutId) {
            window.clearTimeout(timeoutId);
        }
        xhr.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
        var $v_0 = new SP.ResponseInfo();

        $v_0.state = requestInfo.state;
        $v_0.responseAvailable = true;
        if (requestInfo.binaryStringResponseBody) {
            $v_0.body = SP.RequestExecutorInternalSharedUtility.$13(xhr.response);
        }
        else {
            $v_0.body = xhr.responseText;
        }
        $v_0.statusCode = xhr.status;
        $v_0.statusText = xhr.statusText;
        $v_0.contentType = xhr.getResponseHeader('content-type');
        $v_0.allResponseHeaders = xhr.getAllResponseHeaders();
        $v_0.headers = SP.RequestExecutor.$1H($v_0.allResponseHeaders);
        if (xhr.status >= 200 && xhr.status < 300 || xhr.status === 1223) {
            if (requestInfo.success) {
                requestInfo.success($v_0);
            }
        }
        else {
            var $v_1 = SP.RequestExecutorErrors.httpError;
            var $v_2 = xhr.statusText;

            if (requestInfo.error) {
                requestInfo.error($v_0, $v_1, $v_2);
            }
        }
    }
};
SP.RequestExecutor.internalOnMessage = function SP_RequestExecutor$internalOnMessage(e) {
    if (SP.RequestExecutorUtility.$0(e)) {
        return;
    }
    var $v_0 = e.data;
    var $v_1 = e.origin;

    if (SP.RequestExecutorUtility.$1($v_1) || SP.RequestExecutorUtility.$1($v_0)) {
        return;
    }
    if (!SP.RequestExecutor.$3) {
        return;
    }
    var $v_2 = JSON.parse($v_0);

    if (SP.RequestExecutorUtility.$0($v_2)) {
        return;
    }
    if (SP.RequestExecutorUtility.$1($v_2.postMessageId)) {
        return;
    }
    var $v_3 = SP.RequestExecutor.$3[$v_2.postMessageId];

    if (SP.RequestExecutorUtility.$0($v_3)) {
        return;
    }
    var $v_4 = SP.RequestExecutor.$O($v_1);
    var $v_5 = SP.RequestExecutor.$O($v_3.appWebProxyUrl);

    if ($v_4.toLowerCase() !== $v_5.toLowerCase()) {
        return;
    }
    var $v_6 = $v_2.postMessageId;

    delete SP.RequestExecutor.$3[$v_6];
    if (!SP.RequestExecutorUtility.$0($v_3.timeoutId)) {
        window.clearTimeout($v_3.timeoutId);
        $v_3.timeoutId = 0;
        delete $v_3.timeoutId;
    }
    if ($v_2.command === 'Ping') {
        var $v_7 = $v_3.appWebProxyUrl;

        SP.RequestExecutor.$1e($v_7);
    }
    else if ($v_2.command === 'Query') {
        var $v_8 = new SP.ResponseInfo();

        $v_8.responseAvailable = $v_2.responseAvailable;
        $v_8.body = $v_2.responseText;
        $v_8.statusCode = $v_2.statusCode;
        $v_8.statusText = $v_2.statusText;
        $v_8.contentType = $v_2.contentType;
        $v_8.state = $v_3.requestInfo.state;
        $v_8.allResponseHeaders = $v_2.allResponseHeaders;
        $v_8.headers = SP.RequestExecutor.$1H($v_2.allResponseHeaders);
        $v_8.binaryStringResponseBody = $v_2.binaryStringResponseBody;
        if (SP.RequestExecutorUtility.$0($v_8.body)) {
            $v_8.body = '';
        }
        var $v_9 = 0;
        var $v_A = null;

        if ($v_2.errorCode) {
            $v_9 = $v_2.errorCode;
            $v_A = $v_2.errorMessage;
        }
        else {
            if ($v_2.statusCode >= 200 && $v_2.statusCode < 300 || $v_2.statusCode === 1223) {
                $v_9 = 0;
                $v_A = null;
            }
            else {
                $v_9 = SP.RequestExecutorErrors.httpError;
                $v_A = $v_2.statusText;
            }
        }
        if (!$v_9) {
            if ($v_3.requestInfo.success) {
                $v_3.requestInfo.success($v_8);
            }
        }
        else {
            if ($v_3.requestInfo.error) {
                $v_3.requestInfo.error($v_8, $v_9, $v_A);
            }
        }
    }
};
SP.RequestExecutor.get_$11 = function SP_RequestExecutor$get_$11() {
    return !!document.body || SP.RequestExecutor.$y;
};
SP.RequestExecutor.$10 = function SP_RequestExecutor$$10($p0, $p1) {
    if (!SP.RequestExecutor.$Z) {
        SP.RequestExecutor.$Z = [];
    }
    if (!SP.RequestExecutor.$Y) {
        SP.RequestExecutor.$Y = [];
    }
    SP.RequestExecutor.$Z.push($p0);
    SP.RequestExecutor.$Y.push($p1);
};
SP.RequestExecutor.internalProcessWindowLoad = function SP_RequestExecutor$internalProcessWindowLoad() {
    SP.RequestExecutor.$y = true;
    var $v_0 = SP.RequestExecutor.$Z;

    SP.RequestExecutor.$Z = null;
    var $v_1 = SP.RequestExecutor.$Y;

    SP.RequestExecutor.$Y = null;
    if ($v_0) {
        for (var $v_2 = 0; $v_2 < $v_0.length; $v_2++) {
            var $v_3 = $v_0[$v_2];
            var $v_4 = $v_1[$v_2];

            $v_3.executeAsync($v_4);
        }
    }
};
SP.RequestExecutor.$j = function SP_RequestExecutor$$j() {
    var $v_0 = window.XMLHttpRequest;

    if (SP.RequestExecutorUtility.$7($v_0)) {
        throw SP.RequestExecutorUtility.$M(SP.RequestExecutorResources.getString('RE_BrowserNotSupported'), SP.RequestExecutorExceptions.browserNotSupported);
    }
    return new XMLHttpRequest();
};
SP.RequestExecutor.$1H = function SP_RequestExecutor$$1H($p0) {
    if (SP.RequestExecutorUtility.$1($p0)) {
        return null;
    }
    var $v_0 = {};
    var $v_1 = new RegExp('\r?\n');
    var $v_2 = $p0.split($v_1);

    for (var $v_3 = 0; $v_3 < $v_2.length; $v_3++) {
        var $v_4 = $v_2[$v_3];

        if (!SP.RequestExecutorUtility.$1($v_4)) {
            var $v_5 = $v_4.indexOf(':');

            if ($v_5 > 0) {
                var $v_6 = $v_4.substr(0, $v_5);
                var $v_7 = $v_4.substr($v_5 + 1);

                $v_6 = SP.RequestExecutorNative.trim($v_6);
                $v_7 = SP.RequestExecutorNative.trim($v_7);
                $v_0[$v_6.toUpperCase()] = $v_7;
            }
        }
    }
    return $v_0;
};
SP.RequestExecutor.$1l = function SP_RequestExecutor$$1l($p0) {
    if (SP.RequestExecutorUtility.$0($p0)) {
        throw SP.RequestExecutorUtility.$G('requestInfo');
    }
    SP.RequestExecutorUtility.$5($p0.url, 'requestInfo.url', 'string', true);
    SP.RequestExecutorUtility.$5($p0.body, 'requestInfo.body', 'string', false);
    SP.RequestExecutorUtility.$5($p0.success, 'requestInfo.success', 'function', false);
    SP.RequestExecutorUtility.$5($p0.error, 'requestInfo.error', 'function', false);
    SP.RequestExecutorUtility.$5($p0.timeout, 'requestInfo.timeout', 'number', false);
    SP.RequestExecutorUtility.$5($p0.method, 'requestInfo.method', 'string', false);
    SP.RequestExecutorUtility.$5($p0.binaryStringRequestBody, 'requestInfo.BinaryRequestBody', 'boolean', false);
    SP.RequestExecutorUtility.$5($p0.binaryStringResponseBody, 'requestInfo.BinaryResponseBody', 'boolean', false);
    if (SP.RequestExecutorUtility.$1($p0.method)) {
        $p0.method = 'GET';
    }
    else {
        $p0.method = $p0.method.toUpperCase();
    }
    if (SP.RequestExecutorUtility.$0($p0.timeout)) {
        $p0.timeout = 90000;
    }
    if ($p0.timeout < 0) {
        throw SP.RequestExecutorUtility.$G('requestInfo.timeout');
    }
    if (SP.RequestExecutorUtility.$1($p0.url)) {
        throw SP.RequestExecutorUtility.$G('requestInfo.url');
    }
};
SP.RequestExecutor.$1U = function SP_RequestExecutor$$1U($p0, $p1) {
    var $v_0 = $p0;
    var $v_1 = $v_0.indexOf('/_layouts');

    if ($v_1 > 0) {
        $v_0 = $v_0.substr(0, $v_1);
    }
    var $v_2 = SP.RequestExecutorResources.getString($p1);

    $v_2 = SP.RequestExecutorUtility.$N($v_2, $v_0);
    return $v_2;
};
SP.RequestExecutor.internalLoginButtonHandler = function SP_RequestExecutor$internalLoginButtonHandler(panelId) {
    var $v_0 = SP.RequestExecutorNotificationPanel.$1A(panelId);

    if (!SP.RequestExecutorUtility.$1($v_0)) {
        var $v_1 = SP.RequestExecutorNotificationPanel.$1X($v_0);

        if (SP.RequestExecutorUtility.$1($v_1)) {
            SP.RequestExecutor.$1d($v_0);
        }
        else {
            var $v_2 = $v_0;

            $v_2 = SP.RequestExecutorUtility.$h($v_2);
            $v_2 = $v_2 + 'SP.ReturnUrl=' + SP.RequestExecutorHttpUtility.$1M($v_1, false, false);
            window.top.location.href = $v_2;
        }
    }
    SP.RequestExecutorNotificationPanel.$v(panelId);
};
SP.RequestExecutor.internalCancelButtonHandler = function SP_RequestExecutor$internalCancelButtonHandler(panelId) {
    var $v_0 = SP.RequestExecutorNotificationPanel.$1A(panelId);

    if (!SP.RequestExecutorUtility.$1($v_0)) {
        SP.RequestExecutor.$w($v_0, 'RE_CannotAccessSiteCancelled');
    }
    SP.RequestExecutorNotificationPanel.$v(panelId);
};
SP.RequestExecutor.prototype = {
    $A_0: null,
    $S_0: null,
    $1E_0: null,
    $R_0: 0,
    get_formDigestHandlingEnabled: function SP_RequestExecutor$get_formDigestHandlingEnabled() {
        return this.formDigestHandlingEnabled;
    },
    set_formDigestHandlingEnabled: function SP_RequestExecutor$set_formDigestHandlingEnabled(value) {
        this.formDigestHandlingEnabled = value;
        return value;
    },
    get_iFrameSourceUrl: function SP_RequestExecutor$get_iFrameSourceUrl() {
        return this.iFrameSourceUrl;
    },
    set_iFrameSourceUrl: function SP_RequestExecutor$set_iFrameSourceUrl(value) {
        this.iFrameSourceUrl = value;
        return value;
    },
    get_$m_0: function SP_RequestExecutor$get_$m_0() {
        return SP.RequestExecutor.$b(this.$1E_0, this.get_iFrameSourceUrl());
    },
    executeAsync: function SP_RequestExecutor$executeAsync(requestInfo) {
        SP.RequestExecutor.$1l(requestInfo);
        if (SP.RequestExecutorUtility.$1C(requestInfo.url)) {
            if ((SP.RequestExecutor.$O(requestInfo.url)).toLowerCase() !== (SP.RequestExecutor.$O(this.$A_0)).toLowerCase()) {
                throw SP.RequestExecutorUtility.$G('requestInfo.url');
            }
        }
        else {
            requestInfo.url = SP.RequestExecutor.$b(this.$A_0, requestInfo.url);
        }
        if (requestInfo.binaryStringRequestBody || requestInfo.binaryStringResponseBody) {
            SP.RequestExecutor.$1N();
        }
        if (this.$R_0 === 2) {
            SP.RequestExecutor.$n();
            this.$15_0();
            if (SP.RequestExecutor.get_$11()) {
                this.$17_0(this.get_$m_0(), requestInfo);
            }
            else {
                SP.RequestExecutor.$10(this, requestInfo);
            }
        }
        else if (this.$R_0 === 3) {
            this.$1Q_0(requestInfo);
        }
        else {
            SP.RequestExecutor.$n();
            this.$1R_0(requestInfo);
        }
    },
    attemptLogin: function SP_RequestExecutor$attemptLogin(returnUrl, success, error) {
        SP.RequestExecutorUtility.$a(returnUrl, 'returnUrl');
        SP.RequestExecutorUtility.$5(success, 'success', 'function', true);
        SP.RequestExecutorUtility.$5(error, 'error', 'function', false);
        if (this.$R_0 === 2) {
            SP.RequestExecutor.$n();
            this.$15_0();
            var $v_0 = this.get_$m_0();

            SP.RequestExecutorNotificationPanel.$z($v_0, returnUrl);
            var $v_1 = new SP.RequestInfo();

            $v_1.$p_0 = true;
            var $$t_A = this;

            $v_1.success = function($p1_0) {
                SP.RequestExecutorNotificationPanel.$z($v_0, null);
                success($p1_0);
            };
            var $$t_B = this;

            $v_1.error = function($p1_0, $p1_1, $p1_2) {
                SP.RequestExecutorNotificationPanel.$z($v_0, null);
                if (error) {
                    error($p1_0, $p1_1, $p1_2);
                }
            };
            if (SP.RequestExecutor.get_$11()) {
                this.$17_0(this.get_$m_0(), $v_1);
            }
            else {
                SP.RequestExecutor.$10(this, $v_1);
            }
        }
        else {
            var $v_2 = new SP.ResponseInfo();

            $v_2.responseAvailable = true;
            $v_2.body = '';
            success($v_2);
        }
    },
    $17_0: function SP_RequestExecutor$$17_0($p0, $p1) {
        SP.RequestExecutor.$g++;
        var $v_0 = new SP.AppWebProxyRequestInfo();

        $v_0.command = 'Query';
        $v_0.postMessageId = SP.RequestExecutorInternalSharedUtility.$e + SP.RequestExecutor.$g.toString();
        $v_0.appWebProxyUrl = $p0;
        $v_0.requestInfo = $p1;
        if (!SP.RequestExecutor.$D) {
            SP.RequestExecutor.$D = {};
        }
        if (!SP.RequestExecutor.$9) {
            SP.RequestExecutor.$9 = {};
        }
        if (!SP.RequestExecutor.$P) {
            SP.RequestExecutor.$P = {};
        }
        if (!SP.RequestExecutor.$K) {
            SP.RequestExecutor.$K = {};
        }
        if (!SP.RequestExecutor.$L) {
            SP.RequestExecutor.$L = {};
        }
        if (!SP.RequestExecutor.$V) {
            SP.RequestExecutor.$V = {};
        }
        if (!SP.RequestExecutor.$3) {
            SP.RequestExecutor.$3 = {};
        }
        if (!SP.RequestExecutor.$J) {
            SP.RequestExecutor.$J = {};
        }
        if (!SP.RequestExecutor.$C) {
            SP.RequestExecutor.$C = {};
        }
        var $v_1 = SP.RequestExecutor.$D[$p0];

        if (!$v_1) {
            $v_1 = [];
            SP.RequestExecutor.$D[$p0] = $v_1;
        }
        SP.RequestExecutor.$3[$v_0.postMessageId] = $v_0;
        if ($v_0.requestInfo.timeout > 0) {
            var $v_3 = SP.RequestExecutorNative.getIFrameRequestTimeoutCallback($v_0.postMessageId);

            $v_0.timeoutId = window.setTimeout($v_3, $v_0.requestInfo.timeout);
        }
        var $v_2 = SP.RequestExecutor.$i($p0);

        if ($v_2) {
            if (SP.RequestExecutor.$P[$p0] && SP.RequestExecutor.$K[$p0]) {
                if ($v_0.requestInfo.$p_0) {
                    SP.RequestExecutor.$12($v_0);
                }
                else {
                    SP.RequestExecutor.$B($v_0);
                }
            }
            else {
                $v_1.push($v_0);
            }
        }
        else {
            $v_1.push($v_0);
            $v_2 = SP.RequestExecutor.$1O($p0);
        }
    },
    $1Q_0: function SP_RequestExecutor$$1Q_0($p0) {
        var $v_0 = SP.RequestExecutor.$b(this.$A_0, '_api/contextinfo');

        $v_0 = SP.RequestExecutor.$1B(this.$S_0, $v_0);
        var $$t_4 = this;

        SP.RequestExecutor.$16(this.$A_0, $v_0, this.get_formDigestHandlingEnabled(), $p0, function($p1_0, $p1_1) {
            $$t_4.$18_0($p1_0, $p1_1);
        });
    },
    $18_0: function SP_RequestExecutor$$18_0($p0, $p1) {
        var $v_0 = SP.RequestExecutor.$j();
        var $v_1;

        if (this.$R_0 === 3) {
            $v_1 = SP.RequestExecutor.$1B(this.$S_0, $p1.url);
        }
        else {
            $v_1 = SP.RequestExecutorUtility.$c($p1.url);
        }
        $v_0.open($p1.method, $v_1);
        var $v_2 = false;

        if ($p1.headers) {
            var $$dict_5 = $p1.headers;

            for (var $$key_6 in $$dict_5) {
                var $v_5 = {
                    key: $$key_6,
                    value: $$dict_5[$$key_6]
                };

                if (!SP.RequestExecutorUtility.$1($v_5.key)) {
                    $v_0.setRequestHeader($v_5.key, $v_5.value);
                    if ($v_5.key.toLowerCase() === 'x-requestdigest') {
                        $v_2 = true;
                    }
                }
            }
        }
        if (!$v_2 && !SP.RequestExecutorUtility.$1($p0)) {
            $v_0.setRequestHeader('X-RequestDigest', $p0);
        }
        if ($p1.binaryStringResponseBody) {
            SP.RequestExecutorInternalSharedUtility.$1L($v_0);
        }
        var $v_3 = 0;

        if ($p1.timeout > 0) {
            $v_3 = window.setTimeout(SP.RequestExecutorNative.getXMLHttpRequestTimeoutCallback($v_0, $p1), $p1.timeout);
        }
        $v_0.onreadystatechange = SP.RequestExecutorNative.getXMLHttpRequestOnreadystatechangeCallback($v_0, $p1, $v_3);
        var $v_4 = $p1.body;

        if ($p1.binaryStringRequestBody) {
            $v_4 = SP.RequestExecutorInternalSharedUtility.$14($p1.body);
        }
        $v_0.send($v_4);
    },
    $1R_0: function SP_RequestExecutor$$1R_0($p0) {
        var $v_0 = SP.RequestExecutor.$b(this.$A_0, '_api/contextinfo');

        $v_0 = SP.RequestExecutorUtility.$c($v_0);
        var $$t_4 = this;

        SP.RequestExecutor.$16(this.$A_0, $v_0, this.get_formDigestHandlingEnabled(), $p0, function($p1_0, $p1_1) {
            $$t_4.$18_0($p1_0, $p1_1);
        });
    },
    $15_0: function SP_RequestExecutor$$15_0() {
        if (SP.RequestExecutor.$Q) {
            return;
        }
        var $v_0 = window.SP.RequestExecutor.internalOnMessage;

        if (!SP.RequestExecutorUtility.$7(window.addEventListener)) {
            window.addEventListener('message', $v_0, false);
            SP.RequestExecutor.$Q = true;
        }
        else if (!SP.RequestExecutorUtility.$7(window.attachEvent)) {
            window.attachEvent('onmessage', $v_0);
            SP.RequestExecutor.$Q = true;
        }
        else {
            throw SP.RequestExecutorUtility.$r();
        }
    }
};
SP.RequestExecutorErrors = function SP_RequestExecutorErrors() {
};
SP.RequestExecutorExceptions = function SP_RequestExecutorExceptions() {
};
SP.RequestExecutorInternalSharedUtility = function SP_RequestExecutorInternalSharedUtility() {
};
SP.RequestExecutorInternalSharedUtility.$O = function SP_RequestExecutorInternalSharedUtility$$O($p0) {
    var $v_0 = $p0.indexOf('://');

    $v_0 = $p0.indexOf('/', $v_0 + 3);
    if ($v_0 > 0) {
        $p0 = $p0.substr(0, $v_0);
    }
    if (($p0.substr(0, 8)).toLowerCase() === 'https://' && $p0.substr($p0.length - 4, 4) === ':443') {
        $p0 = $p0.substr(0, $p0.length - 4);
    }
    else if (($p0.substr(0, 7)).toLowerCase() === 'http://' && $p0.substr($p0.length - 3, 3) === ':80') {
        $p0 = $p0.substr(0, $p0.length - 3);
    }
    return $p0;
};
SP.RequestExecutorInternalSharedUtility.$1Y = function SP_RequestExecutorInternalSharedUtility$$1Y($p0) {
    $p0 = SP.RequestExecutorInternalSharedUtility.$O($p0);
    if (($p0.substr(0, 8)).toLowerCase() === 'https://') {
        $p0 = $p0.substr(8);
    }
    else if (($p0.substr(0, 7)).toLowerCase() === 'http://') {
        $p0 = $p0.substr(7);
    }
    return $p0;
};
SP.RequestExecutorInternalSharedUtility.$14 = function SP_RequestExecutorInternalSharedUtility$$14($p0) {
    var $v_0 = null;

    if (typeof $p0 == 'string') {
        var buffer = new ArrayBuffer($p0.length);
        var byteArray = new Uint8Array(buffer);

        for (var i = 0; i < $p0.length; i++) {
            byteArray[i] = $p0.charCodeAt(i) & 0xff;
        }
        var bb = null;

        if (window.BlobBuilder) {
            bb = new BlobBuilder();
        }
        else if (window.MozBlobBuilder) {
            bb = new MozBlobBuilder();
        }
        if (bb) {
            bb.append(buffer);
            $v_0 = bb.getBlob();
        }
        else {
            $v_0 = buffer;
        }
    }
    ;
    return $v_0;
};
SP.RequestExecutorInternalSharedUtility.$13 = function SP_RequestExecutorInternalSharedUtility$$13($p0) {
    var $v_0 = '';

    if ($p0) {
        var byteArray = new Uint8Array($p0);

        for (var i = 0; i < $p0.byteLength; i++) {
            ret = ret + String.fromCharCode(byteArray[i]);
        }
    }
    ;
    return $v_0;
};
SP.RequestExecutorInternalSharedUtility.$1L = function SP_RequestExecutorInternalSharedUtility$$1L($p0) {
    $p0.responseType = 'arraybuffer';
};
SP.RequestExecutorMessageProcessor = function SP_RequestExecutorMessageProcessor($p0, $p1) {
    this.$H_0 = $p0;
    this.$2_0 = $p1;
    if (SP.RequestExecutorUtility.$1(this.$2_0.method)) {
        this.$2_0.method = 'GET';
    }
    else {
        this.$2_0.method = this.$2_0.method.toUpperCase();
    }
    this.$I_0 = 0;
};
SP.RequestExecutorMessageProcessor.$B = function SP_RequestExecutorMessageProcessor$$B($p0, $p1) {
    var $v_0 = JSON.stringify($p0);

    window.parent.postMessage($v_0, $p1);
};
SP.RequestExecutorMessageProcessor.$1V = function SP_RequestExecutorMessageProcessor$$1V($p0) {
    if (!SP.RequestExecutorMessageProcessor.$U) {
        SP.RequestExecutorMessageProcessor.$U = {};
    }
    $p0 = SP.RequestExecutorMessageProcessor.$t($p0);
    var $v_0 = SP.RequestExecutorMessageProcessor.$U[$p0];

    if (!$v_0) {
        return null;
    }
    var $v_1 = new Date();

    if ($v_0.expiration < $v_1.getTime()) {
        return null;
    }
    return $v_0.digestValue;
};
SP.RequestExecutorMessageProcessor.$t = function SP_RequestExecutorMessageProcessor$$t($p0) {
    var $v_0;

    $v_0 = $p0.indexOf('?');
    if ($v_0 > 0) {
        $p0 = $p0.substr(0, $v_0);
    }
    $v_0 = $p0.indexOf('#');
    if ($v_0 > 0) {
        $p0 = $p0.substr(0, $v_0);
    }
    $p0 = $p0.toLowerCase();
    $v_0 = $p0.indexOf('/_layouts');
    if ($v_0 > 0) {
        $p0 = $p0.substr(0, $v_0);
    }
    $v_0 = $p0.indexOf('/_vti_');
    if ($v_0 > 0) {
        $p0 = $p0.substr(0, $v_0);
    }
    $v_0 = $p0.indexOf('/_api');
    if ($v_0 > 0) {
        $p0 = $p0.substr(0, $v_0);
    }
    if ($p0.charAt($p0.length - 1) !== '/') {
        $p0 = $p0 + '/';
    }
    $p0 += '_api/contextinfo';
    return $p0;
};
SP.RequestExecutorMessageProcessor.init = function SP_RequestExecutorMessageProcessor$init(settings) {
    SP.RequestExecutorMessageProcessor.$E = settings;
    if (SP.RequestExecutorMessageProcessor.$E) {
        if (SP.RequestExecutorUtility.$7(SP.RequestExecutorMessageProcessor.$E.formDigestHandlingEnabled)) {
            SP.RequestExecutorMessageProcessor.$E.formDigestHandlingEnabled = false;
        }
        if (SP.RequestExecutorUtility.$7(SP.RequestExecutorMessageProcessor.$E.initErrorCode)) {
            SP.RequestExecutorMessageProcessor.$E.initErrorCode = 0;
        }
        if (SP.RequestExecutorUtility.$7(SP.RequestExecutorMessageProcessor.$E.initErrorMessage)) {
            SP.RequestExecutorMessageProcessor.$E.initErrorMessage = '';
        }
    }
    if (!SP.RequestExecutorMessageProcessor.$Q) {
        var $v_0 = window.SP.RequestExecutorMessageProcessor.internalOnMessage;

        if (window.addEventListener) {
            window.addEventListener('message', $v_0, false);
            SP.RequestExecutorMessageProcessor.$Q = true;
        }
        else if (window.attachEvent) {
            window.attachEvent('onmessage', $v_0);
            SP.RequestExecutorMessageProcessor.$Q = true;
        }
    }
};
SP.RequestExecutorMessageProcessor.internalOnMessage = function SP_RequestExecutorMessageProcessor$internalOnMessage(e) {
    if (SP.RequestExecutorUtility.$0(e)) {
        return;
    }
    var $v_0 = SP.RequestExecutorMessageProcessor.$E;

    if (!$v_0) {
        return;
    }
    var $v_1 = e.data;
    var $v_2 = e.origin;

    if (SP.RequestExecutorUtility.$1($v_2)) {
        return;
    }
    if (SP.RequestExecutorUtility.$1($v_1)) {
        return;
    }
    var $v_3 = JSON.parse($v_1);

    if (SP.RequestExecutorUtility.$0($v_3)) {
        return;
    }
    if (SP.RequestExecutorUtility.$1($v_3.postMessageId)) {
        return;
    }
    if ($v_3.postMessageId.substr(0, SP.RequestExecutorInternalSharedUtility.$e.length) !== SP.RequestExecutorInternalSharedUtility.$e) {
        return;
    }
    if (SP.RequestExecutorUtility.$1($v_3.command)) {
        return;
    }
    if ($v_3.command === 'Ping') {
        SP.RequestExecutorMessageProcessor.$l($v_3, 0, '', $v_2);
        return;
    }
    if ($v_3.command !== 'Query' || SP.RequestExecutorUtility.$1($v_3.url) || SP.RequestExecutorUtility.$1($v_3.method)) {
        return;
    }
    if ($v_0.initErrorCode < 0) {
        SP.RequestExecutorMessageProcessor.$l($v_3, $v_0.initErrorCode, $v_0.initErrorMessage, $v_2);
        return;
    }
    if ((!$v_0.trustedOriginAuthorities || !$v_0.trustedOriginAuthorities.length) && !$v_0.originAuthorityValidator) {
        SP.RequestExecutorMessageProcessor.$l($v_3, SP.RequestExecutorErrors.noTrustedOrigins, SP.RequestExecutorResources.getString('RE_NoTrustedOrigins'), $v_2);
        return;
    }
    var $v_4 = false;
    var $v_5 = (SP.RequestExecutorInternalSharedUtility.$1Y($v_2)).toLowerCase();

    if ($v_0.trustedOriginAuthorities) {
        for (var $v_8 = 0; $v_8 < $v_0.trustedOriginAuthorities.length; $v_8++) {
            var $v_9 = $v_0.trustedOriginAuthorities[$v_8].toLowerCase();

            if ($v_9 === $v_5) {
                $v_4 = true;
                break;
            }
        }
    }
    if (!$v_4 && $v_0.originAuthorityValidator) {
        $v_4 = $v_0.originAuthorityValidator($v_5);
    }
    if (!$v_4) {
        SP.RequestExecutorMessageProcessor.$l($v_3, SP.RequestExecutorErrors.domainDoesNotMatch, SP.RequestExecutorResources.getString('RE_DomainDoesNotMatch'), $v_2);
        return;
    }
    var $v_6 = new SP.RequestExecutorMessageProcessor($v_2, $v_3);
    var $v_7 = 0;

    if ($v_7 > 0) {
        window.setTimeout(function() {
            $v_6.$1I_0();
        }, $v_7);
    }
    else {
        $v_6.$1I_0();
    }
};
SP.RequestExecutorMessageProcessor.$l = function SP_RequestExecutorMessageProcessor$$l($p0, $p1, $p2, $p3) {
    var $v_0 = new SP.PostMessageResponseInfo();

    $v_0.command = $p0.command;
    $v_0.postMessageId = $p0.postMessageId;
    $v_0.responseAvailable = false;
    $v_0.errorCode = $p1;
    $v_0.errorMessage = $p2;
    SP.RequestExecutorMessageProcessor.$B($v_0, $p3);
};
SP.RequestExecutorMessageProcessor.prototype = {
    $H_0: null,
    $2_0: null,
    $I_0: 0,
    $4_0: null,
    $1I_0: function SP_RequestExecutorMessageProcessor$$1I_0() {
        if (this.$2_0.method === 'GET' || !SP.RequestExecutorMessageProcessor.$E.formDigestHandlingEnabled) {
            this.$u_0(null);
        }
        else {
            var $v_0 = SP.RequestExecutorMessageProcessor.$1V(this.$2_0.url);

            if (SP.RequestExecutorUtility.$1($v_0)) {
                this.$1S_0();
            }
            else {
                this.$u_0($v_0);
            }
        }
    },
    $1S_0: function SP_RequestExecutorMessageProcessor$$1S_0() {
        var $v_0 = SP.RequestExecutorMessageProcessor.$t(this.$2_0.url);

        $v_0 = SP.RequestExecutorUtility.$c($v_0);
        var $v_1 = SP.RequestExecutor.$j();

        $v_1.open('POST', $v_0);
        $v_1.setRequestHeader('ACCEPT', 'application/json;odata=verbose');
        if (this.$2_0.timeout > 0) {
            var $$t_3 = this;

            this.$I_0 = window.setTimeout(function() {
                $v_1.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
                $v_1.abort();
                var $v_2 = new SP.PostMessageResponseInfo();

                $v_2.command = 'Query';
                $v_2.errorCode = SP.RequestExecutorErrors.requestAbortedOrTimedout;
                $v_2.errorMessage = SP.RequestExecutorResources.getString('RE_RequestAbortedOrTimedout');
                $v_2.postMessageId = $$t_3.$2_0.postMessageId;
                $v_2.responseAvailable = false;
                SP.RequestExecutorMessageProcessor.$B($v_2, $$t_3.$H_0);
            }, this.$2_0.timeout);
        }
        var $$t_4 = this;

        $v_1.onreadystatechange = function() {
            if ($v_1 && $v_1.readyState === 4) {
                if ($$t_4.$I_0) {
                    window.clearTimeout($$t_4.$I_0);
                    $$t_4.$I_0 = 0;
                }
                $v_1.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
                $$t_4.$1T_0($v_1);
            }
        };
        $v_1.send('');
    },
    $1T_0: function SP_RequestExecutorMessageProcessor$$1T_0($p0) {
        if ($p0.status !== 200) {
            var $v_5 = this.$d_0($p0);

            $v_5.errorCode = SP.RequestExecutorErrors.httpError;
            var $v_6 = SP.RequestExecutorResources.getString('RE_RequestUnexpectedResponseWithContentTypeAndStatus');

            $v_5.errorMessage = SP.RequestExecutorUtility.$N($v_6, $p0.getResponseHeader('content-type'), $p0.status.toString());
            SP.RequestExecutorMessageProcessor.$B($v_5, this.$H_0);
            return;
        }
        var $v_0 = $p0.getResponseHeader('content-type');

        if (SP.RequestExecutorUtility.$1($v_0) || ($v_0.toLowerCase()).indexOf('json') < 0) {
            var $v_7 = this.$d_0($p0);

            $v_7.errorCode = SP.RequestExecutorErrors.unexpectedResponse;
            $v_7.errorMessage = SP.RequestExecutorResources.getString('RE_RequestUnexpectedResponse');
            SP.RequestExecutorMessageProcessor.$B($v_7, this.$H_0);
            return;
        }
        var $v_1 = $p0.getResponseHeader('SharePointError');

        if (!SP.RequestExecutorUtility.$1($v_1)) {
            var $v_8 = this.$d_0($p0);

            $v_8.errorCode = SP.RequestExecutorErrors.unexpectedResponse;
            $v_8.errorMessage = SP.RequestExecutorResources.getString('RE_RequestUnexpectedResponse');
            SP.RequestExecutorMessageProcessor.$B($v_8, this.$H_0);
            return;
        }
        var $v_2 = JSON.parse($p0.responseText);

        if (SP.RequestExecutorUtility.$0($v_2)) {
            var $v_9 = this.$d_0($p0);

            $v_9.errorCode = SP.RequestExecutorErrors.unexpectedResponse;
            $v_9.errorMessage = SP.RequestExecutorResources.getString('RE_RequestUnexpectedResponse');
            SP.RequestExecutorMessageProcessor.$B($v_9, this.$H_0);
            return;
        }
        var $v_3 = new SP.RequestExecutorMessageProcessorFormDigestInfo();

        try {
            $v_3.digestValue = $v_2.d.GetContextWebInformation.FormDigestValue;
            var $v_A = $v_2.d.GetContextWebInformation.FormDigestTimeoutSeconds;

            $v_3.expiration = (new Date()).getTime() + $v_A * 750;
        }
        catch ($$e_B) {
            var $v_B = this.$d_0($p0);

            $v_B.errorCode = SP.RequestExecutorErrors.unexpectedResponse;
            $v_B.errorMessage = SP.RequestExecutorResources.getString('RE_RequestUnexpectedResponse');
            SP.RequestExecutorMessageProcessor.$B($v_B, this.$H_0);
            return;
        }
        if (!SP.RequestExecutorMessageProcessor.$U) {
            SP.RequestExecutorMessageProcessor.$U = {};
        }
        var $v_4 = SP.RequestExecutorMessageProcessor.$t(this.$2_0.url);

        SP.RequestExecutorMessageProcessor.$U[$v_4] = $v_3;
        this.$u_0($v_3.digestValue);
    },
    $d_0: function SP_RequestExecutorMessageProcessor$$d_0($p0) {
        var $v_0 = new SP.PostMessageResponseInfo();

        $v_0.command = 'Query';
        $v_0.errorCode = 0;
        $v_0.errorMessage = null;
        $v_0.postMessageId = this.$2_0.postMessageId;
        $v_0.responseAvailable = true;
        $v_0.statusText = $p0.statusText;
        $v_0.statusCode = $p0.status;
        $v_0.contentType = $p0.getResponseHeader('content-type');
        $v_0.allResponseHeaders = $p0.getAllResponseHeaders();
        $v_0.responseText = $p0.responseText;
        return $v_0;
    },
    $u_0: function SP_RequestExecutorMessageProcessor$$u_0($p0) {
        this.$4_0 = SP.RequestExecutor.$j();
        var $v_0 = this.$2_0.url;

        $v_0 = SP.RequestExecutorUtility.$c($v_0);
        this.$4_0.open(this.$2_0.method, $v_0);
        var $v_1 = false;

        if (this.$2_0.headers) {
            var $$dict_3 = this.$2_0.headers;

            for (var $$key_4 in $$dict_3) {
                var $v_3 = {
                    key: $$key_4,
                    value: $$dict_3[$$key_4]
                };

                if ($v_3.key.toLowerCase() === 'x-requestdigest') {
                    $v_1 = true;
                }
                this.$4_0.setRequestHeader($v_3.key, $v_3.value);
            }
        }
        if (!$v_1 && !SP.RequestExecutorUtility.$1($p0)) {
            this.$4_0.setRequestHeader('X-RequestDigest', $p0);
        }
        if (this.$2_0.binaryStringResponseBody) {
            SP.RequestExecutorInternalSharedUtility.$1L(this.$4_0);
        }
        if (this.$2_0.timeout > 0) {
            var $$t_7 = this;

            this.$I_0 = window.setTimeout(function() {
                $$t_7.$1j_0();
            }, this.$2_0.timeout);
        }
        var $$t_8 = this;

        this.$4_0.onreadystatechange = function() {
            $$t_8.$1i_0();
        };
        var $v_2;

        if (this.$2_0.binaryStringRequestBody) {
            $v_2 = SP.RequestExecutorInternalSharedUtility.$14(this.$2_0.body);
        }
        else {
            $v_2 = this.$2_0.body;
        }
        this.$4_0.send($v_2);
    },
    $1j_0: function SP_RequestExecutorMessageProcessor$$1j_0() {
        if (this.$4_0) {
            this.$4_0.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
            this.$4_0.abort();
            var $v_0 = new SP.PostMessageResponseInfo();

            $v_0.command = 'Query';
            $v_0.errorCode = SP.RequestExecutorErrors.requestAbortedOrTimedout;
            $v_0.errorMessage = SP.RequestExecutorResources.getString('RE_RequestAbortedOrTimedout');
            $v_0.postMessageId = this.$2_0.postMessageId;
            $v_0.responseAvailable = false;
            SP.RequestExecutorMessageProcessor.$B($v_0, this.$H_0);
        }
    },
    $1i_0: function SP_RequestExecutorMessageProcessor$$1i_0() {
        if (this.$4_0 && this.$4_0.readyState === 4) {
            if (this.$I_0) {
                window.clearTimeout(this.$I_0);
                this.$I_0 = 0;
            }
            this.$4_0.onreadystatechange = SP.RequestExecutorNative.emptyCallback;
            var $v_0 = new SP.PostMessageResponseInfo();

            $v_0.command = 'Query';
            $v_0.errorCode = 0;
            $v_0.errorMessage = null;
            $v_0.postMessageId = this.$2_0.postMessageId;
            $v_0.responseAvailable = true;
            $v_0.statusText = this.$4_0.statusText;
            $v_0.statusCode = this.$4_0.status;
            $v_0.contentType = this.$4_0.getResponseHeader('content-type');
            $v_0.allResponseHeaders = this.$4_0.getAllResponseHeaders();
            $v_0.binaryStringResponseBody = this.$2_0.binaryStringResponseBody;
            if (this.$2_0.binaryStringResponseBody) {
                $v_0.responseText = SP.RequestExecutorInternalSharedUtility.$13(this.$4_0.response);
            }
            else {
                $v_0.responseText = this.$4_0.responseText;
            }
            SP.RequestExecutorMessageProcessor.$B($v_0, this.$H_0);
        }
    }
};
SP.RequestExecutorNotificationPanel = function SP_RequestExecutorNotificationPanel() {
};
SP.RequestExecutorNotificationPanel.$1k = function SP_RequestExecutorNotificationPanel$$1k($p0) {
    if (!SP.RequestExecutorNotificationPanel.$F) {
        SP.RequestExecutorNotificationPanel.$F = {};
    }
    if (SP.RequestExecutorUtility.$0(SP.RequestExecutorNotificationPanel.$F[$p0])) {
        SP.RequestExecutorNotificationPanel.$F[$p0] = SP.RequestExecutorNotificationPanel.$x;
        SP.RequestExecutorNotificationPanel.$x++;
    }
    var $v_0 = SP.RequestExecutorNotificationPanel.$F[$p0];
    var $v_1 = document.getElementById('SP_RequestExecutor_NotificationPanel' + $v_0.toString());

    if ($v_1) {
        return;
    }
    $v_1 = document.createElement('DIV');
    $v_1.id = 'SP_RequestExecutor_NotificationPanel' + $v_0.toString();
    $v_1.style.position = 'absolute';
    $v_1.style.width = '420px';
    $v_1.style.borderStyle = 'solid';
    $v_1.style.borderWidth = '1px';
    $v_1.style.padding = '5px';
    $v_1.className = 'ms-subtleEmphasis';
    var $v_2 = SP.RequestExecutorNotificationPanel.$1Z() + 30;
    var $v_3 = SP.RequestExecutorNotificationPanel.$1W() + SP.RequestExecutorNotificationPanel.$1a() - 420 - 50;

    if ($v_3 < 0) {
        $v_3 = 0;
    }
    $v_1.style.left = $v_3.toString() + 'px';
    $v_1.style.top = $v_2.toString() + 'px';
    var $v_4 = SP.RequestExecutorUtility.$k(SP.RequestExecutorResources.getString('RE_OpenWindowMessage'));
    var $v_5 = '<input type=\'button\' onclick=\'SP.RequestExecutor.internalLoginButtonHandler(' + $v_0.toString() + ');return false;\' value=\'' + SP.RequestExecutorUtility.$k(SP.RequestExecutorResources.getString('RE_OpenWindowButtonText')) + '\' />';
    var $v_6 = '<a href=\'#\' onclick=\'SP.RequestExecutor.internalCancelButtonHandler(' + $v_0.toString() + ');return false;\'>' + SP.RequestExecutorUtility.$k(SP.RequestExecutorResources.getString('RE_DismissOpenWindowMessageLinkText')) + '</a>';
    var $v_7 = SP.RequestExecutorUtility.$k(SP.RequestExecutorResources.getString('RE_FixitHelpMessage'));

    $v_7 = SP.RequestExecutorUtility.$N($v_7, '<a href=\'http://go.microsoft.com/fwlink/?LinkId=255261\' target=\'_blank\'>', '</a>');
    $v_1.innerHTML = '<div class=\'ms-textXLarge ms-error\'>' + $v_4 + '</div><div style=\'padding-top:5px\'>' + $v_5 + '<span style=\'width:15px\'>&#160;&#160;&#160;</span>' + $v_6 + '</div>' + '<div style=\'padding-top:5px\'><span class=\'ms-metadata\'>' + $v_7 + '</span></div>';
    document.body.appendChild($v_1);
    if (!SP.RequestExecutorNotificationPanel.$1K) {
        window.setTimeout(function() {
            SP.RequestExecutor.internalCancelButtonHandler($v_0);
        }, 120000);
    }
};
SP.RequestExecutorNotificationPanel.$1A = function SP_RequestExecutorNotificationPanel$$1A($p0) {
    if (SP.RequestExecutorNotificationPanel.$F) {
        var $$dict_1 = SP.RequestExecutorNotificationPanel.$F;

        for (var $$key_2 in $$dict_1) {
            var $v_0 = {
                key: $$key_2,
                value: $$dict_1[$$key_2]
            };

            if ($v_0.value === $p0) {
                return $v_0.key;
            }
        }
    }
    return null;
};
SP.RequestExecutorNotificationPanel.$v = function SP_RequestExecutorNotificationPanel$$v($p0) {
    var $v_0 = document.getElementById('SP_RequestExecutor_NotificationPanel' + $p0.toString());

    if ($v_0) {
        $v_0.parentNode.removeChild($v_0);
    }
};
SP.RequestExecutorNotificationPanel.$1g = function SP_RequestExecutorNotificationPanel$$1g($p0) {
    if (SP.RequestExecutorNotificationPanel.$F) {
        var $v_0 = SP.RequestExecutorNotificationPanel.$F[$p0];

        if (!SP.RequestExecutorUtility.$0($v_0)) {
            SP.RequestExecutorNotificationPanel.$v($v_0);
        }
    }
};
SP.RequestExecutorNotificationPanel.$z = function SP_RequestExecutorNotificationPanel$$z($p0, $p1) {
    if (!SP.RequestExecutorNotificationPanel.$T) {
        SP.RequestExecutorNotificationPanel.$T = {};
    }
    SP.RequestExecutorNotificationPanel.$T[$p0] = $p1;
};
SP.RequestExecutorNotificationPanel.$1X = function SP_RequestExecutorNotificationPanel$$1X($p0) {
    if (!SP.RequestExecutorNotificationPanel.$T) {
        SP.RequestExecutorNotificationPanel.$T = {};
    }
    return SP.RequestExecutorNotificationPanel.$T[$p0];
};
SP.RequestExecutorNotificationPanel.$1a = function SP_RequestExecutorNotificationPanel$$1a() {
    var $v_0 = window.innerWidth;

    if (SP.RequestExecutorUtility.$0($v_0)) {
        $v_0 = document.documentElement.clientWidth;
    }
    if (SP.RequestExecutorUtility.$0($v_0)) {
        $v_0 = document.body.clientWidth;
    }
    return $v_0;
};
SP.RequestExecutorNotificationPanel.$1W = function SP_RequestExecutorNotificationPanel$$1W() {
    var $v_0 = window.pageXOffset;

    if (!SP.RequestExecutorUtility.$0($v_0)) {
        return $v_0;
    }
    if (!SP.RequestExecutorUtility.$0(document.documentElement) && !SP.RequestExecutorUtility.$0(document.documentElement.scrollLeft)) {
        return document.documentElement.scrollLeft;
    }
    return document.body.scrollLeft;
};
SP.RequestExecutorNotificationPanel.$1Z = function SP_RequestExecutorNotificationPanel$$1Z() {
    var $v_0 = window.pageYOffset;

    if (!SP.RequestExecutorUtility.$0($v_0)) {
        return $v_0;
    }
    if (!SP.RequestExecutorUtility.$0(document.documentElement) && !SP.RequestExecutorUtility.$0(document.documentElement.scrollTop)) {
        return document.documentElement.scrollTop;
    }
    return document.body.scrollTop;
};
SP.RequestExecutorResources = function SP_RequestExecutorResources() {
};
SP.RequestExecutorResources.getString = function SP_RequestExecutorResources$getString($p0) {
    var $v_0 = null;
    var $v_1 = (($p0.charAt(0)).toString()).toLowerCase() + $p0.substr(1);
    var $v_2 = ($p0.substr(0, 2)).toLowerCase() + $p0.substr(2);

    if (window.SP && window.SP.Res) {
        $v_0 = window.SP.Res[$v_1];
        if (SP.RequestExecutorUtility.$1($v_0)) {
            $v_0 = window.SP.Res[$v_2];
        }
    }
    if (SP.RequestExecutorUtility.$1($v_0) && window.SP && window.SP.RuntimeRes) {
        $v_0 = window.SP.RuntimeRes[$v_1];
    }
    if (SP.RequestExecutorUtility.$1($v_0)) {
        $v_0 = SP.RequestExecutorRes[$v_1];
    }
    if (SP.RequestExecutorUtility.$1($v_0)) {
        $v_0 = $p0;
    }
    return $v_0;
};
SP.RequestExecutorUtility = function SP_RequestExecutorUtility() {
};
SP.RequestExecutorUtility.$1 = function SP_RequestExecutorUtility$$1($p0) {
    var $v_0 = null;

    return $p0 === $v_0 || typeof $p0 === 'undefined' || !$p0.length;
};
SP.RequestExecutorUtility.$0 = function SP_RequestExecutorUtility$$0($p0) {
    var $v_0 = null;

    return $p0 === $v_0 || typeof $p0 === 'undefined';
};
SP.RequestExecutorUtility.$7 = function SP_RequestExecutorUtility$$7($p0) {
    return typeof $p0 === 'undefined';
};
SP.RequestExecutorUtility.$N = function SP_RequestExecutorUtility$$N($p0) {
    var $p1 = [];

    for (var $$pai_8 = 1; $$pai_8 < arguments.length; ++$$pai_8) {
        $p1[$$pai_8 - 1] = arguments[$$pai_8];
    }
    var $v_0 = '';
    var $v_1 = 0;

    while ($v_1 < $p0.length) {
        var $v_2 = SP.RequestExecutorUtility.$19($p0, $v_1, '{');

        if ($v_2 < 0) {
            $v_0 = $v_0 + $p0.substr($v_1);
            break;
        }
        else {
            var $v_3 = SP.RequestExecutorUtility.$19($p0, $v_2, '}');

            if ($v_3 > $v_2) {
                $v_0 = $v_0 + $p0.substr($v_1, $v_2 - $v_1);
                var $v_4 = $p0.substr($v_2 + 1, $v_3 - $v_2 - 1);
                var $v_5 = parseInt($v_4);

                $v_0 = $v_0 + $p1[$v_5];
                $v_1 = $v_3 + 1;
            }
            else {
                throw SP.RequestExecutorUtility.$r();
            }
        }
    }
    return $v_0;
};
SP.RequestExecutorUtility.$19 = function SP_RequestExecutorUtility$$19($p0, $p1, $p2) {
    var $v_0 = $p0.indexOf($p2, $p1);

    while ($v_0 >= 0 && $v_0 < $p0.length - 1 && $p0.charAt($v_0 + 1) === $p2) {
        $p1 = $v_0 + 2;
        $v_0 = $p0.indexOf($p2, $p1);
    }
    return $v_0;
};
SP.RequestExecutorUtility.$h = function SP_RequestExecutorUtility$$h($p0) {
    var $v_0;

    $v_0 = $p0.indexOf('#');
    if ($v_0 >= 0) {
        throw SP.RequestExecutorUtility.$M(SP.RequestExecutorUtility.$N(SP.RequestExecutorResources.getString('RE_InvalidArgumentOrField'), 'url'), SP.RequestExecutorExceptions.invalidArgumentOrField);
    }
    $v_0 = $p0.indexOf('?');
    if ($v_0 < 0) {
        $p0 = $p0 + '?';
    }
    else {
        if ($p0.charAt($p0.length - 1) !== '&') {
            $p0 = $p0 + '&';
        }
    }
    return $p0;
};
SP.RequestExecutorUtility.$a = function SP_RequestExecutorUtility$$a($p0, $p1) {
    if (!SP.RequestExecutorUtility.$1C($p0)) {
        throw SP.RequestExecutorUtility.$G($p1);
    }
};
SP.RequestExecutorUtility.$1C = function SP_RequestExecutorUtility$$1C($p0) {
    return ($p0.substr(0, 8)).toLowerCase() === 'https://' || ($p0.substr(0, 7)).toLowerCase() === 'http://';
};
SP.RequestExecutorUtility.$5 = function SP_RequestExecutorUtility$$5($p0, $p1, $p2, $p3) {
    if ($p3) {
        if (SP.RequestExecutorUtility.$0($p0) || typeof $p0 !== $p2) {
            throw SP.RequestExecutorUtility.$M(SP.RequestExecutorUtility.$N(SP.RequestExecutorResources.getString('RE_InvalidArgumentOrField'), $p1), SP.RequestExecutorExceptions.invalidArgumentOrField);
        }
    }
    else {
        if (!SP.RequestExecutorUtility.$0($p0) && typeof $p0 !== $p2) {
            throw SP.RequestExecutorUtility.$M(SP.RequestExecutorUtility.$N(SP.RequestExecutorResources.getString('RE_InvalidArgumentOrField'), $p1), SP.RequestExecutorExceptions.invalidArgumentOrField);
        }
    }
};
SP.RequestExecutorUtility.$M = function SP_RequestExecutorUtility$$M($p0, $p1) {
    var $v_0 = new Error($p0);

    $v_0.message = $p0;
    $v_0.errorCode = $p1;
    return $v_0;
};
SP.RequestExecutorUtility.$G = function SP_RequestExecutorUtility$$G($p0) {
    var $v_0 = SP.RequestExecutorUtility.$N(SP.RequestExecutorResources.getString('RE_InvalidArgumentOrField'), $p0);

    return SP.RequestExecutorUtility.$M($v_0, SP.RequestExecutorExceptions.invalidArgumentOrField);
};
SP.RequestExecutorUtility.$r = function SP_RequestExecutorUtility$$r() {
    var $v_0 = SP.RequestExecutorResources.getString('RE_InvalidOperation');

    return SP.RequestExecutorUtility.$M($v_0, SP.RequestExecutorExceptions.invalidOperation);
};
SP.RequestExecutorUtility.$q = function SP_RequestExecutorUtility$$q() {
    var $v_0 = SP.RequestExecutorResources.getString('RE_BrowserNotSupported');

    return SP.RequestExecutorUtility.$M($v_0, SP.RequestExecutorExceptions.browserNotSupported);
};
SP.RequestExecutorUtility.$s = function SP_RequestExecutorUtility$$s() {
    var $v_0 = SP.RequestExecutorResources.getString('RE_BrowserBinaryDataNotSupported');

    return SP.RequestExecutorUtility.$M($v_0, SP.RequestExecutorExceptions.browserNotSupported);
};
SP.RequestExecutorUtility.$c = function SP_RequestExecutorUtility$$c($p0) {
    return SP.RequestExecutorHttpUtility.$1M($p0, true, true);
};
SP.RequestExecutorUtility.$k = function SP_RequestExecutorUtility$$k($p0) {
    $p0 = $p0.replace(new RegExp('&', 'g'), '&amp;');
    $p0 = $p0.replace(new RegExp('\"', 'g'), '&quot;');
    $p0 = $p0.replace(new RegExp('\'', 'g'), '&#39;');
    $p0 = $p0.replace(new RegExp('<', 'g'), '&lt;');
    $p0 = $p0.replace(new RegExp('>', 'g'), '&gt;');
    return $p0;
};
SP.RequestExecutorRes = function SP_RequestExecutorRes() {
};
SP.RequestExecutorHttpUtility = function SP_RequestExecutorHttpUtility() {
};
SP.RequestExecutorHttpUtility.$1M = function SP_RequestExecutorHttpUtility$$1M($p0, $p1, $p2) {
    var $v_0 = '';
    var $v_1;
    var $v_2 = 0;
    var $v_3 = ' \"%<>\'&';
    var $v_4 = null;

    if ($p0 === $v_4 || typeof $p0 === 'undefined' || !$p0.length) {
        return '';
    }
    for ($v_2 = 0; $v_2 < $p0.length; $v_2++) {
        var $v_5 = $p0.charCodeAt($v_2);
        var $v_6 = $p0.charAt($v_2);

        if ($p1 && ($v_6 === '#' || $v_6 === '?')) {
            $v_0 += $p0.substr($v_2);
            break;
        }
        if ($v_5 <= 127) {
            if ($p2) {
                $v_0 += $v_6;
            }
            else {
                if ($v_5 >= 97 && $v_5 <= 122 || $v_5 >= 65 && $v_5 <= 90 || $v_5 >= 48 && $v_5 <= 57 || $v_5 >= 32 && $v_5 <= 95 && $v_3.indexOf($v_6) < 0) {
                    $v_0 += $v_6;
                }
                else if ($v_5 <= 15) {
                    $v_0 += '%0' + ($v_5.toString(16)).toUpperCase();
                }
                else if ($v_5 <= 127) {
                    $v_0 += '%' + ($v_5.toString(16)).toUpperCase();
                }
            }
        }
        else if ($v_5 <= 2047) {
            $v_1 = 192 | $v_5 >> 6;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
            $v_1 = 128 | $v_5 & 63;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
        }
        else if (($v_5 & 64512) !== 55296) {
            $v_1 = 224 | $v_5 >> 12;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
            $v_1 = 128 | ($v_5 & 4032) >> 6;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
            $v_1 = 128 | $v_5 & 63;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
        }
        else if ($v_2 < $p0.length - 1) {
            $v_5 = ($v_5 & 1023) << 10;
            $v_2++;
            var $v_7 = $p0.charCodeAt($v_2);

            $v_5 |= $v_7 & 1023;
            $v_5 += 65536;
            $v_1 = 240 | $v_5 >> 18;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
            $v_1 = 128 | ($v_5 & 258048) >> 12;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
            $v_1 = 128 | ($v_5 & 4032) >> 6;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
            $v_1 = 128 | $v_5 & 63;
            $v_0 += '%' + ($v_1.toString(16)).toUpperCase();
        }
    }
    return $v_0;
};
if (SP.PostMessageRequestInfo.registerClass)
    SP.PostMessageRequestInfo.registerClass('SP.PostMessageRequestInfo');
if (SP.PostMessageResponseInfo.registerClass)
    SP.PostMessageResponseInfo.registerClass('SP.PostMessageResponseInfo');
if (SP.RequestInfo.registerClass)
    SP.RequestInfo.registerClass('SP.RequestInfo');
if (SP.AppWebProxyRequestInfo.registerClass)
    SP.AppWebProxyRequestInfo.registerClass('SP.AppWebProxyRequestInfo');
if (SP.ResponseInfo.registerClass)
    SP.ResponseInfo.registerClass('SP.ResponseInfo');
if (SP.RequestExecutorMessageProcessorFormDigestInfo.registerClass)
    SP.RequestExecutorMessageProcessorFormDigestInfo.registerClass('SP.RequestExecutorMessageProcessorFormDigestInfo');
if (SP.RequestExecutorMessageProcessorInitInfo.registerClass)
    SP.RequestExecutorMessageProcessorInitInfo.registerClass('SP.RequestExecutorMessageProcessorInitInfo');
if (SP.PostMessageCommands.registerClass)
    SP.PostMessageCommands.registerClass('SP.PostMessageCommands');
if (SP.ProxyWebRequestExecutorInternal.registerClass)
    SP.ProxyWebRequestExecutorInternal.registerClass('SP.ProxyWebRequestExecutorInternal');
if (SP.ProxyWebRequestExecutor.registerClass)
    SP.ProxyWebRequestExecutor.registerClass('SP.ProxyWebRequestExecutor', Sys.Net.WebRequestExecutor);
if (SP.ProxyWebRequestExecutorFactory.registerClass)
    SP.ProxyWebRequestExecutorFactory.registerClass('SP.ProxyWebRequestExecutorFactory');
if (SP.RequestExecutor.registerClass)
    SP.RequestExecutor.registerClass('SP.RequestExecutor');
if (SP.RequestExecutorErrors.registerClass)
    SP.RequestExecutorErrors.registerClass('SP.RequestExecutorErrors');
if (SP.RequestExecutorExceptions.registerClass)
    SP.RequestExecutorExceptions.registerClass('SP.RequestExecutorExceptions');
if (SP.RequestExecutorInternalSharedUtility.registerClass)
    SP.RequestExecutorInternalSharedUtility.registerClass('SP.RequestExecutorInternalSharedUtility');
if (SP.RequestExecutorMessageProcessor.registerClass)
    SP.RequestExecutorMessageProcessor.registerClass('SP.RequestExecutorMessageProcessor');
if (SP.RequestExecutorNotificationPanel.registerClass)
    SP.RequestExecutorNotificationPanel.registerClass('SP.RequestExecutorNotificationPanel');
if (SP.RequestExecutorResources.registerClass)
    SP.RequestExecutorResources.registerClass('SP.RequestExecutorResources');
if (SP.RequestExecutorUtility.registerClass)
    SP.RequestExecutorUtility.registerClass('SP.RequestExecutorUtility');
if (SP.RequestExecutorRes.registerClass)
    SP.RequestExecutorRes.registerClass('SP.RequestExecutorRes');
if (SP.RequestExecutorHttpUtility.registerClass)
    SP.RequestExecutorHttpUtility.registerClass('SP.RequestExecutorHttpUtility');
SP.PostMessageCommands.ping = 'Ping';
SP.PostMessageCommands.query = 'Query';
SP.RequestExecutor.$D = null;
SP.RequestExecutor.$P = null;
SP.RequestExecutor.$K = null;
SP.RequestExecutor.$L = null;
SP.RequestExecutor.$9 = null;
SP.RequestExecutor.$V = null;
SP.RequestExecutor.$J = null;
SP.RequestExecutor.$C = null;
SP.RequestExecutor.$3 = null;
SP.RequestExecutor.$g = 0;
SP.RequestExecutor.$Q = false;
SP.RequestExecutor.$f = null;
SP.RequestExecutor.$X = null;
SP.RequestExecutor.$y = false;
SP.RequestExecutor.$Z = null;
SP.RequestExecutor.$Y = null;
SP.RequestExecutorErrors.requestAbortedOrTimedout = -1001;
SP.RequestExecutorErrors.unexpectedResponse = -1002;
SP.RequestExecutorErrors.httpError = -1002;
SP.RequestExecutorErrors.noAppWeb = -1003;
SP.RequestExecutorErrors.domainDoesNotMatch = -1004;
SP.RequestExecutorErrors.noTrustedOrigins = -1005;
SP.RequestExecutorErrors.iFrameLoadError = -1006;
SP.RequestExecutorExceptions.invalidArgumentOrField = -2001;
SP.RequestExecutorExceptions.invalidOperation = -2002;
SP.RequestExecutorExceptions.browserNotSupported = -2003;
SP.RequestExecutorInternalSharedUtility.$e = 'SP.RequestExecutor';
SP.RequestExecutorInternalSharedUtility.$1P = '_debugDelay_';
SP.RequestExecutorMessageProcessor.$U = null;
SP.RequestExecutorMessageProcessor.$E = null;
SP.RequestExecutorMessageProcessor.$Q = false;
SP.RequestExecutorNotificationPanel.$F = null;
SP.RequestExecutorNotificationPanel.$x = 0;
SP.RequestExecutorNotificationPanel.$T = null;
SP.RequestExecutorNotificationPanel.$1K = false;
SP.RequestExecutorRes.rE_NoTrustedOrigins = 'There is no trusted URLs configured for the app deployment.';
SP.RequestExecutorRes.rE_InvalidOperation = 'Invalid operation.';
SP.RequestExecutorRes.rE_CannotAccessSiteOpenWindowFailed = 'This page cannot open a window to access the web site \"{0}\" or it cannot reference the opened window. Please browse to that web site, and then browse to this page again.';
SP.RequestExecutorRes.rE_OpenWindowButtonText = 'Fix It';
SP.RequestExecutorRes.rE_BrowserNotSupported = 'The required functionalities are not supported by your browser. Please make sure you are using IE 8 or above, or other modern browser. Please make sure the \'X-UA-Compatible\' meta tag is set to be \'IE=8\' or above.';
SP.RequestExecutorRes.rE_RequestUnexpectedResponseWithContentTypeAndStatus = 'Unexpected response from the server. The content type of the response is \"{0}\". The status code is \"{1}\".';
SP.RequestExecutorRes.rE_InvalidArgumentOrField = 'Invalid field or parameter {0}.';
SP.RequestExecutorRes.rE_OpenWindowMessage = 'Sorry, we had some trouble accessing your site.';
SP.RequestExecutorRes.rE_CannotAccessSite = 'This page cannot access the web site \"{0}\". Please browse to that web site, and then browse to this page again.';
SP.RequestExecutorRes.rE_RequestAbortedOrTimedout = 'The request was aborted or timed out.';
SP.RequestExecutorRes.rE_DismissOpenWindowMessageLinkText = 'Dismiss';
SP.RequestExecutorRes.rE_BrowserBinaryDataNotSupported = 'Your browser doesn\'t support some HTML5 features like the File API operations. Please use a browser that does support these features.';
SP.RequestExecutorRes.rE_CannotAccessSiteCancelled = 'This page cannot access the web site \"{0}\". The login is cancelled or timed out. Please browse to that web site, and then browse to this page again.';
SP.RequestExecutorRes.rE_FixitHelpMessage = 'If the \"Fix it\" button doesn\'t solve the issue, {0}click here for more information{1}.';
SP.RequestExecutorRes.rE_RequestUnexpectedResponse = 'Unexpected response from server.';
SP.RequestExecutorRes.rE_DomainDoesNotMatch = 'Your domain doesn\'t match the expected domain for this app deployment.';
SP.RequestExecutorNative = function() {
};
SP.RequestExecutorNative.getIFrameOnloadCallback = function(appWebProxyUrl) {
    var ret = function() {
        SP.RequestExecutor.internalProcessIFrameOnload(appWebProxyUrl);
    };

    return ret;
};
SP.RequestExecutorNative.getXMLHttpRequestOnreadystatechangeCallback = function(xhr, requestInfo, timeoutId) {
    var ret = function() {
        SP.RequestExecutor.internalProcessXMLHttpRequestOnreadystatechange(xhr, requestInfo, timeoutId);
    };

    return ret;
};
SP.RequestExecutorNative.getXMLHttpRequestTimeoutCallback = function(xhr, requestInfo) {
    var ret = function() {
        SP.RequestExecutor.internalProcessXMLHttpRequestTimeoutCallback(xhr, requestInfo);
    };

    return ret;
};
SP.RequestExecutorNative.getIFrameRequestTimeoutCallback = function(postMessageId) {
    var ret = function() {
        SP.RequestExecutor.internalProcessIFrameRequestTimeoutCallback(postMessageId);
    };

    return ret;
};
SP.RequestExecutorNative.getProxyWebRequestExecutorSuccessCallback = function(executor) {
    var ret = function(responseInfo) {
        SP.ProxyWebRequestExecutorInternal.processSuccessCallback(executor, responseInfo);
    };

    return ret;
};
SP.RequestExecutorNative.getProxyWebRequestExecutorErrorCallback = function(executor) {
    var ret = function(responseInfo, errorCode, errorMessage) {
        SP.ProxyWebRequestExecutorInternal.processErrorCallback(executor, responseInfo, errorCode, errorMessage);
    };

    return ret;
};
SP.RequestExecutorNative.emptyCallback = function() {
};
SP.RequestExecutorNative.trim = function(str) {
    return str.replace(/^\s+|\s+$/g, '');
};
if (window.document.addEventListener) {
    window.addEventListener("load", SP.RequestExecutor.internalProcessWindowLoad, false);
}
else if (window.document.attachEvent) {
    window.attachEvent("onload", SP.RequestExecutor.internalProcessWindowLoad);
}
if (typeof Sys != "undefined" && Sys && Sys.Application) {
    Sys.Application.notifyScriptLoaded();
}
if (typeof NotifyScriptLoadedAndExecuteWaitingJobs == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("sp.requestexecutor.js");
}
