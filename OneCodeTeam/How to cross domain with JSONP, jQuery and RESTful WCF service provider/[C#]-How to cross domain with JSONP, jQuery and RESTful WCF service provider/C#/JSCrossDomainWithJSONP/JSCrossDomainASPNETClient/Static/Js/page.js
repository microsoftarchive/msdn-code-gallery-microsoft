/****************************** Module Header ******************************\
Module Name:  page.js
Project:      JSCrossDomainWCFProvider
Copyright (c) Microsoft Corporation.
	 
Page javascript script to read a JSONP request and update UI
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

var pageNum = 2;
var url = "http://localhost:50500/Agent.aspx?callBack=";
var data;
var pageIndex = 1;
var pageCount;

function init() {
    $("#gdjson").bind("click", getAllUsersByJQuery);
    $("#gdori").bind("click", getAllUsersByOriginalJS);
    $(".first, .last, .previous, .next").bind("click", fillView);
}

//Get data by using $.getJSON
function getAllUsersByJQuery(event) {
    event && event.preventDefault();

    $.getJSON(url + "?", jsonpCallBack);
}

//Get data by using original JavaScript with "createElement"
function getAllUsersByOriginalJS(event) {
    event && event.preventDefault();

    var jsonpScript = document.createElement("script");
    jsonpScript.type = "text/javascript";
    jsonpScript.src = url + "jsonpCallBack";
    document.head.appendChild(jsonpScript);
}

//Call back function
function jsonpCallBack(users) {
    data = users
    pageCount = data.length % pageNum == 0 ? data.length / pageNum : parseInt(data.length / pageNum) + 1;

    $("#datacount").text(data.length);
    $("#pagecount").text(pageCount);
    $("#pageindex").text(pageIndex);
    $("#error").empty();
    fillView();
}

//Navigate bar << < > >>
function fillView(event) {
    if (!data) {
        $("#error").text("Users count is 0, please click query button to get data from service");
        return;
    }

    event && event.preventDefault();

    switch ($(this).attr("class") || "first") {
        case "first":
            pageIndex = 1;
            break;
        case "previous":
            pageIndex != 1 && pageIndex--;
            break;
        case "next":
            pageIndex != pageCount && pageIndex++;
            break;
        case "last":
            pageIndex = pageCount;
            break;
    }
    getUserListFromData();
    $("#pageindex").text(pageIndex);
}

//Get data, fill to table and paging
function getUserListFromData() {
    var start = (pageIndex - 1) * pageNum;
    var end;
    if (pageIndex == pageCount) {
        end = data.length;
    }
    else {
        end = pageIndex * pageNum;
    }

    var rows = [];
    while (start != end) {
        var tempRow = "<tr>";
        tempRow += "<td>" + data[start]["Id"] + "</td>";
        tempRow += "<td>" + data[start]["Name"] + "</td>";
        tempRow += "<td>" + data[start]["Age"] + "</td>";
        tempRow += "<td>" + (data[start]["Sex"] ? "Female" : "Male") + "</td>";
        tempRow += "<td>" + data[start]["Comments"] + "</td>";
        tempRow += "</tr>";
        rows.push(tempRow);
        tempRow = null;
        start++;
    }
    $("#gridview tr[class!=title]").remove();
    $("#gridview").append(rows.join(""));
    rows = [];
}
