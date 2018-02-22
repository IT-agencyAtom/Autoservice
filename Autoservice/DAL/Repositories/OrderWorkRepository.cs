using System.Data.Entity;
using System.Linq;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using Mehdime.Entity;
using System;
using System.Collections.Generic;

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
            baseOrderWork.MasterPercentage = work.MasterPercentage;
            baseOrderWork.MasterId = work.MasterId;
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
        public List<OrderWork> GetAllSalaries(DateTime from, DateTime to)
        {
            return Context.OrderWorks.Include(w=>w.Order)
                .Include(w => w.Master)
                .Include(w=>w.Work)
                .Where(w => w.Order.StartDate >= from && w.Order.StartDate <= to)
                .ToList();
        }
        public List<OrderWork> GetAllSalariesByMaster(DateTime from, DateTime to,Guid masterGuid)
        {
            return Context.OrderWorks.Include(w => w.Order)
                .Include(w => w.Master)
                .Include(w => w.Work)
                .Where(w => w.Order.StartDate >= from && w.Order.StartDate <= to&&w.MasterId==masterGuid)
                .ToList();
        }
    }
}
