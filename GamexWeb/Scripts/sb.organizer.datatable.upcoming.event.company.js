$(document).ready(function () {
    var table = $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[0, "asc"]],
        "ajax": {
            "url": "/Organizer/LoadAttendedCompanyList/" + exhibitionId,
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
                "searchable": false,
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
                "render": function (data, type, full, meta) {
                    return '<a href="/Organizer/Exhibition/Upcoming/'
                        + full.ExhibitionId + 'Company' + full.CompanyId +
                        '"><i class="fas fa-edit"></i>View Detail</a>';
                }
            }
        ]
    });
});