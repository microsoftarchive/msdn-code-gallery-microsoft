Type.registerNamespace('SPODocCreatorPageComponent');
var ctx = null;
var site = null;
var web = null;
var list = null;
var rootFolder = null;
var contentTypes = null;
var querySucceeded = false;
SPODocCreatorPageComponent = function () {
    SPODocCreatorPageComponent.initializeBase(this);
 }
 SPODocCreatorPageComponent.initialize = function () {
     ExecuteOrDelayUntilScriptLoaded(Function.createDelegate(null, SPODocCreatorPageComponent.initializePageComponent), 'SP.Ribbon.js');
     if (SP.ListOperation.Selection.getSelectedList() !== null) {
         ctx = new SP.ClientContext.get_current();
         site = ctx.get_site();
         web = ctx.get_web();
         list = web.get_lists().getById(SP.ListOperation.Selection.getSelectedList());
         rootFolder = list.get_rootFolder();
         contentTypes = list.get_contentTypes();
         ctx.load(site);
         ctx.load(web);
         ctx.load(list);
         ctx.load(rootFolder);
         ctx.load(contentTypes);
         ctx.executeQueryAsync(Function.createDelegate(this, contentTypesLoaded), Function.createDelegate(this, contentTypesFailed));
     }
 }
 function contentTypesLoaded(sender, args) {
     querySucceeded = true;
 }
 function contentTypesFailed(sender, args) {
     querySucceeded = false;    
 }
  SPODocCreatorPageComponent.initializePageComponent = function () {
     var ribbonPageManager = SP.Ribbon.PageManager.get_instance();
     if (null !== ribbonPageManager) {
         ribbonPageManager.addPageComponent(SPODocCreatorPageComponent.instance);
     }
 }
 SPODocCreatorPageComponent.prototype = {
     init: function () {
     },
     getFocusedCommands: function () {
         return ['SPODocCreatorPageComponent.Command.FieldControl.GroupCommand', 'SPODocCreatorPageComponent.Command.FieldControl.TabCommand', 'SPODocCreatorPageComponent.Command.FieldControl.ContextualGroupCommand', 'SPODocCreatorPageComponent.Command.FieldControl.RibbonCommand'];
     },
     getGlobalCommands: function () {
         return ['SPODocCreatorPageComponent.Command.PopulateExcelTemplates', 'SPODocCreatorPageComponent.Command.CreateDefaultExcelDocumentCommand'];
     },
     canHandleCommand: function (commandId) {
         if ((commandId === 'SPODocCreatorPageComponent.Command.PopulateExcelTemplates') || (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultExcelDocumentCommand')) {
             return true;
         }
         else {
             return false;
         }
     },
     handleCommand: function (commandId, properties, sequence) {
         if (commandId === 'SPODocCreatorPageComponent.Command.PopulateExcelTemplates') {
             properties.PopulationXML = getDropdownItemsXml('Excel');
         }
         if (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultExcelDocumentCommand') {
             window.location = stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/xlviewer.aspx?new=1&SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&DefaultItemOpen=1';
         }
     },
     isFocusable: function () {
         return true;
     },
     receiveFocus: function () {
         return true;
     },
     yieldFocus: function () {
         return true;
     }
 }
 function resolveWeb(webUrl) {
     if (webUrl === '/') {
         return ('');
     }
     else {
         return (webUrl);
     }
 }
 function stripSites(sUrl) {
     var returnUrl;
     var iPosSites = sUrl.indexOf('/sites/');
     if (iPosSites != -1) {
         returnUrl = sUrl.substr(0, iPosSites); 
         return (returnUrl);
     } 
     else {
         return (sUrl);
     } 
 }
 function getDropdownItemsXml(docType) {
     var sb = new Sys.StringBuilder();
     var ctEnumerator;
     var ct;
     var ctUrl;
     if (docType === "Excel") {
         sb.append('<Menu Id=\'SPODocCreator.RibbonControl.CreateExcel.Menu\'>');
         sb.append('<MenuSection DisplayMode=\'Menu16\' Id=\'SPODocCreator.RibbonControl.CreateExcel.Menu.Create\'>');
         sb.append('<Controls Id=\'SPODocCreator.RibbonControl.CreateExcel.Menu.Create.Controls\'>');
         //Uncomment the following six lines if you want the 'Blank Excel Workbook' option added to the menu         
         //sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreateExcel.Menu.' + 'DefaultExcel' + '\'');
         //sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
         //sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/xlviewer.aspx?new=1&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
         //sb.append(' LabelText=\'' + 'Blank Excel Workbook' + '\'');
         //sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/excel_2010small.png' + '\'');
         //sb.append('/>');
         if (querySucceeded) {
             if (list.get_contentTypesEnabled()) {
                 ctEnumerator = contentTypes.getEnumerator();
                 while (ctEnumerator.moveNext()) {
                     ct = ctEnumerator.get_current();
                     ctUrl = ct.get_documentTemplateUrl().toString();
                     if (
                    (ctUrl.substr(ctUrl.length - 5, 5) === '.xltx')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.xltm')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.xlsx')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.xlsm')
                    || (ctUrl.substr(ctUrl.length - 4, 4) === '.xlt')
                    || (ctUrl.substr(ctUrl.length - 4, 4) === '.xls')
                 ) {
                         sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreateExcel.Menu.' + ct.get_id() + '\'');
                         sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
                         sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/xlviewer.aspx?new=1&amp;id=' + stripSites(site.get_url()) + ct.get_documentTemplateUrl() + '&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
                         sb.append(' LabelText=\'' + ct.get_name() + '\'');
                         sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/excel_2010small.png' + '\'');
                         sb.append('/>');
                     }
                 }
             }
         }
         sb.append('</Controls>');
         sb.append('</MenuSection>');
         sb.append('</Menu>');
         return sb.toString();
     }
}
SPODocCreatorPageComponent.registerClass('SPODocCreatorPageComponent', CUI.Page.PageComponent);
SPODocCreatorPageComponent.instance = new SPODocCreatorPageComponent();
NotifyScriptLoadedAndExecuteWaitingJobs("SPODocCreatorPageComponent.js");
