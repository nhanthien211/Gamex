$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[0, "asc"]],
        "ajax": {
            "url": "../LoadCompanyList",
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
                "render": function (data, type, full, meta) {
                    return '<a href="/Admin/Manage/Company/View/' +
                        full.CompanyId +
                        '"><i class="fa fa-edit"></i>View Detail & Edit</a>';
                }
            }
        ]
    });
});