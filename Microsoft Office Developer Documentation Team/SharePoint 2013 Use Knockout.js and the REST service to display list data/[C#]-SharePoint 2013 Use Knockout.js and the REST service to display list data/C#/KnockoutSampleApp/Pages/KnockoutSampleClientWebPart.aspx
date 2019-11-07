<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<WebPartPages:AllowFraming ID="AllowFraming" runat="server" />

<html>
<head>
    <title></title>
    <style>
        .liveExample {
            padding: 1em;
            background-color: #EEEEDD;
            border: 1px solid #CCC;
            max-width: 760px;
        }

            .liveExample input {
                font-family: Arial;
            }

            .liveExample b {
                font-weight: bold;
            }

            .liveExample p {
                margin-top: 0.9em;
                margin-bottom: 0.9em;
            }

            .liveExample select[multiple] {
                width: 100%;
                height: 8em;
            }

            .liveExample h2 {
                margin-top: 0.4em;
            }

        .ko-grid {
            margin-bottom: 1em;
            width: 25em;
            border: 1px solid silver;
            background-color: White;
        }

            .ko-grid th {
                text-align: left;
                background-color: Black;
                color: White;
            }

            .ko-grid td, th {
                padding: 0.4em;
            }

            .ko-grid tr:nth-child(odd) {
                background-color: #DDD;
            }

        .ko-grid-pageLinks {
            margin-bottom: 1em;
        }

            .ko-grid-pageLinks a {
                padding: 0.5em;
            }

                .ko-grid-pageLinks a.selected {
                    background-color: Black;
                    color: White;
                }

        .liveExample {
            height: 400px;
            overflow: auto;
        }
    </style>
    <script src="../Scripts/jquery-1.7.1.min.js"></script>
    <script src="../Scripts/knockout-2.2.1.js"></script>
    <script src="../Scripts/knockout.simpleGrid.1.3.js"></script>
    <script type="text/javascript">
        // Set the style of the client web part page to be consistent with the host web.
        function setStyleSheet() {
            var hostUrl = ""
            if (document.URL.indexOf("?") != -1) {
                var params = document.URL.split("?")[1].split("&");
                for (var i = 0; i < params.length; i++) {
                    p = decodeURIComponent(params[i]);
                    if (/^SPHostUrl=/i.test(p)) {
                        hostUrl = p.split("=")[1];
                        document.write("<link rel=\"stylesheet\" href=\"" + hostUrl + "/_layouts/15/defaultcss.ashx\" />");
                        break;
                    }
                }
            }
            if (hostUrl == "") {
                document.write("<link rel=\"stylesheet\" href=\"/_layouts/15/1033/styles/themable/corev15.css\" />");
            }
        }
        setStyleSheet();
    </script>

    <script>
        function getQueryStringParameter(paramToRetrieve) {
            var params = document.URL.split("?").length > 1 ?
                document.URL.split("?")[1].split("&") : [];
            var strParams = "";
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split("=");
                if (singleParam[0] == paramToRetrieve)
                    return singleParam[1];
            }
        }

        $(function () {
            var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
            jQuery.ajax({
                url: appWebUrl + "/_api/web/lists/GetByTitle('Customers')/items",
                type: 'GET',
                headers: {
                    'accept': 'application/json;odata=verbose',
                    'X-RequestDigest': $('#__REQUESTDIGEST').val()
                },
                success: function (data) {
                    var initialData = data.d.results;
                    var PagedGridModel = function (items) {
                        this.items = ko.observableArray(items);

                        this.gridViewModel = new ko.simpleGrid.viewModel({
                            data: this.items,
                            columns: [
                                { headerText: "Customer ID", rowText: "Title" },
                                { headerText: "Company Name", rowText: "CompanyName1" },
                                { headerText: "Contact Name", rowText: "ContactName1" },
                                { headerText: "Contact Title", rowText: "ContactTitle1" },
                                { headerText: "Address", rowText: "Address1" },
                                { headerText: "City", rowText: "City1" },
                                { headerText: "Country", rowText: "Country1" },
                                { headerText: "Phone", rowText: "Phone1" }
                            ],
                            pageSize: 4
                        });
                    };

                    ko.applyBindings(new PagedGridModel(initialData));
                },
                error: function () {
                }
            });
        });
    </script>
</head>
<body>
    <div class='liveExample'>
        <div data-bind='simpleGrid: gridViewModel'></div>
    </div>
</body>
</html>
