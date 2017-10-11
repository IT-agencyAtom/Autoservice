using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Autoservice.DAL.Repositories
{
    public class ActivityRepository<TContext> : RepositoryBase<Activity, TContext>, IActivityRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public ActivityRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
        public List<Activity> GetAllActivitiesByOrder(Order order)
        {
            return Context.Activities.Include(a => a.User).Where(a => a.Order.Id == order.Id).ToList();
        }
    }
}
