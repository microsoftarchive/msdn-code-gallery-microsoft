//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="creditcard.js" />


var CreditCards = (function () {

    var creditCards = [];
    var numberOfCreditCardFields = 7;

    // Set credit card information for the user.
    function CreditCard(title, cardHolderName, type, number, verificatioNumber, expiryDate, pin) {

        this.title = title;
        this.cardHolderName = cardHolderName;
        this.type = type;
        this.number = number;
        this.verificationNumber = verificatioNumber;
        this.expiryDate = expiryDate;
        this.pin = pin;

        // Create an underscore-delimited ("_") string that contains the account information.
        this.toString = function () {
            var creditCardToString = this.title + "_" + this.cardHolderName + "_" + this.type +
                                     "_" + this.number + "_" + this.verificationNumber + "_" +
                                     this.expiryDate + "_" + this.pin;

            return creditCardToString;
        };

        return this;
    }

    return {

        // Retrieve the number of credit card accounts.
        length: function () {
            return creditCards.length;
        },

        // Add an account.
        add: function (title, cardHolderName, type, number, verificationNumber, expiryDate, pin) {
            var creditCard = new CreditCard(title, cardHolderName, type, number, verificationNumber, expiryDate, pin);
            creditCards.push(creditCard);
        },

        // Delete an account.
        remove: function (arrayIndex) {
            delete creditCards[arrayIndex];
        },

        // Update an account.
        update: function (arrayIndex, title, cardHolderName, type, number, verificationNumber, expiryDate, pin) {
            creditCards[arrayIndex].title = title;
            creditCards[arrayIndex].cardHolderName = cardHolderName;
            creditCards[arrayIndex].type = type;
            creditCards[arrayIndex].number = number;
            creditCards[arrayIndex].verificationNumber = verificationNumber;
            creditCards[arrayIndex].expiryDate = expiryDate;
            creditCards[arrayIndex].pin = pin;
        },

        // Add a new option to the drop down list.
        displayNewOption: function (arrayIndex) {
            var newOption = document.createElement("OPTION");
            document.getElementById("creditCardSelectList").options.add(newOption);
            newOption.innerHTML = creditCards[arrayIndex].title;
            newOption.value = arrayIndex;
        },

        // Change the inner HTML to the title of the new option.
        displayUpdatedOption: function (optionIndex, arrayIndex) {
            document.getElementById("creditCardSelectList").options[optionIndex].innerHTML = creditCards[arrayIndex].title;
        },

        // Retrieve the account by index.
        get: function (arrayIndex) {
            var returnCreditCard = new CreditCard(creditCards[arrayIndex].title, creditCards[arrayIndex].cardHolderName,
                                                  creditCards[arrayIndex].type, creditCards[arrayIndex].number,
                                                  creditCards[arrayIndex].verificationNumber, creditCards[arrayIndex].expiryDate,
                                                  creditCards[arrayIndex].pin);

            return returnCreditCard;
        },

        // Retrieve an array that contains the accounts.
        toString: function () {
            var creditCardsToString = [], i = 0;
            for (i = 0; i < creditCards.length; i++) {
                if (creditCards[i]) {
                    creditCardsToString.push(creditCards[i].toString());
                }
            }
            return creditCardsToString.join();
        },

        // Initialize account information.
        initialize: function (stringToInitialize) {

            // Return if the input string is empty.
            if (!stringToInitialize) {
                return false;
            }

            var i = 0;
            var stringOfCreditCards = [];

            // The individual accounts in the string are separated by commas.
            stringOfCreditCards = stringToInitialize.split(",");

            // Add each account in the input string to the CreditCards object.
            // Each item of information in the string for each account is separated by an underscore ("_").
            // There can be up to seven accounts in a CreditCards object.
            for (i = 0; i < stringOfCreditCards.length; i++) {
                var singleCard = [];
                singleCard = stringOfCreditCards[i].split("_");
                if (singleCard.length !== numberOfCreditCardFields) {
                    return false;
                }
                this.add(singleCard[0], singleCard[1], singleCard[2], singleCard[3], singleCard[4], singleCard[5], singleCard[6]);
                this.displayNewOption(/*@static_cast(String)*/i);
            }

            // Retrieve the index of the selected account.
            document.getElementById("creditCardSelectList").selectedIndex = (document.getElementById("creditCardSelectList").options.length) - 1;

            // Display the details for the selected account.
            displayCreditCardDetails();
            return true;
        }

    };
}());

// Add a new CreditCard object.
function addCreditCard() {
  
    // Initialize an array index variable to the number of accounts.
    var arrayIndex = CreditCards.length();
    
    // Set the default values for a new CreditCard object.
    var title = "CreditCard";
    var cardHolderName = "";
    var type = "";
    var number = "";
    var verificationNumber = "";
    var expiryDate = "";
    var pin = "";

    // Add the new Account.
    CreditCards.add(title, cardHolderName, type, number, verificationNumber, expiryDate, pin);

    // Display the new account.
    CreditCards.displayNewOption(/*@static_cast(String)*/arrayIndex);
    document.getElementById("creditCardSelectList").selectedIndex = (document.getElementById("creditCardSelectList").options.length) - 1;
    displayCreditCardDetails();

    // Retreive an updated account array as a string. 
    var updateDataToString = CreditCards.toString();
    sdkSample.updateData(updateDataToString, "creditCard");
}

// Delete an existing credit card account.
function deleteCreditCard() {

    // Retrieve the index of the account selected for deletion.
    var selectedIndex = document.getElementById("creditCardSelectList").selectedIndex;

    // Return if the index does not exist.
    if (selectedIndex === -1) {
        return;
    }

    // Delete the account by index number.
    var creditCardSelectList = document.getElementById("creditCardSelectList");
    var arrayIndex = creditCardSelectList.options[selectedIndex].value;
    CreditCards.remove(arrayIndex);

    // Set the index of the deleted account to null and reset the selected index.
    creditCardSelectList.options[selectedIndex] = null;
    var optionsLength = creditCardSelectList.options.length;
    creditCardSelectList.selectedIndex = selectedIndex % optionsLength;
    displayCreditCardDetails();

    // Retrieve the updated accounts array as a string. 
    var updateDataToString = CreditCards.toString();
    sdkSample.updateData(updateDataToString, "creditCard");
}

// Display the details for a selected account.
function displayCreditCardDetails() {
    
    // Retrieve the selected index.
    var selectedIndex = document.getElementById("creditCardSelectList").selectedIndex;

    // If the selected index does not exist, set all account details to
    // to empty strings.
    if (selectedIndex === -1) {
        document.getElementById("creditCard_Title").value = "";
        document.getElementById("creditCard_CardholderName").value = "";
        document.getElementById("creditCard_Type").value = "";
        document.getElementById("creditCard_Number").value = "";
        document.getElementById("creditCard_VerificationNumber").value = "";
        document.getElementById("creditCard_ExpiryDate").value = "";
        document.getElementById("creditCard_Pin").value = "";
        return;
    }
        
    // If the selected index is found, retrieve all account details.
    var arrayIndex = document.getElementById("creditCardSelectList").options[selectedIndex].value;
    var creditCard = CreditCards.get(arrayIndex);
    document.getElementById("creditCard_Title").value = creditCard.title;
    document.getElementById("creditCard_CardholderName").value = creditCard.cardHolderName;
    document.getElementById("creditCard_Type").value = creditCard.type;
    document.getElementById("creditCard_Number").value = creditCard.number;
    document.getElementById("creditCard_VerificationNumber").value = creditCard.verificationNumber;
    document.getElementById("creditCard_ExpiryDate").value = creditCard.expiryDate;
    document.getElementById("creditCard_Pin").value = creditCard.pin;

}

function updateCreditCard() {

    // Clear the application's last error.
    sdkSample.clearLastError();

    // Retrieve the selected index.
    var selectedIndex = document.getElementById("creditCardSelectList").selectedIndex;

    // Return if the selected index does not exist.
    if (selectedIndex === -1) {
        return;
    }

    // Update account information.
    var arrayIndex = document.getElementById("creditCardSelectList").options[selectedIndex].value;
    var title = document.getElementById("creditCard_Title").value;
    var cardHolderName = document.getElementById("creditCard_CardholderName").value;
    var type = document.getElementById("creditCard_Type").value;
    var number = document.getElementById("creditCard_Number").value;
    var verificationNumber = document.getElementById("creditCard_VerificationNumber").value;
    var expiryDate = document.getElementById("creditCard_ExpiryDate").value;
    var pin = document.getElementById("creditCard_Pin").value;
        
    // The account title cannot be empty.
    if (!title) {
        sdkSample.displayError("Title cannot be empty");
        return;
    }

    // The length of any field cannot exceed the maximum allowed.
    if (title.length > sdkSample.maxLength || cardHolderName.length > sdkSample.maxLength ||
        type.length > sdkSample.maxLength || number.length > sdkSample.maxLength ||
        verificationNumber.length > sdkSample.maxLength || expiryDate.length > sdkSample.maxLength ||
        pin.length > sdkSample.maxLength) {
	    sdkSample.displayError("Update failed. Maximum limit: " + /*@static_cast(String)*/sdkSample.maxLength);
            return;
    }

    // The field values can optionally be validated against a format.
    CreditCards.update(arrayIndex, title, cardHolderName, type, number, verificationNumber, expiryDate, pin);
    CreditCards.displayUpdatedOption(selectedIndex, arrayIndex);
       
    // Retrieve the updated account array as a string.
    var updateDataToString = CreditCards.toString();
    sdkSample.updateData(updateDataToString, "creditCard");
}
