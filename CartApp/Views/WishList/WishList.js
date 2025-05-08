function addToWishList(productId) {
    $.ajax({
        url: '/WishList/AddToWishList',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                alert('Product added to wishlist!');
            }
        }
    });
}

function removeFromWishList(productId) {
    $.ajax({
        url: '/WishList/RemoveFromWishList',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                location.reload();
            }
        }
    });
}