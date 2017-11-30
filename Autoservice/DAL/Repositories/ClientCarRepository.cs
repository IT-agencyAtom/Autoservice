using System.Data.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using Mehdime.Entity;

namespace Autoservice.DAL.Repositories
{
    public class ClientCarRepository<TContext> : RepositoryBase<ClientCar, TContext>, IClientCarRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public ClientCarRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
    }
}
