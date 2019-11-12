var xmlHttp = null; // XMLHttpRequest object
var ddlCountry = null; // DropDownList for country
var ddlRegion = null; // DropDownList for region
var ddlCity = null;  // DropDownList for city
var hdfRegion = null; // Use to save region DropDownList options and restore it when page is rendering
var hdfCity = null;  // Use to save city DropDownList options and restore it when page is loadrenderinged
var hdfRegionSelectValue = null; // Use to save region DropDownList selected options and restore it when page is rendering
var hdfCitySelectValue = null; // Use to save city DropDownList selected options and restore it when page is rendering
var hdfCountrySelectValue = null;   // Use to save country DropDownList selected options and restore it when page is rendering

// Instance XMLHttpRequest
window.onload = function loadXMLHttp() {
    if (window.XMLHttpRequest) {
        xmlHttp = new XMLHttpRequest();
    } else if (window.ActiveXObject) {
        try {
            xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        catch (e) { }
    }

    ddlCountry = document.getElementById('ddlCountry');
    ddlRegion = document.getElementById('ddlRegion');
    ddlCity = document.getElementById('ddlCity');
    hdfRegion = document.getElementById('hdfRegion');
    hdfCity = document.getElementById('hdfCity');
    hdfRegionSelectValue = document.getElementById('hdfRegionSelectValue');
    hdfCitySelectValue = document.getElementById('hdfCitySelectValue');
    hdfCountrySelectValue = document.getElementById('hdfCountrySelectValue');
    ShowFirstOption(ddlCountry); // Add "Please Select a xxxx" option in the top of country DropDownList 
    RestoreDropdownlist(); // Restore dropdownlist when page is rendering

}

// Restore dropdownlist when page is rendering
function RestoreDropdownlist() {
    // Restore region dropdownlist
    if (hdfRegion.value != "") {
        addOption(ddlRegion, hdfRegion.value);
        ddlRegion.selectedIndex = hdfRegionSelectValue.value;
    } // Restore city dropdownlist
    if (hdfCity.value != "") {
        addOption(ddlCity, hdfCity.value);
        ddlCity.selectedIndex = hdfCitySelectValue.value;
    }
    ddlCountry.selectedIndex = hdfCountrySelectValue.value;
}

// Remove region and city dropdownlist options when country dropdownlist selected option is changed.
function ddlCountryOnChange() {
    RemoveOptionsFromDDL(ddlCity);
    RemoveOptionsFromDDL(ddlRegion);
    hdfCountrySelectValue.value = ddlCountry.selectedIndex;
    var url = 'GetDataForCallBack.aspx?country=' + ddlCountry.options[ddlCountry.selectedIndex].value;
    sendRequest(url, 'ddlRegion');

}

// Country dropdownlist selected option is changed
function ddlRegionOnChange() {
    var url = 'GetDataForCallBack.aspx?Region=' + ddlRegion.options[ddlRegion.selectedIndex].value;
    hdfRegionSelectValue.value = ddlRegion.selectedIndex;
    sendRequest(url, 'ddlCity');
}

// Send callback to aspx page
function sendRequest(url, ddl) {
    if (xmlHttp) {
        xmlHttp.open("GET", url, true);
        if (ddl == 'ddlRegion') {
            xmlHttp.onreadystatechange = BindDdlRegion;
        } else if (ddl == 'ddlCity') {
            xmlHttp.onreadystatechange = BindDdlCity;
        }
        EnableSubmitButton(true); // Disable submit button
        xmlHttp.send(null);
    }

}

// Save city dropdownlist selected index
function ddlCityOnChange() {
    hdfCitySelectValue.value = ddlCity.selectedIndex;
}

// Bind Region dropdownlist when response is sent back from server
function BindDdlRegion() {
    if (xmlHttp.readyState == 4) {
        if (xmlHttp.status == 200) {
            var result = xmlHttp.responseText;
            addOption(ddlRegion, result);   // Add options to region dropdownlist based on response 
            EnableSubmitButton(false); // Enable submit button
            hdfRegion.value = result; // Save region value in hide field
        } else {
            document.getElementById('lblResult').innerHTML = xmlHttp.status;
        }
    }
}

// Bind City dropdownlist when response is sent back from server
function BindDdlCity() {
    if (xmlHttp.readyState == 4) {
        if (xmlHttp.status == 200) {
            var result = xmlHttp.responseText;
            addOption(ddlCity, result);   // Add options to city dropdownlist based on response 
            EnableSubmitButton(false);  // Enable submit button
            hdfCity.value = result;  // Save city value in hide field
        } else {
            document.getElementById('lblResult').innerHTML = xmlHttp.status;
        }
    }
}

// Add options to dropdownlist
function addOption(selectbox, strResult) {
    RemoveOptionsFromDDL(selectbox);
    if (strResult != "" && selectbox != null) {

        var array = strResult.split(',');
        for (i = 0; i < array.length; i++) {
            var optn = document.createElement("OPTION");
            optn.text = array[i];
            optn.value = array[i];
            selectbox.options.add(optn);
        }

    }
    ShowFirstOption(selectbox); // Add "Please Select a xxxx" option in the top of country DropDownList 
}

// Save selected options in hide field so that we can access it from codebehind class
function SaveSelectedData() {

    hdfCitySelectValue.value = ddlCity.selectedIndex;
    hdfCountrySelectValue.value = ddlCountry.selectedIndex;
    hdfRegionSelectValue.value = ddlRegion.selectedIndex;

    if (ddlCity != null && ddlCountry != null && ddlRegion != null
     && ddlCity.length > 0 && ddlCountry.length > 0 && ddlRegion.length > 0
     && ddlCity.selectedIndex != '0' && ddlCountry.selectedIndex != '0' && ddlRegion.selectedIndex != '0') {

        var strResult = ddlCountry.options[ddlCountry.selectedIndex].value + '; '
        + ddlRegion.options[ddlRegion.selectedIndex].value + '; '
        + ddlCity.options[ddlCity.selectedIndex].value;

        document.getElementById('hdfResult').value = strResult;


    }
    else {
        document.getElementById('hdfResult').value = 'Please select option from DropDownList.';
    }
}

// Remove dropdownlist all options
function RemoveOptionsFromDDL(selectbox) {
    var iCount = selectbox.length;
    while (iCount > 0) {
        selectbox.remove(0);
        iCount = selectbox.length;
    }
}

// Add "Please Select a xxxx" option in the top of country DropDownList 
function ShowFirstOption(selectbox) {
    var optnFirst = document.createElement("OPTION");
    optnFirst.text = "Please Select a ";
    optnFirst.value = "-1";
    selectbox.options.add(optnFirst, 0);
    selectbox.selectedIndex = 0;
}

// Enable or diasble submit button
function EnableSubmitButton(TrueOrFalse) {
    var obj = document.getElementById('btnSubmit');
    obj.disabled = TrueOrFalse;
}


