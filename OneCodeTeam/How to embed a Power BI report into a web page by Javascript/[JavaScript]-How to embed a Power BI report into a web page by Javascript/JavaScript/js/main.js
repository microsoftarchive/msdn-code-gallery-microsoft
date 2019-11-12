
var AccessKey;
var WorkspaceCollection;
var Reports;

//Embed Power BI reprt
//Button click event
$("#btnEmbedPBI").click(function () {
    var reportId = $("#reportList").val();
    if (reportId && Reports && AccessKey && WorkspaceCollection) {
        var report = Reports.find(function (data) { return data.id == reportId; });
        var workspaceID = $("#workspaceList").val();
        embedReport(report, AccessKey, WorkspaceCollection, workspaceID);
    }
    else {
        alert('Please choose a report.');
    }
});

//Get workspaces according to the WorkspaceCollectionName & AccessKey
$("#btnGetWorkspaces").click(function () {
    var workspaceCollection = $("#wcnInput").val();
    var accessKey = $("#keyInput").val();
    if (workspaceCollection && accessKey) {
        WorkspaceCollection = workspaceCollection;
        AccessKey = accessKey;
        getWorkspaces(workspaceCollection, accessKey);
    } else {
        alert('Please input your WorkspaceCollectionName and AccessKey.');
    }
});

//Load report list when workspace changed
$("#workspaceList").on("change", function () {
    var workspaceId = $("#workspaceList").val();
    if (WorkspaceCollection && AccessKey && workspaceId) {
        getReports(workspaceId, WorkspaceCollection, AccessKey);
    }
})

//get access token
function getAccessToken(accessKey, workspaceId, reportId, workspaceCollectionName) {
    var token1 = '{"alg": "HS256","typ": "JWT"}';
    var nbf = new Date().getTime() / 1000 | 0;
    var exp = new Date().setTime(new Date().getTime() + 60 * 60 * 1000) / 1000 | 0;
    var token2 = '{"ver":"0.2.0","wcn":"' + workspaceCollectionName + '","wid": "' + workspaceId + '","rid":"' + reportId + '","iss":"PowerBISDK","aud":"https://analysis.windows.net/powerbi/api","exp":' + exp + ',"nbf":' + nbf + '}'
    var endcodedToken = encode_helper(token1) + '.' + encode_helper(token2);
    var hash = CryptoJS.HmacSHA256(endcodedToken, accessKey);
    var hashInBase64 = CryptoJS.enc.Base64.stringify(hash);
    var sig = formatString(hashInBase64);
    var accessToken = endcodedToken + '.' + sig;
    return accessToken;
}

function encode_helper(arg) {
    var response = btoa(arg);
    return formatString(response);
}

//replace special char in the string
function formatString(arg) {
    var response = arg;
    response = response.replace(/\//g, "_");
    response = response.replace(/\+/g, "-");
    response = response.replace(/=+$/, '');;
    return response;
}

//Get workspaces
function getWorkspaces(workspaceCollectionName, accessKey) {
    var requestUrl = "https://api.powerbi.com/v1.0/collections/" + workspaceCollectionName + "/workspaces";
    var request = new Request(requestUrl, {
        headers: new Headers({
            'Authorization': 'AppKey ' + accessKey
        })
    });
    fetch(request).then(function (response) {
        if (response.ok) {
            return response.json()
                .then(function (data) {
                    listWorkspaces(data.value, workspaceCollectionName, accessKey);
                });
        }
    });

}

//List workspaces, and get reports of the first workspace
function listWorkspaces(data, workspaceCollectionName, accessKey) {
    $.each(data, function (n, value) {
        $('#workspaceList').append($('<option>', { value: value.workspaceId }).text(value.displayName));
    });
    if (data.length > 0) {
        getReports(data[0].workspaceId, workspaceCollectionName, accessKey);
    }
}

//Get reports
function getReports(workspaceId, workspaceCollectionName, accessKey) {
    var reportRequest = "https://api.powerbi.com/v1.0/collections/" + workspaceCollectionName + "/workspaces/" + workspaceId + "/reports";
    var request1 = new Request(reportRequest, {
        headers: new Headers({
            'Authorization': 'AppKey ' + accessKey
        })
    });
    fetch(request1).then(function (response) {
        if (response.ok) {
            return response.json().then(function (data) {
                listReports(data.value);
            });
        }
    });
}

//list reports
function listReports(data) {
    Reports = data;
    $.each(data, function (n, value) {
        $('#reportList').append($('<option>', { value: value.id }).text(value.name));
    });
}

//embed report to page
function embedReport(report, accessKey, workspaceCollectionName, workspaceId) {
    var embedUrl = report.embedUrl;
    var name = report.name;
    var reportId = report.id;
    var webUrl = report.webUrl;
    var token = getAccessToken(accessKey, workspaceId, reportId, workspaceCollectionName);
    var embedConfiguration = {
        type: 'report',
        accessToken: token,
        id: reportId,
        embedUrl: embedUrl
    };
    var $reportContainer = $('#reportContainer');
    var report = powerbi.embed($reportContainer.get(0), embedConfiguration);
}