/* === GLOBAL CART FUNCTIONS (Không đổi) === */

function getCart() {
  return JSON.parse(localStorage.getItem('cart')) || [];
}

function saveCart(cart) {
  localStorage.setItem('cart', JSON.stringify(cart));
}

function addToCart(id, name, price, image) {
  let cart = getCart();
  const productIndex = cart.findIndex(item => item.id === id);
  
  // Lấy số lượng từ input (nếu có, chỉ dùng cho trang chi tiết)
  const quantityInput = document.getElementById('product-quantity');
  const quantityToAdd = quantityInput ? parseInt(quantityInput.value) : 1;
  
  if (productIndex > -1) {
    // Sản phẩm đã có, tăng số lượng
    cart[productIndex].quantity += quantityToAdd;
  } else {
    // Sản phẩm mới, thêm vào giỏ
    cart.push({ id, name, price, image, quantity: quantityToAdd });
  }

  saveCart(cart);
  updateCartCount();
  showToast(`${name} đã thêm ${quantityToAdd} vào giỏ!`);
}

function removeFromCart(id) {
  let cart = getCart();
  cart = cart.filter(item => item.id !== id);
  saveCart(cart);
  displayCart(); 
  updateCartCount(); 
}

function updateCartItemQuantity(id, quantity) {
  let cart = getCart();
  const productIndex = cart.findIndex(item => item.id === id);

  if (productIndex > -1) {
    const newQuantity = parseInt(quantity);
    if (newQuantity > 0) {
      cart[productIndex].quantity = newQuantity;
    } else {
      cart = cart.filter(item => item.id !== id);
    }
  }

  saveCart(cart);
  displayCart(); 
  updateCartCount(); 
}

function updateCartCount() {
  const cart = getCart();
  const totalQuantity = cart.reduce((sum, item) => sum + item.quantity, 0);
  
  const cartCountElement = document.getElementById('cart-count');
  if (cartCountElement) {
    cartCountElement.textContent = totalQuantity;
    cartCountElement.style.display = totalQuantity > 0 ? 'block' : 'none';
  }
}

function calculateCartTotals() {
    const cart = getCart();
    let subtotal = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const tax = subtotal * 0.1; // 10% VAT
    const total = subtotal + tax;
    return { subtotal, tax, total };
}

function displayCart() {
  // ... (Code displayCart giữ nguyên)
  const cartItemsContainer = document.getElementById('cart-items-container');
  const cartTotalContainer = document.getElementById('cart-total-container');
  const cartEmptyMessage = document.getElementById('cart-empty-msg');
  
  if (!cartItemsContainer) return;

  const cart = getCart();
  const totals = calculateCartTotals();

  if (cart.length === 0) {
    cartItemsContainer.innerHTML = '';
    if (cartTotalContainer) cartTotalContainer.style.display = 'none';
    if (cartEmptyMessage) cartEmptyMessage.style.display = 'block';
  } else {
    if (cartTotalContainer) cartTotalContainer.style.display = 'flex';
    if (cartEmptyMessage) cartEmptyMessage.style.display = 'none';

    let itemsHtml = '';

    cart.forEach(item => {
      const itemTotal = item.price * item.quantity;
      
      itemsHtml += `
        <tr>
          <td>
            <div class="d-flex align-items-center">
              <img src="${item.image}" alt="${item.name}" class="me-3">
              <span>${item.name}</span>
            </div>
          </td>
          <td>${item.price} VNĐ</td>
          <td>
            <input 
              type="number" 
              class="form-control quantity-input" 
              value="${item.quantity}" 
              min="1"
              onchange="updateCartItemQuantity('${item.id}', this.value)"
            >
          </td>
          <td>${itemTotal} VNĐ</td>
          <td>
            <button class="btn btn-sm btn-outline-danger" onclick="removeFromCart('${item.id}')">
              <i class="bi bi-trash"></i>
            </button>
          </td>
        </tr>
      `;
    });

    cartItemsContainer.innerHTML = itemsHtml;

    document.getElementById('subtotal').textContent = `${totals.subtotal}`;
    document.getElementById('tax').textContent = `${totals.tax}`;
    document.getElementById('total').textContent = `${totals.total}`;
  }
}

function displayCheckoutTotal() {
    // ... (Code displayCheckoutTotal giữ nguyên)
    const checkoutTotalContainer = document.getElementById('checkout-total-summary');
    if (!checkoutTotalContainer) return;

    const cart = getCart();
    if (cart.length === 0) {
        checkoutTotalContainer.innerHTML = '<p class="text-danger">Giỏ hàng trống. Vui lòng quay lại trang Sản phẩm.</p>';
        const checkoutForm = document.getElementById('checkout-form');
        if(checkoutForm) checkoutForm.style.display = 'none';
        return;
    }

    const totals = calculateCartTotals();
    
    let itemsHtml = cart.map(item => `
        <li class="list-group-item d-flex justify-content-between lh-sm">
            <div>
                <h6 class="my-0" name="TENSP">${item.name}</h6>
                <hr/>
                Mã sản phẩm: <span class="my-0 text-muted" name="MASP">${item.id}</span>
                <br/>
                Đơn giá: <span class="my-0 text-muted" name="GIA">${item.price} VNĐ</span>
                <br/>
                Số lượng: <span class="my-0" name="SOLUONG">${item.quantity}</span>

                <input type="hidden" name="MASP" value="${item.id}" />
                <input type="hidden" name="TENSP" value="${item.name}" />
                <input type="hidden" name="GIA" value="${item.price}" />
                <input type="hidden" name="SOLUONG" value="${item.quantity}" />
            </div>
            <span class="text-muted" name="TAMTINH">${(item.price * item.quantity)} VNĐ</span>

            <input type="hidden" name="TAMTINH" value="${(item.price * item.quantity)}" />
             
        </li>
    `).join('');
    //Thuộc tính THANHTIEN vứt qua HOADON có vẻ hợp lý hơn
    checkoutTotalContainer.innerHTML = `
        <h4 class="d-flex justify-content-between align-items-center mb-3">
            <span class="text-primary">Đơn hàng</span>
            <span class="badge bg-primary rounded-pill">${cart.length}</span>
        </h4>
        <ul class="list-group mb-3">
            ${itemsHtml}
            <li class="list-group-item d-flex justify-content-between">
                <span>Tạm tính (USD)</span>
                <strong>${totals.subtotal} VNĐ</strong>
            </li>
            <li class="list-group-item d-flex justify-content-between">
                <span>Thuế (10% VAT)</span>
                <strong>${totals.tax} VNĐ</strong>
            </li>
            <li class="list-group-item d-flex justify-content-between fw-bold bg-light">
                <span>Tổng cộng (USD)</span>
                <strong class="fs-5 text-dark" name="THANHTIEN">${totals.total} VNĐ</strong>

                <input type="hidden" name="THANHTIEN" value="${totals.total}"/>
            </li>
        </ul>
    `;
}

function processCheckout(event) {
    event.preventDefault();
    const cart = getCart();

    if (cart.length === 0) {
        alert('Giỏ hàng trống. Không thể thanh toán.');
        return;
    }
    
    const name = document.getElementById('firstName').value + ' ' + document.getElementById('lastName').value;
    
    alert(`Cảm ơn ${name}! Đơn hàng của bạn trị giá $${calculateCartTotals().total.toFixed(2)} đã được đặt thành công.`);
    
    localStorage.removeItem('cart');
    updateCartCount();
    
    window.location.href = goBackHome;
}

function applyProductFilters() {
  // ... (Code applyProductFilters giữ nguyên)
  const searchInput = document.getElementById('filterSearch');
  const tagInput = document.getElementById('filterTag');
  const priceInput = document.getElementById('filterPrice'); 
  
  if (!searchInput || !tagInput || !priceInput) return;

  const searchTerm = searchInput.value.toLowerCase();
  const selectedTag = tagInput.value;
  const selectedPrice = priceInput.value; 
  const productList = document.querySelectorAll('.product-col');

  let minPrice = 0;
  let maxPrice = Infinity;
  if (selectedPrice !== 'all') {
      const priceParts = selectedPrice.split('-');
      if (priceParts.length === 2) {
          minPrice = parseInt(priceParts[0]) || 0;
          maxPrice = parseInt(priceParts[1]) || Infinity;
      } else if (selectedPrice === '1000plus') {
          minPrice = 1000;
          maxPrice = Infinity;
      }
  }

  productList.forEach(product => {
    const name = product.dataset.name.toLowerCase();
    const tag = product.dataset.tag;
    const price = parseInt(product.dataset.price); 

    const nameMatch = name.includes(searchTerm);
    const tagMatch = (selectedTag === 'all' || selectedTag === tag);
    const priceMatch = (price >= minPrice && price <= maxPrice);

    if (nameMatch && tagMatch && priceMatch) {
      product.style.display = 'block';
    } else {
      product.style.display = 'none';
    }
  });
}


/* === EVENT LISTENERS (UPDATED) === */

document.addEventListener('DOMContentLoaded', () => {
  
  updateCartCount();

  displayCart();
  
  displayCheckoutTotal();
  
  //// NEW: Hiển thị chi tiết sản phẩm
  //displayProductDetail();

  const checkoutForm = document.getElementById('checkout-form');
  if (checkoutForm) {
      checkoutForm.addEventListener('submit', processCheckout);
  }

  const searchInput = document.getElementById('filterSearch');
  const tagInput = document.getElementById('filterTag');
  const priceInput = document.getElementById('filterPrice');
  
  if (searchInput && tagInput && priceInput) {
    searchInput.addEventListener('keyup', applyProductFilters);
    tagInput.addEventListener('change', applyProductFilters);
    priceInput.addEventListener('change', applyProductFilters);
  }

});