namespace Autoservice.Migrations
{
    using Autoservice.DAL.Entities;
    using System;
    using System.Collections.Generic;
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
            Guid firstClientId = Guid.NewGuid();
            Guid firstCarId = Guid.NewGuid();
            Guid secondClientId = Guid.NewGuid();
            Guid secondCarId = Guid.NewGuid();
            context.Clients.AddOrUpdate(c => c.Id,
                new Client
                {
                    Id = firstClientId,
                    Name = "Ivan Ivanov"
                },
                new Client
                {
                    Id = secondClientId,
                    Name = "Petr Petrov"
                }
                );
            context.Cars.AddOrUpdate(c => c.RegistrationNumber,
                new Car
                {
                    Id = firstCarId,
                    RegistrationNumber = "c345cc69",
                    Brand = "Volkswagen",
                    Model = "Passat",
                    Type = Car.CarType.Automobile,
                    Mileage = 60000
                },
                new Car
                {
                    Id= secondCarId,
                    RegistrationNumber = "t673ok98",
                    Brand = "Skoda",
                    Model = "Octavia",
                    Type = Car.CarType.Automobile,
                    Mileage = 27456
                }
                );
            context.Orders.AddOrUpdate(o => o.PersonalNumber,
                new Order
                {
                    StartDate = DateTime.Now.AddDays(-45),
                    PersonalNumber = "a4124681cc",
                    ClientId = firstClientId,
                    RepairZone = "Zone X65DA",
                    CarId = firstCarId,
                    Status = OrderStatus.Closed,
                    SpareParts = new List<SparePart>(),
                    Works = new List<Work>(),
                    Masters = new List<Master>(),
                    Activities = new List<Activity>()
                },
                new Order
                {
                    StartDate = DateTime.Now.AddDays(-23),
                    PersonalNumber = "a4124682cc",
                    ClientId = secondClientId,
                    RepairZone = "Zone X61DS",
                    CarId = secondCarId,
                    Status = OrderStatus.Closed,
                    SpareParts = new List<SparePart>(),
                    Works = new List<Work>(),
                    Masters = new List<Master>(),
                    Activities = new List<Activity>()
                }
                );


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
