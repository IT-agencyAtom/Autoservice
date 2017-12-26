using System.Data.Entity;
using System.Linq;
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

        public void SaveClient(Client client)
        {
            var baseClient = DbContext.Clients.SingleOrDefault(c => c.Id == client.Id);
            if (baseClient == null)
            {
                baseClient = new Client(client);
            }

            baseClient.Discount = client.Discount;
            baseClient.Name = client.Name;
            baseClient.Phone = client.Phone;
            //order.Car.ClientId = order.Car.Client.Id;
            //order.Car.Client = null;
        }
    }
}
