using System.Data.Entity;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Common.Context
{
    /// <summary>
    /// Database context.</summary>
    public class AutoServiceDBContext : DbContext, IAutoServiceDBContext
    {
        //LocalConnection and ProductionConnection - database connections strings in App.config file
        //LocalConnection for debug, ProductionConnection for release
#if DEBUG
        public AutoServiceDBContext() : base("name=LocalConnection") {
#else
        public AutoServiceDBContext() : base("name=ProductionConnection") {
#endif
        }

        public AutoServiceDBContext(string connectionNameOrString)
            : base(connectionNameOrString)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<ClientCar> ClientCars { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<OrderWork> OrderWorks { get; set; }
        public DbSet<WorkTemplate> WorkTemplates { get; set; }
        public DbSet<OrderSparePart> OrderSpareParts { get; set; }
        public DbSet<SparePartsFolder> SparePartsFolders { get ; set; }
    }
}