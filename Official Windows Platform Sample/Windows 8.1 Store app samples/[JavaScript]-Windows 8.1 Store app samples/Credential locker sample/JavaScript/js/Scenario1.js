//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario1.html", {
        ready: function (element, options) {
            document.getElementById("ResetButton").addEventListener("click", resetScenario1, false);
            document.getElementById("SaveCredButton").addEventListener("click", AddCredential, false);
            document.getElementById("ReadCredButton").addEventListener("click", ReadCredential, false);
            document.getElementById("DeleteCredButton").addEventListener("click", DeleteCredential, false);
            // Initialize the password vault, this may take less than a second
            // An optimistic initialization at this stage improves the UI performance
            // when any other call to the password vault may be made later on
            asyncVaultLoad();
        }
    });

    var authzInProgress = false;

    function asyncVaultLoad()
    {
        return new WinJS.Promise(function (complete, error, progress) {
            var vault = new Windows.Security.Credentials.PasswordVault();

            // any call to the password vault will load the vault
            var creds = vault.retrieveAll();
            complete();
        });
    }


    function AddCredential() {
        try {
            //
            //get input resource, username, password
            //
            var resource = document.getElementById("InputResourceValue").value;
            var username = document.getElementById("InputUserNameValue").value;
            var password = document.getElementById("InputPasswordValue").value;

            if (resource=== "" || username === "" || password === "") {
                new DebugPrint("Resouce, Username and Password are required when adding a credential");
                return;
            }

            //
            //Adding a credenital to PasswordVault with provided Resouce, Username and Password.
            //
            var vault = new Windows.Security.Credentials.PasswordVault();
            var cred = new Windows.Security.Credentials.PasswordCredential(resource, username, password);
            vault.add(cred);
            new DebugPrint("Credential saved successfully. Resource: "+ cred.resource + " Username: " + cred.userName + " Password: " + cred.password.toString());
        }

        catch (e) {
            new DebugPrint(e.message + " " + e.toString());
            return;
        }

    }
   
    function ReadCredential()
    {
        try {
            //
            //get input resource, username, password
            //
            var resource = document.getElementById("InputResourceValue").value;
            var username = document.getElementById("InputUserNameValue").value;
            var vault = new Windows.Security.Credentials.PasswordVault();
            var cred = null;
            var creds = null;

            //
            //If both resource and username are empty, you can use RetrieveAll() to enumerate all credentials
            //
            if (resource === "" && username === "") {
                new DebugPrint("Retrieving all credentials since resource or username are not specified");
                creds = vault.retrieveAll();
            }
                //
                //If there is only resouce, you can use FindAllByResource() to enumerate by resource.
                //
            else if (username === "")
            {
                new DebugPrint("Retrieving credentials in PasswordVault by resource: " + resource);
                creds = vault.findAllByResource(resource);
            }
                //
                //If there is only username, you can use findbyusername() to enumerate by resource.
                //
            else if (resource === "")
            {
                new DebugPrint("Retrieving credentials in PasswordVault by username: " + username);
                creds = vault.findAllByUserName(username);
            }
            //
            //If both resource and username are provided, you can use Retrieve to search the credential
            //
            else
            {
                new DebugPrint("Retrieving in PasswordVault by resoucre and username: " + resource + "/" + username);
                cred = vault.retrieve(resource,username);
            }
            //
            //printout credenitals
            //
            if (cred!==null)
            {   
                new DebugPrint("Read credential (resource: " + cred.resource + ", user: " + cred.userName + " ) succeeds");
            }
            else if (creds.size > 0)
            {
                new DebugPrint("There are(is) " + creds.size.toString() + " credential(s) found in PasswordVault");
                for (var i = 0; i < creds.size; i++) {
                    //
                    //retrive credenital with resource and username
                    //
                    var singleCred = vault.retrieve(creds.getAt(i).resource, creds.getAt(i).userName);
                    var debugMessage = "Read credential succeeds. Resource: " + singleCred.resource.toString() + " Username: " + singleCred.userName.toString() + " Password: " + singleCred.password.toString() + ".";
                    new DebugPrint(debugMessage);
                }
            }
            else {
                new DebugPrint("Credential not found.");
            }
        }
        catch (e) 
        {
            new DebugPrint(e.message);
            return;
        }

    }

    function DeleteCredential() {

        try {
            //
            //get input resource, username
            //
            var resource = document.getElementById("InputResourceValue").value;
            var username = document.getElementById("InputUserNameValue").value;
            //
            //Delete by resouce name and username
            //
            if (resource === "" || username === "") {
                new DebugPrint("To delete a credential, you need to enter both resoucename and username");
                return;
            }
            else {
                var vault = new Windows.Security.Credentials.PasswordVault();
                var cred = vault.retrieve(resource, username);
                vault.remove(cred);
                new DebugPrint("Credential removed successfully. Resource: " + cred.resource + " Username: " + cred.userName); 
            }

        } catch (e) {
            new DebugPrint(e.message);
            return;
        }

    }


    function resetScenario1() {

        // Get all the result reset

        try {
            document.getElementById("InputResourceValue").value = "";
            document.getElementById("InputUserNameValue").value = "";
            document.getElementById("InputPasswordValue").value = "";
            document.getElementById("Scenario1DebugAreaOutput").value = "";
            var vault = new Windows.Security.Credentials.PasswordVault();
            var creds = vault.retrieveAll();
            for (var i = 0; i < creds.size; i++) {
                //
                //retrive credenital with resource and username
                //
                vault.remove(creds.getAt(i));                
            }
            new DebugPrint("Scenario is reset");

            //
            //enumerate and delete all credentials
            //


        } catch (err) {
            return;
        }

    }

    function DebugPrint(Message) {
        try {
            var currentdate = new Date();
            var datatime = currentdate.getHours().toString() + ":" + currentdate.getMinutes().toString() + ":" + currentdate.getSeconds().toString();
            document.getElementById("ErrorMessage").value = datatime.toString() + " " + Message + "\n" + document.getElementById("ErrorMessage").value;
        } catch (err) {
            return;
        }

    }


})();
