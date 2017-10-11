using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace Autoservice.DAL.Repositories
{
    public class UserRepository<TContext> : RepositoryBase<User, TContext>, IUserRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public UserRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }       
    }
}
