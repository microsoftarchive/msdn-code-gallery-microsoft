<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>

<html>

<head>
        <SharePoint:ScriptLink ID="ScriptLink1" Name="sp.js" LoadAfterUI="true" Localizable="false" runat="server" />
        <SharePoint:ScriptLink ID="ScriptLink2" Name="sp.workflowservices.js" LoadAfterUI="true" Localizable="false" runat="server" />	
</head>

<body>
    <a href="../Pages/Home.aspx">Home</a>
    <form id="Form1" runat="server">
        <asp:ScriptManager id="ScriptManager" runat="server" EnablePageMethods="false" EnablePartialRendering="true" EnableScriptGlobalization="false" EnableScriptLocalization="true" />
        <SharePoint:FormDigest ID="FormDigest1" runat="server" />
        
        Select a web: 
        <input type='radio' name="targetWebButton" onclick='toggleTargetWeb(this)' value='App Web' checked/>App Web&nbsp;&nbsp;
        <input type='radio' name="targetWebButton" onclick='toggleTargetWeb(this)' value='Parent Web' />Parent Web<br /><br />
        Party on it:<br /><br />
        <input type='button' onclick='initHostWebContext()' value='Init WorkflowDeploymentService' /><br /><br />
        <input type='button' onclick='validateWorkflowDefinition(this.form)' value='Validate Workflow Definition' />&nbsp;&nbsp;
        <input type='button' onclick='saveWorkflowDefinition(this.form)' value='Save Workflow Definition' />&nbsp;&nbsp;
        <input type='button' onclick='pulbishWorkflowDefinition()' value='Pulbish (last saved) Workflow Definition' /><br /><br />
        <input type='button' onclick='enumerateWorkflowDefinitions()' value='Enumerate Workflow Definitions' />&nbsp;&nbsp;
        <input type='button' onclick='getDesignerActions()' value='Call GetDesignerActions' /><br /><br />
        Workflow Definition Display Name:<br />
        <textarea name="displayNameTextarea" rows="1" cols="100">My_Workflow</textarea><br />
        Workflow Definition XAML:<br />
        <textarea name="xamlTextarea" rows="20" cols="100"">&lt;p:Activity x:Class=&quot;Test.EmptyWorkflow&quot; xmlns:p=&quot;http://schemas.microsoft.com/netfx/2009/xaml/activities&quot; xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;&gt;&lt;/p:Activity&gt; </textarea>

        <script language="javascript">
            var _targetWeb = 'App Web';
            var _hostCtx;
            var _hostWeb;
            var _result;
            var _wsm;
            var _wds;
            var _wdsReady = false;
            var _sampleXaml = '<p:Activity x:Class="Test.EmptyWorkflow" xmlns:p="http://schemas.microsoft.com/netfx/2009/xaml/activities" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"></p:Activity>';

            function initXaml(form) {
                form.xamlTextarea.value = _sampleXaml;
            }

            function toggleTargetWeb(arg) {
                _targetWeb = arg.value == 'App Web' ? 'App Web' : 'Parent Web';
                alert('Please reinitialize WorkflowDeploymentService');
            }

            function initHostWebContext() {
                var surl = _spPageContextInfo.webAbsoluteUrl;
                if (_targetWeb == 'Parent Web') {
                    var lastSlash = surl.lastIndexOf("/");
                    if (lastSlash > 8) {
                        surl = surl.substring(0, lastSlash);
                    }
                }

                alert("Host web url is: " + surl);

                _hostCtx = new SP.ClientContext(surl);
                _hostWeb = _hostCtx.get_web();
                _hostCtx.load(_hostWeb);
                _hostCtx.executeQueryAsync(initWorkflowServicesManager);
            }

            function initWorkflowServicesManager() {
                //alert("Host web retrieved.  Now retrieving WorkflowServicesManager");
                _wsm = new SP.WorkflowServices.WorkflowServicesManager.newObject(_hostCtx, _hostWeb);
                _hostCtx.load(_wsm);
                _hostCtx.executeQueryAsync(initWorkflowDeploymentService);
            }

            function initWorkflowDeploymentService() {
                //alert("WorkflowServicesManger retrieved.  Now retrieving WorkflowDeplyomentService.");
                _wds = _wsm.getWorkflowDeploymentService();
                _hostCtx.load(_wds);
                _hostCtx.executeQueryAsync(
                    function () {
                        alert("WorkflowDeploymentService retrieved");
                        _wdsReady = true;
                    }
                );
            }

            function validateWorkflowDefinition(form) {
                if (!_wdsReady) {
                    alert("WorkflowDeploymentService is not ready");
                    return;
                }

                _result = _wds.validateActivity(form.xamlTextarea.value);
                _hostCtx.executeQueryAsync(
                    function () {
                        alert("Result of validateWorkflowDefinition:" + _result.get_value());
                    }
                );
            }

            var _wDef;
            var _wDefId;

            function saveWorkflowDefinition(form) {
                _wDef = new SP.WorkflowServices.WorkflowDefinition.newObject(_hostCtx, _hostWeb);
                _hostCtx.load(_wDef);
                _hostCtx.executeQueryAsync(
                    function () {
                        //alert("Blank workflow definition object created");
                        _wDef.set_xaml(form.xamlTextarea.value);
                        _wDef.set_displayName(form.displayNameTextarea.value);

                        if (!_wdsReady) {
                            alert("WorkflowDeploymentService is not ready");
                            return;
                        }

                        _result = _wds.saveDefinition(_wDef);
                        _hostCtx.executeQueryAsync(
                            function () {
                                alert("Result of saveWorkflowDefinition:" + _result.get_value());
                                _wDefId = _result.get_value();
                            }
                        );
                    }
                );
            }

            function pulbishWorkflowDefinition() {
                if (!_wdsReady) {
                    alert("WorkflowDeploymentService is not ready");
                    return;
                }

                _wds.publishDefinition(_wDefId);
                _hostCtx.executeQueryAsync(
                    function () {
                        alert("pulbishWorkflowDefinition succeeded");
                    },
                    function (sender, args) {
                        alert('pulbishWorkflowDefinition failed with error: ' + args.get_message());
                    }
                )
            }

            var _wDefCollection;

            function enumerateWorkflowDefinitions() {
                if (!_wdsReady) {
                    alert("WorkflowDeploymentService is not ready");
                    return;
                }

                _wDefCollection = _wds.enumerateDefinitions(false);
                _hostCtx.load(_wDefCollection);
                _hostCtx.executeQueryAsync(
                    function () {
                        var definitions = '';
                        var wDefEnumerator = _wDefCollection.getEnumerator();
                        while (wDefEnumerator.moveNext()) {
                            definitions = definitions + '\n';
                            definitions = definitions + wDefEnumerator.get_current().get_displayName() + '\n';
                        }
                        alert("Result of enumerateWorkflowDefinition:\n" + definitions);
                    }
                )
            }

            function getDesignerActions() {
                if (!_wdsReady) {
                    alert("WorkflowDeploymentService is not ready");
                    return;
                }

                _result = _wds.getDesignerActions(_hostWeb);
                _hostCtx.executeQueryAsync(
                    function () {
                        alert("Result of getDesignerActions:" + _result.get_value());
                    }
                );
            }

            
        </script>
    </form>
</body>
</html>