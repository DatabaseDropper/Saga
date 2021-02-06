using JsonSubTypes;
using Newtonsoft.Json;
using System;

namespace Shared
{
    [JsonConverter(typeof(JsonSubtypes), nameof(QueueItem.Direction))]
    [JsonSubtypes.KnownSubType(typeof(QueueShopItem),  QueueDirection.Shop)]
    [JsonSubtypes.KnownSubType(typeof(QueueWarehouseItem), QueueDirection.Warehouse)]
    public abstract class QueueItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public QueueDirection Direction { get; set; }

        public QueueItem(QueueDirection direction)
        {
            Direction = direction;
        }
    }
}
