using Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Shop
{
    static class Program
    {
        public static Random Random = new Random();

        static void Main()
        {
            var exchangePath = ConfigurationManager.AppSettings.Get("ExchangeUri");
            var dbPath = ConfigurationManager.AppSettings.Get("ShopDatabaseUri");

            var db = new FileDb(dbPath);
            var queue = new QueueManager(exchangePath);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1: Create new Order.");
                Console.WriteLine("2: Handle Queue.");
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.D1)
                {
                    CreateNewOrder(db, queue);
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    HandleQueue(db, queue);
                }
            }
        }

        private static void HandleQueue(FileDb db, QueueManager queue)
        {
            var data = queue.LoadEntries<QueueShopItem>(QueueDirection.Shop);
            UpdateRows(db, data);
            queue.Remove(data.ConvertAll(x => x.Id));
            SendEmailUpdate(data);
        }

        private static void SendEmailUpdate(List<QueueShopItem> data)
        {
            foreach (var entry in data)
            {
                if (entry.Accepted)
                {
                    SendFakturka();
                }
                else
                {
                    SendSorry();
                }
            }
        }

        private static void UpdateRows(FileDb db, List<QueueShopItem> queue_data)
        {
            var orders = db.GetAll<Order>();

            foreach (var order in orders)
            {
                var update_data = queue_data.FirstOrDefault(x => x.ShopOrderId == order.Id);

                if (update_data is not null)
                {
                    order.Accepted = update_data.Accepted;
                }
            }

            db.Overwrite(orders);
        }

        private static void CreateNewOrder(FileDb db, QueueManager queue)
        {
            var quantity = Random.Next(1, 50);
            var order = new Order(quantity);
            db.Create(order);
            queue.Save(new QueueWarehouseItem(quantity, order.Id));
            Logger.Log("Order has been created", ConsoleColor.Green);
        }

        private static void SendFakturka()
        {
            Logger.Log("Sending Invoice", ConsoleColor.Green);
        }

        private static void SendSorry()
        {
            Logger.Log("Sending Sorry", ConsoleColor.Red);
        }
    }
}
