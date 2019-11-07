

(function () {
  overrideAllItems();
  overrideNewForm();
})();

function overrideAllItems() {
  // create and initialize object to override context
  var allItemsOverride = {};
  allItemsOverride.BaseViewID = 1;
  allItemsOverride.ListTemplateType = 10000;
  // initialize template 
  allItemsOverride.Templates = {};
  allItemsOverride.Templates.Header = allItemsRenderAnnouncementsHeader;
  allItemsOverride.Templates.Footer = allItemsRenderAnnouncementsFooter;
  allItemsOverride.Templates.Item = allItemsRenderAnnouncements;


  allItemsOverride.OnPreRender = allItemsOnPreRender;
  allItemsOverride.OnPostRender = allItemsOnPostRender;

  // register with page template manager
  SPClientTemplates.TemplateManager.RegisterTemplateOverrides(allItemsOverride);
}

function allItemsOnPreRender(ctx) {
  //alert("allItemsOnPreRender: " + JSON.stringify(ctx));
}

function allItemsOnPostRender(ctx) {
  // alert("allItemsOnPostRender" + JSON.stringify(ctx));
}

function allItemsRenderAnnouncementsHeader(ctx) {
  // render new form link in custom header
  var newItemFormUrl = ctx.newFormUrl;
  var newItemLink = "<a onclick='NewItem2(event, '" + newItemFormUrl + "'); return false;' href='" + newItemFormUrl + " target='_self' data-viewctr='0'>New Item</a>";
  return "<div style='border:solid 1px #aaa; padding:4px; background-color: #ddd;' >" + newItemLink + "</div>";
}

function allItemsRenderAnnouncements(ctx) {
  // create item style
  var itemStyle = "width:600px;margin:12px;";
  var titleStyle = "background-color:black;color:white;padding:2px; padding-left:12px;font-size:1.25em;border-top-left-radius:16px";
  var bodyStyle = "border:black 1px solid;background-color:#ddd;color:#333;padding:4px;border-bottom-right-radius:16px";

  // create and return HTML for each item
  return "<div style='" + itemStyle + "'>" +
             "<div style='" + titleStyle + "'>" + ctx.CurrentItem.Title + "</div>" +
             "<div style='" + bodyStyle + "'>" + ctx.CurrentItem.Body + "</div>" +
         "</div>";
}

function allItemsRenderAnnouncementsFooter(ctx) {
  return "<div style='height:4px;background-color:black' />";
}

function overrideNewForm() {

  // create and initialize object to override context
  var newFormOverride = {};
  newFormOverride.BaseViewID = "NewForm";
  newFormOverride.ListTemplateType = 10000;

  // initialize template 
  newFormOverride.Templates = {};

  newFormOverride.Templates.Fields = {
    'Title': { 'NewForm': newFormTitleField },
  };

  SPClientTemplates.TemplateManager.RegisterTemplateOverrides(newFormOverride);
}


function newFormTitleField(rCtx) {

  var _myData = SPClientTemplates.Utility.GetFormContextForCurrentField(rCtx);
  var _inputId = _myData.fieldName + '_' + _myData.fieldSchema.Id + '_$TextField';

  window.addTitle = function () {
    var autoTitle = "New announcement made at " + (new Date()).toLocaleDateString();
    var titleField = document.getElementById(_inputId);
    titleField.value = autoTitle;

  }

  return "<div style='border:1px solid #ccc;padding:4px;background-color:#eee;'>" +
         "<div><input value='Add Auto Title' type='button' onclick='addTitle()' style='margin:0px;margin-bottom:4px;padding:4px;color:red;backgroundcolor:#bbb;' /></div>" +
         SPFieldText_Edit(rCtx) +
         "</div>";

}