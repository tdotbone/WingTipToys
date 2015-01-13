using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WingTipToys.Models;
using System.Web.ModelBinding;

namespace WingTipToys
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public IQueryable<Product> GetProduct([QueryString("productID")] int? productID)
        {
            var _db = new WingTipToys.Models.ProductContext();
            IQueryable<Product> query = _db.Products;
            if (productID.HasValue && productID > 0)
            {
                query = query.Where(p => p.ProductID == productID);
            }
            else
            {
                query = null;
            }
            return query;
        }
    }
}

//This code checks for a "productID" query-string value. If a valid query-string value is found, the matching product is displayed. 
//If no query-string is found, or the query-string value is not valid, no product is displayed on the ProductDetails.aspx page.