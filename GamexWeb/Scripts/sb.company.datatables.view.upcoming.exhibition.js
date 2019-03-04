$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[1, "asc"]],
        "ajax": {
            "url": "../LoadUpcomingExhibitionList",
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
                "data": "ExhibitionName",
                "name": "Exhibition",
                "autoWidth": true
            },
            {
                "data": "Time",
                "name": "Time",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="/Company/Exhibition/' +
                        full.ExhibitionId +
                        '"><i class="fas fa-edit"></i>View Detail</a>';
                }
            }
        ]
    });
});