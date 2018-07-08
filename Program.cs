using System;
using System.Diagnostics;
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
                var p1 = session.Load<Product>("products/1-A");
                var p2 = session.Load<Product>("products/1-A");
                Debug.Assert(ReferenceEquals(p1, p2));
                System.Console.WriteLine(p.Name);
            }
        }
    }

}
