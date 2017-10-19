using System.Collections.Generic;
using Autoservice.DAL.Entities;

namespace Autoservice.DAL.Services
{
    /// <summary>
    /// Database application service interface.</summary>
    public interface IGeneralService
    {       
        /// <summary>
        /// Update user.</summary>
        void UpdateUser(User user);
        /// <summary>
        /// Add user.</summary>
        void AddUser(User user);
        /// <summary>
        /// Delete user.</summary>
        void DeleteUser(User user);
        /// <summary>
        /// Get all users.</summary>
        List<User> GetAllUsers();
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        List<Order> GetAllOrders();
        List<Client> GetAllClients();
        List<Car> GetAllCars();
        List<Work> GetAllWorks();
        List<Master> GetAllMasters();
        List<Activity> GetAllActivities();        
        void AddClient(Client client);
        void DeleteClient(Client selectedClient);
        void UpdateWork(Work work);
        void AddWork(Work work);
        void DeleteWork(Work selectedWork);
        void UpdateOrder(Order order);
        void AddOrder(Order order);
        void UpdateClient(Client client);
        void DeleteOrder(Order selectedOrder);
    }
}
