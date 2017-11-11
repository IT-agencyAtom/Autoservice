using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Repositories.Interfaces
{
    public interface IOrderWorkRepository : IEntityRepository<OrderWork>
    {
        void DeleteWorks(Order order);
        void SaveWork(OrderWork work);
    }
}
