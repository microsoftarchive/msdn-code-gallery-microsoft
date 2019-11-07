// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=392286
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll().then(function completed() {
                // Retrieve select
                var countryselect = document.getElementById("countryselect");
                var provinceselect = document.getElementById("provinceselect");
                var cityselect = document.getElementById("cityselect");

                // Register the event handler
                countryselect.addEventListener("change", countryChanged, false);
                provinceselect.addEventListener("change", provinceChanged, false);
                cityselect.addEventListener("change", cityChanged, false);
            }));
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    var objCountrySelect;
    var objProvinceSelect;
    var objCitySelect;

    function countryChanged(eventInfo) {

        objCountrySelect = document.getElementById("countryselect");
        objProvinceSelect = document.getElementById("provinceselect");

        var selectedCountry = objCountrySelect.options[objCountrySelect.selectedIndex].value;
        var selectedText = objCountrySelect.options[objCountrySelect.selectedIndex].text;


        // show selected Country
        document.getElementById("country").innerText = selectedText;
        document.getElementById("selectedContry").innerText = selectedText;

        if (selectedCountry == 1) {
            objProvinceSelect.innerHTML = "<select><option>Province1</option><option>Province2</option></select>";
            document.getElementById("province").innerText = objProvinceSelect.options[0].text;
            document.getElementById("selectedProvince").innerText = objProvinceSelect.options[0].text;

            // Select first item
            objProvinceSelect.options[0].selected = "selected";
            document.getElementById("city").innerText = "Province1 City1";
            fillCitySelect(objProvinceSelect.options[0].text);

        }
        else {
            objProvinceSelect.innerHTML = "<select><option>Province3</option><option>Province4</option></select>";
            document.getElementById("province").innerText = objProvinceSelect.options[0].text;
            document.getElementById("selectedProvince").innerText = objProvinceSelect.options[0].text;

            // Select first item
            objProvinceSelect.options[0].selected = "selected";
            document.getElementById("city").innerText = "Province3 City1";
            fillCitySelect(objProvinceSelect.options[0].text);
        }
    }

    function provinceChanged(eventInfo) {
        objProvinceSelect = document.getElementById("provinceselect");
        var selectedProviceText = objProvinceSelect.options[objProvinceSelect.selectedIndex].text;

        // show selected Country
        document.getElementById("province").innerText = selectedProviceText
        document.getElementById("selectedProvince").innerText = selectedProviceText;


        // Fill City Select
        fillCitySelect(selectedProviceText);
        document.getElementById("city").innerText = selectedProviceText + " City1";
    }

    function cityChanged(eventInfo) {
        objCitySelect = document.getElementById("cityselect");
        var selectedCityText = objCitySelect.options[objCitySelect.selectedIndex].text;

        // show selected Country
        document.getElementById("city").innerText = selectedCityText;
    }

    // Fill City Select
    function fillCitySelect(province) {
        objCitySelect = document.getElementById("cityselect");
        objCitySelect.innerHTML = "<select><option>" + province + " City1" + "</option><option>" + province + " City2" + "</option><option>" + province + " City3" + "</option></select>";

        objCitySelect.options[0].selected = "selected";
    }

    app.start();
})();