﻿using static System.Console;
using System.Diagnostics;
using System.Linq;
using Raven.Client.Documents;
using NorthwindModels;
using System;
using System.Collections.Generic;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Operations;

namespace raven_bootcamp
{
    using static Console;
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Choose unit: ");

            int unitNumber;
            if (!int.TryParse(ReadLine(), out unitNumber))
            {
                WriteLine("Unit is invalid.");
            }

            if (unitNumber == 0)
            {
                throw new ArgumentException($"Unit {unitNumber} not valid");
            }
            RunUnit(unitNumber);

            WriteLine("Goodbye!");
        }

        private static void RunUnit(int unitNumber)
        {
            switch (unitNumber)
            {
                case 1:
                    RunUnit1();
                    break;
                case 2:
                    RunUnit2();
                    break;
                case 3:
                    RunUnit3();
                    break;
            }
        }
        private static void RunUnit1()
        {
            // LOAD documents and related documents
            WriteLine("Please, enter an order # (e.g. 7. 0 to exit): ");

            int orderNumber;
            if (!int.TryParse(ReadLine(), out orderNumber))
            {
                WriteLine("Order # is invalid.");
            }

            if (orderNumber == 0)
            {
                throw new ArgumentException($"OrderNumber {orderNumber} not valid");
            }

            PrintOrder(orderNumber);

            // QUERY and RQL
            WriteLine("Now, enter a companyId # (e.g. 39. 0 to exit) to find orders by company id: ");

            int companyId;
            if (!int.TryParse(ReadLine(), out companyId))
            {
                WriteLine("Company # is invalid.");
            }

            if (companyId == 0)
            {
                throw new ArgumentException($"Company Id {companyId} not valid");
            }

            QueryOrdersForCompanyId(companyId);

            // C(r)UD
            // SaveChanges should be called only once per session.
            WriteLine("C(r)UD example");
            Category category;
            string id;
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                // Create
                category = new Category
                {
                    Description = "Descrizione Categoria Fede",
                    Name = "Categoria Fede"
                };
                session.Store(category);
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                // Update
                id = category.Id;
                var savedCategory = session.Load<Category>(id);
                savedCategory.Name = savedCategory.Name + " UPDATED";
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                // Delete
                session.Delete(id);
                session.SaveChanges();
            }
        }

        private static void PrintOrder(int orderNumber)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var order = session
                    .Include<Order>(o => o.Company)
                    .Include(o => o.Employee)
                    .Include(o => o.Lines.Select(l => l.Product))
                    .Load($"orders/{orderNumber}-A");

                if (order == null)
                {
                    WriteLine($"Order #{orderNumber} not found.");
                    return;
                }

                WriteLine($"Order #{orderNumber}");

                var c = session.Load<Company>(order.Company);
                WriteLine($"Company : {c.Id} - {c.Name}");

                var e = session.Load<Employee>(order.Employee);
                WriteLine($"Employee: {e.Id} - {e.LastName}, {e.FirstName}");

                foreach (var orderLine in order.Lines)
                {
                    var p = session.Load<Product>(orderLine.Product);
                    WriteLine($"   - {orderLine.ProductName}," +
                              $" {orderLine.Quantity} x {p.QuantityPerUnit}");
                }
            }
        }

        private static void QueryOrdersForCompanyId(int companyId)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {

                // var orders = (from order in session.Query<Order>()
                //             .Include(o => o.Company)
                //               where order.Company == $"companies/{companyId}-A"
                //               select order
                //               ).ToList();
                // RQL version
                var orders = session.Advanced.RawQuery<Order>(
                    "from Orders " + // Occhio allo spazio!
                    $"where Company=='companies/{companyId}-A'" + // Occhio al case! C maiuscolo
                    "include Company"
                );

                var company = session.Load<Company>($"companies/{companyId}-A");

                if (company == null)
                {
                    WriteLine($"Company #{companyId} not found.");
                    return;
                }

                WriteLine($"Company #{companyId}");
                WriteLine($"Orders for company {company.Name}");

                foreach (var order in orders)
                {
                    WriteLine($"{order.Id} - {order.OrderedAt}");
                }
            }
        }
        private static void RunUnit2()
        {
            CreateStaticIndex();
            SearchTerm();
            MapAndReduce();
            ServerSideProjections();
            CustomizeForStaleIndexes();
        }
        private static void CreateStaticIndex()
        {
            Console.WriteLine("Create Static Index");
            // Create index programmatically and use static index.
            var store = DocumentStoreHolder.Store;
            new Employees_ByFirstAndLastName().Execute(store);
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var results = session
                    .Query<Employee, Employees_ByFirstAndLastName>()
                    .Where(x => x.FirstName == "Robert")
                    .ToList();

                foreach (var employee in results)
                {
                    Console.WriteLine($"{employee.LastName}, {employee.FirstName}");
                }
            }
        }
        private static void SearchTerm()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                Console.Write("\nSearch terms (e.g. Peter*): ");
                var searchTerms = Console.ReadLine();

                foreach (var result in Search(session, searchTerms))
                {
                    Console.WriteLine($"{result.SourceId}\t{result.Type}\t{result.Name}");
                }
            }
        }
        public static IEnumerable<People_Search.Result> Search(IDocumentSession session, string searchTerms)
        {
            var results = session.Query<People_Search.Result, People_Search>()
                .Search(
                    r => r.Name,
                    searchTerms
                )
                .ProjectInto<People_Search.Result>()
                .ToList();

            return results;
        }
        private static void MapAndReduce()
        {

            Console.WriteLine("\nMap And Reduce: ");
            // RQL version:
            // from index 'Products/ByCategory'
            // include Category
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var results = session
                    .Query<MapAndReduce_Products_ByCategory.Result, MapAndReduce_Products_ByCategory>()
                    .Include(x => x.Category)
                    .ToList();

                foreach (var result in results)
                {
                    // var category = session.Load<Category>(result.Category);
                    Console.WriteLine($"{result.Category} has {result.Count} items.");
                }
            }
        }
        private static void ServerSideProjections()
        {

            Console.WriteLine("\nServer-side projection: ");
            // RQL version:
            // from Employees
            // let format = Func<Employee, string> fo
            // include Category
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var results = session
                    .Query<Employee>()
                    .Select(e => new { Fullname = e.FirstName + " " + e.LastName })
                    .ToList();

                Console.WriteLine($"{results.First()}");
                Console.WriteLine("\n Another version");
                var resultsWithFunc = (from e in session.Query<Employee>()
                                       let format = (Func<Employee, string>)(p => p.FirstName + " " + p.LastName)
                                       select new
                                       {
                                           Fullname = format(e)
                                       }).ToList();

                Console.WriteLine("\n RQL version");
                Console.WriteLine(@"
                declare function output(e) {
                    var format = function(p){ return p.FirstName + "" "" + p.LastName; };
                    return { FullName: format(e) };
                }
                from Employees as e select output(e)
                ");
            }
        }
        private static void CustomizeForStaleIndexes()
        {
            WriteLine("Customize for stale indexes:");
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                QueryStatistics stats;
                WriteLine("Start of query with Customize and WaitForNonStaleResults");
                var query = session
                    .Query<Order>().Statistics(out stats)
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)));
                WriteLine("End of query with Customize and WaitForNonStaleResults");
                if (stats.IsStale)
                {
                    WriteLine("Query is stale");
                }
                var orders = (
                    from order in query
                    where order.Company == "companies/1"
                    orderby order.OrderedAt
                    select order
                    )
                    .ToList();
            }
        }

        private static void RunUnit3()
        {
            Console.WriteLine("1) Patching document without loading the entire document");
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                session.Advanced.Patch<Order, OrderLine>("orders/816-A",
                x => x.Lines,
                lines => lines.Add(new OrderLine
                {
                    Product = "products/1-a",
                    ProductName = "Chai",
                    PricePerUnit = 18M,
                    Quantity = 1,
                    Discount = 0
                })
                );
                session.SaveChanges();
            }


            Console.WriteLine("2) Patching Products document batch");
            var operation = DocumentStoreHolder.Store
            .Operations
                .Send(new PatchByQueryOperation(@"from Products as p
                                where p.Discontinued = false
                                update
                                {
                                    p.PricePerUnit = p.PricePerUnit * 1.1
                                }"));
            operation.WaitForCompletion();

            Console.WriteLine("3) Documents that match a pre-defined criteria are sent in batches from the server to the client. The client sends an acknowledgment to the server once it is done with processing the batch.The server keeps track of the latest document that was acknowledged by the client, so that processing can be continued from the latest acknowledged position if it was paused or interrupted.");

            var subscriptionWorker = DocumentStoreHolder.Store
                .Subscriptions
                .GetSubscriptionWorker<Order>("Big Orders");

            var subscriptionRuntimeTask = subscriptionWorker.Run(batch =>
            {
                foreach (var order in batch.Items)
                {
            // business logic here.
            Console.WriteLine(order.Id);
                }
            });

            WriteLine("Press any key to exit...");
            ReadKey();

        }
    }

}
