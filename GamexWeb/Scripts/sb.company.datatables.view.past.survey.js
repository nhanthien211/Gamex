$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        "ajax": {
            "url": "/Company/Exhibition/Past/" + id + "/Survey/Manage",
            "type": "POST",
            "datatype": "json"
        },

        "columnDefs": [
            {
                "targets": [0],
                "searchable": true,
                "orderable": false
            },
            {
                "targets": [1],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [2],
                "searchable": false,
                "orderable": false
            }
        ],

        "columns": [
            {
                "data": "SurveyTitle",
                "name": "Survey Title",
                "autoWidth": true
            },
            {
                "data": "ResponseCount",
                "name": "Number of response",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    if (full.ResponseCount > 0) {
                        return '<a class="downloadResponse" href="/Company/ExportSurveyResponse/' +
                            full.SurveyId +
                            '"><i class="fas fa-file-download"></i>  Download Response</a>';
                    }
                    return '';
                }
            }
        ]
    });
});

$(document).on("click", "a.downloadResponse", function () {
    $.fileDownload($(this).prop('href'))
        .fail(function () {
                $('#failModal').modal();
            }
    );
    return false; //this is critical to stop the click event which will trigger a normal file download
});