<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETGroupedGridView.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CSASPNETGroupedGridView</title>
    <script type="text/javascript" src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.4.min.js"></script>
    <script type="text/javascript">

        // This js function will automatially combine the GridView cells
        // gridviewId: The id of the gridview.
        function GroupGridViewCells(gridviewId) {

            // Get the number of the rows
            var rowNum = $(gridviewId + " tr").length;

            // Get the number of the columns;
            var colNum = $(gridviewId + " tr:eq(0)>th").length;

            // Get the current cell
            var cell = null;

            // Get the previous cell
            var previouscell = null;

            // Begin to loop from the second row to the end
            for (var col = 0; col < colNum; ++col) {

                for (var row = 1; row < rowNum; ++row) {

                    cell = $(gridviewId + " tr:eq(" + row + ")>td:eq(" + col + ")").first();

                    // We haven't the previous row to compare with, so keep this first
                    if (row == 1) {
                        previouscell = $(gridviewId + " tr:eq(" + row + ")>td:eq(" + col + ")").first();
                        previouscell.attr("rowspan", "1");
                    }
                    else {

                        // If the current value has the same value as the previous one,
                        // the previous one's rowspan should be increased by 1, and you should
                        // hide the current cell.
                        if (cell.html() == previouscell.html()) {

                            previouscell.attr("rowspan", parseInt(previouscell.attr("rowspan") + 1));
                            cell.css("display", "none");
                        }
                        else {
                            // If the current cell's value doesn't equal to the previous one
                            // we should restart keep the cell for future comparation.
                            previouscell = $(gridviewId + " tr:eq(" + row + ")>td:eq(" + col + ")");
                            previouscell.attr("rowspan", "1");
                            previouscontent = previouscell.html();
                        }
                    }
                }
            }
        }

        // Call the js function like this:
        $(function () {
            GroupGridViewCells("#groupedGridView");
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td style="padding: 20px">
                    <h3>
                        General GridView</h3>
                    <asp:GridView ID="generalGridView" runat="server" AllowPaging="True" 
                        OnPageIndexChanged="generalGridView_PageIndexChanged"
                        OnPageIndexChanging="generalGridView_PageIndexChanging">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:GridView>
                </td>
                <td style="padding: 20px">
                    <h3>
                        Grouped GridView</h3>
                    <asp:GridView ID="groupedGridView" runat="server" AllowPaging="True" 
                        OnPageIndexChanged="groupedGridView_PageIndexChanged"
                        OnPageIndexChanging="groupedGridView_PageIndexChanging">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
