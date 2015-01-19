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

        //Tutorial page 89, add the GetTotal() method
        public decimal GetTotal()
        {
            ShoppingCartID = GetCartID();
            // Multiply product price by quantity of that product to get
            // the current price for each of those products in the cart.
            // Sum all product price totals to get the cart total.

            decimal? total = decimal.Zero;
            total = (decimal?)(from cartItems in _db.ShoppingCartItems
                               where cartItems.CartID == ShoppingCartID
                               select (int?)cartItems.Quantity *
                               cartItems.Product.UnitPrice).Sum();
            return total ?? decimal.Zero;
        }

        //Tutorial pages 95-99
        //On the ShoppingCart.aspx page, controls have been added for updating the quantity of an item and removing an item. 
        //adding the following code that will make these controls work.

        public ShoppingCartActions GetCart(HttpContext context)
        {
            using  (var cart = new ShoppingCartActions())
            {
                cart.ShoppingCartID = cart.GetCartID();
                return cart;
            }
        } 

        public void UdateShoppingCartDatabase(String cartID, ShoppingCartUpdates[] CartItemUpdates)
        {
            using (var db = new WingTipToys.Models.ProductContext())
            {
                try
                {
                    int CartItemCount = CartItemUpdates.Count();
                    List<CartItem> myCart = GetCartItems();
                    foreach (var cartItem in myCart)
                    {
                        // Iterate through all rows within shopping cart list
                        for (int i = 0; i < CartItemCount; i++)
                        {
                            if (cartItem.Product.ProductID == CartItemUpdates[i].ProductId)
                            {
                                if (CartItemUpdates[i].PurchaseQuantity < 1 ||
                                   CartItemUpdates[i].RemoveItem == true)
                                {
                                    RemoveItem(cartID, cartItem.ProductID);
                                }
                                else
                                {
                                    UpdateItem(cartID, cartItem.ProductID, CartItemUpdates[i].PurchaseQuantity);
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("Error: Unable to Update Cart Database - " + 
                        exp.Message.ToString(), exp);
                }
            }
        }

        public void RemoveIem(string removeCartID, int removeProductID)
        {
            using (var db = new WingTipToys.Models.ProductContext())
            {
                try
                {
                    var myItem = (from c in db.ShoppingCartItems where c.CartID == 
                    removeCartID && c.Product.ProductID == removeProductID select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        // Remove Item.
                        db.ShoppingCartItems.Remove(myItem);
                        _db.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("Error: Unable to Remove Cart Item - " +
                    exp.Message.ToString(), exp);
                }
            }
        }

        public void UpdateItem(string updateCartID, int updateProductID, int quantity)
        {
            using (var db = new WingTipToys.Models.ProductContext())
            {
                try
                {
                    var myItem = (from c in db.ShoppingCartItems
                        where c.CartID == updateCartID && c.Product.ProductID == updateProductID
                        select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        myItem.Quantity = quantity;
                        db.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("Error: Unable to Update Cart Item - " +
                    exp.Message.ToString(), exp);
                }
            }
        }

        public void EmptyCart()
        {
            ShoppingCartID = GetCartID();
            var cartItems = _db.ShoppingCartItems.Where(c => c.CartID == ShoppingCartID);
            foreach (var cartItem in cartItems)
            {
                _db.ShoppingCartItems.Remove(cartItem);
            }
            // Save changes
            _db.SaveChanges();
        }

        public int Getcount()
        {
            ShoppingCartID = GetCartID();

            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _db.ShoppingCartItems 
                          where cartItems.CartID == ShoppingCartID
                          select (int?) cartItems.Quantity).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public struct ShoppingCartUpdates
        {
            public int ProductId;
            public int PurchaseQuantity;
            public int RemoveItem;
        }
    }
}