using System;
using System.Reflection;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace raven_bootcamp
{
    public static class DocumentStoreHolder
    {
        private static readonly Lazy<IDocumentStore> LazyStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Urls = new[] { "http://localhost:8080" },
                Database = "Northwind"
            };
            store.Initialize();

            // Trova tutte le classi indice e le invia al server.
            var asms = Assembly.GetExecutingAssembly();
            IndexCreation.CreateIndexes(asms, store);

            return store;
        });
        public static IDocumentStore Store => LazyStore.Value;
    }
}