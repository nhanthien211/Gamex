$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[0, "asc"]],
        "ajax": {
            "url": "../LoadCompanyRequest",
            "type": "POST",
            "datatype": "json"
        },

        "columnDefs": [
            {
                "targets": [0],
                "searchable": true,
                "orderable": true
            },
            {
                "targets": [1],
                "searchable": true,
                "orderable": false
            },
            {
                "targets": [2],
                "searchable": true,
                "orderable": false
            },
            {
                "targets": [3],
                "orderable": false
            }
        ],

        "columns": [
            {
                "data": "CompanyName",
                "name": "Company Name",
                "autoWidth": true
            },
            {
                "data": "TaxNumber",
                "name": "Tax Number",
                "autoWidth": true
            },
            {
                "data": "Email",
                "name": "Email",
                "autoWidth": true
            },
            {
                "render": function(data, type, full, meta) {
                    return '<form action="/Admin/ApproveOrDeny" method="post"> ' +
                        '<input type="hidden" name="companyId" value="' + full.CompanyId + '"/>' +
                        '<input type="hidden" name="email" value="' + full.Email + '"/>' +
                        '<button type="submit" name="isApproved" value="true" class="btn btn-outline-success" style="margin-right: 10px;">Approve</button>' +
                        '<button type="submit" name="isApproved" value="false" class="btn btn-outline-danger">Reject</button>' + 
                        '</form>';
                }
            }
        ]
    });
});