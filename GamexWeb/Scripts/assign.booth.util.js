$(document).ready(function () {
    var booth = boothCount;
    $(document).on('click', '.btn-add', function (e) {

        e.preventDefault();

        var proposedList = $('#booth-list'),
            currentEntry = $(this).parent().parent(),
            newEntry = $(currentEntry.clone());

        newEntry.find('input').val('');
        newEntry.find('input').attr('name', 'Booth[' + booth + '].BoothNumber');
        newEntry.find('input').attr('id', 'Booth[' + booth + '].BoothNumber');
        //        newEntry.find('input').attr('aria-describedby', 'Answers[' + proposedAnswer + ']-error');
        //        newEntry.find('span.field-validation-error').attr('id', 'Answers[' + proposedAnswer + ']-error');

        newEntry.appendTo(proposedList);
        booth++;
        //                proposedList.find('.entry:not(:last) .btn-add')
        newEntry.find('.btn-add').removeClass('btn-add').addClass('btn-remove')
            .removeClass('btn-success').addClass('btn-danger')
            .html('<i class="fas fa-times"></i>');

    }).on('click', '.btn-remove', function (e) {
        $(this).parents('.entry:first').remove();
        var i = 0;
        $('.entry').each(function () {
            $(this).find('input').attr('name', 'Booth[' + i + '].BoothNumber');
            $(this).find('input').attr('id', 'Booth[' + i + '].BoothNumber');
            i++;
        });
        booth--;
        e.preventDefault();
        return false;
    });
});