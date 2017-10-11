using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;

namespace Autoservice.DAL.Repositories
{
    public class MasterRepository<TContext> : RepositoryBase<Master, TContext>, IMasterRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public MasterRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
    }
}
