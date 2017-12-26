using System.Data.Entity;
using System.Linq;
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

        public void SaveWork(WorkTemplate baseWorkTemplate, Work work)
        {
            var baseWork = baseWorkTemplate.Works.SingleOrDefault(w => w.Id == work.Id);
            if (baseWork == null)
            {
                baseWork = new Work();
                baseWorkTemplate.Works.Add(baseWork);
            }

            baseWork.Name = work.Name;
            baseWork.Price = work.Price;
        }

        public void DeleteWorks(WorkTemplate workTemplate)
        {
            var baseWorkTemplate = Context.WorkTemplates.Include(wt => wt.Works).SingleOrDefault(o => o.Id == workTemplate.Id);

            baseWorkTemplate?.Works.Where(
                    baseWork => workTemplate.Works.All(work => work.Id != baseWork.Id)).ToList()
                .ForEach(deleted => Context.Works.Remove(deleted));
        }
    }
}
