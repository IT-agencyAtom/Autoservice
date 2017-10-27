using System.Collections.Generic;
using NLog;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Repositories.Interfaces;
using System;

namespace Autoservice.DAL.Services
{
    /// <summary>
    /// Database application service.</summary>
    public class GeneralService : ServiceBase, IGeneralService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICarRepository _carRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMasterRepository _masterRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ISparePartRepository _sparePartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWorkRepository _workRepository;        

        protected Logger _logger;

        public GeneralService(
            IDbWorker dbWorker,IActivityRepository activityRepository,ICarRepository carRepository,IClientRepository clientRepository,IMasterRepository masterRepository,            
            IOrderRepository orderRepository,ISparePartRepository sparePartRepository, IUserRepository userRepository, IWorkRepository workRepository)
            : base(dbWorker)
        {
            _activityRepository = activityRepository;
            _carRepository = carRepository;
            _clientRepository = clientRepository;
            _masterRepository = masterRepository;
            _orderRepository = orderRepository;
            _sparePartRepository = sparePartRepository;                        
            _userRepository = userRepository;
            _workRepository = workRepository;           
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
        public Master GetMasterByWork(Work work)
        {
            using (Db.BeginReadOnlyWork())
            {
                return _masterRepository.FindById(work.MasterId);
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
                return _orderRepository.GetAll(o=>o.Car,o=>o.Client,o=>o.Works,o=>o.Activities);
            }
        }        

        public Order GetOrderById(Guid id)
        {
            using (Db.BeginReadOnlyWork())
            {
                return _orderRepository.Get(o => o.Id == id);
            }
        }


        public void AddOrder(Order order)
        {
            using (var scope = Db.BeginWork())
            {
                order.Car = null;
                order.Client = null;
                order.Activities = new List<Activity>();
                order.Works = new List<Work>();
                order.SpareParts = new List<SparePart>();
                _orderRepository.Add(order);

                scope.SaveChanges();
            }
        }

        public void UpdateOrder(Order order)
        {
            using (var scope = Db.BeginWork())
            {
                order.Car = null;
                order.Client = null;
                order.Activities = new List<Activity>();
                order.Works = new List<Work>();
                order.SpareParts = new List<SparePart>();
                _orderRepository.Update(order);
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

        public List<Work> GetAllWorks()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _workRepository.GetAll(w=>w.Master);
            }
        }

        public void AddWork(Work work)
        {
            using (var scope = Db.BeginWork())
            {
                work.Master = null;
                work.Order = null;
                _workRepository.Add(work);                
                scope.SaveChanges();
            }
        }

        public void UpdateWork(Work work)
        {
            using (var scope = Db.BeginWork())
            {
                work.Master = null;
                work.Order = null;
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
                    _workRepository.Delete(baseWork);

                    scope.SaveChanges();
                }
            }
        }

        public List<SparePart> GetAllSpareParts()
        {
            using (Db.BeginReadOnlyWork())
            {
                return _sparePartRepository.GetAll();
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
                return _activityRepository.GetAll(a => a.Order,a=>a.User);
            }
        }
    }
}
