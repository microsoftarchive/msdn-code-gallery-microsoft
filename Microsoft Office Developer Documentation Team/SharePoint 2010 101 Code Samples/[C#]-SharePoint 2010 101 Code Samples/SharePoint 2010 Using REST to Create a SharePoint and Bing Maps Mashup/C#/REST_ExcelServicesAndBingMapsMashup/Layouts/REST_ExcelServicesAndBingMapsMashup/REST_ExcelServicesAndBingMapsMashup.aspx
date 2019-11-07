<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebControls.LayoutsPageBase" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <!-- To deploy this example you change the Site URL property for the project so that it deploys to your SharePoint site
    The default value is "http://intranet.contoso.com" -->

    <!-- To use Bing Maps we must link to the map control script -->
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>

    <script type="text/javascript">
        //This variable will hold the map object
        var bingMap = null;

        //Set up the map
        function getMap() {
            //Create and load the map object
            bingMap = new VEMap('bingMap');
            //Create a longidtude and latitude object for the map center
            var longLat = new VELatLong(38.5, -96.8);
            bingMap.LoadMap(longLat, 3);
            //Attach the ClickHandler function to handle the click event
            bingMap.AttachEvent("onclick", clickHandler);
        }

        //This function fires when the user clicks the map
        function clickHandler(eventArgs) {
            //Check that it's the onclick event
            if (eventArgs.eventName == "onclick") {
                //Check that it's the left mouse button
                if (eventArgs.leftMouseButton) {
                    //Create a LatLong object that stores where the user clicked
                    var x = eventArgs.mapX;
                    var y = eventArgs.mapY;
                    var clickedPixel = new VEPixel(x, y);
                    var clickedLatLong = bingMap.PixelToLatLong(clickedPixel);
                    //Use the FindLocations function to call getResults
                    bingMap.FindLocations(clickedLatLong, getResults);
                }
            }
        }

        //This function processes the name of the place clicked and displays the right statistics
        function getResults(locations) {
            //This variable will hold the location name as we try to extract the state
            var locationName = "";
            if (locations != null) {
                //Store the full location name
                locationName += locations[0].Name;
            }
            if (locationName.length > 0) {
                // Remove the street address
                if (locationName.indexOf(",") > 0)
                    locationName = locationName.substring(locationName.indexOf(",") + 1, locationName.length);
                // Remove the city
                if (locationName.indexOf(",") > 0)
                    locationName = locationName.substring(locationName.indexOf(",") + 1, locationName.length);
                // Remove leading space from state name or abreviation
                if (locationName.substring(0, 1) == " ")
                    locationName = locationName.substring(1, locationName.length);
                // Remove the country
                if (locationName.indexOf(",") > 0)
                    locationName = locationName.substring(0, locationName.indexOf(","));
                // Remove the zip code
                if (locationName.substring(locationName.indexOf(" ") + 1, locationName.length).indexOf(" ") > 0) {
                    // The state name has a space in it
                    t = locationName.substring(locationName.indexOf(" ") + 1, locationName.length);
                    locationName = locationName.substring(0, locationName.indexOf(" ") + t.indexOf(" ") + 2);
                } else {
                    // The state is abreviated or does not have a space in its name
                    locationName = locationName.substring(0, locationName.indexOf(" "));
                }
                // Call the ExcelRest service to display the right chart
                // You must edit the src attribute to point to your own SharePoint site and the location of the Excel spreadsheet
                document.getElementById('displayDiv').innerHTML = "<img src='http://intranet.contoso.com/_vti_bin/ExcelRest.aspx/Shared%20Documents/StateRankings.xlsx/Model/Charts(%27Report%27)?Ranges(%27State%27)=" + locationName + "'/>";
            }
        }

    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h2>
        <a href="javascript:getMap()">Click Here to Display the Map</a>
    </h2>
    <!-- This div displays the map -->
    <div id='bingMap' style="position:absolute; left:0px; top:0px; width:600px; height:400px;"></div>
    <!-- This div displays the results -->
    <div id="displayDiv" style="position:absolute; left:0px; top:410px; width:567px;height:408px"></div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    REST Demonstration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    An Excel and Bing Maps Mashup
</asp:Content>
