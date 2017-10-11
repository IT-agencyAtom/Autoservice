using System;
using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;

namespace Autoservice.DAL.Common.Implementation
{
    /// <summary>
    /// Database context factory
    /// </summary>
    public class SwitchableDbContextFactory : IDbContextFactory
    {
        private readonly string _connectionString;

        public SwitchableDbContextFactory(string connString)
        {
            _connectionString = connString;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            if (typeof(TDbContext) == typeof(AutoServiceDBContext))
                return new AutoServiceDBContext(_connectionString) as TDbContext;

            return Activator.CreateInstance<TDbContext>();
        }
    }

    /// <summary>
    /// Production factory
    /// </summary>
    public class ProductionDbContextFactory : SwitchableDbContextFactory
    {
        public ProductionDbContextFactory()
            : base("ProductionConnection")
        {

        }
    }

    /// <summary>
    /// Local factory
    /// </summary>
    public class LocalDbContextFactory : SwitchableDbContextFactory
    {
        public LocalDbContextFactory()
            : base("LocalConnection")
        {

        }
    }
}
