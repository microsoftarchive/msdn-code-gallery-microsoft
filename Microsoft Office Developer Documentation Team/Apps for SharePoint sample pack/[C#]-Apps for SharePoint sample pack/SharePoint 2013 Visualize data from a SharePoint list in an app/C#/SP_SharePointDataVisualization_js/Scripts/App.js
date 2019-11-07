// Variables used in various functions and callbacks
var context;
var list;
var listItems;

// This code runs when the DOM is ready and creates a 
//context object which is needed to use the SharePoint object model
$(document).ready(function () {

    context = new SP.ClientContext.get_current();

    // The PopulationData list is deployed as part of this App. It has
    // the population data for the top ten countrys in it.
    // The following lines allow us to obtain the data from the list
    list = context.get_web().get_lists().getByTitle('PopulationData');
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml('<View><RowLimit>10</RowLimit></View>');
    listItems = list.getItems(camlQuery);
    context.load(listItems, 'Include(Country, Population)');

    // Make an asynchronous call, which will load our list
    context.executeQueryAsync(

        // Success callback
        function () {
            startTable();
        },

        // Failure callback
        function (sender, args) {
            var dataArea = document.getElementById("populationData");
            // Remove all nodes from the chart <DIV> so we have a clean space to write to
            while (dataArea.hasChildNodes()) {
                dataArea.removeChild(dataArea.lastChild);
            }
            dataArea.appendChild(document.createTextNode("Failed to get population data. Error: "
                + args.get_message()));
        });
});

// This function is called in the success callback handler (above)
// It simply iterates through the list and displays the population data in a table 
function startTable() {
    var tableArea = document.getElementById("populationData");
    // Remove all nodes from the chart <DIV> so we have a clean space to write to
    while (tableArea.hasChildNodes()) {
        tableArea.removeChild(tableArea.lastChild);
    }
    var leftColumn = document.createElement("div");
    leftColumn.setAttribute("style", "float:left;width:100px;background-color:#AFAFAF");
    var rightColumn = document.createElement("div");
    rightColumn.setAttribute("style", "float:left;width:100px;background-color:#FAFAFA");
    var listItemEnumerator = listItems.getEnumerator();
    while (listItemEnumerator.moveNext()) {
        var listItem = listItemEnumerator.get_current();
        var countryName = listItem.get_fieldValues()["Country"];
        var countryPopulation = listItem.get_fieldValues()["Population"];

        var countryLabel = document.createElement("div");
        countryLabel.setAttribute("style", "float:none;margin:5px;height:25px;width:120px;background-color:#AFAFAF;color:#FFFFFF");
        countryLabel.appendChild(document.createTextNode(countryName));
        leftColumn.appendChild(countryLabel);

        var populationLabel = document.createElement("div");
        populationLabel.setAttribute("style", "float:none;margin:5px;height:25px;width:120px;background-color:#FAFAFA");
        populationLabel.appendChild(document.createTextNode(countryPopulation));
        rightColumn.appendChild(populationLabel);
    }
    tableArea.appendChild(leftColumn);
    tableArea.appendChild(rightColumn);
}

// Click handler for the 'Visualize Stacked' button in the default.aspx page
function stack() {
    buildChart("Stacked");
}

// Click handler for the 'Visualize Tiled' button in the default.aspx page
function tile() {
    buildChart("Tiled");
}

function buildChart(visualSetting) {
    var chartArea = document.getElementById("populationVisualization");

    // Remove all nodes from the chart <DIV> so we have a clean space to write to
    while (chartArea.hasChildNodes()) {
        chartArea.removeChild(chartArea.lastChild);
    }
    
    var countryData = [];
    var listItemEnumerator = listItems.getEnumerator();
    while (listItemEnumerator.moveNext()) {
        var listItem = listItemEnumerator.get_current();
        var countryName = listItem.get_fieldValues()["Country"];
        var countryPopulation = listItem.get_fieldValues()["Population"];
        countryData.push({ "countryName": countryName, "population": countryPopulation });
    }
    // Take a copy of our array, simply so that we can sort it and then find the largest population.
    // The largest population is used later so that we can scale the areas of all the other smaller countrys
    // against the largest one
    var tempData = countryData.slice(0);
    tempData.sort(sort_by('population', true, parseInt));
    var largestPopulation = tempData[0].population;

    // If we are going to show the data stacked, then we will now sort the actual data so that the countries
    // with the largest populations are added to the chart first. This is so they can contain the successively smaller
    // countries. NOTE: The reason we don't want to sort for the Tiled UI is that this looks better with unordered tiles.
    if (visualSetting == "Stacked") {
        countryData.sort(sort_by('population', true, parseInt));
    }

    var countryCount = countryData.length;
    for (var country = 0; country < countryCount; country++) {
        var proportionalSize;
        if (visualSetting == "Stacked") {

            // We have already determined the largest population, so all countries can now be built such that
            // their area in pixels is relative to that of the largest country.
            // Of course, because we are building squares, the width and height need to be the square root of the relative
            // proportion, so that the final square has an area that relates properly to the largest country's square.
            // The factor of 160000 is simply so that squares are of a reasonable size
            proportionalSize = Math.sqrt((countryData[country].population / largestPopulation) * 160000);
        }
        else {

            // Same calculation as above, but the factor of 50000 is because we want smaller tiles than above,
            // so that we more can fit in the visible area of the chart
            proportionalSize = Math.sqrt((countryData[country].population / largestPopulation) * 50000);

        }

        // The following div is going to be a square that represents the country's population by its area size
        var countryDiv = document.createElement("div");
        if (visualSetting == "Stacked") {

            // Set styles to achieve two main things:
            // 1. Make the div into a square with sides equal to the proportionalSize variable that we determined above. 
            //    This will mean that the area of the div represents the population (i.e. width*height)
            // 2. Set the background color of the square to be a different shade of green than all the other countries.
            //    Effectively, we start with a base color of RGB(000,100,000) and then assign a green component that
            //    is on a point equally spaced between 100 and 255, depending on how many countries there are.
            //    By the way, using the .toString(16) method converts the decimal number into a hex value, which is 
            //    perfect for our needs.
            countryDiv.setAttribute("style", "text-align:right;color:#ffffff;padding:10px;margin:10px;float:left;width:"
                + proportionalSize + "px;height:"
                + proportionalSize + "px;background-color:#00"
                + (100 + (parseInt((155 / countryCount) * country))).toString(16) + "00");
        }
        else {

            // Same as above, execpt we want different padding/margins for when the squares are tiled
            countryDiv.setAttribute("style", "text-align:right;color:#ffffff;padding:5px;margin:2px;float:left;width:"
                + proportionalSize + "px;height:"
                + proportionalSize + "px;background-color:#00"
                + (100 + (parseInt((155 / countryCount) * country))).toString(16) + "00");
        }

        // Set the tooltip of the div so that it shows the population number when the mouse hovers over the square
        countryDiv.setAttribute("title", countryData[country].population);

        // Add a text node to display the country's name
        countryDiv.appendChild(document.createTextNode(countryData[country].countryName));
        chartArea.appendChild(countryDiv);

        // On the first iteration, the visualArea variable is set to the actual chart div in the content app page.
        // If we are going to tile, then that's OK --- we'll just keep adding new floating divs, and the browser will arrange them for us
        // depending on the size of the area.
        // However, if we are going to stack the divs inside each other, then we  need to set the visualArea variable
        // to the current div. That way, when the next iteration occurs, the previous div will become the place where the new div gets added.
        if (visualSetting == "Stacked") {
            chartArea = countryDiv;
        }
    }

}

// Helper function for sorting the data in our array in reverse order
var sort_by = function (field, reverse, primer) {
    var key = function (country) { return primer ? primer(country[field]) : country[field] };
    return function (a, b) {
        var A = key(a), B = key(b);
        return ((A < B) ? -1 : (A > B) ? +1 : 0) * [-1, 1][+!reverse];
    }
}