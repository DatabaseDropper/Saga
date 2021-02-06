using System;

namespace Shared
{
    public class QueueWarehouseItem : QueueItem
    {
        public QueueWarehouseItem(int quantity, Guid shopId) : base(QueueDirection.Warehouse)
        {
            Quantity = quantity;
            ShopOrderId = shopId;
        }

        public int Quantity { get; set; }

        public Guid ShopOrderId { get; set; }
    }
}
