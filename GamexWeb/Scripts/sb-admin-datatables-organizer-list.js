$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[0, "asc"]],
        "ajax": {
            "url": "../LoadOrganizerList",
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
                "searchable": true,
                "orderable": false
            },
            {
                "targets": [2],
                "searchable": true,
                "orderable": true   
            },
            {
                "targets": [3],
                "searchable": false,
                "orderable": false
            }
        ],

        "columns": [
            {
                "data": "FullName",
                "name": "Organizer Name",
                "autoWidth": true
            },
            {
                "data": "Email",
                "name": "Email",
                "autoWidth": true
            },
            {
                "data": "Status",
                "name": "Status",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    if (full.Status === "Activated") {
                        return '<form action="/Admin/ActivateOrDeactivateAccount" method="post"> ' +
                            '<input type="hidden" name="userId" value="' + full.Id + '"/>' +
                            '<button type="submit" name="isActivate" value="false" class="btn btn-outline-danger">Deactivate</button></form>';
                    } else {
                        return '<form action="/Admin/ActivateOrDeactivateAccount" method="post"> ' +
                            '<input type="hidden" name="userId" value="' + full.Id + '"/>' +
                            '<button type="submit" name="isActivate" value="true" class="btn btn-outline-success">Activate</button></form>';
                    }
                }
            }
        ]
    });
});