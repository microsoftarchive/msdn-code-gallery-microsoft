<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Chrome control host page</title>

    <script 
        type="text/javascript" 
        src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.2.min.js">
    </script>
    <script 
        type="text/javascript" 
        src="ChromeLoader.js">
    </script>
        <script 
        type="text/javascript" 
        src="CrossDomainExec.js">
    </script>
</head>

<!-- The body is initally hidden. 
     The onCssLoaded callback allows you to 
     display the content after the required
     resources for the chrome control have
     been loaded.  -->
<body style="display: none">

    <!-- Chrome control placeholder -->
    <div id="chrome_ctrl_placeholder"></div>
    <h1 class="ms-accentText">Main content</h1>

    <!-- This is the placeholder for the announcements -->
    <div id="renderAnnouncements"></div>
</body>
</html>
