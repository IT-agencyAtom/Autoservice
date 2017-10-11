﻿using System.Collections.Generic;
using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Repositories.Interfaces
{
    public interface IActivityRepository : IEntityRepository<Activity>
    {
        List<Activity> GetAllActivitiesByOrder(Order order);
    }
}
