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
            Client firstClient, secondClient;
            Car firstCar, secondCar;
            Work or, cu, bsp;
            Order o2 = null;
            Master m1, m2;
            Activity a1, a2, a3;
            User u1 = null;

            context.Users.AddOrUpdate(u => u.Login,
               u1 = new User
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
               });


            context.Activities.AddOrUpdate(a=>a.Status,
                a1 = new Activity
                {
                    StartTime = DateTime.Now.AddDays(-7),
                    EndTime = DateTime.Now.AddDays(-6.5),
                    Status = ActivityStatus.Closed,
                    Order = o2,
                    User = u1
                },
                a2 = new Activity
                {
                    StartTime = DateTime.Now.AddDays(-6.32),
                    EndTime = DateTime.Now.AddDays(-6.11),
                    Status = ActivityStatus.InOperation,
                    Order = o2,
                    User = u1
                },
                a3 = new Activity
                {
                    StartTime = DateTime.Now.AddDays(-5.51),
                    EndTime = DateTime.Now.AddDays(-5.28),
                    Status = ActivityStatus.New,
                    Order = o2,
                    User = u1
                }

            );
            context.Masters.AddOrUpdate(m => m.Name,
                m1 = new Master
                {
                    Name = "Maxim Maximov",
                    Position = "Senior Technician"
                },
                m2 = new Master
                {
                    Name = "Fedor Dedorov",
                    Position = "Junior Techician"
                }
                );

            context.Works.AddOrUpdate(w => w.Name,
                new Work
                {
                    Name = "Bumper Replacement",
                    Price = 6000
                },
                or = new Work
                {
                    Name = "Oil Replacement",
                    Price = 1800
                },
                cu = new Work
                {
                    Name = "Checkup",
                    Price = 4500
                },
                bsp = new Work
                {
                    Name = "Brake Shoes Replacement",
                    Price = 5000
                }
                );

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
                    Id = Guid.NewGuid(),
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
                    TotalPrice = 22000,
                    PaymentMethod = PaymentMethod.Cash,
                    SpareParts = new List<SparePart>(),
                    Works = new List<Work> { or, cu, bsp },
                    Masters = new List<Master> { m1, m2 },
                    Activities = new List<Activity>() 
                },
                o2 = new Order
                {
                    StartDate = DateTime.Now.AddDays(-23),
                    PersonalNumber = "a4124682cc",
                    ClientId = secondClient.Id,
                    RepairZone = "Zone X61DS",
                    CarId = secondCar.Id,
                    Status = OrderStatus.Closed,
                    TotalPrice = 16550,
                    PaymentMethod = PaymentMethod.BankCard,
                    SpareParts = new List<SparePart>(),
                    Works = new List<Work>(),
                    Masters = new List<Master>(),
                    Activities = new List<Activity> { a1, a2, a3 }
                },
                new Order
                {
                    StartDate = DateTime.Now.AddDays(-15).AddHours(5.14),
                    PersonalNumber = "a4124667cc",
                    ClientId = firstClient.Id,
                    RepairZone = "Zone X61LS",
                    CarId = firstCar.Id,
                    Status = OrderStatus.Closed,
                    TotalPrice = 36900,
                    PaymentMethod = PaymentMethod.Cash,
                    SpareParts = new List<SparePart>(),
                    Works = new List<Work>(),
                    Masters = new List<Master>(),
                    Activities = new List<Activity>()
                }
                );        
        }
    }
}
