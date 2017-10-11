namespace Autoservice.Migrations
{
    using Autoservice.DAL.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Autoservice.DAL.Common.Context.AutoServiceDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DAL.Common.Context.AutoServiceDBContext context)
        {
            context.Users.AddOrUpdate(u => u.Login,
               new User
               {
                   Login = "1",
                   Password = "1",
                   Role = "Admin"
               },
               new User
               {
                   Login = "2",
                   Password = "2",
                   Role = "User"
               }
           );
        }
    }
}
