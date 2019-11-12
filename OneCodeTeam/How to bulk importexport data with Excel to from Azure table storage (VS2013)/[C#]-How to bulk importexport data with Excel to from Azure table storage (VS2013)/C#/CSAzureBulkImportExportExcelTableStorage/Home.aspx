<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="CSAZureBulkImportExportExcelTableStorage.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.10.2.min.js">   
    </script>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#btn_Import").click(function () {
                var containerName = $("#txt_TableName").val();
                var patrn = /^[a-z]([a-z0-9])*$/;
                var result = patrn.test(containerName);
                if (!result) {
                    $("#lbl_NameError").css("visibility", "visible");
                    return false;
                }
                else {
                    if (containerName.length >= 3 && containerName.length <= 63) {
                        $("#lbl_NameError").css("visibility", "hidden");
                        //check files name
                        var filename = $("#ful_FileUpLoad").val();
                        if (filename.length > 0) {
                            return true;
                        } else {
                            alert("Select the files you want to upload!");
                            return false;
                        }
                    } else {
                        $("#lbl_NameError").css("visibility", "visible");
                        return false;
                    }
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="font-weight: 600">List all tables Of stroageAccount</div>
            <div style="margin-left: 20px">(1) Select the storage table you want to export to excel.</div>
            <div style="margin-left: 20px">(2) Check" Show table details" to view 10 records of each selected table under table storage.  </div>
            <div style="margin-left: 20px">(3) Click “Export To Excel”.  </div>
            <br />
            <asp:CheckBoxList ID="ckb_TableName" runat="server" OnSelectedIndexChanged="ckb_TableName_SelectedIndexChanged" AutoPostBack="true" Font-Size="16px">
            </asp:CheckBoxList>
            <br />
            <asp:CheckBox ID="chk_ShowDetails" runat="server" Checked="false" Text="Show table details" Font-Size="16px" OnCheckedChanged="chk_ShowDetails_CheckedChanged" AutoPostBack="true" EnableViewState="true" />
            <asp:Table ID="tbl_TableDetailList" runat="server" EnableViewState="true" Font-Size="15px">
                <asp:TableRow></asp:TableRow>
            </asp:Table>

        </div>
        <br />
        <div>
            <asp:Button ID="btn_ExportData" runat="server" Text="Export To Excel" OnClick="btn_ExportData_Click" />
        </div>
        <br />
        <br />
        <div>
            <div style="font-weight: 600">Input the name of the storage table to which you want to import excel files </div>
            <div style="margin-left: 20px">
                (1) Table names must be valid DNS names,3-63 characters in length.<br />
                (2) Beginning with a letter and containing only alphanumeric characters.<br />
                (3) Table names are case-sensitive<br />
            </div>
            <br />
           <label id="lbl_NewTableName" >Table name:</label> <asp:TextBox ID="txt_TableName" runat="server"></asp:TextBox>
            <label id="lbl_NameError" style="color: red; visibility: hidden">Table name is error ,please check it! </label>
            <br /> <br />
            <label id="lbl_FileInfo" style="font-weight: 600">Select the excel files you want to import</label> 
            <br /> 
            <asp:FileUpload runat="server" AllowMultiple="true" ID="ful_FileUpLoad"  />
            <asp:Button ID="btn_Import" runat="server" Text="Import" OnClick="btn_Import_Click" />
        </div>
    </form>
</body>
</html>
