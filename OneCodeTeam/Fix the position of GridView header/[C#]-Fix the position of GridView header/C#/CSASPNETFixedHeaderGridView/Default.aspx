<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETFixedHeaderGridView.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="JScript/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="JScript/ScrollableGridPlugin.js" type="text/javascript"></script>
    <script type = "text/javascript">
        $(document).ready(function () {
            //Invoke Scrollable function.
            $('#<%=gvwList.ClientID %>').Scrollable({
                ScrollHeight: 600,        
            });
        });

</script>
</head>
<body >
    <form id="form1" runat="server">
    <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto"  >
        <asp:GridView ID="gvwList" runat="server" CellSpacing="2" 
    AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="a" FooterText="title a" 
            HeaderText="titile a" />
                <asp:BoundField DataField="b" FooterText="title b" 
            HeaderText="titile b" />
                <asp:BoundField DataField="c" FooterText="title c" 
            HeaderText="titile c" />
                <asp:BoundField DataField="d" FooterText="title d" 
            HeaderText="titile d" />
                <asp:BoundField DataField="e" FooterText="title e" 
            HeaderText="titile e" />
                <asp:BoundField DataField="f" FooterText="title f" 
            HeaderText="titile f" />
                <asp:BoundField DataField="g" FooterText="title g" 
            HeaderText="titile g" />
                <asp:BoundField DataField="h" FooterText="title h" 
            HeaderText="titile h" />
                <asp:BoundField DataField="i" FooterText="title i" 
            HeaderText="titile i" />
                <asp:BoundField DataField="j" FooterText="title j" 
            HeaderText="titile j" />
            </Columns>
        </asp:GridView>
    </asp:Panel>

    </form>
</body>
</html>
