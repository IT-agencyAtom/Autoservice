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
            Client firstClient;
            Car firstCar;
            Client secondClient;
            Car secondCar;
            
            context.Clients.AddOrUpdate(c => c.Name,
                firstClient = new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "Ivan Ivanov"
                },
                secondClient = new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "Petr Petrov"
                }
                );
            context.Cars.AddOrUpdate(c => c.RegistrationNumber,
                firstCar = new Car
                {
                    Id = Guid.NewGuid(),
                    RegistrationNumber = "c345cc69",
                    Brand = "Volkswagen",
                    Model = "Passat",
                    Type = Car.CarType.Automobile,
                    Mileage = 60000
                },
                secondCar = new Car
                {
                    Id= Guid.NewGuid(),
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
                    ClientId = firstClient.Id,
                    RepairZone = "Zone X65DA",
                    CarId = firstCar.Id,
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
                    ClientId = secondClient.Id,
                    RepairZone = "Zone X61DS",
                    CarId = secondCar.Id,
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
