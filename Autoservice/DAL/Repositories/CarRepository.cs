using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;

namespace Autoservice.DAL.Repositories
{
    public class CarRepository<TContext> : RepositoryBase<Car, TContext>, ICarRepository
        where TContext : DbContext, IAutoServiceDBContext
    {
        public CarRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }
    }
}
