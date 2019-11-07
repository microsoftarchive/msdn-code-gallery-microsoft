<%@ Assembly Name="TranslateJavaScriptSample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e36f831b43346f2b" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TranslateJavaScriptSample.aspx.cs" Inherits="csomjs.Layouts.csomjs.TranslateJavaScriptSample" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<sharepoint:scriptlink name="SP.Translation.js" localizable="false" defer="true" runat="server"/>
<% #if SOME_UNDEFINED_CONSTANT %>
<script type="text/ecmascript" src="SP.Translation.debug.js" ></script>
<% #endif %>
 <script type="text/ecmascript" language="ecmascript">
     var result;
     var asyncJob;

     function onQuerySucceededTranslationJobInfoArray() {
         var i;
         var s;
         s = "";
         //the result is array of TranslationJobInfo
         for (i = 0; i < result.length; i++) {
              s += " JobId: " + result[i].get_jobId();
              s += "\n Name: " + result[i].get_name();
              s += "\n Canceled: " + result[i].get_canceled();
              s += "\n CancelTime: " + result[i].get_cancelTime();                            
              s += "\n partiallySubmitted: " + result[i].get_partiallySubmitted();
              s += "\n submittedTime: " + result[i].get_submittedTime();
              s += "\n";
             }
         document.getElementById("resultText").value = s;
     }

     function onQuerySucceededListAllLang() {
         var i;
         var s;
         s = "";
         //the result is array of string (language)
         for (i = 0; i < result.length; i++) {
             s += result[i] + ", ";          
         }
         document.getElementById("resultText").value = s;
     }

     function onQuerySucceededListAllFileExt() {
         var i;
         var s;
         s = "";
         //the result is array of string (fileExtension)
         for (i = 0; i < result.length; i++) {
             s += result[i] + ", ";
         }
         document.getElementById("resultText").value = s;
     }

     function onQuerySucceededFileExt() {
         var i;
         var s;
         s = "";
         //the result is array of string (fileExtension)
         for (i = 0; i < result.length; i++) {
             s += result[i] + ", ";
         }
         document.getElementById("resultText").value = s;
     }

     function onQuerySucceededTestLang() {
         //SP.BooleanResult() = SP.Result() in sp.runtime.debug.js
        document.getElementById("resultText").value = result.get_value();
     }

     function onQuerySucceededTestFileExt() {
         //SP.BooleanResult() = SP.Result() in sp.runtime.debug.js
         document.getElementById("resultText").value = result.get_value();
     }

     function onQuerySucceededGetFileSize() {
         //SP.BooleanResult() = SP.Result() in sp.runtime.debug.js
         document.getElementById("resultText").value = result.get_value();
     }

     function onQuerySucceededSync() {
         //the result is SP.Translation.TranslationItemInfo
         var s;
         s += " translationId: " + result.get_translationId();
         s += "\n Input File: " + result.get_inputFile();
         s += "\n Output File: " + result.get_outputFile();
         s += "\n Succeeded: " + result.get_succeeded();
         s += "\n Canceled: " + result.get_canceled();
         s += "\n Failed: " + result.get_failed();
         s += "\n In Progress: " + result.get_inProgress();
         s += "\n Not Started: " + result.get_notStarted();         
         s += "\n Error Code: " + result.get_errorCode();
         s += "\n Error Message: " + result.get_errorMessage();
         document.getElementById("resultText").value += s;
     }

     function onQuerySucceededASync() {
         //SP.Translation.TranslationJob
         var s;
         s += "Job Id: " + this.asyncJob.get_jobId();
         s += "\n Name: " + this.asyncJob.get_name();
         s += "\n OutputSaveBehavior: " + this.asyncJob.get_outputSaveBehavior();
         document.getElementById("resultText").value = s;
     }

     function onQueryFailed(sender, args) {
         alert('request failed ' + args.get_message() + '\n' + args.get_stackTrace());
     }
     
     function onQuerySucceededCancelJob() {
         //do nothing
     }
     
     function onQuerySucceededGetItem() {
         //the result is array of TranslationItemInfo
         var i;
         var s;
         for (i = 0; i < result.length; i++) {
             s += "Translation Id: " + result[i].get_translationId();
             s += "\n Input File: " + result[i].get_inputFile();
             s += "\n Output File: " + result[i].get_outputFile();
             s += "\n In Progress: " + result[i].get_inProgress();
             s += "\n Not Started: " + result[i].get_notStarted();
             s += "\n Succeeded: " + result[i].get_succeeded();
             s += "\n Canceled: " + result[i].get_canceled();
             s += "\n Failed: " + result[i].get_failed();          
             s += "\n Error Code: " + result[i].get_errorCode();
             s += "\n Error Message: " + result[i].get_errorMessage() + "\n";
         }
         document.getElementById("resultText").value += s;
     }

     function getClientContext() {
         var clientContext = new SP.ClientContext(document.getElementById("server").value);
         //var clientContext = new SP.ClientContext.get_current();
         return clientContext;
     }

     function SyncButton_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();
         var job = SP.Translation.SyncTranslator.newObject(clientContext, document.getElementById("culture").value);
         job.set_outputSaveBehavior(SP.Translation.SaveBehavior.alwaysOverwrite);                 
         document.getElementById("resultText").value = job.get_outputSaveBehavior();
         result = job.translate(document.getElementById("inputFile").value, document.getElementById("outputFile").value);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededSync),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function ASyncFileButton_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();
         this.asyncJob = null;
         this.asyncJob = SP.Translation.TranslationJob.newObject(clientContext, document.getElementById("culture").value);
         this.asyncJob.set_outputSaveBehavior(SP.Translation.SaveBehavior.alwaysOverwrite);
         document.getElementById("resultText").value = this.asyncJob.get_outputSaveBehavior();
         this.asyncJob.addFile(document.getElementById("inputFile").value, document.getElementById("outputFile").value);
         this.asyncJob.set_name(document.getElementById("jobName").value);
         this.asyncJob.start();
         clientContext.load(this.asyncJob);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededASync),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function ASyncFolderButton_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();
         this.asyncJob = null;
         this.asyncJob = SP.Translation.TranslationJob.newObject(clientContext, document.getElementById("culture").value);
         this.asyncJob.set_outputSaveBehavior(SP.Translation.SaveBehavior.alwaysOverwrite);
         document.getElementById("resultText").value = this.asyncJob.get_outputSaveBehavior();
         var inputFolder = clientContext.get_web().getFolderByServerRelativeUrl(document.getElementById("inputFile").value);
         var outputFolder = clientContext.get_web().getFolderByServerRelativeUrl(document.getElementById("outputFile").value);
         this.asyncJob.addFolder(inputFolder, outputFolder, true);
         this.asyncJob.set_name(document.getElementById("jobName").value);
         this.asyncJob.start();
         clientContext.load(this.asyncJob);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededASync),
Function.createDelegate(this, this.onQueryFailed)); 
        document.getElementById("status").value = "Done";
     }

     function ASyncLibButton_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();
         this.asyncJob = null;
         this.asyncJob = SP.Translation.TranslationJob.newObject(clientContext, document.getElementById("culture").value);
         this.asyncJob.set_outputSaveBehavior(SP.Translation.SaveBehavior.alwaysOverwrite);
         document.getElementById("resultText").value = this.asyncJob.get_outputSaveBehavior();
         //C# List inList = cc.Web.Lists.GetByTitle(inputList);
         var inputLib = clientContext.get_web().get_lists().getByTitle(document.getElementById("inputFile").value);
         var outputLib = clientContext.get_web().get_lists().getByTitle(document.getElementById("outputFile").value);
         this.asyncJob.addLibrary(inputLib, outputLib);
         this.asyncJob.set_name(document.getElementById("jobName").value);
         this.asyncJob.start();
         clientContext.load(this.asyncJob);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededASync),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function GetAllJobs_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();            
         result = SP.Translation.TranslationJobStatus.getAllJobs(clientContext);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededTranslationJobInfoArray),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function GetAllActiveJobs_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         result = SP.Translation.TranslationJobStatus.getAllActiveJobs(clientContext);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededTranslationJobInfoArray),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function ListAllLang_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         result = SP.Translation.TranslationJob.enumerateSupportedLanguages(clientContext);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededListAllLang),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function ListAllFileExt_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         result = SP.Translation.TranslationJob.enumerateSupportedFileExtensions(clientContext);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededListAllFileExt),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function TestLang_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         result = SP.Translation.TranslationJob.isLanguageSupported(clientContext, document.getElementById("inputItem").value);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededTestLang),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function TestFileExt_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         result = SP.Translation.TranslationJob.isFileExtensionSupported(clientContext, document.getElementById("inputItem").value);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededTestFileExt),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function GetFileSize_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         result = SP.Translation.TranslationJob.getMaximumFileSize(clientContext, document.getElementById("inputItem").value);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededGetFileSize),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function CancelJob_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         result = SP.Translation.TranslationJob.cancelJob(clientContext, document.getElementById("inputItem").value);
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededCancelJob),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";
     }

     function GetItem_Click() {
         document.getElementById("status").value = "start";
         var clientContext = getClientContext();
         var contextSite = clientContext.get_site();         
         this.asyncJob = null;
         this.asyncJob = SP.Translation.TranslationJobStatus.newObject(clientContext, document.getElementById("inputItem").value);
         result = this.asyncJob.getItems(document.getElementById("inputType").value);         
         clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceededGetItem),
Function.createDelegate(this, this.onQueryFailed));
         document.getElementById("status").value = "Done";         
     }

     function CleanUp_Click() {
         document.getElementById("server").value = "";
         document.getElementById("status").value = "";
         document.getElementById("inputItem").value = "";
         document.getElementById("inputType").value = "";
         document.getElementById("resultText").value = "";
         document.getElementById("culture").value = "";
         document.getElementById("inputFile").value = "";
         document.getElementById("outputFile").value = "";
         document.getElementById("jobName").value = "";         
     }

 </script>   
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
Status <input type="text" id="status" /><br />
Result: <textarea id="resultText" rows="8" cols="100"></textarea> <br />
<p>==========================================================</p>
ServerRelativeUrl: <input type="text" id="server" size="100"/><br />
[Samples: put "/" for "http://abc", "/site1" for "http://abc/site1"]<br />
[This is required for all actions]
<p>==========================================================</p>
<input class="ms-ButtonHeightWidth" type="button" value="GetAllJobs" onclick="GetAllJobs_Click()" /><br /><br />
<input class="ms-ButtonHeightWidth" type="button" value="GetAllActiveJobs" onclick="GetAllActiveJobs_Click()" /><br /><br />
<input class="ms-ButtonHeightWidth" type="button" value="ListAllLangs" onclick="ListAllLang_Click()" /><br /><br />
<input class="ms-ButtonHeightWidth" type="button" value="ListAllFileExts" onclick="ListAllFileExt_Click()" /><br /><br />
Input: <input type="text" id="inputItem" size="100"/><br />
Type (for GetItem only): <input type="text" id="inputType" size="100"/><br /><br />
Put language in Input textbox (i.e. th): <input class="ms-ButtonHeightWidth" type="button" value="IsLangSupported" onclick="TestLang_Click()" /><br /><br />
Put file extension in Input textbox (i.e. docx): <input class="ms-ButtonHeightWidth" type="button" value="IsFileExtSupported" onclick="TestFileExt_Click()" /><br /><br />
Put file extension in Input textbox: <input class="ms-ButtonHeightWidth" type="button" value="GetFileSize" onclick="GetFileSize_Click()" /><br /><br />
Put job id (Guid) in Input textbox (i.e. 00000000-0000-1055-803d-6fa8421c8be3): <input class="ms-ButtonHeightWidth" type="button" value="CancelJob" onclick="CancelJob_Click()" /><br /><br />
Put job id (Guid) in Input textbox and Type# in Type textbox: <input class="ms-ButtonHeightWidth" type="button" value="GetItems" onclick="GetItem_Click()" /><br /><br />
[succeeded: 1, inProgress: 2, notStarted: 4, failed: 8, canceled: 16]
<br />
<p>==========================================================</p>
Target Language (i.e th): <input type="text" id="culture" /><br /><br />
Input: <input type="text" id="inputFile" size="100"/><br /><br />
Output: <input type="text" id="outputFile" size="100"/><br /><br />
Job Name (for Async Only): <input type="text" id="jobName" size="100"/><br /><br />
Put full URL of file names in Input/Output textboxes: <input class="ms-ButtonHeightWidth" type="button"  id="Sync" value="Sync" onclick="SyncButton_Click()" /><br /><br />
Put full URL of file names in Input/Output textboxes: <input class="ms-ButtonHeightWidth" type="button"  id="ASyncFile" value="ASyncFile" onclick="ASyncFileButton_Click()" /><br /><br />
Put serverRelativeUrl of the folders in Input/Output textboxes: <input class="ms-ButtonHeightWidth" type="button"  id="AsyncFolder" value="ASyncFolder" onclick="ASyncFolderButton_Click()" /><br /><br />
Put Library names in Input/Output textboxes: <input class="ms-ButtonHeightWidth" type="button"  id="AsyncLib" value="ASyncLib" onclick="ASyncLibButton_Click()" /><br /><br />
<p>==========================================================</p>
Clean Up text boxes: <input class="ms-ButtonHeightWidth" type="button"  id="CleanUp" value="CleanUp" onclick="CleanUp_Click()" /><br /><br />
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Translation
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
TSA: CSOM-Javascript
</asp:Content>
