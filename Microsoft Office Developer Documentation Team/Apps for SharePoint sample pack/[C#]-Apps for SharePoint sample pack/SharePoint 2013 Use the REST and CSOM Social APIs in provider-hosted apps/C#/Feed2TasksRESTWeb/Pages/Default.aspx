<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Feed2TasksRESTWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="Stylesheet" href="../Styles/styles.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/sp.ui.controls.js"></script>
    <script type="text/javascript" src="../Scripts/chromecontrol.js"></script>
    <title>Create Tasks from Feed</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="chrome_ctrl_placeholder"></div>
        <h2>How to use this sample</h2>
        <div class="tasks-section tasks-explanationText">
            <p>This sample is not intended to launched directly. It should be launched from the ribbon extension in a Task List shown in the figure.</p>
            <p>Before starting, you should read the notes below to get it working correctly.</p>
            <img src="../Images/CreateTasks.png" alt="Create Tasks" />
        </div>
        <h3>Setup</h3>
        <div class="tasks-section tasks-explanationText">
            <p>This sample should be deployed to a site that has both a Task list and the Site Feed activated. The sample assumes the presence of a Task list named "Tasks".</p>
            <p>Before using the app, log in as another user and create several posts in the Site Feed. These posts should mention the user who will use the app and be tagged with #Assignment.</p>
            <p>The app looks for posts that mention the current user and are tagged with #Assignment. These posts are candidates to be turned into new tasks.</p>
        </div>
        <h3>Usage</h3>
        <div class="tasks-section tasks-explanationText">
            <p>One the appropriate posts are made, log in as the user who will create the tasks. Then navigate to the Tasks list where you can click the "Create Tasks" button.</p>
            <p>The app will be launched and you will see a table of posts mentioning you and tagged with #Assignment.</p>
            <p>From this table, you can create new tasks that will appear in the Task list on the site.</p>
        </div>
        <asp:Label ID="messages" runat="server" />
    </form>
</body>
</html>
