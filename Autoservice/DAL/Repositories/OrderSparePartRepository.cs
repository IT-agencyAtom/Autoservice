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

        public void DeleteSpareParts(Order order)
        {
            var baseOrder = Context.Orders.SingleOrDefault(o => o.Id == order.Id);

            baseOrder?.SpareParts.Where(baseSparePart => order.SpareParts.All(sparePart => sparePart.Id != baseSparePart.Id)).ToList().ForEach(deleted=>Context.OrderSpareParts.Remove(deleted));
        }

        public void SaveSparePart(OrderSparePart sparePart)
        {
            var baseOrderSparePart = sparePart.IsNew ? sparePart : Context.OrderSpareParts.Single(ow => ow.Id == sparePart.Id);

            if (baseOrderSparePart.SparePart != null)
            {
                baseOrderSparePart.SparePartId = sparePart.SparePart.Id;
                baseOrderSparePart.SparePart = null;
            }
            if (sparePart.IsNew)
                Add(baseOrderSparePart);
        }
    }
}
