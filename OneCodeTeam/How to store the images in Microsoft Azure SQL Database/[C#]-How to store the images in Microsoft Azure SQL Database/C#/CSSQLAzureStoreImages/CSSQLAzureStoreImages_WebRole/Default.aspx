<%@ Page Title="Home Page" Language="C#"  AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSSQLAzureStoreImages_WebRole._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Store the Images in Windows Azure</title>
    <meta http-equiv="X-UA-Compatible" content="IE=7" />
    <style type="text/css">
        body
        {
            font-family: Verdana;
            font-size: 12px;
        }
        h1
        {
            font-size: x-large;
            font-weight: bold;
        }
        h2
        {
            font-size: large;
            font-weight: bold;
        }
        img
        {
            width: 30em;
            height: 26em;
            margin: 0em;
        }
        li
        {
            list-style: none;

        }
        ul
        {
            padding: 1em;
        }
        
        .form
        {
            width: 50em;
        }
        .form li span
        {
            width: 30%;
            float: left;
            font-weight: bold;
        }
        .form li input
        {
            width: 70%;
            float: left;
        }
        .form input
        {
            float: right;
        }
        
        .item
        {
            font-size: smaller;
            font-weight: bold;
            float: left;
        }
        .item ul li
        {
            min-width: 30em;
            min-height:0.4em;
        }
        .item ul li span
        {
            padding: 0.25em;
            background-color: #ffeecc;
            display: block;
            font-style: italic;
            font-weight: bold;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>
            Image Gallery</h1>
        <div class="form">
            <ul>
                <li><span>Location</span><asp:DropDownList ID="imageLocation" runat="server" 
                        AutoPostBack="True">
                </asp:DropDownList></li>
                <li><span>Name</span><asp:TextBox ID="imageName" runat="server" /></li>
                <li><span>Description</span><asp:TextBox ID="imageDescription" runat="server" /></li>
                <li><span>Filename</span><asp:FileUpload ID="imageFile" runat="server" /></li>
            </ul>
            <asp:Button ID="upload" runat="server" OnClick="Upload_Click" Text="Upload Image" />
        </div>
        <div style="float: left;">
            Status:
            <asp:Label ID="status" runat="server" />
        </div>
        <br />
        <asp:ListView ID="images" runat="server" OnItemDataBound="OnDataBound">
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
            </LayoutTemplate>
            <EmptyDataTemplate>
                <h2>
                    No Data Available</h2>
            </EmptyDataTemplate>
            <ItemTemplate>
                <div class="item">
                    <ul style="width: 30em; float: left; clear: left;">
                        <li>
                                  <asp:Image ID="img" runat="server"   />
                        </li>
                        <li><span><%# Eval("ImageName") %></span> </li>
                        <li>
                            
                            <asp:LinkButton ID="deleteBlob" OnClientClick="return confirm('Delete image?');"
                                CommandName="Delete" CommandArgument='<%# Eval("ImageId")%>' runat="server" Text="Delete"
                                OnCommand="OnDeleteImage" />
                            
                            <asp:LinkButton ID="CopyBlob" OnClientClick="return confirm('Copy image?');" CommandName="Copy"
                                CommandArgument='<%# Eval("ImageId")%>' runat="server" Text="Copy" OnCommand="OnCopyImage" />
                        </li>
                    </ul>
                </div>
            </ItemTemplate>
        </asp:ListView>
    </div>
    </form>
</body>
</html>

