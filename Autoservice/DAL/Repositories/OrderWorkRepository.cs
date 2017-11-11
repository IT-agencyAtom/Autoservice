using System.Data.Entity;
using System.Linq;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using Mehdime.Entity;

namespace Autoservice.DAL.Repositories
{
    public class OrderWorkRepository<TContext> : RepositoryBase<OrderWork, TContext>, IOrderWorkRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public OrderWorkRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public void DeleteWorks(Order order)
        {
            var baseOrder = Context.Orders.SingleOrDefault(o => o.Id == order.Id);

            baseOrder?.Works.Where(
                    baseWork => order.Works.All(work => work.Id != baseWork.Id)).ToList()
                .ForEach(deleted => Context.OrderWorks.Remove(deleted));
        }

        public void SaveWork(OrderWork work)
        {
            var baseOrderWork = work.IsNew ? work : Context.OrderWorks.Single(ow => ow.Id == work.Id);

            baseOrderWork.Price = work.Price;
            if (baseOrderWork.Work != null)
            {
                baseOrderWork.WorkId = work.Work.Id;
                baseOrderWork.Work = null;
            }
            if (baseOrderWork.Master != null) {
                baseOrderWork.MasterId = work.Master.Id;
                baseOrderWork.Master = null;
            }

            if (work.IsNew)
                Add(baseOrderWork);
        }
    }
}
