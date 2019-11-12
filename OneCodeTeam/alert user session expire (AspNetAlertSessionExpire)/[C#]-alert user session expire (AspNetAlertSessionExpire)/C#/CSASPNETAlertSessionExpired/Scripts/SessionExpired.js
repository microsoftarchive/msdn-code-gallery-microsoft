///<reference path="jquery-1.4.1-vsdoc.js" />
/****************************** Module Header ******************************\
* Module Name:    SessionExpired.js
* Project:       CSASPNETAlertSessionExpired
* Copyright (c) Microsoft Corporation
* The project illustrates how to design a simple user control which is used to 
* alter the session is about to expired. 
* We use jQuery, ASP.NET AJAX at client side.
* In this sample, we added a AlterSessionExpired control and Script Manager on
* the Master page, and It will alert the user for long time idle, whether to 
* extend the session before it will be expired in one minute.
* 
* In this file, We set methods to let the users to select whether extend the 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/



var isextend = false;
var isfirst = true;
    function TableHidden() {
        $("table[id*='tbSessionExpired']").css("display", "none");
        $("table[id*='tbSessionExpired']").css("visibility", "hidden");
    }

    function TableShow() {
        $("table[id*='tbSessionExpired']").css("display", "block");
        $("table[id*='tbSessionExpired']").css("visibility", "visible");
    }

    function SetTimeextend() {
        isextend = true;
        clientCallServer();
        TableHidden();
    }

    function SetTimeExpired() {
        isextend = false;
        TableHidden();
        CheckButtonDisable();
    }



    function TimeextendOrExpired() {

        setInterval("SetTime()", 1000);
        setInterval("ShowExpired()", 31000);

        if (isfirst) {
            isfirst = false;
            TableShow();
            i = 30;
        }
        else if (isextend) {
            TableShow();
            i = 30;
        }

    }

    function ShowExpired() {
        if (isextend == false) {
            $("span[id*='spSessionExpired'").css("display", "block");
            CheckButtonEnable();
            TableHidden();
        }
    }


    function CheckButtonDisable() {
    
        $("input[id*='btnSessionState']").attr("disabled","disabled");
    }


    function CheckButtonEnable() {

        $("input[id*='btnSessionState']").removeAttr("disabled");
    }