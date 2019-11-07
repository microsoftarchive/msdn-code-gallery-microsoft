//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="program.js" />

(function () {

    var passwordSubmitButton = null,
        passwordInput = null,
        password = null;

    // Initialize the encryption and decryption parameters.
    var cipherAlgNameString = Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.aesCbcPkcs7,
        ivString = "0123456789abcdef",
        algBlockSizeInBytes = 16;

    // Initialize the key derivation parameters.
    var kdfSaltString = "Secret Saver",
        kdfAlgNameString = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmNames.pbkdf2Sha256,
        kdfIterationCount = 10000;

    // Initialize variables to indicate application state after load.
    var loadStatus = true,
        filesRead = 0,
        maxFiles = 3; // There are 3 schemas and hence 3 file to be read

    // Initialize the maximum length of a field in the user interface.
    var maxLength = 32;
    sdkSample.maxLength = maxLength;

    function id(elementId) {
        return document.getElementById(elementId);
    }


    // Derive a cryptographic key from a password.
    function deriveKeyFromPassword() {
        var derivedKeyBuffer;
        
        // Convert the password to binary.
        var secret = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(password, Windows.Security.Cryptography.BinaryStringEncoding.utf8);

        // Initialize the password-based key derivation function (PBKDF2) parameters.
        var salt = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(kdfSaltString, Windows.Security.Cryptography.BinaryStringEncoding.utf8);
        var pbkdf2Params = Windows.Security.Cryptography.Core.KeyDerivationParameters.buildForPbkdf2(salt, kdfIterationCount);

        // Open the PBKDF2_SHA256 algorithm provider.
        var algorithmProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.openAlgorithm(kdfAlgNameString);

        // Create a secret key.
        var secretKey = algorithmProvider.createKey(secret);

        // Peform the derivation.
        derivedKeyBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.deriveKeyMaterial(secretKey, pbkdf2Params, algBlockSizeInBytes);
    
        return derivedKeyBuffer;
    }

    // Encrypt a data buffer.
    function encryptDataBuffer(derivedKeyBuffer, stringToEncrypt, algNameString, keysize, ivBuffer) {
        var encryptedBuffer;
    
        // Convert the input string, stringToEncrypt, to binary.
        var inputDataBuffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(stringToEncrypt, Windows.Security.Cryptography.BinaryStringEncoding.utf8);

        // Open the algorithm provider specified by the algNameString input parameter.
        var algorithmProvider = Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider.openAlgorithm(algNameString);

        // Create a symmetric key.
        var symmetricKey = algorithmProvider.createSymmetricKey(derivedKeyBuffer);

        // Encrypt the input string.
        encryptedBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.encrypt(symmetricKey, inputDataBuffer, ivBuffer);
    
        return encryptedBuffer;
    }

    // Decrypt a data buffer.
    function decryptDataBuffer(derivedKeyBuffer, stringToDecrypt, algNameString, keysize, ivBuffer) {
        var decryptedBuffer = null;
        
        try {
        
            // Convert the input string, stringToDecrypt, to binary.
            var inputDataBuffer = Windows.Security.Cryptography.CryptographicBuffer.decodeFromBase64String(stringToDecrypt);

            // Open the algorithm provider specified by the algNameString input parameter.
            var algorithmProvider = Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider.openAlgorithm(algNameString);

            // Create a symmetric key.
            var symmetricKey = algorithmProvider.createSymmetricKey(derivedKeyBuffer);

            // Decrypt the input string.
            decryptedBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.decrypt(symmetricKey, inputDataBuffer, ivBuffer);
        }
        catch (e) {
            // Ignore decryption errors
        }
        return decryptedBuffer;
    }

    // Copy a string to the specified file.
    function writeToFile(stringToWrite, fileName) {
        
        // Initialize a roaming folder variable.
        var roamingFolder = Windows.Storage.ApplicationData.current.roamingFolder;

        // Create and open an asynchronous file for writing.
        roamingFolder.createFileAsync(fileName, Windows.Storage.CreationCollisionOption.replaceExisting).done(function (file) {
            file.openAsync(Windows.Storage.FileAccessMode.readWrite).done(function (stream) {

                // Create a DataWriter object.
                var writer = new Windows.Storage.Streams.DataWriter(stream);

                // Write to the file.
                writer.writeString(stringToWrite);
                writer.storeAsync().done(function () {
                    stream.flushAsync().done(function () {
                    }, function (errorOnOutputStreamFlushAsync) {});
		    writer.close();
                }, function (errorOnWriterStoreAsync) {
                    writer.close();
                });
            }, function (errorOnOpenAsync) {});
        }, function (errorOnCreateFileAsync) {});
    }

    // Read a string from the specified file.
    function readFromFileAndInitAppState(fileName) {
        var stringReadFromFile = null;

        // Initialize a roaming folder variable.
        var roamingFolder = Windows.Storage.ApplicationData.current.roamingFolder;

        // Open tne file asynchronously.
        roamingFolder.getFileAsync(fileName).done(function (file) {
            file.openAsync(Windows.Storage.FileAccessMode.read).done(function (stream) {
                var size = stream.size;
                var inputStream = stream.getInputStreamAt(0);

                // Create a DataReader object.
                var reader = new Windows.Storage.Streams.DataReader(inputStream);

                // Read from the file.
                reader.loadAsync(size).done(function () {
                    stringReadFromFile = reader.readString(size);
		    reader.close();
                    if (stringReadFromFile) {
                        loadStatus = initializeAppState(stringReadFromFile, fileName);
                    }
                    callBackToComputeLoadState();
                }, function (errorOnReaderLoadAsync) {
                    reader.close();
                    callBackToComputeLoadState();
                });
            }, function (errorOnOpenFile) {
                callBackToComputeLoadState();
            });
        }, function (errorOnGetFileAsync) {
            callBackToComputeLoadState();
        });
    }

    // Initialize the application state.
    function initializeAppState(stringToDecrypt, fileName) {
        var initializeResult = true;
    
        if (!stringToDecrypt) {
            return false;
        }
        // Derive a key from the password.
        var derivedKeyBuffer = deriveKeyFromPassword();

        // Convert the initialization vector string to binary.
        var ivBuffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(ivString, Windows.Security.Cryptography.BinaryStringEncoding.utf8);

        // Decrypt the data.
        var decryptedDataBuffer = decryptDataBuffer(derivedKeyBuffer, stringToDecrypt, cipherAlgNameString, derivedKeyBuffer.length, ivBuffer);
        if (!decryptedDataBuffer) {
            return false;
        }
        var decryptedDataString = Windows.Security.Cryptography.CryptographicBuffer.convertBinaryToString(Windows.Security.Cryptography.BinaryStringEncoding.utf8, decryptedDataBuffer);

        // Initialize the CreditCards, BankAccounts, or SoftwareProducts object.
        if (fileName === "creditCard") {
            initializeResult = CreditCards.initialize(decryptedDataString);
        }
        else if (fileName === "bankAccount") {
            initializeResult = BankAccounts.initialize(decryptedDataString);
        }
        else if (fileName === "softwareProduct") {
            initializeResult = SoftwareProducts.initialize(decryptedDataString);
        }
    
        return initializeResult;
    }

    // Compute the application load state.
    function callBackToComputeLoadState() {
        if (!loadStatus) {
            showIncorrectPasswordPage();
            return;
        }

        if (filesRead !== maxFiles) {
            filesRead++;
        }

        if (filesRead === maxFiles) {
            if (loadStatus) {
                hidePasswordInput();
                showInputOutput();
            }
        }
        return;
    }

    // Update and encrypt the credit card, bank account, or product information.
    function updateData(stringToEncrypt, fileName) {
        if (!stringToEncrypt) {
            writeToFile("", fileName);
            return;
        }

        // Derive a key from the password.
        var derivedKeyBuffer = deriveKeyFromPassword();

        // Convert the initialization vector string to binary.
        var ivBuffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(ivString, Windows.Security.Cryptography.BinaryStringEncoding.utf8);

        // Encrypt the input string.
        var encryptedDataBuffer = encryptDataBuffer(derivedKeyBuffer, stringToEncrypt, cipherAlgNameString, derivedKeyBuffer.length, ivBuffer);
        var encryptedDataString = Windows.Security.Cryptography.CryptographicBuffer.encodeToBase64String(encryptedDataBuffer);

        // Write the encrypted data to the input file name.
        writeToFile(encryptedDataString, fileName);
    }
    sdkSample.updateData = updateData;

    // Hide the password from view.
    function hidePasswordInput() {

        // Retrieve all password elements.
        var passwordInputForm = document.getElementById("passwordInputForm");
        var passwordInputLabel = document.getElementById("passwordInputLabel");

        passwordInputLabel.innerHTML = "";
        passwordInputForm.className = "item hide";
        passwordSubmitButton.className = "item hide";
    }
    
    // Hide input and output.
    function hideInputOutputItems() {
        var items = document.querySelectorAll("#input .item, #output .item");
        for (var i = 0, len = items.length; i < len; i++) {
            items[i].className = "item hide";
        }
    }

    // Show input and output.
    function showInputOutput() {
        
        var scenarios = document.getElementById("scenarios");
        scenarios.className = "options shown";
        document.getElementById("scenariosLabel").innerHTML = "Select:";

        var inputDetails = document.querySelectorAll("#input .details");
        for (var i = 0, len = inputDetails.length; i < len; i++) {
            inputDetails[i].className = "details shown";
        }

        var output = document.getElementById("output");
        output.style.display = "block";

        sdkSample.selectScenario(1);
    }

    // Show error messages related to password validation
    function showStartupErrorMessage(messageToShow) {

        var output = document.getElementById("output");
        output.style.display = "block";

        hideInputOutputItems();
        sdkSample.displayError(messageToShow);
    }

    // If an incorrect passweord is entered, provide the opportunity to correct it.
    function showIncorrectPasswordPage() {

        // Retrieve all password elements.
        var passwordInputForm = document.getElementById("passwordInputForm");
        var passwordInputLabel = document.getElementById("passwordInputLabel");
        var scenarios = document.getElementById("scenarios");

        passwordInputLabel.innerHTML = "Enter password";
        passwordInputForm.className = "item shown";
        passwordSubmitButton.className = "item shown";

        scenarios.className = "options hide";
        document.getElementById("scenariosLabel").innerHTML = "";

        showStartupErrorMessage("Incorrect password");
    }
    
    
    // Input password string.
    function setInputPasswordString(passwordInputString) {
        loadStatus = true;
        filesRead = 0;

        var output = document.getElementById("output");

        if (!passwordInputString) {
            showStartupErrorMessage("Please enter a password");
            return;
        }

        if (passwordInputString.length > sdkSample.maxLength) {
            showStartupErrorMessage("Password is too long. Maximum characters:" + sdkSample.maxLength);
            return;
        }

        password = passwordInputString;
            
        // Load state for the CreditCards, SoftwareProducts, and BankAccounts objects.
        readFromFileAndInitAppState("creditCard");
        readFromFileAndInitAppState("softwareProduct");
        readFromFileAndInitAppState("bankAccount");

        return;
    }

    // Initialize the application.
    function initialize() {

        passwordSubmitButton = document.getElementById("passwordSubmit");
        passwordInput = document.getElementById("passwordInput");

       
        // Add an event listener for the password submit button scenarios.
        passwordSubmitButton.addEventListener("click",
            /*@static_cast(EventListener)*/function (e) {
                setInputPasswordString(passwordInput.value);
            },
            false);
        
        // Add other event listeners
        id("creditCardSelectList").addEventListener("change", /*@static_cast(EventListener)*/displayCreditCardDetails, false);
        id("creditCardAdd").addEventListener("click", /*@static_cast(EventListener)*/addCreditCard, false);
        id("creditCardDelete").addEventListener("click", /*@static_cast(EventListener)*/deleteCreditCard, false);
        id("creditCardUpdate").addEventListener("click", /*@static_cast(EventListener)*/updateCreditCard, false);
        id("bankAccountSelectList").addEventListener("change", /*@static_cast(EventListener)*/displayBankAccountDetails, false);
        id("bankAccountAdd").addEventListener("click", /*@static_cast(EventListener)*/addBankAccount, false);
        id("bankAccountDelete").addEventListener("click", /*@static_cast(EventListener)*/deleteBankAccount, false);
        id("bankAccountUpdate").addEventListener("click", /*@static_cast(EventListener)*/updateBankAccount, false);
        id("softwareProductSelectList").addEventListener("change", /*@static_cast(EventListener)*/displaySoftwareProductDetails, false);
        id("softwareProductAdd").addEventListener("click", /*@static_cast(EventListener)*/addSoftwareProduct, false);
        id("softwareProductDelete").addEventListener("click", /*@static_cast(EventListener)*/deleteSoftwareProduct, false);
        id("softwareProductUpdate").addEventListener("click", /*@static_cast(EventListener)*/updateSoftwareProduct, false);
        id("scenarios").addEventListener("change", /*@static_cast(EventListener)*/onScenarioChanged, false);
    }

    function onScenarioChanged() {
        sdkSample.displayStatus("");
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();
