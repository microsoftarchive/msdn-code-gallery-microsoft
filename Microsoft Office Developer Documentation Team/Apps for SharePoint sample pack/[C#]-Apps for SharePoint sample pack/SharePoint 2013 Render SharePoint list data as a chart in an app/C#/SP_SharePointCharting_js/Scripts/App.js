// Variables used in various functions and callbacks
var context;
var list;
var listItems;

// This code runs when the DOM is ready and creates a context object 
// which is needed to use the SharePoint object model
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
            startChart();
        },

        // Failure callback
        function (sender, args) {
            var chartArea = document.getElementById("populationChart");
            // Remove all nodes from the chart <DIV> so we have a clean space to write to
            while (chartArea.hasChildNodes()) {
                chartArea.removeChild(chartArea.lastChild);
            }
            chartArea.appendChild(document.createTextNode("Failed to get population data. Error: "
                + args.get_message()));
        });
});

// This function is called in the success callback handler (above)
// It simply iterates through the list and displays the population data in a custom
// bar chart
function startChart() {
    var chartArea = document.getElementById("populationChart");
    // Remove all nodes from the chart <DIV> so we have a clean space to write to
    while (chartArea.hasChildNodes()) {
        chartArea.removeChild(chartArea.lastChild);
    }

    // Create an axis where we will put country names
    var yAxis = document.createElement("div");
    yAxis.setAttribute("style", "float:left;height:600px;width:200px;background-color:#FFFFFF");
   
    // Create an area where we will draw bars to represent populations
    var plotArea = document.createElement("div");
    plotArea.setAttribute("style", "float:left;height:600px;width:500px;background-color:#AFAFAF");


    // Iterate through the SharePoint list data
    var listItemEnumerator = listItems.getEnumerator();
    while (listItemEnumerator.moveNext()) {
        var listItem = listItemEnumerator.get_current();
        var countryName = listItem.get_fieldValues()["Country"];
        var countryPopulation = listItem.get_fieldValues()["Population"];

        // Create a label from the current country's name and add it to the y-axis
        var barLabel = document.createElement("div");
        barLabel.setAttribute("style", "float:none;margin:5px;height:50px;width:200px;background-color:#FAFAFA");
        barLabel.appendChild(document.createTextNode(countryName));
        yAxis.appendChild(barLabel);

        // Create a bar with a width that represents the size of the current country's population
        // and add it to the plot area
        var bar = document.createElement("div");
        var barWidth = 500/(2000000000/countryPopulation);
        bar.setAttribute("style", "float:none;margin:5px;height:50px;width:" + barWidth + "px;background-color:#FAFAFA");
        bar.setAttribute("title", countryPopulation);
        plotArea.appendChild(bar);
    }

    // Add the y-axis (country labels) and the plot area (full of bars representing populations)
    // to the chart.
    chartArea.appendChild(yAxis);
    chartArea.appendChild(plotArea);

    // We have an image that looks like an axis with values ranging from zero to two billion
    // So for simplicity, we deploy that image with the app and then add it to the chart under the plot
    // area. In a real world solution, you would probably create the x-axis dynamically, to take into account 
    // the possibilities of populations growing past two billion.
    var xAxis = document.createElement("img");
    xAxis.setAttribute("src", "../images/ChartAxis.png");
    xAxis.setAttribute("style", "float:right;");
    var labelSpacer = document.createElement("div");
    labelSpacer.setAttribute("style", "float:left;height:50px;width:200px;background-color:#FFFFFF");
    labelSpacer.appendChild(document.createTextNode("Population:"));
    chartArea.appendChild(labelSpacer);
    chartArea.appendChild(xAxis);
}

