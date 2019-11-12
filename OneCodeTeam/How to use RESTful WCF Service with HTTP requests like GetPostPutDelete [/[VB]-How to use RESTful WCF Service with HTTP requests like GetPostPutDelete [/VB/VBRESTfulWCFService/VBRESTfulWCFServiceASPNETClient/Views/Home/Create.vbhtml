@ModelType VBRESTfulWCFServiceASPNETClient.User

@Code
    ViewData("Title") = "Create"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code

<h2>Create</h2>

<script src="@Url.Content("~/Scripts/jquery.validate.min.js")" type="text/javascript"></script>
<script src="@Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js")" type="text/javascript"></script>

@Using Html.BeginForm()
    @Html.ValidationSummary(True)
    @<fieldset>
        <legend>User</legend>

        <div class="editor-label">
            @Html.LabelFor(Function(model) model.Name)
        </div>
        <div class="editor-field">
            @Html.EditorFor(Function(model) model.Name)
            @Html.ValidationMessageFor(Function(model) model.Name)
        </div>

        <div class="editor-label">
            @Html.LabelFor(Function(model) model.Age)
        </div>
        <div class="editor-field">
            @Html.EditorFor(Function(model) model.Age)
            @Html.ValidationMessageFor(Function(model) model.Age)
        </div>

        <div class="editor-label">
            @Html.LabelFor(Function(model) model.Comments)
        </div>
        <div class="editor-field">
            @Html.EditorFor(Function(model) model.Comments)
            @Html.ValidationMessageFor(Function(model) model.Comments)
        </div>

        <p>
            <input type="submit" value="Create" />
        </p>
    </fieldset>
End Using

<div>
    @Html.ActionLink("Back to List", "Index")
</div>
