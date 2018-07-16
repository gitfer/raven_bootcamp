using System.Linq;
using NorthwindModels;
using Raven.Client.Documents.Indexes;

namespace raven_bootcamp
{
    public class Employees_ByFirstAndLastName : AbstractIndexCreationTask<Employee>
    {
        public Employees_ByFirstAndLastName()
        {
            Map = (employees) =>
                from employee in employees
                select new
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName
                };
        }
    }
}