@ModelType IEnumerable(Of VBRESTfulWCFServiceASPNETClient.User)
@Code
    ViewData("Title") = "RESTWCFMVC"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<p class="button operation">
    @Html.ActionLink("Create User", "Create")
</p>
<table id="gridview">
    <tr class="title">
        <th>Id
        </th>
        <th>Name
        </th>
        <th>Age
        </th>
        <th>Sex
        </th>
        <th>Comments
        </th>
        <th>&nbsp;</th>
        <th>&nbsp;</th>
    </tr>

    @code
        If Not IsNothing(Model) Then
            For Each userItem In Model
                @<tr>
                    <td>
                        @userItem.Id
                    </td>
                    <td>
                         @userItem.Name
                    </td>
                    <td>
                        @userItem.Age
                    </td>
                    <td>
                        @userItem.Sex
                    </td>
                    <td>
                        @userItem.Comments
                    </td>
                    <td>@Html.ActionLink("Edit", "Edit", New With {.id = userItem.Id}, New With {.classname = "useredit"})</td>
                    <td>@Html.ActionLink("Delete", "Delete", New With {.id = userItem.Id}, New With {.classname = "userdel"})</td>
                </tr>
            Next
        End If
    End Code

</table>
