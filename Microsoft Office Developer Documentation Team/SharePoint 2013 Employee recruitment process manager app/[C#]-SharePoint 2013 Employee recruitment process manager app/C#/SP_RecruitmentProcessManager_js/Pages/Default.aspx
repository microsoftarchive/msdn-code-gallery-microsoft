<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.10.2.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-timepicker.js"></script>

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />
    <link rel="stylesheet" type="text/css" href="../Content/jquery-ui-1.10.2.custom.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Recruitment Process Manager
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <SharePoint:ScriptLink ID="ScriptLink1" Name="clienttemplates.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink2" Name="clientforms.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink3" Name="clientpeoplepicker.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink4" Name="autofill.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink5" Name="sp.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink6" Name="sp.runtime.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink7" Name="sp.core.js" runat="server" LoadAfterUI="true" Localizable="false" />


    <!-- Job Vacancies View -->
    <div id="AllVacancies" style="display: none; float: left; width: 300px;">
        <div id="errAllVacancies" class="errorClass"></div>
        <div id="AllVacanciesHeading" class="listHeading">Job Vacancies</div>
        <div class="clicker" onclick="addNewVacancy();">+ <u>New Job Vacancy</u></div>
        <div id="vacancyList"></div>
    </div>
    

    <!-- Add New Vacancy form -->
    <div id="AddNewVacancy" title="Add New Vacancy" style="display: none; width: 600px; height: auto; float: left; background-color: #BFEFFF;border:1px solid #0072C6 ;">
        <div class="clear">&nbsp;</div>
        <div class="formTitle">Add New Vacancy</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Job Title *</div>
        <input type="text" id="newJobTitle" style="width: 360px" />
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Number of Vacancies *</div>
        <input type="text" id="newNoofVacancies" style="width: 360px" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Job Description</div>
        <textarea id="newJobDescription" style="width: 360px;" rows="3" cols="75"></textarea>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Prerequisites</div>
        <textarea id="newPrerequisites" style="width: 360px;" rows="3" cols="75"></textarea>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Locations</div>
        <input type="text" id="newLocation" style="width: 360px" />
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Work Hours</div>
        <input type="text" id="newWorkHours" style="width: 360px" />
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Salary</div>
        <input type="text" id="newSalary" style="width: 360px" />
        <div class="clear">&nbsp;</div>
         <div class="clear">&nbsp;</div>

        <div class="button" onclick="cancelNewVacancy();" style="margin-right: 35px;">Cancel</div>
        <div class="button" onclick="saveNewVacancy();">Save</div>
        <div class="clear">&nbsp;</div>
        <div class="clear" style="margin-bottom:20px;">&nbsp;</div>
    </div>

    <!-- Edit Vacancy form -->
    <div id="vacancyDetails" title="Vacancy Details" style="display: none; width: 600px; height: auto; float: left; background-color: #BFEFFF;border:1px solid #0072C6 ;">
        <div class="clear">&nbsp;</div>
        <div class="formTitle">Vacancy Details</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Job Title *</div>
        <input type="text" id="editJobTitle" style="width: 360px" />
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Number of Vacancies *</div>
        <input type="text" id="editNoofVacancies" style="width: 360px" />
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Job Description</div>
        <textarea id="editJobDescription" style="width: 360px;" rows="3" cols="75"></textarea>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Prerequisites</div>
        <textarea id="editPrerequisites" style="width: 360px;" rows="3" cols="75"></textarea>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Locations</div>
        <input type="text" id="editLocation" style="width: 360px" />
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Work Hours</div>
        <input type="text" id="editWorkHours" style="width: 360px" />
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Salary</div>
        <input type="text" id="editSalary" style="width: 360px" />
        <div class="clear" style="width:20px;">&nbsp;</div>

        <div id="errorInterview"></div>
        <div class="interviewListLabel">Interviewees</div>
        <div id="interviewList"></div>
        <div class="clear">&nbsp;</div>
        <div class="interviewListLabel">Job Offered</div>
        <div id="jobOfferedList"></div>


        <div class="button" onclick="cancelEditVacancy();" style="margin-right: 35px;">Cancel</div>
        <div class="button" onclick="saveEditVacancy();">Save</div>
        <div class="button" onclick="deleteVacancy();">Delete</div>
        <div class="clear" style="height: 45px;">&nbsp;</div>
        <div class="button" id="sheduleInterview" style="float: right; width: 250px; margin-right: 35px;" onclick="sheduleInterview();">Schedule Interview</div>
        <div class="clear" style="height: 45px;">&nbsp;</div>
        <div class="button" id="createJobDescription" style="float: right; width: 250px; margin-right: 35px;" onclick="createJobDescription();">Print Preview</div>
        <div class="clear" style="height: 40px;margin-bottom:10px">&nbsp;</div>
    </div>

    <!-- New interview dialog-->
    <div id="addInterView" title="Add Interview" style="display: none;">
        <div id="errorAddInterview" class="errorClass"></div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Location</div>
        <input type="text" id="newInterviewLoc" style="width: 360px" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Date</div>
        <input type="text" id="newInterviewDate" style="width: 200px" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Interviewee</div>
        <input type="text" id="newInterviewee" style="width: 360px" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Interviewer</div>
        <div id="peoplePickerDiv" style="float: left;"></div>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="dialogButton" onclick="cancelNewInterview();">Cancel</div>
        <div class="dialogButton" onclick="saveNewInterview();">Save</div>

        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
    </div>


    <!-- Edit interview dialog-->
    <div id="editInterView" title="Edit Interview" style="display: none;">
        <div id="errEditInterView" class="errorClass"></div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Location</div>
        <input type="text" id="editInterviewLoc" style="width: 360px" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Date</div>
        <input type="text" id="editInterviewDate" style="width: 200px" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Interviewee</div>
        <input type="text" id="editInterviewee" style="width: 360px" />
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <div class="formLabel">Interviewer</div>
        <div id="showInterviewers" style="float:left;margin-right:10px;" class="formLabel"></div>
         <div id="changeInterviewer" class="interviewerClicker" onclick="changeInterviewer();"><u>Change Interviewer</u></div>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Comments</div>
        <textarea id="editComments" rows="3" cols="48" ></textarea>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>

        <input type="checkbox" id="jobOffered" name="jobOffered" style="margin-left:18px;" > Job Offered<br>

        <div class="dialogButton" onclick="cancelEditInterview();">Cancel</div>
        <div class="dialogButton" id="saveEditInterview" onclick="saveEditInterview();">Save</div>
        <div class="dialogButton" id="deleteInterview" onclick="deleteInterview();">Delete</div>
        <div class="dialogButton" id="createOfferLetter" onclick="createOfferLetter();" style="width:175px;display:none;">Print Offer Letter</div>
        <div class="clear">&nbsp;</div>
        <div class="clear">&nbsp;</div>
    </div>

    <!-- Add New Attendees in Edit offsite Form -->
    <div id="editInterviewer" title="Add New Interviewer" style="display: none;">
        <div class="clear">&nbsp;</div>
        <div class="formLabel">Interviewer</div>
        <div id="editPeoplePickerDiv" style="float: left; width: 200px;"></div>
        <div class="clear" style="height:70px;">&nbsp;</div>
        <div class="dialogButton" onclick="cancelInterviewer();" style="margin-right: 15px;">Cancel</div>
        <div class="dialogButton" onclick="saveInterviewer();">Save</div>
    </div>
</asp:Content>
