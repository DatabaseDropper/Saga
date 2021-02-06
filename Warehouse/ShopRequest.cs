using System;

namespace Warehouse
{
    public class ShopRequest
    {
        public ShopRequest(Guid shopExternalId, int quantity)
        {
            ShopExternalId = shopExternalId;
            Quantity = quantity;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ShopExternalId { get; set; }

        public int Quantity { get; set; }
    }
}
