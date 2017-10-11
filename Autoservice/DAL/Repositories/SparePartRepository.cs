using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;

namespace Autoservice.DAL.Repositories
{
    public class SparePartRepository<TContext> : RepositoryBase<SparePart, TContext>, ISparePartRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public SparePartRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
    }
}
