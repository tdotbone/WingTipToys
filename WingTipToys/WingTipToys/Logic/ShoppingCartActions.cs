using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingTipToys.Models;

namespace WingTipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartID { get; set; }
        private ProductContext _db = new ProductContext();
        public const string CartSessionKey = "CartID";

        public void AddToCart(int id)
        {
            // Retrieve the product from the database.
            ShoppingCartID = GetCartID();

            var cartItem = _db.ShoppingCartItems.SingleOrDefault(c => c.CartID == ShoppingCartID && c.ProductID == id);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new CartItem
                {
                    ItemID = Guid.NewGuid().ToString(),
                    ProductID = id,
                    CartID = ShoppingCartID,
                    Product = _db.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                _db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart then add 1 to the quantity.
                cartItem.Quantity++;
            }
            _db.SaveChanges();
        }

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        public string GetCartID()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.
                    Guid tempCartID = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartID.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartID = GetCartID();

            return _db.ShoppingCartItems.Where(c => c.CartID == ShoppingCartID).ToList();
        }
    }
}