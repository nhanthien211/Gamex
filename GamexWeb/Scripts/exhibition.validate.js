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
        this.value = '';
        document.getElementById('imagePreview').src = '';
    }
};

function clearFileError() {
    $('#fileUploadResult').html('');
}