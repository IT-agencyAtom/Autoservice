using System.Data.Entity;
using System.Linq;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using Mehdime.Entity;

namespace Autoservice.DAL.Repositories
{
    public class OrderSparePartRepository<TContext> : RepositoryBase<OrderSparePart, TContext>, IOrderSparePartRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public OrderSparePartRepository(IAmbientDbContextLocator locator) : base(locator)
        {

        }
    }
}
