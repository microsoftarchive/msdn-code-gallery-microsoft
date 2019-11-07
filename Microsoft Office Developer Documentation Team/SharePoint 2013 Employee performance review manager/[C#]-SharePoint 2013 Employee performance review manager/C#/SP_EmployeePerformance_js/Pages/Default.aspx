<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />
    <link rel="stylesheet" type="text/css" href="../Content/jquery-ui-1.10.2.custom.css" />
    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/App.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.10.2.js"></script>


</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <SharePoint:ScriptLink name="clienttemplates.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink name="clientforms.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink name="clientpeoplepicker.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink name="autofill.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink name="sp.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink name="sp.runtime.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink name="sp.core.js" runat="server" LoadAfterUI="true" Localizable="false" />




    <div id="peoplePicker" style="display: none; float: left; width: 300px;">

        <div id="peoplePickerDiv" style="float: left; width: 200px;"></div>
        <div onclick="getUserInfo()" class="peoplePickerButton" id="addEmployeeButton">+</div>


        <div class="clear">&nbsp;</div>
        <!-- Employee View -->
        <div id="AllEmployee" style="">
            <div id="errAllEmployee" class="errorClass"></div>
            <div id="EmployeeHeading" class="listHeading">Employee Names</div>
            <div id="EmployeeList"></div>
        </div>
    </div>


    <!-- Employee Review -->
    <div id="reviewForm" style="display: none; background-color: #5c9ccc; width: 750px; float: left;">
        <div class="clear" style="height: 30px;">&nbsp;</div>
        <div class="formTitle">Employee Reviews</div>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div id="ReviewDetails" style="margin-top: 20px; float: left; width: 630px; margin-left: 50px; background-color: white; padding: 10px;">
            <div class="formLabel">Employee Name</div>
            <input type="text" id="employeeName" disabled="disabled" style="width: 400px;" />
            <div class="clear">&nbsp;</div>
            
        </div>
        <div class="clear">&nbsp;</div>
        <div style="font-family: 'Segoe UI'; font-size: 20px; color: white; font-weight: bold; padding-left: 50px;">Reviews</div>
        <div class="clear">&nbsp;</div>
        <div id="errReview" class="errorClass" style="margin-left: 10px;"></div>
        <div class="clear">&nbsp;</div>
        <div id="reviewList" style="margin-left: 10px; width:300px;"></div>
        <div class="clear">&nbsp;</div>
        <div id="newReview" class="clicker" style="display: none; float: left; width: 110px;" onclick="addNewReview();">+New Review</div>
        <div class="clear">&nbsp;</div>
        <%--add new review--%>
        <div id="showReview" style="display: none; margin-top: 10px; margin-left: 50px; margin-bottom: 30px; width: 630px; background-color: white; float: left; padding: 10px;">
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="formLabel">Comments</div>
            <input type="text" id="comment" style="width: 400px; height: 40px;" />
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="formLabel">Status</div>
            <select id="newReviewStatus" style="width: 150px;" disabled="disabled">
                <option value="Active" selected="selected">Active</option>
                <option value="Completed">Completed</option>
            </select>
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="formLabel">Dates</div>
            <div class="formLabel" style="width: 195px">Start Date</div>
            <div class="formLabel">End Date</div>
            <div class="formLabel">&nbsp;</div>
            <input type="text" id="startDate" readonly="readonly" style="width: 150px" />
            <input type="text" id="endDate" readonly="readonly" style="width: 150px; margin-left: 40px" />
            <div class="clear" style="height: 145px;">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>

            <div class="button" onclick="cancelReview();" style="margin-right: 15px;">Cancel</div>
            <div id="saveNewRevButton" class="button" onclick="saveNewReview();">Save</div>
        </div>


        <%--edit review--%>
        <div id="editReview" style="display: none; margin-top: 10px; margin-left: 50px; margin-bottom: 30px; width: 630px; background-color: white; float: left; padding: 10px;">
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="formLabel">Comments</div>
            <input type="text" id="editComments" style="width: 400px; height: 40px;" />
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="formLabel">Status</div>
            <select id="editStatus" style="width: 150px;" disabled="disabled">
                <option value="Active" selected="selected">Active</option>
                <option value="Completed">Completed</option>
            </select>
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="formLabel">Dates</div>
            <div class="formLabel" style="width: 195px">Start Date</div>
            <div class="formLabel">End Date</div>
            <div class="formLabel">&nbsp;</div>
            <input type="text" id="editStartDate" readonly="readonly" style="width: 150px" />
            <input type="text" id="editEndDate" readonly="readonly" style="width: 150px; margin-left: 40px" />
            <div class="clear" style="height: 145px;">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div class="clear">&nbsp;</div>
            <div id="objDetails" style="display: none;">
                <div style="font-family: 'Segoe UI'; font-size: 20px; color: black; margin-left: 15px;">Objectives</div>
                <div id="errObj" class="errorClass"></div>
                <div id="newObjective" class="newObjClicker" onclick="addNewObjective();">+New Objective</div>
                <div class="clear">&nbsp;</div>
                <div class="clear">&nbsp;</div>
                <div id="objectiveList"></div>
            </div>
            <div class="button" onclick="cancelReview();" style="margin-right: 15px;">Cancel</div>
            <div id="saveEditRevButton" class="button" onclick="saveEditReview();">Save</div>
        </div>
    </div>


    <!-- New Objective Dialog -->

    <div id="newObjectiveDialog" title="Set New Objective" style="display: none; float: left; padding: 10px;">
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Objective Name</div>
        <input type="text" id="newObjName" style="width: 400px;" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Priority</div>
        <select id="newObjPriority" style="width: 150px">
            <option value="High">High</option>
            <option value="Medium">Medium</option>
            <option value="Low">Low</option>
        </select>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Status</div>
        <select id="newObjStatus" style="width: 150px">
            <option value="Active" selected="selected">Active</option>
            <option value="Completed">Completed</option>
            <option value="Deferred">Deferred</option>
            <option value="Cancelled">Cancelled</option>
        </select>
        <div class="clear" style="height: 30px;">&nbsp;</div>
        <div class="objButton" onclick="cancelObjective();">Cancel</div>
        <div class="objButton" onclick="saveNewObjective();">Save</div>
    </div>


    <div id="editObjDialog" title="Edit Objective" style="display: none; float: left; padding: 10px;">
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Objective Name</div>
        <input type="text" id="editObjName" style="width: 400px;" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Priority</div>
        <select id="editObjPriority" style="width: 150px">
            <option value="High">High</option>
            <option value="Medium">Medium</option>
            <option value="Low">Low</option>
        </select>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Status</div>
        <select id="editObjStatus" style="width: 150px">
            <option value="Active" selected="selected">Active</option>
            <option value="Completed">Completed</option>
            <option value="Deferred">Deferred</option>
            <option value="Cancelled">Cancelled</option>
        </select>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Comments</div>
        <input type="text" id="editEmpComments" disabled="disabled" style="width: 400px; height: 40px;" />
        <div class="clear" style="height: 30px;">&nbsp;</div>
        <div class="objButton" onclick="cancelEditObjective();">Cancel</div>
        <div class="objButton" onclick="deleteEditObj();">Delete</div>
        <div class="objButton" onclick="saveEditObj();">Save</div>
    </div>

    <!-- Emplyoyee View Objectives -->
    <div id="AllObjectives" style="display: none; float: left; width: 210px;">
        <div id="errAllObjectives" class="errorClass"></div>
        <div id="ObjectiveHeading" class="listHeading">Objectives</div>
        <div id="EmpRevList"></div>
        <div id="EmpObjList"></div>
    </div>


    <!-- Emplyoyee View Edit Objectives -->
    <div id="ObjectiveDetails" class="empObj" style="display: none;">
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formTitle">Objective Detils</div>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="empFormLabel">Objective Name</div>
        <input type="text" id="objName" style="width: 400px;" disabled="disabled" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="empFormLabel">Priority</div>
        <select id="objPriority" style="width: 150px" disabled="disabled">
            <option value="High">High</option>
            <option value="Medium">Medium</option>
            <option value="Low">Low</option>
        </select>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="empFormLabel">Status</div>
        <select id="objStatus" style="width: 150px" disabled="disabled">
            <option value="Active" selected="selected">Active</option>
            <option value="Completed">Completed</option>
            <option value="Deferred">Deferred</option>
            <option value="Cancelled">Cancelled</option>
        </select>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="empFormLabel">Employee Comments</div>
        <input type="text" id="empComments" style="width: 400px; height: 40px;" />
        <div class="clear" style="height: 30px;">&nbsp;</div>
        <div class="empbutton" onclick="cancelEmpObj();">Cancel</div>
        <div class="empbutton" onclick="saveEmpComments();">Save</div>
    </div>

</asp:Content>
