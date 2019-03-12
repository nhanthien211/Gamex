$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[1, "asc"]],
        "ajax": {
            "url": "/Company/Survey/" + surveyId + "/Question/Upcoming",
            "type": "POST",
            "datatype": "json"
        },

        "columnDefs": [
            {
                "targets": [0],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [1],
                "searchable": true,
                "orderable": true
            },
            {
                "targets": [2],
                "searchable": false,
                "orderable": false
            }
        ],
        "columns": [
            {
                "data": "Question",
                "name": "Question",
                "autoWidth": true
            },
            {
                "data": "QuestionType",
                "name": "Question Type",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="/Company/Exhibition/' +
                        exhibitionId +
                        '/Survey/' + surveyId + '/Upcoming/Question/' +
                        full.QuestionId + '/Type/' + full.Type + '"><i class="fas fa-edit"></i>View Detail</a>';
                }
            }
        ]
    });
});