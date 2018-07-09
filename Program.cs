﻿using static System.Console;
using System.Diagnostics;
using System.Linq;
using Raven.Client.Documents;
using NorthwindModels;
using System;

namespace raven_bootcamp
{
    class Program
    {
        static void Main(string[] args)
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
            WriteLine("Goodbye!");
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
    }

}
