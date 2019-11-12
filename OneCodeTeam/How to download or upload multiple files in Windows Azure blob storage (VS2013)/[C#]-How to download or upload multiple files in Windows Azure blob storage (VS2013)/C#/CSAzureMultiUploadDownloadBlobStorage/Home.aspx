<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="CSAzureMultiUploadDownloadBlobStorage.Home" EnableViewState="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>download /upload multiple files from/to  Windows Azure blob storage</title>
    <script src="Scripts/jquery-1.10.2.min.js">   
    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            $("#btn_Upload").click(function () {
                var containerName = $("#txt_container").val();
                var patrn = /^[a-z0-9]([a-z0-9]|(?:(-)(?!\2)))*$/;
                var result = patrn.test(containerName);
                if (!result) {
                    $("#lbl_containerError").css("visibility", "visible");
                    return false;
                }
                else {
                    if (containerName.length >= 3 && containerName.length <= 63) {
                        $("#lbl_containerError").css("visibility", "hidden");
                        //check files name
                        var filename = $("#FileUpload1").val();
                        if (filename.length >0) {
                            return true;
                        } else {
                            alert("Select the files you want to upload!");
                            return false;
                        }
                    } else {
                        $("#lbl_containerError").css("visibility", "visible");
                        return false;
                    }
                }
            });
        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <label id="lbl_Filename" title="lbl_Filename" style="font-weight: 600">List all Blob Containers: </label>
        <br />
        <br />
        <div>Select the container which contains the blobs you want to download.</div>
        <div>Select the blobs you want to download.  </div>
        <div>Click “Download”.  </div>

        <br />
        <asp:Panel ID="pnl_ListContainer" runat="server">
            <asp:CheckBoxList ID="cbxl_container" runat="server" OnSelectedIndexChanged="cbxl_container_SelectedIndexChanged" AutoPostBack="true">
            </asp:CheckBoxList>
        </asp:Panel>

        <div>
            <br />
            <div style="font-weight: 600">List all blobs in  the container selected.</div>
            <br />
            <asp:Panel ID="pnl_BlobList" runat="server" EnableViewState="true">
                <asp:Label ID="lbl_BlobError" runat="server" Height="200px" ForeColor="Red" Visible="false"> There’s no blob information</asp:Label>
                <br />
                <asp:Table ID="tbl_blobList" runat="server" EnableViewState="true">
                    <asp:TableRow ID="tr_title" VerticalAlign="Middle" HorizontalAlign="Center" BorderStyle="Inset" BorderColor="Black" BorderWidth="1px">
                    </asp:TableRow>
                    <asp:TableRow ID="tr_ListBlob" VerticalAlign="Middle" HorizontalAlign="Justify" BorderStyle="Inset" BorderColor="Black" BorderWidth="1px">
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
        </div>
        <br />
        <br />
       
        <asp:Button ID="btn_Downlaod" runat="server" Text="DownLoad" OnClick="btn_Downlaod_Click" />
        <asp:Label ID="lbl_DownlaodError" runat="server" ForeColor="Red"></asp:Label>
        <asp:Label ID="lbl_DownloadSuccess" runat="server" ForeColor="Red"></asp:Label>
        <br />
        <br />
        <br />
        <br />

        <div>

            <div style="font-weight: 600">Input the name of the container to which you want to upload files </div>
            <div style="margin-left: 38px">
                (1) You can input a new container name or existing container name.<br />
                (2) The name should be between 3 and 63 characters long.<br />
                (3) Container names can contain only lowercase letters, numbers, and hyphens, and must begin with a letter or a number. 
                  The name can't&nbsp;&nbsp; contain two consecutive hyphens.
            </div>
            <br />
            <label>container name:</label>
            <asp:TextBox ID="txt_container" runat="server" Width="177px"></asp:TextBox>

            <label id="lbl_containerError" style="color: red; visibility: hidden"> Container name is error ,please check it!</label>
            <br />
            <br />
            <div style="font-weight: 600">Select the files you want to upload </div>

            <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" Width="528px" />
            <asp:Button ID="btn_Upload" runat="server" Text="Upload" OnClick="btn_Upload_Click" />
        </div>
    </form>
</body>
</html>
