//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="softwareproduct.js" />

var SoftwareProducts = (function () {

    var softwareProducts = [];
    var numberOfSoftwareProductFields = 4;

    // Set the product information.
    function SoftwareProduct(title, version, licenseKey, email) {
        this.title = title;
        this.version = version;
        this.licenseKey = licenseKey;
        this.email = email;

        // Create an underscore-delimited ("_") string that contains the product information.
        this.toString = function () {
            var softwareProductToString = this.title + "_" + this.version + 
                                          "_" + this.licenseKey + "_" + this.email;
            return softwareProductToString;
        };

        return this;
    }

    return {

        // Retrieve the number of products.
        length: function () {
            return softwareProducts.length;
        },

        // Add a new product.
        add: function (title, version, licenseKey, email) {
            var softwareProduct = new SoftwareProduct(title, version, licenseKey, email);
            softwareProducts.push(softwareProduct);
        },

        // Delete a product.
        remove: function (arrayIndex) {
            delete softwareProducts[arrayIndex];
        },

        // Update information for a product.
        update: function (arrayIndex, title, version, licenseKey, email) {
            softwareProducts[arrayIndex].title = title;
            softwareProducts[arrayIndex].version = version;
            softwareProducts[arrayIndex].licenseKey = licenseKey;
            softwareProducts[arrayIndex].email = email;
        },

        // Add a new option to the drop down list.
        displayNewOption: function (arrayIndex) {
            var newOption = document.createElement("OPTION");
            document.getElementById("softwareProductSelectList").options.add(newOption);
            newOption.innerHTML = softwareProducts[arrayIndex].title;
            newOption.value = /*@static_cast(String)*/arrayIndex;
        },

        // Change the inner HTML to the title of the new option.
        displayUpdatedOption: function (optionIndex, arrayIndex) {
            document.getElementById("softwareProductSelectList").options[optionIndex].innerHTML = softwareProducts[arrayIndex].title;
        },

        // Retrieve the product by index.
        get: function (arrayIndex) {
            var returnSoftwareProduct = new SoftwareProduct(softwareProducts[arrayIndex].title, softwareProducts[arrayIndex].version,
                                                            softwareProducts[arrayIndex].licenseKey, softwareProducts[arrayIndex].email);

            return returnSoftwareProduct;
        },

        // Retrieve an array that contains the products.
        toString: function () {
            var softwareProductsToString = [];
            var i = 0;
            for (i = 0; i < softwareProducts.length; i++) {
                if (softwareProducts[i]) {
                    softwareProductsToString.push(softwareProducts[i].toString());
                }
            }
            return softwareProductsToString.join();
        },

        // Initialize product information.
        initialize: function (stringToInitialize) {

            // Return if the input string is empty.
            if (!stringToInitialize) {
                return false;
            }

            var i = 0;
            var stringOfSoftwareProducts = [];

            // The individual products in the string are separated by commas.
            stringOfSoftwareProducts = stringToInitialize.split(",");

            // Add each product in the input string to the SoftwareProducts object.
            // Each item of information in the string for each product is separated by an underscore ("_").
            // There can be up to four products in a SoftwareProducts object.
            for (i = 0; i < stringOfSoftwareProducts.length; i++) {
                var singleProduct = [];
                singleProduct = stringOfSoftwareProducts[i].split("_");
                if (singleProduct.length !== numberOfSoftwareProductFields) {
                    return false;
                }
                this.add(singleProduct[0], singleProduct[1], singleProduct[2], singleProduct[3]);
                this.displayNewOption(i);
            }

            // Retrieve the index of the selected product.
            document.getElementById("softwareProductSelectList").selectedIndex = (document.getElementById("softwareProductSelectList").options.length) - 1;

            // Display the details for the selected product.
            displaySoftwareProductDetails();
            return true;
        }
    };
}());

// Add a new SoftwareProduct object.
function addSoftwareProduct() {

    // Initialize an array index variable to the number of products.
    var arrayIndex = SoftwareProducts.length();

    // Set the default values for a new SoftwareProduct object.
    var title = "SoftwareProduct";
    var version = "";
    var licenseKey = "";
    var email = "";

    // Add the new product.
    SoftwareProducts.add(title, version, licenseKey, email);

    // Display the new product.
    SoftwareProducts.displayNewOption(arrayIndex);
    document.getElementById("softwareProductSelectList").selectedIndex = (document.getElementById("softwareProductSelectList").options.length) - 1;
    displaySoftwareProductDetails();

    // Retreive an updated product array as a string.
    var updateDataToString = SoftwareProducts.toString();
    sdkSample.updateData(updateDataToString, "softwareProduct");
}

// Delete an existing product.
function deleteSoftwareProduct() {
    
    // Retrieve the index of the product selected for deletion.
    var selectedIndex = document.getElementById("softwareProductSelectList").selectedIndex;

    // Return if the index does not exist.
    if (selectedIndex === -1) {
        return;
    }

    // Delete the product by index number.
    var softwareProductSelectList = document.getElementById("softwareProductSelectList");
    var arrayIndex = softwareProductSelectList.options[selectedIndex].value;
    SoftwareProducts.remove(arrayIndex);

    // Set the index of the deleted product to null and reset the selected index.
    softwareProductSelectList.options[selectedIndex] = null;
    var optionsLength = softwareProductSelectList.options.length;
    softwareProductSelectList.selectedIndex = selectedIndex % optionsLength;
    displaySoftwareProductDetails();

    // Retrieve the updated products array as a string. 
    var updateDataToString = SoftwareProducts.toString();
    sdkSample.updateData(updateDataToString, "softwareProduct");
}

// Display the details for a selected product.
function displaySoftwareProductDetails() {

    // Retrieve the selected index.
    var selectedIndex = document.getElementById("softwareProductSelectList").selectedIndex;

    // If the selected index does not exist, set all product details to
    // to empty strings.
    if (selectedIndex === -1) {
        document.getElementById("softwareProduct_Title").value = "";
        document.getElementById("softwareProduct_Version").value = "";
        document.getElementById("softwareProduct_LicenseKey").value = "";
        document.getElementById("softwareProduct_Email").value = "";
        return;
    }
        
    // If the selected index is found, retrieve all product details.
    var arrayIndex = document.getElementById("softwareProductSelectList").options[selectedIndex].value;
    var softwareProduct = SoftwareProducts.get(arrayIndex);
    document.getElementById("softwareProduct_Title").value = softwareProduct.title;
    document.getElementById("softwareProduct_Version").value = softwareProduct.version;
    document.getElementById("softwareProduct_LicenseKey").value = softwareProduct.licenseKey;
    document.getElementById("softwareProduct_Email").value = softwareProduct.email;
}

function updateSoftwareProduct() {
 
    // Clear the application's last error.
    sdkSample.clearLastError();

    // Retrieve the selected index.
    var selectedIndex = document.getElementById("softwareProductSelectList").selectedIndex;

    // Return if the selected index does not exist.
    if (selectedIndex === -1) {
        return;
    }

    var arrayIndex = document.getElementById("softwareProductSelectList").options[selectedIndex].value;
    var title = document.getElementById("softwareProduct_Title").value;
    var version = document.getElementById("softwareProduct_Version").value;
    var licenseKey = document.getElementById("softwareProduct_LicenseKey").value;
    var email = document.getElementById("softwareProduct_Email").value;

    // The product title cannot be empty.
    if (!title) {
        sdkSample.displayError("Title cannot be empty");
        return;
    }

    // The length of any field cannot exceed the maximum allowed.
    if (title.length > sdkSample.maxLength || version.length > sdkSample.maxLength ||
        licenseKey.length > sdkSample.maxLength || email.length > sdkSample.maxLength) {
            sdkSample.displayError("Update failed. Maximum limit: " + /*@static_cast(String)*/sdkSample.maxLength);
            return;
    }

    // The field values can optionally be validated against a format.
    SoftwareProducts.update(arrayIndex, title, version, licenseKey, email);
    SoftwareProducts.displayUpdatedOption(selectedIndex, arrayIndex);
       
    // Retrieve the updated product array as a string. 
    var updateDataToString = SoftwareProducts.toString();
    sdkSample.updateData(updateDataToString, "softwareProduct");
 }
