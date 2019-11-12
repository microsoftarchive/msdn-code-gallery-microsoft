<?php
error_reporting( E_ALL );
ini_set('display_errors', 1);

require_once "php-jwt/JWT.php";

try {
    $key = "Z4+KuFe1w7hXPbFkui/1Q8bWoxE+OB7+H1auotX1Kf4=";

    $appweburl = $_GET["SPAppWebUrl"];
    $appweburl_part = parse_url($appweburl);

    $msg = '';
    $options = '';

    if (!isset($_REQUEST['SPAppToken'])){
        $accToken = $_COOKIE["moss_access_token"];
    }
    else{
        $token = $_REQUEST['SPAppToken'];

        // decode the context token to extract the info required by ACS
    	$msg = JWT::decode($token, $key, false);

        $oAuthSrv = $msg->appctx;
        $oAuthUrl = json_decode($oAuthSrv);
        $acs_server = $oAuthUrl->SecurityTokenServiceUri;        

        $resources = str_replace('@','/' . $appweburl_part["host"] . '@' ,$msg->appctxsender);

	    // build the POST data as URLEncoded string
	    $postdata = array(
			'grant_type' => 'refresh_token',
			'client_id' => $msg->aud,
			'client_secret' => $key,
			'refresh_token' => $msg->refreshtoken,
			'resource' => $resources
	    );		
	    $querydata = http_build_query($postdata);
		
	    // Add additional headers
	    $opts = array(
			'Content-type: application/x-www-form-urlencoded', 
			'Expect: 100-continue'
	    );
		
    	// Request the token from ACS (manually build the POST request)
	    $ch = curl_init();
	    curl_setopt($ch, CURLOPT_URL, $acs_server);
	    curl_setopt($ch, CURLOPT_HTTPHEADER, $opts);
	    curl_setopt($ch, CURLOPT_HEADER, 0);
	    curl_setopt($ch, CURLOPT_POST, 1);
	    curl_setopt($ch, CURLOPT_POSTFIELDS, $querydata);
	    curl_setopt ($ch, CURLOPT_SSL_VERIFYHOST, 0);
	    curl_setopt ($ch, CURLOPT_SSL_VERIFYPEER, 0);
	    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	    curl_setopt($ch, CURLOPT_SSLVERSION,3);

    	$result = curl_exec($ch);

	    if(curl_errno($ch)) {
		    echo 'Unable to get the access token from ACS<br/>Error: ' . curl_error($ch);
		    die();
	    }
		
	    curl_close ($ch);
	    unset($ch);
	    unset($opts);

        // Get the access_token from the response
	    $json = json_decode($result);
        $accToken = $json->{'access_token'};
        setcookie("moss_access_token", $json->{'access_token'}, time()+36000);
    }

	$opts = array (
		'Authorization: Bearer ' . $accToken
	);

	$ch = curl_init();
    $url = $appweburl . '/_api/contextinfo';
	curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_HTTPHEADER, $opts);
	curl_setopt($ch, CURLOPT_HEADER, 0);
	curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_POSTFIELDS, '');
  	curl_setopt ($ch, CURLOPT_SSL_VERIFYHOST, 0);
	curl_setopt ($ch, CURLOPT_SSL_VERIFYPEER, 0);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

	$result = curl_exec($ch);

    $root = new SimpleXmlElement($result);  
    $ns = $root->getNameSpaces(TRUE);
    $childrenNodes = $root->children($ns['d']);
    $formValue = $childrenNodes->FormDigestValue;

	if(curl_errno($ch)) {
		echo 'Unable to get the list of users from SharePoint<br/>Error: ' . curl_error($ch);
		die();
	}
		
	curl_close ($ch);
	unset($ch);
	unset($opts);

    $opts = array (
	    'Authorization: Bearer ' . $accToken,
        'accept: application/json;odata=verbose'
	);

	$ch = curl_init();
    $url = $appweburl . "/_api/web/GetFolderByServerRelativeUrl('Lists/SharedDoc')/Files";
	curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_HTTPHEADER, $opts);
	curl_setopt($ch, CURLOPT_HEADER, 0);
	curl_setopt($ch, CURLOPT_POST, 0);
  	curl_setopt ($ch, CURLOPT_SSL_VERIFYHOST, 0);
	curl_setopt ($ch, CURLOPT_SSL_VERIFYPEER, 0);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

	$result = curl_exec($ch);

    if(curl_errno($ch)) {
	    echo 'Unable to get the list info ' . curl_error($ch);
		die();
	}
		
	curl_close ($ch);
	unset($ch);
	unset($opts);
		
} 
catch (Exception $e) {
    echo $e->getMessage();
}

?>


<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
	<meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title>Inventory From Database</title>
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/smoothness/jquery-ui-1.9.1.custom.min.css" rel="stylesheet" />
    <link href="css/jquery.contextMenu.css" rel="stylesheet" />
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.2.min.js"></script>
    <script src="js/jquery-ui-1.9.1.custom.min.js"></script>
    <script src="js/jquery.ui.position.js"></script>
    <script src="js/jquery.contextMenu.js"></script>
    <script src="js/knockout-2.1.0.js"></script>
    <script src="https://ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
    <script type="text/javascript">
        var hostweburl,appweburl,models,executorCtx = new Object();

        $(document).ready(function () {

            //Get the URI decoded URLs.
            hostweburl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));
            appweburl = decodeURIComponent(getQueryStringParameter("SPAppWebUrl"));

            // Resources are in URLs in the form:
            // <host_web_url>/_layouts/15/<resource>.js
            var scriptbase = hostweburl + "/_layouts/15/";

            // Load the js file and continue to the success handlers
            $.getScript(scriptbase + "SP.RequestExecutor.js", execCrossDomainRequest);
            $.getScript(scriptbase + "SP.UI.Controls.js", renderChromeControl);

            $('#btnNewDoc').click(function () {
                $("#dialog-modal-upload").dialog({
                    resizable: false,
                    height: 180,
                    modal: true
                });
            });

            models = new ShareDocsViewModel();
            ko.applyBindings(models)
        });

        function ShareDocsViewModel() {
            var self = this;
            self.AvailableNames = [];
            self.sharedDocs = ko.observableArray([]);

            self.getSharedDocByTitle = function (docTitle) {
                curShareDocs = self.sharedDocs();
                for (i = 0; i < curShareDocs.length; i++) {
                    if (docTitle == curShareDocs[i].Title)
                        return curShareDocs[i];
                };
                return null;
            };
            self.getSharedDocIndex = function (curDoc) {
                curShareDocs = self.sharedDocs();
                for (i = 0; i < curShareDocs.length; i++) {
                    if (curDoc.Title == curShareDocs[i].Title)
                        return i;
                };
                return -1;
            }
            self.updateSharedDoc = function (newDoc, docIndex) {
                if (docIndex == undefined) {
                    index = self.getSharedDocIndex(newDoc);
                }
                else {
                    index = docIndex;
                }
                curShareDocs = self.sharedDocs();
                curShareDocs.splice(index, 1);
                self.sharedDocs(curShareDocs);
                curShareDocs.splice(index, 0, newDoc);
                self.sharedDocs(curShareDocs);
                $('#loadingbar').hide();
            }
            self.getDisplayNameByUri = function (uri) {
                var retValue;
                $.each(self.AvailableNames, function (i, value) {
                    if (value.key == uri)
                        retValue = value.value;
                })
                return retValue;
            }
            self.updateTimeFormatAndTitle = function (item) {
                self.updateItem(item);
            }
            self.updateItem = function (item) {
                getUserDisplayName = function (item) {
                    var obj;
                    userUri = item.ModifiedBy.__deferred.uri;
                    $.each(models.AvailableNames(), function (i, value) {
                        if (value.key == userUri) {
                            obj = value;
                        }
                    });
                    if (obj == null) {
                        executorCtx.getUserName(userUri, function (result) {
                            title = JSON.parse(result.body).d.Title;
                            models.AvailableNames.push({ key: userUri, value: title });
                        })
                    }
                }

                item.ModifiedDate = item.TimeLastModified ? (new Date(item.TimeLastModified).getMonth() + 1) + "/" + new Date(item.TimeLastModified).getDate() + "/" + new Date(item.TimeLastModified).getFullYear() : "";
                item.Title = !item.Title ? item.Name : item.Title;
                item.FilePath = hostweburl.toLowerCase().match(/^(http(s)?:\/\/)?([\w-]+\.)+[\w-]+/g) + item.ServerRelativeUrl;
                item.isCheckOut = item.CheckOutType ? "checkin" : "checkout";
                var displayName = models.getDisplayNameByUri(item.ModifiedBy.__deferred.uri);
                item.DisplayName = ko.observable(displayName);

                if (item.DisplayName() == null) {
                    executorCtx.getUserName(item.ModifiedBy.__deferred.uri, function (result) {
                        title = JSON.parse(result.body).d.Title;
                        models.AvailableNames.push({ key: item.ModifiedBy.__deferred.uri, value: title });
                        item.DisplayName(title);
                    })
                }
            }
        }

        // Function to prepare and issue the request to get
        // SharePoint data.
        function execCrossDomainRequest() {
            // Initialize the RequestExecutor with the app web URL.
            executorCtx.executor = new SP.RequestExecutor(appweburl);

            executorCtx.getAllFiles = function (successEvent) {
                $('#loadingbar').show();
                this.executor.executeAsync({
                    url: appweburl + "/_api/web/GetFolderByServerRelativeUrl('Lists/SharedDoc')/Files",
                    method: "GET",
                    headers: { "Accept": "application/json; odata=verbose" },
                    success: function (data) {
                        $('#loadingbar').hide();
                        var jsonObject = JSON.parse(data.body);
                        items = jsonObject.d.results;
                        successEvent(items);
                    },
                    error: function () {
                        $('#loadingbar').hide();
                        alert("Get all files failed.");
                    }
                });
            };

            executorCtx.checkInFile = function (curItem) {
                var uri = curItem.__metadata.uri + "/CheckIn(comment='Check-in Comment', checkintype=1)";
                this.executor.executeAsync({
                    url: uri,
                    method: "POST",
                    headers: { "accept": "application/json;odata=verbose" },
                    success: function () {
                        alert("Check-in successful");
                        executorCtx.getFileByUri(curItem.__metadata.uri, function (data) {
                            models.updateSharedDoc(data);
                        });
                    },
                    error: function () {
                        $('#loadingbar').hide();
                        alert("Check-in failed.");
                    }
                });
            };

            executorCtx.checkOutFile = function (curItem) {
                $('#loadingbar').show();
                var uri = curItem.__metadata.uri + "/CheckOut()";
                this.executor.executeAsync({
                    url: uri,
                    method: "POST",
                    headers: { "accept": "application/json;odata=verbose" },
                    success: function () {
                        alert("Check-out successful");
                        executorCtx.getFileByUri(curItem.__metadata.uri, function (data) {
                            models.updateSharedDoc(data);
                        });
                    },
                    error: function () {
                        $('#loadingbar').hide();
                        alert("Check-out failed");
                    }
                });
            };

            executorCtx.getFileProperties = function (curItem, successEvent) {
                this.executor.executeAsync({
                    url: curItem.ListItemAllFields.__deferred.uri,
                    method: "GET",
                    headers: { "accept": "application/json;odata=verbose" },
                    success: successEvent,
                    error: function () {
                        $('#loadingbar').hide();
                        alert("Get file properties failed");
                    }
                });
            }

            executorCtx.updateFileProperties = function (curItem, obj, updateBody) {
                index = models.getSharedDocIndex(curItem);
                this.executor.executeAsync({
                    url: obj.__metadata.uri,
                    method: "POST",
                    body: updateBody,
                    headers: {
                        "IF-MATCH": obj.__metadata.etag,
                        "X-HTTP-Method": "MERGE",
                        "accept": "application/json;odata=verbose",
                        "content-type": "application/json;odata=verbose"
                    },
                    success: function () {
                        alert("Update Properties Success");
                        executorCtx.getFileByUri(curItem.__metadata.uri, function (data) {
                            models.updateSharedDoc(data, index);
                        });
                    },
                    error: function () {
                        $('#loadingbar').hide();
                        alert("Update file properties failed");
                    }
                });
            }

            executorCtx.getFileByUri = function (fileUri, successEvent) {
                this.executor.executeAsync({
                    url: fileUri,
                    method: "GET",
                    headers: { "Accept": "application/json; odata=verbose" },
                    success: function (data) {
                        $('#loadingbar').hide();
                        var jsonObject = JSON.parse(data.body);
                        models.updateItem(jsonObject.d);
                        successEvent(jsonObject.d);
                    },
                    error: function () {
                        $('#loadingbar').hide();
                        alert("Get file" + fileUri + "failed");
                    }
                });
            }

            executorCtx.getUserName = function (uri, successEvent) {
                this.executor.executeAsync({
                    url: uri,
                    method: "GET",
                    headers: { "Accept": "application/json; odata=verbose" },
                    success: successEvent,
                    error: function () {
                        alert("Get user" + uri + "failed");
                    }
                });
            }

            executorCtx.getAllFiles(function (items) {
                for (var i = 0; i < items.length; i++) {
                    models.updateTimeFormatAndTitle(items[i]);
                    models.sharedDocs.push(items[i]);
                }
                contextMenuBinding();
            });
        }

        function contextMenuBinding() {
            $('#loadingbar').hide();

            $.contextMenu({
                selector: "a",
                build: function ($trigger, e) {
                    return {
                        callback: function (e) {
                            switch (e) {
                                case "Download":
                                    window.open($(this).attr('href'));
                                    break;
                                case "CheckIn":
                                    $('#loadingbar').show();
                                    var curItem = models.getSharedDocByTitle($(this).text());
                                    executorCtx.checkInFile(curItem);
                                    break;
                                case "CheckOut":
                                    $('#loadingbar').show();
                                    var curItem = models.getSharedDocByTitle($(this).text());
                                    executorCtx.checkOutFile(curItem);
                                    break;
                                case "Edit":
                                    $("#dialog-modal-update input").val($(this).text());
                                    var curItem = models.getSharedDocByTitle($(this).text());

                                    $("#dialog-modal-update").dialog({
                                        resizable: false,
                                        height: 180,
                                        modal: true,
                                        buttons: {
                                            Save: function () {
                                                $('#loadingbar').show();
                                                executorCtx.getFileProperties(curItem, function (results) {
                                                    var obj = JSON.parse(results.body).d;
                                                    var payload = JSON.stringify({ '__metadata': { 'type': 'SP.Data.SharedDocListItem' },
                                                        'Title': $('#txtTitle').val()
                                                    });
                                                    executorCtx.updateFileProperties(curItem, obj, payload)
                                                });

                                                $(this).dialog("close");
                                            },
                                            Cancel: function () {
                                                $(this).dialog("close");
                                            }
                                        }
                                    });
                                    break;
                            }
                        },
                        items:
                                {
                                    Download: { name: "Download" },
                                    CheckIn: { name: "Check In" },
                                    CheckOut: { name: "Check Out" },
                                    Edit: { name: "Edit Properites" }
                                }
                    };
                }
            });
        }

        // Function to retrieve a query string value.
        // For production purposes you may want to use
        // a library to handle the query string.
        function getQueryStringParameter(paramToRetrieve) {
            var params = document.URL.split("?")[1].split("&");
            var strParams = "";
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split("=");
                if (singleParam[0] == paramToRetrieve)
                    return singleParam[1];
            }
        }

        function renderChromeControl() {
            var options = {
                "appTitle": "Shared Documents"
            };

            var nav = new SP.UI.Controls.Navigation("SharePointChromeControl", options);
            nav.setVisible(true);
        }

    </script>
</head>
<body>
    <input type="hidden" id="formDigest" value="<?php echo $formValue ?>" />
    <input type="hidden" id="targetUrl" value="<?php echo $appweburl ?>" />
    <div id="SharePointChromeControl"></div>
    <div id="res">
        <table>
            <thead>
                <tr>
                    <th style="width: 50%">Title</th>
                    <th>Last Modified</th>
                    <th>Modified By</th>
                </tr>
            </thead>
            <tbody data-bind="foreach: sharedDocs">
                <tr>
                    <td><a data-bind="text: Title, attr: { href: FilePath}" target="_blank"></a><span class="checkouticon" data-bind="attr: { class: isCheckOut}">
                        <img src="imgs/spcommon.png" /></span></td>
                    <td data-bind="text: ModifiedDate"></td>
                    <td data-bind="text: DisplayName()"></td>
                </tr>
            </tbody>
        </table>
        <div id="loadingbar" style="text-align: center; position: relative; top: -32px;">
            <img class="group-image" src="data:image/gif;base64,R0lGODlhGAAYAJECAP///5mZmf///wAAACH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgACACwAAAAAGAAYAAACQJQvAGgRDI1SyLnI5jr2YUQx10eW5hmeB6Wpkja5SZy6tYzn+g5uMhuzwW6lFtF05CkhxGQm+HKuoDPplOlDFAAAIfkEBQoAAgAsFAAGAAQABAAAAgVUYqeXUgAh+QQFCgACACwUAA4ABAAEAAACBVRip5dSACH5BAUKAAIALA4AFAAEAAQAAAIFVGKnl1IAIfkEBQoAAgAsBgAUAAQABAAAAgVUYqeXUgAh+QQFCgACACwAAA4ABAAEAAACBVRip5dSACH5BAUKAAIALAAABgAEAAQAAAIFVGKnl1IAIfkECQoAAgAsBgAAAAQABAAAAgVUYqeXUgAh+QQJCgACACwAAAAAGAAYAAACJZQvEWgADI1SyLnI5jr2YUQx10eW5omm6sq27gvH8kzX9o3ndAEAIfkECQoAAgAsAAAAABgAGAAAAkCULxFoAAyNUsi5yOY69mFEMddHluYZntyjqY3Vul2yucJo5/rOQ6lLiak0QtSEpvv1lh8l0lQsYqJHaO3gFBQAACH5BAkKAAIALAAAAAAYABgAAAJAlC8RaAAMjVLIucjmOvZhRDHXR5bmGZ7co6mN1bpdsrnCaOf6zkOpzJrYOjHV7Gf09JYlJA0lPBQ/0ym1JsUeCgAh+QQJCgACACwAAAAAGAAYAAACQJQvEWgADI1SyLnI5jr2YUQx10eW5hme3KOpjdW6XbK5wmjn+s5Dqcya2Dox1exn9PSWJeRNSSo+cR/pzOSkHgoAIfkECQoAAgAsAAAAABgAGAAAAkCULxFoAAyNUsi5yOY69mFEMddHluYZntyjqY3Vul2yucJo5/rOQ6nMmtg6MdXsZ/T0liXc6jRbOTHR15SqfEIKACH5BAkKAAIALAAAAAAYABgAAAJAlC8RaAAMjVLIucjmOvZhRDHXR5bmGZ7co6mN1bpdsrnCaOf6zkO4/JgBOz/TrHhC9pYRpNJnqURLwtdT5JFGCgAh+QQJCgACACwAAAAAGAAYAAACPpQvEWgADI1SyLnI5jr2YUQx10eW5jme3NOpTWe5Qpu6tYzn+l558tWywW4lmk/IS6KOr2UtSILOYiYiUVAAADs=" />
        </div>
        <button id="btnNewDoc">Add New Document</button>
    </div>
    <div id="dialog-modal-upload" title="Upload SharePoint Document" style="display: none">
        <iframe src="uploader.html" class="uploadForm"></iframe>
    </div>
    <div id="dialog-modal-update" title="Edit Properties" style="display: none">
        <p>
            Title:
            <input id="txtTitle" type="text" value="" />
        </p>
    </div>
</body>
</html>
