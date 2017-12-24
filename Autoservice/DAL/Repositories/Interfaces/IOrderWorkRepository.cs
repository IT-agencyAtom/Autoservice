using System;
using System.Collections.Generic;
using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Repositories.Interfaces
{
    public interface IOrderWorkRepository : IEntityRepository<OrderWork>
    {
        void DeleteWorks(Order order);
        void SaveWork(OrderWork work);
        List<OrderWork> GetAllSalaries(DateTime from, DateTime to);
        List<OrderWork> GetAllSalariesByMaster(DateTime startDateTime, DateTime endDateTime, Guid masterGuid);
    }
}
