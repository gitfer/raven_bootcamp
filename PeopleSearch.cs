using System.Linq;
using NorthwindModels;
using Raven.Client.Documents.Indexes;

namespace raven_bootcamp
{

    public class People_Search :
        AbstractMultiMapIndexCreationTask<People_Search.Result>
    {
        public class Result
        {
            public string SourceId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public People_Search()
        {
            AddMap<Company>(companies =>
                from company in companies
                select new Result
                {
                    SourceId = company.Id,
                    Name = company.Contact.Name,
                    Type = "Company's contact"
                }
                );

            AddMap<Supplier>(suppliers =>
                from supplier in suppliers
                select new Result
                {
                    SourceId = supplier.Id,
                    Name = supplier.Contact.Name,
                    Type = "Supplier's contact"
                }
                );

            AddMap<Employee>(employees =>
                from employee in employees
                select new Result
                {
                    SourceId = employee.Id,
                    Name = $"{employee.FirstName} {employee.LastName}",
                    Type = "Employee"
                }
                );

            // Marco Name come FieldIndexing.Search, cioè attivo fulltext search su Name
            Index(entry => entry.Name, FieldIndexing.Search);

            // Abilito projections su SourceId
            Store(entry => entry.SourceId, FieldStorage.Yes);
            Store(entry => entry.Name, FieldStorage.Yes);
            Store(entry => entry.Type, FieldStorage.Yes);
        }
    }
}