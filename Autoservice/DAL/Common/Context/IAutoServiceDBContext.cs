using System.Data.Entity;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Common.Context
{
    /// <summary>
    /// Database context interface.</summary>
    public interface IAutoServiceDBContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Activity> Activities { get; set; }
        DbSet<Car> Cars { get; set; }
        DbSet<ClientCar> ClientCars { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<Master> Masters { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<SparePart> SpareParts { get; set; }
        DbSet<Work> Works { get; set; }
        DbSet<OrderWork> OrderWorks { get; set; }
        DbSet<WorkTemplate> WorkTemplates { get; set; }
        DbSet<OrderSparePart> OrderSpareParts { get; set; }
        DbSet<SparePartsFolder> SparePartsFolders { get; set; }

    }
}
