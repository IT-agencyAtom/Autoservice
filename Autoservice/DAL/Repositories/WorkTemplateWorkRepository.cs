using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using System.Linq;

namespace Autoservice.DAL.Repositories
{
    public class WorkTemplateWorkRepository<TContext> : RepositoryBase<WorkTemplateWork, TContext>, IWorkTemplateWorkRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public WorkTemplateWorkRepository(IAmbientDbContextLocator locator) : base(locator)
        {

        }
        public void SaveWork(WorkTemplateWork work)
        {
            if (work.Work != null)
            {
                work.WorkId = work.Work.Id;
                work.Work = null;
            }    
            Add(work);
        }

        public void DeleteWorks(WorkTemplate workTemplate)
        {
            var works = GetAll().Where(w => w.TemplateId == workTemplate.Id).ToList();
            works.ForEach(del=>Context.WorkTemplateWorks.Remove(del));
        }
    }
}
