<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultPage.ascx.cs" Inherits="CSASPNETPreventMultipleWindows.DefaultPage" %>
<script>
    //First, window name will be blank("") and goto this branch of If
    if (window.name == "") {
        window.name = "Default";
        window.open("Default.aspx", "_self");
    }
    //Second, window name change to "Default" and goto this branch of If
    else if (window.name == "Default") {
        //Set window features
        var WindowFeatures = 'width=800,height=600';
        window.open("Main.aspx", "<%=GetWindowName() %>");
        window.opener = top;
        window.close();
    }
    else if (window.name == "InvalidPage") {
        window.close();
    }
    else {
        window.name = "InvalidPage";
        window.open("InvalidPage.aspx", "_self");
    }
</script>
