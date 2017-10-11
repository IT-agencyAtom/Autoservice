using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;

namespace Autoservice.DAL.Repositories
{
    public class ClientRepository<TContext> : RepositoryBase<Client, TContext>, IClientRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public ClientRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
    }
}
