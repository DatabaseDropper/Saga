using Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace Warehouse
{
    static class Program
    {
        public static Random Random = new Random();

        static void Main()
        {
            var path = ConfigurationManager.AppSettings.Get("ExchangeUri");
            var dbPath = ConfigurationManager.AppSettings.Get("WarehouseDatabaseUri");
            var queue = new QueueManager(path);
            var db = new FileDb(dbPath);

            while (true)
            {
                ProcessQueue(db, queue);
                Thread.Sleep(10_000);
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void ProcessQueue(FileDb db, QueueManager manager)
        {
            var data = manager.LoadEntries<QueueWarehouseItem>(QueueDirection.Warehouse);
            var queue_to_remove_list = new List<Guid>();
            var queue_to_add_items = new List<QueueShopItem>();

            foreach (var entry in data)
            {
                var accepted = entry.Quantity <= Random.Next(1, 50);

                if (accepted)
                {
                    Logger.Log("Order has been accepted", ConsoleColor.Green);
                }
                else
                {
                    Logger.Log("Order has been denied", ConsoleColor.Red);
                }

                queue_to_add_items.Add(new QueueShopItem(accepted, entry.ShopOrderId));
                queue_to_remove_list.Add(entry.Id);
                db.Create(new ShopRequest(entry.ShopOrderId, entry.Quantity));
            }

            manager.SaveMany(queue_to_add_items);
            manager.Remove(queue_to_remove_list);
        }
    }
}
