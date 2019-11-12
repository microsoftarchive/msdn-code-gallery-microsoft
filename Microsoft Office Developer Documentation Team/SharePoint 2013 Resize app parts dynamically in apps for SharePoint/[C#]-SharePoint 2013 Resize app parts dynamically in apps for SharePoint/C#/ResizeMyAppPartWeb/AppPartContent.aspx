<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppPartContent.aspx.cs" Inherits="ResizeMyAppPartWeb.AppPartContent" %>

<!DOCTYPE HTML>
<html>
<body>
    <!-- Placeholder for edit mode message -->
    <h1 id="editmodehdr" style="display: none">The app part is in edit mode.
    </h1>
    
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
     <!-- Resize.js is the  JavaScript function, controls the rendering
         logic based on the custom property values -->
    <script type="text/javascript" src="../Scripts/Resize.js"></script>
    <div id="content">
        
       Select the width from the drop down menu to resize this app part :
        <!-- Drop Down Menu -->
        <select name="widthDropDownMenu" id="widthDropDownMenu" onchange="ResizeAppPart();"> 
            <option>Resize to</option>
            <option value="70">70px</option>
            <option value="90">90px</option>
            <option value="110">110px</option>
            <option value="130">130px</option>
            <option value="150">150px</option>
            <option value="280">280px</option>
            <option value="300">300px</option>

        </select> <br/>
      <div>Current width:<p id="UserSelectedWidthOption">Default.</p></div>  
    </div> 
   

</body>
</html>
