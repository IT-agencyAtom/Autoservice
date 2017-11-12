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
            /*Guid firstClientId = Guid.Parse("6cc74d2c-012b-4bdc-bf8c-dd46c4ed2b04"), secondClientId=Guid.Parse("7a2f9aca-f47e-471f-9ab2-cb082e14e310");
            Guid firstCarId = Guid.Parse("a938c86b-e17a-49a8-a065-e1bbe03b5736"), secondCarId = Guid.Parse("fd60a645-3e02-40f8-8007-c419eb8a6fa5");
            Guid o1Id = Guid.Parse("b3f0ab1b-da85-4020-9b4d-f56eb0b2c8ef"),o2Id=Guid.Parse("f3a310df-980e-48a8-a0b1-13e9ae553fbb"), o3Id=Guid.Parse("c1d32578-fe8e-44bc-a335-4a6f54409573");
            Guid m1Id = Guid.Parse("b3f0ab1b-da85-4020-9b4d-f56eb1b2c8ef"), m2Id = Guid.Parse("b3f0ab1b-da85-4020-9b4d-f56eb2b2c8ef");
            Work w1, w2, w3,w4,w5;
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
            context.Activities.AddOrUpdate(a => a.UniqueString,
                new Activity
                {
                    Id = Guid.NewGuid(),
                    UniqueString = "one",
                    StartTime = DateTime.Now.AddDays(-12),
                    EndTime = DateTime.Now.AddDays(-11),
                    OrderId = o1Id,
                    Status = ActivityStatus.New,
                    UserId = u1.Id
                },
                new Activity
                {
                    Id = Guid.NewGuid(),
                    UniqueString = "two",
                    StartTime = DateTime.Now.AddDays(-11),
                    OrderId = o1Id,
                    Status = ActivityStatus.InOperation,
                    UserId = u1.Id
                },
                new Activity
                {
                    Id = Guid.NewGuid(),
                    UniqueString = "three",
                    StartTime = DateTime.Now.AddDays(-7),
                    OrderId = o2Id,
                    Status = ActivityStatus.New,
                    UserId = u1.Id
                },
                new Activity
                {
                    Id= Guid.NewGuid(),
                    UniqueString = "four",
                    StartTime = DateTime.Now.AddDays(-3),
                    OrderId = o3Id,
                    Status = ActivityStatus.New,
                    UserId = u1.Id
                }
                );
            
                                      
            context.Masters.AddOrUpdate(m => m.Name,
                new Master
                {
                    Id = m1Id,
                    Name = "Максим Максимов",
                    Position = "Главный мастер"
                },
                new Master
                {
                    Id = m2Id,
                    Name = "Федор Федоров",
                    Position = "Помошник"
                }
                );

            context.Works.AddOrUpdate(w => w.Name,
                w1 = new Work
                {
                    Name = "Замена бампера",
                    Price = 6000
                },
                w2 = new Work
                {
                    Name = "Замена масла",
                    Price = 1800
                },
                w3 = new Work
                {
                    Name = "Проверка двигателя",
                    Price = 4500
                },
                w4 = new Work
                {
                    Name = "Проверка генератора",
                    Price = 5000
                },
                w5 = new Work
                {
                    Name = "Замена колёс",
                    Price = 7000
                }
                );
            context.Cars.AddOrUpdate(c => c.RegistrationNumber,
                new Car
                {
                    Id = firstCarId,
                    RegistrationNumber = "A345BC69",
                    Brand = "Volkswagen",
                    Model = "Passat",
                    Type = Car.CarType.Automobile,
                    Mileage = 60000,
                    ClientId = firstClientId
                },
                 new Car
                 {
                     Id = secondCarId,
                     RegistrationNumber = "M007CK99",
                     Brand = "Skoda",
                     Model = "Octavia",
                     Type = Car.CarType.Automobile,
                     Mileage = 30189,
                     ClientId = secondClientId
                 },
                 new Car
                 {
                     Id = Guid.NewGuid(),
                     RegistrationNumber = "K108EA197",
                     Brand = "Porsche",
                     Model = "Cayen",
                     Type = Car.CarType.Automobile,
                     Mileage = 3821,
                     ClientId = firstClientId
                     }               
                );

            
            context.Clients.AddOrUpdate(c => c.Phone,
                new Client
                {
                    Id = firstClientId,
                    Name = "Иван Иванов",
                    Phone = "89001002030",
                    Discount = 10              
                },
                new Client
                {
                    Id = secondClientId,
                    Name = "Петр Петров",
                    Phone = "89201003080"
                }
                );
            

            context.Orders.AddOrUpdate(o => o.Number,
                new Order
                {
                    Id = o1Id,
                    StartDate = DateTime.Now.AddDays(-45),
                    RepairZone = "Бокс 1",
                    CarId = firstCarId,
                    TotalPrice = 22000,
                    PaymentMethod = PaymentMethod.Cash                  
                },
                new Order
                {
                    Id = o2Id,
                    StartDate = DateTime.Now.AddDays(-23),
                    RepairZone = "Бокс 2",
                    CarId = secondCarId,
                    TotalPrice = 16550,
                    PaymentMethod = PaymentMethod.BankCard                    
                },
                new Order
                {
                    Id = o3Id,
                    StartDate = DateTime.Now.AddDays(-15).AddHours(5.14),
                    RepairZone = "Бокс 3",
                    CarId = firstCarId,
                    TotalPrice = 36900,
                    PaymentMethod = PaymentMethod.Cash
                }
                );
            context.WorkTemplates.AddOrUpdate(w => w.Name,
                new WorkTemplate
                {
                    Name = "Полная диагностика",
                    Works = new List<Work> {w2,w3,w4 }
                },
                new WorkTemplate
                {
                    Name = "Сезонная замена шин",
                    Works = new List<Work> { w5}
                },
                new WorkTemplate
                {
                    Name = "Ремонт генератора",
                    Works = new List<Work>
                    {
                        new Work{ Name = "Разборка автомобиля", Price = 1000 },
                        new Work{ Name = "Снятие генератора", Price = 600 },
                        new Work{ Name = "Починка генератора", Price = 1200 },
                        new Work{ Name = "Сборка автомобиля", Price = 1000 }
                    }
                }
                );
            context.SpareParts.AddOrUpdate(s => s.Name,
                new SparePart
                {
                    Name = "Дворник - ВАЗ2101",
                    Number = 20
                },
                new SparePart
                {
                    Name = "Глушитель - Ferrari Enzo",
                    Number = 1
                },
                new SparePart
                {
                    Name = "Боковые зеркало - Opel Astra",
                    Number = 4
                },
                new SparePart
                {
                    Name = "Лобовое стекло - ВАЗ2101",
                    Number = 3
                },
                new SparePart
                {
                    Name = "Фара - Ford Focus 2011",
                    Number = 4
                },
                new SparePart
                {
                    Name = "Моторное масло Shell",
                    Number = 100
                },
                new SparePart
                {
                    Name = "Масляный фильтр",
                    Number = 41
                }
                );*/
        }
    }
}
