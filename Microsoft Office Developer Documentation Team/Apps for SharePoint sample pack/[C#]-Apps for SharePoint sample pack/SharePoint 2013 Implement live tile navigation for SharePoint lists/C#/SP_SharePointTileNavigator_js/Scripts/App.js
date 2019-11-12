// Variables used to hold objects for use in callback functions
var context;
var lists;
//var list;
var listItems;
var tileArea;
// This code runs when the DOM is ready and creates a context object which is needed 
// to use the SharePoint object model
$(document).ready(function () {
    
    context = SP.ClientContext.get_current();
    var web = context.get_web();
    lists = context.get_web().get_lists();
    context.load(lists);
    context.executeQueryAsync(function () { renderListTiles(); }, function () { errorLoadingLists(); });
   
});

function errorLoadingLists() {
    tileArea = document.getElementById("tileArea");

    // Remove all nodes from the chart <DIV> so we have a clean space to write to
    while (tileArea.hasChildNodes()) {
        tileArea.removeChild(tileArea.lastChild);
    }

    // Write a message to let the user know the operation has failed
    var errMessage = document.createElement("div");
    errMessage.appendChild(document.createTextNode("Lists could not be retrieved."));
}

function renderListTiles() {
    var listEnumerator = lists.getEnumerator();
    tileArea = document.getElementById("tileArea");
    while (listEnumerator.moveNext()) {
        var list = listEnumerator.get_current();
        var listTitle = list.get_title();
        if ((listTitle == "Employees") || (listTitle == "MarketSize") || (listTitle == "Sales")) {
            var itemCount = list.get_itemCount();
            var tile = document.createElement("a");
            tile.setAttribute("class", "tile");
            tile.setAttribute("href", "../Lists/" + listTitle);
            tile.appendChild(document.createTextNode(listTitle));
            tileArea.appendChild(tile);
            var tileBody = document.createElement("div");
            tileBody.setAttribute("class", "tileNumber");
            tileBody.appendChild(document.createTextNode(itemCount.toString()));
            tile.appendChild(tileBody);
        }
    }
}
