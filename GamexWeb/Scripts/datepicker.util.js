$(function () {
    $('#startDateTimePicker').datetimepicker();
    $('#endDateTimePicker').datetimepicker();
    $("#startDateTimePicker").on("dp.change", function (e) {
        $('#endDateTimePicker').data("DateTimePicker").minDate(e.date);
    });
    $("#endDateTimePicker").on("dp.change", function (e) {
        $('#startDateTimePicker').data("DateTimePicker").maxDate(e.date);
    });
});