//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="bankaccount.js" />

var BankAccounts = (function () {

    var bankAccounts = [];
    var numberOfBankAccountFields = 7;

    // Set account information for the user.
    function BankAccount(title, bankName, accountHolderName, type, routingNumber, accountNumber, pin) {
        this.title = title;
        this.bankName = bankName;
        this.accountHolderName = accountHolderName;
        this.type = type;
        this.routingNumber = routingNumber;
        this.accountNumber = accountNumber;
        this.pin = pin;

        // Create an underscore-delimited ("_") string that contains the account information.
        this.toString = function () {
            var bankAccountToString = this.title + "_" + this.bankName + "_" +
                                      this.accountHolderName + "_" + this.type +
                                      "_" + this.routingNumber + "_" + 
                                      this.accountNumber + "_" + this.pin;

            return bankAccountToString;
        };

        return this;
    }
    return {

        // Retrieve the number of bank accounts.
        length: function () {
            return bankAccounts.length;
        },

        // Add an account.
        add: function (title, bankName, accountHolderName, type, routingNumber, accountNumber, pin) {
            var bankAccount = new BankAccount(title, bankName, accountHolderName, type, routingNumber, accountNumber, pin);
            bankAccounts.push(bankAccount);
        },

        // Delete an account.
        remove: function (arrayIndex) {
            delete bankAccounts[arrayIndex];
        },

        // Update an account.
        update: function (arrayIndex, title, bankName, accountHolderName, type, routingNumber, accountNumber, pin) {
            bankAccounts[arrayIndex].title = title;
            bankAccounts[arrayIndex].bankName = bankName;
            bankAccounts[arrayIndex].accountHolderName = accountHolderName;
            bankAccounts[arrayIndex].type = type;
            bankAccounts[arrayIndex].routingNumber = routingNumber;
            bankAccounts[arrayIndex].accountNumber = accountNumber;
            bankAccounts[arrayIndex].pin = pin;
        },

        // Add a new option to the drop down list,
        displayNewOption: function (arrayIndex) {
            var newOption = document.createElement("OPTION");
            document.getElementById("bankAccountSelectList").options.add(newOption);
            newOption.innerHTML = bankAccounts[arrayIndex].title;
            newOption.value = arrayIndex;
        },

        // Change the inner HTML to the title of the new option.
        displayUpdatedOption: function (optionIndex, arrayIndex) {
            document.getElementById("bankAccountSelectList").options[optionIndex].innerHTML = bankAccounts[arrayIndex].title;
        },

        // Retrieve the bank account by index.
        get: function (arrayIndex) {
            var returnBankAccount = new BankAccount(bankAccounts[arrayIndex].title, bankAccounts[arrayIndex].bankName,
                                                    bankAccounts[arrayIndex].accountHolderName, bankAccounts[arrayIndex].type,
                                                    bankAccounts[arrayIndex].routingNumber, bankAccounts[arrayIndex].accountNumber,
                                                    bankAccounts[arrayIndex].pin);

            return returnBankAccount;
        },

        // Retrieve an array that contains the accounts.
        toString: function () {
            var bankAccountsToString = [];
            var i = 0;
            for (i = 0; i < bankAccounts.length; i++) {
                if (bankAccounts[i]) {
                    bankAccountsToString.push(bankAccounts[i].toString());
                }
            }
            return bankAccountsToString.join();
        },

        // Initialize account information.
        initialize: function (stringToInitialize) {

            // Return if the input string is empty.
            if (!stringToInitialize) {
                return false;
            }

            var i = 0;
            var stringOfBankAccounts = [];

            // The individual bank accounts in the string are separated by commas.
            stringOfBankAccounts = stringToInitialize.split(",");

            // Add each account in the input string to the BankAccounts object.
            // Each item of information in the string for each account is separated by an underscore ("_").
            // There can be up to seven accounts in a BankAccounts object.
            for (i = 0; i < stringOfBankAccounts.length; i++) {
                var singleAccount = [];
                singleAccount = stringOfBankAccounts[i].split("_");
                if (singleAccount.length !== numberOfBankAccountFields) {
                    return false;
                }
                this.add(singleAccount[0], singleAccount[1], singleAccount[2],
                         singleAccount[3], singleAccount[4], singleAccount[5],
                         singleAccount[6]);

                this.displayNewOption(i);
            }

            // Retrieve the index of the selected account.
            document.getElementById("bankAccountSelectList").selectedIndex = (document.getElementById("bankAccountSelectList").options.length) - 1;

            // Display the details for the selected account.
            displayBankAccountDetails();
            return true;
        }
    };
}());

// Add a new BankAccount object.
function addBankAccount() {
   
    // Initialize an array index variable to the number of bank accounts.
    var arrayIndex = BankAccounts.length();

    // Set the default values for a new BankAccount object.
    var title = "BankAccount";
    var bankName = "";
    var accountHolderName = "";
    var type = "";
    var routingNumber = "";
    var accountNumber = "";
    var pin = "";

    // Add the new account.
    BankAccounts.add(title, bankName, accountHolderName, type, routingNumber, accountNumber, pin);

    // Display the new account.
    BankAccounts.displayNewOption(arrayIndex);
    document.getElementById("bankAccountSelectList").selectedIndex = (document.getElementById("bankAccountSelectList").options.length) - 1;
    displayBankAccountDetails();

    // Retrieve the updated accounts array as a string. 
    var updateDataToString = BankAccounts.toString();
    sdkSample.updateData(updateDataToString, "bankAccount");
}

// Delete an existing BankAccount object.
function deleteBankAccount() {

    // Retrieve the index of the account selected for deletion.
    var selectedIndex = document.getElementById("bankAccountSelectList").selectedIndex;

    // Return if the index does not exist.
    if (selectedIndex === -1) {
        return;
    }
        
    // Delete the account by index number.
    var bankAccountSelectList = document.getElementById("bankAccountSelectList");
    var arrayIndex = bankAccountSelectList.options[selectedIndex].value;
    BankAccounts.remove(arrayIndex);

    // Set the index of the deleted account to null and reset the selected index.
    bankAccountSelectList.options[selectedIndex] = null;
    var optionsLength = bankAccountSelectList.options.length;
    bankAccountSelectList.selectedIndex = selectedIndex % optionsLength;
    displayBankAccountDetails();

    // Retrieve the updated accounts array as a string.
    var updateDataToString = BankAccounts.toString();
    sdkSample.updateData(updateDataToString, "bankAccount");
}

// Display the details for a selected account.
function displayBankAccountDetails() {
    
    // Retrieve the selected index.
    var selectedIndex = document.getElementById("bankAccountSelectList").selectedIndex;

    // If the selected index does not exist, set all account details to
    // to empty strings.
    if (selectedIndex === -1) {
        document.getElementById("bankAccount_Title").value = "";
        document.getElementById("bankAccount_BankName").value = "";
        document.getElementById("bankAccount_AccountHolderName").value = "";
        document.getElementById("bankAccount_Type").value = "";
        document.getElementById("bankAccount_RoutingNumber").value = "";
        document.getElementById("bankAccount_AccountNumber").value = "";
        document.getElementById("bankAccount_Pin").value = "";
        return;
    }
        
    // If the selected index is found, retrieve all account details.
    var arrayIndex = document.getElementById("bankAccountSelectList").options[selectedIndex].value;
    var bankAccount = BankAccounts.get(arrayIndex);
    document.getElementById("bankAccount_Title").value = bankAccount.title;
    document.getElementById("bankAccount_BankName").value = bankAccount.bankName;
    document.getElementById("bankAccount_AccountHolderName").value = bankAccount.accountHolderName;
    document.getElementById("bankAccount_Type").value = bankAccount.type;
    document.getElementById("bankAccount_RoutingNumber").value = bankAccount.routingNumber;
    document.getElementById("bankAccount_AccountNumber").value = bankAccount.accountNumber;
    document.getElementById("bankAccount_Pin").value = bankAccount.pin;
}

// Update account information for an individual,
function updateBankAccount() {
    // Clear the application's last error.
    sdkSample.clearLastError();

    // Retrieve the selected index. 
    var selectedIndex = document.getElementById("bankAccountSelectList").selectedIndex;

    // Return if the selected index does not exist.
    if (selectedIndex === -1) {
        return;
    }

    // Update account information.
    var arrayIndex = document.getElementById("bankAccountSelectList").options[selectedIndex].value;
    var title = document.getElementById("bankAccount_Title").value;
    var bankName = document.getElementById("bankAccount_BankName").value;
    var accountHolderName = document.getElementById("bankAccount_AccountHolderName").value;
    var type = document.getElementById("bankAccount_Type").value;
    var routingNumber = document.getElementById("bankAccount_RoutingNumber").value;
    var accountNumber = document.getElementById("bankAccount_AccountNumber").value;
    var pin = document.getElementById("bankAccount_Pin").value;

    // The account title cannot be empty.
    if (!title) {
        sdkSample.displayError("Title cannot be empty");
        return;
    }

        // The length of any field cannot exceed the maximum allowed.
        if (title.length > sdkSample.maxLength || bankName.length > sdkSample.maxLength ||
            accountHolderName.length > sdkSample.maxLength || type.length > sdkSample.maxLength ||
            routingNumber.length > sdkSample.maxLength || accountNumber.length > sdkSample.maxLength ||
            pin.length > sdkSample.maxLength) {
            sdkSample.displayError("Update failed. Maximum limit: " + /*@static_cast(String)*/sdkSample.maxLength);
            return;
    }

    // The field values can optionally be validated against a format.
    BankAccounts.update(arrayIndex, title, bankName, accountHolderName, type, routingNumber, accountNumber, pin);
    BankAccounts.displayUpdatedOption(selectedIndex, arrayIndex);
       
    // Retrieve the updated account array as a string. 
    var updateDataToString = BankAccounts.toString();
    sdkSample.updateData(updateDataToString, "bankAccount");
}
