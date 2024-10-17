﻿using System.ComponentModel.DataAnnotations;

namespace ShoppingDemo.Models
{
    public class ProductQuantityModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu không được bỏ trống số lượng")]

        public int Quantity { get; set; }

        public long ProductId{ get; set; }
        public DateTime DateCreated{ get; set; }

    }
}