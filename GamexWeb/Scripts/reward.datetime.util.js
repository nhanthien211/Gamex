$(function () {
    $(function () {
        $('#startDateTimePicker').datetimepicker({
            icons: {
                time: "far fa-clock",
                date: "fas fa-calendar",
                up: "fas fa-arrow-up",
                down: "fas fa-arrow-down"
            }
        });
        $('#endDateTimePicker').datetimepicker({
            useCurrent: false,
            icons: {
                time: "far fa-clock",
                date: "fas fa-calendar",
                up: "fas fa-arrow-up",
                down: "fas fa-arrow-down"
            }
        });
        $("#startDateTimePicker").on("change.datetimepicker", function (e) {
            $('#endDateTimePicker').datetimepicker('minDate', e.date);
        });
        $("#endDateTimePicker").on("change.datetimepicker", function (e) {
            $('#startDateTimePicker').datetimepicker('maxDate', e.date);
        });
    });
});