using System;
using System.Diagnostics;
using System.Linq;
using Raven.Client.Documents;

namespace raven_bootcamp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var documentStore = new DocumentStore
            {
                Urls = new[] { "http://localhost:8080" },
                Database = "Northwind"
            };

            documentStore.Initialize();

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var p = session.Load<Product>("products/1-A");
                var order = session
                    .Include<Order>(x => x.Company)
                    .Include(x => x.Employee)
                    .Include(x => x.Lines.Select(l => l.Product))
                    .Load("orders/830-A");
                var companyForOrder = session.Load<Company>(order.Company);
                System.Console.WriteLine(order.Freight);
                System.Console.WriteLine(order.Company);
                System.Console.WriteLine(companyForOrder.Name);
            }
        }
    }

}
