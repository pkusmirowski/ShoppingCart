namespace Shop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class Order : IOrder
    {
        public Order(int id, float price, int quantity)
        {
            this.Id = id;
            this.Quantity = quantity;
            this.Price = price;
        }

        public int Id { get; set; }

        public int Quantity { get; set; }

        public float Price { get; set; }
    }
}