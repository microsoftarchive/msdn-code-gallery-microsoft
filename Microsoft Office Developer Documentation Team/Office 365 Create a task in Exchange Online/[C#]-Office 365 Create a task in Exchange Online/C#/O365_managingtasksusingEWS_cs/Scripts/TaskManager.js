// Create Task model from UI to create and get the tasks
$(document).ready(function () {

    $("#btnCreate").click(function () {
        var user = {
            TaskCreatorEmailID: $("#txtCreatorID").val(),
            Password: $("#txtPassword").val()
        }

        var task = {
            TaskTitle: $("#txtTitle").val(),
            TaskMessage: $("#txtMessage").val(),
            TaskStartDate: new Date()
        }

        var UserTask = {
            User: user,
            NewTask: task
        }

        $.ajax({
            url: "/Home/Create",
            type: 'POST',
            dataType: "html",
            data: JSON.stringify({ model: UserTask }),
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                $("#divTasks").html(response);
            },

            error: function (xhr) {
                alert("Problem to connect the server.");
            }
        });
    });
});