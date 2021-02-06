using System;

namespace Shop
{
    public class Order
    {
        public Order(int quantity)
        {
            Quantity = quantity;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public int Quantity { get; }

        public bool? Accepted { get; set; }
    }
}