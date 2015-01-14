﻿using System.ComponentModel.DataAnnotations;


namespace WingTipToys.Models
{
    public class CartItem
    {
        [Key]
        public string ItemID { get; set; }
        public string CartID { get; set; }
        public int Quantity { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
    }
}