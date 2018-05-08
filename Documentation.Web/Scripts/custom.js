$('#upload_btn').on('click', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        $('input[type=\"file\"]').click();
        return false;
});

$(function () {
    $('.datepicker').datepicker({ maxDate: 0, dateFormat: 'm/d/yy' }); //Initialise any date pickers
});