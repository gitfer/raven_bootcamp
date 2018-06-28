﻿using System;
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

            using (var session = documentStore.OpenSession())
            {
                var p = session.Load<dynamic>("products/1-A");
                System.Console.WriteLine(p.Name);
            }
        }
    }
}
