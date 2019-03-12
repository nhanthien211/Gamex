$(document).ready(function () {
    var proposedAnswer = answer;
//    
//    
//    var validator = $("#update-question-form").validate({
//        errorClass: "field-validation-error",
//        errorElement: "div"
//    });


    $(document).on('click', '.btn-add', function (e) {

        e.preventDefault();

        var proposedList = $('#proposed-answer-list'),
            currentEntry = $(this).parent().parent(),
            newEntry = $(currentEntry.clone());

        newEntry.find('input').attr('value', '');
        newEntry.find('input').attr('name', 'Answers[' + proposedAnswer + '].Content');
        newEntry.find('input').attr('id', 'Answers[' + proposedAnswer + '].Content');
//        newEntry.find('input').attr('aria-describedby', 'Answers[' + proposedAnswer + ']-error');
//        newEntry.find('span.field-validation-error').attr('id', 'Answers[' + proposedAnswer + ']-error');

        newEntry.appendTo(proposedList);
        proposedAnswer++;
        //                proposedList.find('.entry:not(:last) .btn-add')
        newEntry.find('.btn-add').removeClass('btn-add').addClass('btn-remove')
            .removeClass('btn-success').addClass('btn-danger')
            .html('<i class="fas fa-times"></i>');

    }).on('click', '.btn-remove', function (e) {
        $(this).parents('.entry:first').remove();
        var i = 0;
        $('.entry').each(function () {
            $(this).find('input').attr('name', 'Answers[' + i + '].content');
            $(this).find('input').attr('id', 'Answers[' + i + '].content');
            i++;
        });
        proposedAnswer--;
        e.preventDefault();
        return false;
    });
});