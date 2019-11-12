Type.registerNamespace('SPODocCreatorPageComponent');
var ctz = null;
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
         ctz = new SP.ClientContext.get_current();
         site = ctz.get_site();
         web = ctz.get_web();
         list = web.get_lists().getById(SP.ListOperation.Selection.getSelectedList());
         rootFolder = list.get_rootFolder();
         contentTypes = list.get_contentTypes();
         ctz.load(site);
         ctz.load(web);
         ctz.load(list);
         ctz.load(rootFolder);
         ctz.load(contentTypes);
         ctz.executeQueryAsync(Function.createDelegate(this, contentTypesLoaded), Function.createDelegate(this, contentTypesFailed));
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
         return ['SPODocCreatorPageComponent.Command.PopulateWordTemplates', 'SPODocCreatorPageComponent.Command.PopulatePowerPointTemplates', 'SPODocCreatorPageComponent.Command.PopulateOneNoteTemplates', 'SPODocCreatorPageComponent.Command.CreateDefaultWordDocumentCommand', 'SPODocCreatorPageComponent.Command.CreateDefaultPowerPointDocumentCommand', 'SPODocCreatorPageComponent.Command.CreateDefaultOneNoteDocumentCommand'];
     },
     canHandleCommand: function (commandId) {
         if ((commandId === 'SPODocCreatorPageComponent.Command.PopulateWordTemplates') || (commandId === 'SPODocCreatorPageComponent.Command.PopulatePowerPointTemplates') || (commandId === 'SPODocCreatorPageComponent.Command.PopulateOneNoteTemplates') || (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultWordDocumentCommand') || (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultPowerPointDocumentCommand') || (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultOneNoteDocumentCommand')) {
             return true;
         }
         else {
             return false;
         }
     },
     handleCommand: function (commandId, properties, sequence) {
         if (commandId === 'SPODocCreatorPageComponent.Command.PopulateWordTemplates') {
             properties.PopulationXML = getDropdownItemsXml('Word');
         }
         if (commandId === 'SPODocCreatorPageComponent.Command.PopulatePowerPointTemplates') {
             properties.PopulationXML = getDropdownItemsXml('PowerPoint');
         }
         if (commandId === 'SPODocCreatorPageComponent.Command.PopulateOneNoteTemplates') {
             properties.PopulationXML = getDropdownItemsXml('OneNote');
         }
         if (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultWordDocumentCommand') {
            window.location = stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/Blank Word Document.docx&SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&DefaultItemOpen=1';
         }
         if (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultPowerPointDocumentCommand') {
             window.location = stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/Blank PowerPoint Presentation.pptx&SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&DefaultItemOpen=1';
         }
         if (commandId === 'SPODocCreatorPageComponent.Command.CreateDefaultOneNoteDocumentCommand') {
             window.location = stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/Blank OneNote Document.one&SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&DefaultItemOpen=1';
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
     if (docType === "Word") {
         sb.append('<Menu Id=\'SPODocCreator.RibbonControl.CreateWord.Menu\'>');
         sb.append('<MenuSection DisplayMode=\'Menu16\' Id=\'SPODocCreator.RibbonControl.CreateWord.Menu.Create\'>');
         sb.append('<Controls Id=\'SPODocCreator.RibbonControl.CreateWord.Menu.Create.Controls\'>');
         //Uncomment the following six lines if you want the 'Blank Word Document' option added to the menu
         //sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreateWord.Menu.' + 'DefaultWord' + '\'');
         //sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
         //sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/Blank Word Document.docx&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
         //sb.append(' LabelText=\'' + 'Blank Word Document' + '\'');
         //sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/word_2010small.png' + '\'');
         //sb.append('/>');
         if (querySucceeded) {
             if (list.get_contentTypesEnabled()) {
                 ctEnumerator = contentTypes.getEnumerator();
                 while (ctEnumerator.moveNext()) {
                     ct = ctEnumerator.get_current();
                     ctUrl = ct.get_documentTemplateUrl().toString();
                     if (
                    (ctUrl.substr(ctUrl.length - 5, 5) === '.dotx')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.dotm')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.docx')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.docm')
                    || (ctUrl.substr(ctUrl.length - 4, 4) === '.dot')
                    || (ctUrl.substr(ctUrl.length - 4, 4) === '.doc')
                 ) {
                         sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreateWord.Menu.' + ct.get_id() + '\'');
                         sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
                         sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + ct.get_documentTemplateUrl() + '&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
                         sb.append(' LabelText=\'' + ct.get_name() + '\'');
                         sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/word_2010small.png' + '\'');
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
     if (docType === "PowerPoint") {
         sb.append('<Menu Id=\'SPODocCreator.RibbonControl.CreatePowerPoint.Menu\'>');
         sb.append('<MenuSection DisplayMode=\'Menu16\' Id=\'SPODocCreator.RibbonControl.CreatePowerPoint.Menu.Create\'>');
         sb.append('<Controls Id=\'SPODocCreator.RibbonControl.CreatePowerPoint.Menu.Create.Controls\'>');
         //Uncomment the following six lines if you want the 'Blank PowerPoint Presentation' option added to the menu  
         //sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreatePowerPoint.Menu.' + 'DefaultPowerPoint' + '\'');
         //sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
         //sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/Blank PowerPoint Presentation.pptx&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
         //sb.append(' LabelText=\'' + 'Blank PowerPoint Presentation' + '\'');
         //sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/powerpoint_2010small.png' + '\'');
         //sb.append('/>');
         if (querySucceeded) {
             if (list.get_contentTypesEnabled()) {
                 ctEnumerator = contentTypes.getEnumerator();
                 while (ctEnumerator.moveNext()) {
                     ct = ctEnumerator.get_current();
                     ctUrl = ct.get_documentTemplateUrl().toString();
                     if (
                    (ctUrl.substr(ctUrl.length - 5, 5) === '.potx')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.potm')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.pptx')
                    || (ctUrl.substr(ctUrl.length - 5, 5) === '.pptm')
                    || (ctUrl.substr(ctUrl.length - 4, 4) === '.pot')
                    || (ctUrl.substr(ctUrl.length - 4, 4) === '.ppt')
                 ) {
                         sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreatePowerPoint.Menu.' + ct.get_id() + '\'');
                         sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
                         sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + ct.get_documentTemplateUrl() + '&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
                         sb.append(' LabelText=\'' + ct.get_name() + '\'');
                         sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/powerpoint_2010small.png' + '\'');
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
     if (docType === "OneNote") {
         sb.append('<Menu Id=\'SPODocCreator.RibbonControl.CreateOneNote.Menu\'>');
         sb.append('<MenuSection DisplayMode=\'Menu16\' Id=\'SPODocCreator.RibbonControl.CreateOneNote.Menu.Create\'>');
         sb.append('<Controls Id=\'SPODocCreator.RibbonControl.CreateOneNote.Menu.Create.Controls\'>');
         //Uncomment the following six lines if you want the 'Blank OneNote Document' option added to the menu
         //sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreateOneNote.Menu.' + 'DefaultOneNote' + '\'');
         //sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
         //sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/Blank OneNote Document.one&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
         //sb.append(' LabelText=\'' + 'Blank OneNote Document' + '\'');
         //sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/onenote_2010small.png' + '\'');
         //sb.append('/>');         
         if (querySucceeded) {
             if (list.get_contentTypesEnabled()) {
                 ctEnumerator = contentTypes.getEnumerator();
                 while (ctEnumerator.moveNext()) {
                     ct = ctEnumerator.get_current();
                     ctUrl = ct.get_documentTemplateUrl().toString();
                     if (
                    (ctUrl.substr(ctUrl.length - 4, 4) === '.one')
                    || (ctUrl.substr(ctUrl.length - 7, 7) === '.onepkg')
                 ) {
                         sb.append('<Button Id=\'SPODocCreator.RibbonControl.CreateOneNote.Menu.' + ct.get_id() + '\'');
                         sb.append(' Command=\'SPODocCreatorPageComponent.Command.CreateDocumentCommand\'');
                         sb.append(' MenuItemId=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/_layouts/CreateNewDocument.aspx?id=' + stripSites(site.get_url()) + ct.get_documentTemplateUrl() + '&amp;SaveLocation=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '&amp;Source=' + stripSites(site.get_url()) + rootFolder.get_serverRelativeUrl() + '/Forms/AllItems.aspx&amp;DefaultItemOpen=1' + '\'');
                         sb.append(' LabelText=\'' + ct.get_name() + '\'');
                         sb.append(' Image16by16=\'' + stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()) + '/SharePointOnlineDocCreatorList/onenote_2010small.png' + '\'');
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
