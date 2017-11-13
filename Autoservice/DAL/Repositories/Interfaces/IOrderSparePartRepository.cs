using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Repositories.Interfaces
{
    public interface IOrderSparePartRepository : IEntityRepository<OrderSparePart>
    {
        void DeleteSpareParts(Order order);
        void SaveSparePart(OrderSparePart sparePart);
    }
}