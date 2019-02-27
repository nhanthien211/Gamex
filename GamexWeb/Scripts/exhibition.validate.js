var thumbnailFile = document.getElementById('Logo');
thumbnailFile.value = '';
thumbnailFile.onchange = function (e) {
    $('#fileUploadResult').removeClass();
    var fileExtension = this.value.match(/\.([^\.]+)$/)[1];
    fileExtension = fileExtension.toLowerCase();
    if (fileExtension === 'png' || fileExtension === 'jpg' || fileExtension === 'jpeg') {
        var size = thumbnailFile.files[0].size;
        if (size > (10 * 1024 * 1024)) {
            //bigger than 10MB, too much 
            $(this).next().after().text('Choose File');
            $('#fileUploadResult').addClass('field-validation-error');
            $('#fileUploadResult').html('Uploaded file must be under 10MB');
            $('#Logo-error').html('');
            this.value = '';
            document.getElementById('imagePreview').src = '';
            return;
        }
        $(this).next().after().text($(this).val().split('\\').slice(-1)[0]);
        $('#fileUploadResult').addClass('field-validation-valid');
        $('#fileUploadResult').html('');
        document.getElementById('imagePreview').src = window.URL.createObjectURL(thumbnailFile.files[0]);
    } else {
        $(this).next().after().text('Choose File');
        $('#fileUploadResult').addClass('field-validation-error');
        $('#fileUploadResult').html('Only .jpg .jpeg and .png is supported');
        $('#Logo-error').html('');
        this.value = '';
        document.getElementById('imagePreview').src = '';
    }
};

function clearFileError() {
    $('#fileUploadResult').html('');
}

//Restrict client side input

//set min date
var today = new Date();
var dd = today.getDate();
var mm = today.getMonth() + 1; //January is 0
var yyyy = today.getFullYear();
if (dd < 10) {
    dd = '0' + dd;
}
if (mm < 10) {
    mm = '0' + mm;
}
today = yyyy + '-' + mm + '-' + dd;
document.getElementById("StartDate").setAttribute("min", today);
document.getElementById("EndDate").setAttribute("min", today);

$("#StartDate").change(function () {
    $("#EndDate").val('');
    var startDateString = $("#StartDate").val();
    var startDate = new Date(startDateString.toString());
    var dd = startDate.getDate();
    var mm = startDate.getMonth() + 1; //January is 0
    var yyyy = startDate.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var endDateMin = yyyy + '-' + mm + '-' + dd;
    $("#EndDate").attr('min', endDateMin);
});