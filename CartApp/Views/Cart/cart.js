function addToCart(productId) {
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                alert('Product added to cart!');
                updateCartCount(response.cartItemCount);
            }
        }
    });
}

function updateCartCount(count) {
    $('#cartCount').text(count);
}

function updateQuantity(productId, quantity) {
    $.ajax({
        url: '/Cart/UpdateQuantity',
        type: 'POST',
        data: { productId: productId, quantity: quantity },
        success: function (response) {
            if (response.success) {
                location.reload();
            }
        }
    });
}

function removeItem(productId) {
    $.ajax({
        url: '/Cart/RemoveItem',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                location.reload();
            }
        }
    });
}