var context;
var web;
var list;
var appWebUrl;
var customersListUrl;
var count;


// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    customersListUrl = appWebUrl + '/lists/customers';
    context = SP.ClientContext.get_current();
    web = context.get_web();
    getList();

});

// This function prepares, loads, and then executes a SharePoint query to get the current list information
function getList() {
    list = web.get_lists().getByTitle('Customers');
    context.load(list);
    context.executeQueryAsync(onSuccess, onFail);
}

// This function is executed if the above OM call is successful
function onSuccess() {
    var count = list.get_itemCount();
    if (count > 0) {
        $('#message').html('Customers List Url: <a href="' + customersListUrl + '">' + customersListUrl + '</a>');
    }
    else {
        //add sample data to Customers list.
        $('#message').html('<button>Generate Sample Data</button>').find('button').click(function () {
            $(this).prop('disabled', true);

            count = sampledata.length;

            for (var i = 0; i < sampledata.length; i++) {
                jQuery.ajax({
                    url: appWebUrl + "/_api/web/lists/GetByTitle('Customers')/items",
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { 'type': 'SP.Data.CustomersListItem' },
                        'Title': sampledata[i].CustomerID,
                        'CompanyName1': sampledata[i].CompanyName,
                        'ContactName1': sampledata[i].ContactName,
                        'ContactTitle1': sampledata[i].ContactTitle,
                        'Address1': sampledata[i].Address,
                        'City1': sampledata[i].City,
                        'Country1': sampledata[i].Country,
                        'Phone1': sampledata[i].Phone
                    }),
                    headers: {
                        'accept': 'application/json;odata=verbose',
                        'content-type': 'application/json;odata=verbose',
                        'X-RequestDigest': $('#__REQUESTDIGEST').val()
                    },
                    success: function () {
                        if (--count== 0) {
                            $('#message button').hide();
                            $('#message').append('Successfully added sample data to the Customers list. <br/><br/>');
                            $('#message').append('Customers List Url: <a href="' + customersListUrl + '">' + customersListUrl + '</a>');
                        }
                    },
                    error: function () {
                        $('#message button').hide();
                        $('#message').append('Failed to add sample data to the Customers list.');
                    }
                });
            }

            return false;
        });
    }
}

// This function is executed if the above call fails
function onFail(sender, args) {
    alert('Failed to get list. Error:' + args.get_message());
}

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
