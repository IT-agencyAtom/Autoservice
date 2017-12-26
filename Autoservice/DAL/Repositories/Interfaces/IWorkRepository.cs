using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Repositories.Interfaces
{
    public interface IWorkRepository : IEntityRepository<Work>
    {
        void SaveWork(WorkTemplate baseWorkTemplate, Work work);
        void DeleteWorks(WorkTemplate workTemplate);
    }
}
