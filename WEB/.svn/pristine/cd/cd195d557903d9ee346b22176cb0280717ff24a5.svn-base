
$(document).ready(function () {
    //Add button click event
    $('#addEmployee').click(function () {
        //validation and add order items

            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.pc', $newRow).val($('#UseID').val());

            //Replace add button with remove button
            $('#addEmployee', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

            //remove id attribute from new clone row
            $('#UseID,#ActOut,#ActIn,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();
            //append clone row
            $('#orderdetailsItems').append($newRow);

            //clear select data
            $('#UseID').val('');
            $('#ActOut,#ActIn').val('');
    })

    //remove button click event
    $('#orderdetailsItems').on('click', '.remove', function () {
        $(this).parents('tr').remove();
    });

});
