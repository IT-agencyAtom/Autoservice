using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;

namespace Autoservice.DAL.Repositories
{
    public class WorkTemplateRepository<TContext> : RepositoryBase<WorkTemplate, TContext>, IWorkTemplateRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public WorkTemplateRepository(IAmbientDbContextLocator locator) : base(locator)
        {

        }
    }
}
