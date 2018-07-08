using System;
using Raven.Client.Documents;

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
            return store.Initialize();
        });
        public static IDocumentStore Store => LazyStore.Value;
    }
}