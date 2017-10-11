using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;

namespace Autoservice.DAL.Repositories
{
    public class WorkRepository<TContext> : RepositoryBase<Work, TContext>, IWorkRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public WorkRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
    }
}
