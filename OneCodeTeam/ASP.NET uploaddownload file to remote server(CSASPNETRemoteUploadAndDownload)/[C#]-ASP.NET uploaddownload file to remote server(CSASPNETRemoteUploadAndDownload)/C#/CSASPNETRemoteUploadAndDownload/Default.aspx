<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSRemoteUploadAndDownload.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-family: Calibri;">
        <table>
            <tr>
                <th colspan="2">
                    <p style="font-size: x-large">
                        RemoteUpload:</p>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbUpUrl" runat="server" Text="Url:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbUploadUrl" runat="server" Width="261px"></asp:TextBox>
                    <asp:RadioButtonList ID="rbUploadList" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True">HTTP</asp:ListItem>
                        <asp:ListItem>FTP</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbUpFile" runat="server" Text="File:"></asp:Label>
                </td>
                <td>
                    <asp:FileUpload ID="FileUpload" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click"
                        Height="100%" Width="25%" />
                    <br />
                    <p style="font-size: medium">
                        <strong>Description</strong>: Suppose we will upload a file to virtual directory
                        of remote server &quot;<a href="http://www.mysite.com/mydocument/">http://www.mysite.com/myfiles/</a>&quot;,&nbsp;&nbsp;
                        we just need to fill in Url textbox like: &quot;<a href="http://www.mysite.com/myfiles/">http://www.mysite.com/myfiles/</a>&quot;.&nbsp;If
                        the server is ftp server, the url will be like:&quot;<a href="ftp://www.mysite.com/myfiles/">ftp://www.mysite.com/myfiles/</a>&quot;.
                        And then click the &quot;browse&quot; button to choose file in local which you want
                        to upload. After clicking the &quot;upload&quot; button, the result will be shown
                        on page.</p>
                </td>
            </tr>
        </table>
        <br />
        <table>
            <tr>
                <th colspan="2">
                    <p style="font-size: x-large">
                        RemoteDownLoad:e"> RemoteDownLoad:</p>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbDnUrl" runat="server" Text="Url:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbDownloadUrl" runat="server" Width="388px"></asp:TextBox>
                    <asp:RadioButtonList ID="rbDownloadList" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True">HTTP</asp:ListItem>
                        <asp:ListItem>FTP</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbDnFile" runat="server" Text="DownLoadFilePath:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbDownloadPath" runat="server" Width="267px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnDownLoad" runat="server" Text="Download" OnClick="btnDownLoad_Click"
                        Width="25%" Height="100%" />
                    <br />
                    <p style="font-size: medium">
                        <strong>Description</strong> : Since we need to download specified file from a virtual
                        directory of remote server remote server &quot;<a href="http://www.mysite.com/mydocument/">http://www.mysite.com/myfile/</a>&quot;,&nbsp;
                        you can use this sample to download it. Suppose the file which we need to download
                        from server virtual directory named &quot;myimage.gif&quot;, in downloading Url
                        textbox will be entered similar to: &quot;<a href="http://www.mysite.com/myfile/myimage.gif">http://www.mysite.com/myfile/myimage.gif</a>&quot;.
                        DownLoadFilePath is a physical directory we used to save download file such as &quot;<a
                            href="file:///C:/Temp/Download/">C:\Temp\Download\</a>&quot;&nbsp; in local.
                        Click the &quot;download&quot; button and result will be shown on page.</p>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
