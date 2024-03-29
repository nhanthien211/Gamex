﻿$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        
        "ajax": {
            "url": "/Company/Exhibition/Upcoming/" + id + "/Survey/Manage",
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
            }
        ],

        "columns": [
            {
                "data": "SurveyTitle",
                "name": "Survey Title",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="/Company/Exhibition/Upcoming/' +
                        full.ExhibitionId +
                        '/Survey/' + full.SurveyId +'"><i class="fas fa-edit"></i>View Detail</a>';
                }
            }
        ]
    });
});
//Company / Exhibition / { exhibitionId } / Survey / { id }