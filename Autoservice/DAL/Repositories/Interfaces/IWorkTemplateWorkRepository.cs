using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Repositories.Interfaces
{
    public interface IWorkTemplateWorkRepository : IEntityRepository<WorkTemplateWork>
    {
        void DeleteWorks(WorkTemplate workTemplate);
        void SaveWork(WorkTemplateWork work);
    }
}