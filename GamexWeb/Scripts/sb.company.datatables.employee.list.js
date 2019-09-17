$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[0, "asc"]],
        "ajax": {
            "url": "../LoadEmployeeRequestList",
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
                "data": "FullName",
                "name": "Employee Name",
                "autoWidth": true
            },
            {
                "data": "Email",
                "name": "Email",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    return '<form action="/Company/ApproveOrDeny" method="post"> ' +
                        '<input type="hidden" name="UserId" value="' + full.UserId + '"/>' +
                        '<button type="submit" name="isApproved" value="true" class="btn btn-outline-success" style="margin-right: 10px;">Approve</button>' +
                        '<button type="submit" name="isApproved" value="false" class="btn btn-outline-danger">Reject</button>' +
                        '</form>';
                }
            }
        ]
    });
});