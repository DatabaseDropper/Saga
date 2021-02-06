using System;

namespace Shared
{
    public class QueueShopItem : QueueItem
    {
        public QueueShopItem(bool accepted, Guid shopOrderId) : base(QueueDirection.Shop)
        {
            Accepted = accepted;
            ShopOrderId = shopOrderId;
        }

        public Guid ShopOrderId { get; set; }

        public bool Accepted { get; set; }
    }
}
