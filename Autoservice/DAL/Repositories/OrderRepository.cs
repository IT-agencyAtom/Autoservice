using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;

namespace Autoservice.DAL.Repositories
{
    public class OrderRepository<TContext> : RepositoryBase<Order, TContext>, IOrderRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public OrderRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
    }
}
