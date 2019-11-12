function drawTable(data) {

    if (!data || !data.value || data.value.length < 1) {
        return;
    }


    var table = $("#clientInfos");
    table.find("tr").remove();

    var headers = GetHeaders(data);

    var headertr = $("<tr></tr>");
    table.append(headertr);

    // add headers to table
    $.each(headers, function (i, header) {
        headertr.append("<th>" + header + "</th>");
    });

    // add row content to table
    $.each(data.value, function (i, row) {
        var rowtr = $("<tr></tr>");
        for (var j = 0; j < headers.length; j++) {
            rowtr.append("<td>" + row[headers[j]] + "</td>");
        }

        table.append(rowtr);
    });
}

function GetHeaders(obj) {
    var cols = new Array();
    for (var row in obj.value) {
        for (var key in obj.value[row]) {
            if (cols.indexOf(key) < 0) {
                cols.push(key);
            }
        }
    }
    return cols;
}