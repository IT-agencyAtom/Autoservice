using System.Collections.Generic;
using NLog;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using System;
using System.Linq;

namespace Autoservice.DAL.Services
{
    /// <summary>
    /// Database application service.</summary>
    public class GeneralService : ServiceBase, IGeneralService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICarRepository _carRepository;
        private readonly IClientCarRepository _clientCarRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMasterRepository _masterRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ISparePartRepository _sparePartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWorkRepository _workRepository;
        private readonly IOrderWorkRepository _orderWorkRepository;
        private readonly IWorkTemplateRepository _workTemplateRepository;
        private readonly IOrderSparePartRepository _orderSparePartRepository;
        private readonly ISparePartsFolderRepository _sparePartsFolderRepository;
        private readonly IWorkTemplateWorkRepository _workTemplateWorkRepository;

        protected Logger _logger;

        public GeneralService(
            IDbWorker dbWorker,IActivityRepository activityRepository,ICarRepository carRepository,IClientRepository clientRepository,IMasterRepository masterRepository,            
            IOrderRepository orderRepository,ISparePartRepository sparePartRepository, IUserRepository userRepository, IWorkRepository workRepository,
            IOrderWorkRepository orderWorkRepository,IWorkTemplateRepository workTemplateRepository,IOrderSparePartRepository orderSparePartRepository,ISparePartsFolderRepository sparePartsFolderRepository,
            IClientCarRepository clientCarRepository,IWorkTemplateWorkRepository workTemplateWorkRepository)
            : base(dbWorker)
        {
            _activityRepository = activityRepository;
            _carRepository = carRepository;
            _clientCarRepository = clientCarRepository;
            _clientRepository = clientRepository;
            _masterRepository = masterRepository;
            _orderRepository = orderRepository;
            _sparePartRepository = sparePartRepository;                        
            _userRepository = userRepository;
            _workRepository = workRepository;
            _orderWorkRepository = orderWorkRepository;
            _workTemplateRepository = workTemplateRepository;
            _orderSparePartRepository = orderSparePartRepository;
            _sparePartsFolderRepository = sparePartsFolderRepository;
            _workTemplateWorkRepository = workTemplateWorkRepository;
        }       
      

        public List<User> GetAllUsers()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _userRepository.GetAll();
            }
        }       

        public void AddUser(User user)
        {
            using (var scope = Db.BeginWork())
            {
                _userRepository.Add(user);

                scope.SaveChanges();
            }
        }

        public void UpdateUser(User user)
        {
            using (var scope = Db.BeginWork())
            {
                _userRepository.Update(user);

                scope.SaveChanges();
            }
        }

        public void DeleteUser(User user)
        {
            using (var scope = Db.BeginWork())
            {
                var baseUser = _userRepository.Get(u => u.Id == user.Id);
                if (baseUser != null)
                {
                    _userRepository.Delete(baseUser);

                    scope.SaveChanges();
                }
            }
        }

        public List<ClientCar> GetAllClientCars()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _clientCarRepository.GetAll(c => c.Car);
            }
        }

        public List<Car> GetAllCars()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _carRepository.GetAll();
            }
        }

        public void AddCar(Car car)
        {
            using (var scope = Db.BeginWork())
            {
                _carRepository.Add(car);

                scope.SaveChanges();
            }
        }

        public void UpdateCar(Car car)
        {
            using (var scope = Db.BeginWork())
            {
                _carRepository.Update(car);

                scope.SaveChanges();
            }
        }

        public void DeleteCar(Car car)
        {
            using (var scope = Db.BeginWork())
            {
                var baseCar = _carRepository.Get(u => u.Id == car.Id);
                if (baseCar != null)
                {
                    _carRepository.Delete(baseCar);

                    scope.SaveChanges();
                }
            }
        }

        public void AddClientCar(ClientCar clientCar)
        {
            using (var scope = Db.BeginWork())
            {
                if (clientCar.Car != null)
                {
                    clientCar.CarId = clientCar.Car.Id;
                    clientCar.Car = null;
                }

                _clientCarRepository.Add(clientCar);

                scope.SaveChanges();
            }
        }

        public void UpdateClientCar(ClientCar clientCar)
        {
            using (var scope = Db.BeginWork())
            {
                if (clientCar.Car != null)
                {
                    clientCar.CarId = clientCar.Car.Id;
                    clientCar.Car = null;
                }

                _clientCarRepository.Update(clientCar);

                scope.SaveChanges();
            }
        }

        public void DeleteClientCar(ClientCar clientCar)
        {
            using (var scope = Db.BeginWork())
            {
                var baseClientCar = _clientCarRepository.Get(u => u.Id == clientCar.Id);
                if (baseClientCar != null)
                {
                    _clientCarRepository.Delete(baseClientCar);

                    scope.SaveChanges();
                }
            }
        }

        public List<Client> GetAllClients()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _clientRepository.GetAll(c=>c.Cars);
            }
        }       

        public void AddClient(Client client)
        {
            using (var scope = Db.BeginWork())
            {
                _clientRepository.Add(client);

                scope.SaveChanges();
            }
        }

        public void UpdateClient(Client client)
        {
            using (var scope = Db.BeginWork())
            {
                _clientRepository.Update(client);

                scope.SaveChanges();
            }
        }

        public void DeleteClient(Client client)
        {
            using (var scope = Db.BeginWork())
            {
                var baseClient = _clientRepository.Get(u => u.Id == client.Id);
                if (baseClient != null)
                {
                    _clientRepository.Delete(baseClient);

                    scope.SaveChanges();
                }
            }
        }

        public List<Master> GetAllMasters()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _masterRepository.GetAll();
            }
        }

        public void AddMaster(Master master)
        {
            using (var scope = Db.BeginWork())
            {
                _masterRepository.Add(master);

                scope.SaveChanges();
            }
        }

        public void UpdateMaster(Master master)
        {
            using (var scope = Db.BeginWork())
            {
                _masterRepository.Update(master);

                scope.SaveChanges();
            }
        }

        public void DeleteMaster(Master master)
        {
            using (var scope = Db.BeginWork())
            {
                var baseMaster = _masterRepository.Get(u => u.Id == master.Id);
                if (baseMaster != null)
                {
                    _masterRepository.Delete(baseMaster);

                    scope.SaveChanges();
                }
            }
        }

        public List<Order> GetAllOrders()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _orderRepository.GetAll(o=>o.Car.Client,o=>o.Car.Car, o=>o.Works.Select(w => w.Master), o => o.Works.Select(w => w.Work), o=>o.Activities,o=>o.SpareParts.Select(s=>s.SparePart));
            }
        }        

        public Order GetOrderById(Guid id)
        {
            using (Db.BeginReadOnlyWork())
            {
                return _orderRepository.Get(o => o.Id == id, i => i.Car.Client, i => i.Works,i=>i.SpareParts);
            }
        }


        public void AddOrder(Order order)
        {
            order.TotalPrice = order.Works.Sum(w => w.Price);

            if (order.Car?.Client != null)
            {
                //_clientRepository.SaveClient(order.Car?.Client);
                order.Car.Client = null;
            }
            if (order.Car != null)
            {
                order.ClientCarId = order.Car.Id;
                order.Car = null;
            }

            using (var scope = Db.BeginWork())
            {
                _orderWorkRepository.DeleteWorks(order);

                foreach (var work in order.Works)
                {
                    work.OrderId = order.Id;
                    _orderWorkRepository.SaveWork(work);
                }
                _orderRepository.Add(order);

                scope.SaveChanges();
            }
        }

        public void UpdateOrder(Order order)
        {
            using (var scope = Db.BeginWork())
            {
                _orderWorkRepository.DeleteWorks(order);
                _orderSparePartRepository.DeleteSpareParts(order);

                foreach (var work in order.Works.Where(w => w.Work != null && w.Master != null))
                {
                    work.OrderId = order.Id;
                    //test_____
                    work.MasterId = work.Master.Id;
                    work.WorkId = work.Work.Id;
                    work.Order = null;
                    //_________
                    _orderWorkRepository.SaveWork(work);
                }
                foreach (var sPart in order.SpareParts.Where(s => s.IsNew == true && s.Source == 0))
                {
                    var part = sPart.SparePart;
                    part.Number -= sPart.Number;
                    UpdateSparePart(part);
                }

                foreach (var sparePart in order.SpareParts.Where(s=>s.SparePart!=null && s.Number!=0))
                {
                    sparePart.OrderId = order.Id;
                    sparePart.Order = null;
                    _orderSparePartRepository.SaveSparePart(sparePart);
                }
                

                var baseOrder = _orderRepository.Get(o => o.Id == order.Id);
                if (baseOrder == null)
                    return;

                baseOrder.TotalPrice = order.TotalPrice;
                baseOrder.Notes = order.Notes;
                baseOrder.PaymentMethod = order.PaymentMethod;
                baseOrder.RepairZone = order.RepairZone;

                scope.SaveChanges();
            }
        }

        public void DeleteOrder(Order order)
        {
            using (var scope = Db.BeginWork())
            {
                var baseOrder = _orderRepository.Get(u => u.Id == order.Id);
                if (baseOrder != null)
                {
                    _orderRepository.Delete(baseOrder);

                    scope.SaveChanges();
                }
            }
        }

        public List<Activity> GetAllActivitiesByOrder(Order order)
        {
            using (Db.BeginReadOnlyWork())
            {
                return _activityRepository.GetAllActivitiesByOrder(order);
            }
        }

        public void UpdateActivity(Activity activity)
        {
            using (var scope = Db.BeginWork())
            {
                activity.Order = null;
                activity.User = null;
                _activityRepository.Update(activity);

                scope.SaveChanges();
            }
        }

        public void AddActivity(Activity activity)
        {
            using (var scope = Db.BeginWork())
            {
                activity.Order = null;
                activity.User = null;
                _activityRepository.Add(activity);
                scope.SaveChanges();
            }
        }

        public List<WorkTemplate> GetAllWorkTemplates()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _workTemplateRepository.GetAll(w=>w.Works,w=>w.Works.Select(ws=>ws.Work));
            }
        }

        public void AddWorkTemplate(WorkTemplate workTemplate)
        {
            using (var scope = Db.BeginWork())
            {
                _workTemplateRepository.Add(workTemplate);
                for (var i = 0; i < workTemplate.Works.Count; ++i)
                {
                    var work = workTemplate.Works.ElementAt(i);
                    _workTemplateWorkRepository.SaveWork(work);
                }
                scope.SaveChanges();
            }
        }

        public void UpdateWorkTemplate(WorkTemplate workTemplate)
        {
            using (var scope = Db.BeginWork())
            {
                var baseWorkTemplate = _workTemplateRepository.Get(wt => wt.Id == workTemplate.Id);
                baseWorkTemplate.Name = workTemplate.Name;

                _workTemplateWorkRepository.DeleteWorks(workTemplate);

                for (var i = 0; i < workTemplate.Works.Count; ++i)
                {
                    var work = workTemplate.Works.ElementAt(i);
                    _workTemplateWorkRepository.SaveWork(work);
                }
                scope.SaveChanges();
            }
        }

        public void DeleteWorkTemplate(WorkTemplate workTemplate)
        {
            using (var scope = Db.BeginWork())
            {
                var baseWork = _workTemplateRepository.Get(w => w.Id == workTemplate.Id, w => w.Works);
                if (baseWork != null)
                {
                    _workTemplateWorkRepository.DeleteWorks(workTemplate);
                    _workTemplateRepository.Delete(baseWork);
                    scope.SaveChanges();
                }
            }
        }




        public List<Work> GetAllWorks()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _workRepository.GetAll();
            }
        }

        public void AddWork(Work work)
        {
            using (var scope = Db.BeginWork())
            {
                _workRepository.Add(work);                
                scope.SaveChanges();
            }
        }

        public void UpdateWork(Work work)
        {
            using (var scope = Db.BeginWork())
            {
                _workRepository.Update(work);
                scope.SaveChanges();
            }
        }

        public void DeleteWork(Work work)
        {
            using (var scope = Db.BeginWork())
            {
                var baseWork = _workRepository.Get(u => u.Id == work.Id);                
                if (baseWork != null)
                {
                    var tWorks = _workTemplateWorkRepository.GetAll().Where(w => w.WorkId == work.Id);
                    foreach (var tw in tWorks)
                    {
                        var baseTWork = _workTemplateWorkRepository.Get(u => u.Id == tw.Id);
                        _workTemplateWorkRepository.Delete(baseTWork);
                    }
                    _workRepository.Delete(baseWork);

                    scope.SaveChanges();
                }
            }
        }


        public List<SparePartsFolder> GetAllSparePartsFolders()
        {
            using (Db.BeginReadOnlyWork())
            {
                return  _sparePartsFolderRepository.GetAll(f=>f.Folders,f=>f.SpareParts);
            }
        }

        public void AddSparePartsFolder(SparePartsFolder sparePartsFolder)
        {
            using (var scope = Db.BeginWork())
            {
                _sparePartsFolderRepository.Add(sparePartsFolder);
                scope.SaveChanges();
            }
        }

        public void UpdateSparePartsFolder(SparePartsFolder sparePartsFolder)
        {
            using (var scope = Db.BeginWork())
            {
                _sparePartsFolderRepository.Update(sparePartsFolder);
                scope.SaveChanges();
            }
        }

        public void AddCars(List<Car> cars)
        {
            using (var scope = Db.BeginWork())
            {
                foreach (var car in cars)
                {
                    _carRepository.Add(car);
                }

                scope.SaveChanges();
            }
        }

        public void DeleteSparePartsFolder(SparePartsFolder sparePartsFolder)
        {
            using (var scope = Db.BeginWork())
            {
                var basePart = _sparePartsFolderRepository.Get(u => u.Id == sparePartsFolder.Id, spf => spf.SpareParts);
                if (basePart != null)
                {
                    basePart.SpareParts.Where(
                            baseSparePart => sparePartsFolder.SpareParts.All(sparePart => sparePart.Id != baseSparePart.Id)).ToList()
                        .ForEach(deleted => _sparePartRepository.Delete(deleted));

                    _sparePartsFolderRepository.Delete(basePart);

                    scope.SaveChanges();
                }
            }
        }


        public List<SparePart> GetAllSpareParts()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _sparePartRepository.GetAll(s=>s.Parent);
            }
        }

        public void AddSparePart(SparePart sparePart)
        {
            using (var scope = Db.BeginWork())
            {
                _sparePartRepository.Add(sparePart);

                scope.SaveChanges();
            }
        }

        public void UpdateSparePart(SparePart sparePart)
        {
            using (var scope = Db.BeginWork())
            {
                _sparePartRepository.Update(sparePart);

                scope.SaveChanges();
            }
        }

        public void DeleteSparePart(SparePart sparePart)
        {
            using (var scope = Db.BeginWork())
            {
                var baseSparePart = _sparePartRepository.Get(u => u.Id == sparePart.Id);
                if (baseSparePart != null)
                {
                    _sparePartRepository.Delete(baseSparePart);

                    scope.SaveChanges();
                }
            }
        }

        public List<Activity> GetAllActivities()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _activityRepository.GetAll(a=>a.Order,a=>a.User);
            }
        }
        public List<OrderWork> GetAllSalaries(DateTime startDateTime,DateTime endDateTime)
        {
            using (Db.BeginReadOnlyWork())
            {
                return _orderWorkRepository.GetAllSalaries(startDateTime,endDateTime);
            }
        }
        public List<OrderWork> GetAllSalariesByMaster(DateTime startDateTime, DateTime endDateTime,Guid masterGuid)
        {
            using (Db.BeginReadOnlyWork())
            {
                return _orderWorkRepository.GetAllSalariesByMaster(startDateTime, endDateTime,masterGuid);
            }
        }

        public void AddWorkTemplateWork(WorkTemplateWork work)
        {
            using (var scope = Db.BeginWork())
            {
                work.Template = null;
                work.Work = null;
                _workTemplateWorkRepository.Add(work);
                scope.SaveChanges();
            }
        }
    }
}
