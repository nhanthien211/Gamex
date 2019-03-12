$(document).ready(function () {
    var proposedAnswer = 1;
    var currentEntry;
    $('#QuestionType').on('change',
        function () {
            var questionType = ($('#QuestionType').val());
            var url;
            switch (questionType) {
                case "1":
                    url = "/Company/TextQuestion";
                    break;
                case "2":
                case "3":
                    url = "/Company/MultipleChoice";
                    break;
                default:
                    $('#partial-view').html('');
                    return;
            }
            $.ajax({
                url: url,
                cache: false,
                type: "POST",
                success: function (data) {
                    $('#partial-view').html(data);
                    /* little fade in effect */
                    $('#partial-view').fadeIn("Slow");
                    $('[name^="Answers"]').each(function () {
                        $(this).rules('add',
                            {
                                required: true,
                                maxlength: 100,
                                messages: {
                                    required: "Enter answer",
                                    maxlength: "Answer cannot exceed 100 characters"
                                }
                            });
                    });
                },
                error: function (response) {
                    alert("error : " + response);
                }
            });
        });
    var validator = $("#create-question-form").validate({
        errorClass: "field-validation-error",
        errorElement: "span",
        rules: {
            QuestionTitle: {
                required: true,
                maxlength: 1000
            }
        },
        message: {
            QuestionTitle: {
                required: "Please enter your question",
                maxlength: "Question cannot exceed 1000 characters"
            }
        }
    });

    $(document).on('click', '.btn-add', function (e) {
        validator.resetForm();
        validator.reset();
        e.preventDefault();

        var proposedList = $('#proposed-answer-list'),
            currentEntry = $(this).parent().parent(),
            newEntry = $(currentEntry.clone());

        newEntry.find('input').val('');
        newEntry.find('input').attr('name', 'Answers[' + proposedAnswer + ']');
        newEntry.find('input').attr('id', 'Answers[' + proposedAnswer + ']');
        newEntry.find('input').attr('aria-describedby', 'Answers[' + proposedAnswer + ']-error');
        newEntry.find('span.field-validation-error').attr('id', 'Answers[' + proposedAnswer + ']-error');

        newEntry.appendTo(proposedList);

        proposedAnswer++;
        //                proposedList.find('.entry:not(:last) .btn-add')
        newEntry.find('.btn-add').removeClass('btn-add').addClass('btn-remove')
            .removeClass('btn-success').addClass('btn-danger')
            .html('<i class="fas fa-times"></i>');

        $('[name^="Answers"]').each(function () {
            $(this).rules('add',
                {
                    required: true,
                    maxlength: 100,
                    messages: {
                        required: "Enter answer",
                        maxlength: "Answer cannot exceed 100 characters"
                    }
                });
        });
    }).on('click', '.btn-remove', function (e) {
        $(this).parents('.entry:first').remove();
        var i = 0;
        $('.entry').each(function () {
            $(this).find('input').attr('name', 'Answers[' + i + ']');
            $(this).find('input').attr('id', 'Answers[' + i + ']');
            $(this).find('input').attr('aria-describedby', 'Answers[' + i + ']-error');
            $(this).find('span.field-validation-error').attr('id', 'Answers[' + i + ']-error');
            i++;
        });
        proposedAnswer--;
        e.preventDefault();
        return false;
    });
});