$(document).ready(function() {
    // Rating selection
    $('.rating-input i').click(function() {
        const rating = $(this).data('rating');
        $('.rating-input i').removeClass('fas').addClass('far');
        $(this).prevAll().addBack().removeClass('far').addClass('fas');
        $('#selectedRating').val(rating);
    });

    // Submit review
    $('#reviewForm').submit(function(e) {
        e.preventDefault();
        const productId = $('input[name="productId"]').val();
        const rating = $('#selectedRating').val();
        const comment = $('textarea[name="comment"]').val();

        $.ajax({
            url: '/Product/AddReview',
            type: 'POST',
            data: {
                productId: productId,
                rating: rating,
                comment: comment
            },
            success: function(response) {
                if (response.success) {
                    location.reload();
                }
            }
        });
    });
});