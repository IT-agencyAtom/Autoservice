﻿using System;
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
        List<ClientCar> GetAllClientCars();
        List<Work> GetAllWorks();
        List<Master> GetAllMasters();
        List<WorkTemplate> GetAllWorkTemplates();        
        void AddClient(Client client);
        void DeleteClient(Client selectedClient);
        void UpdateWork(Work work);
        void AddWork(Work work);
        void DeleteWork(Work selectedWork);
        void UpdateOrder(Order order);
        void AddOrder(Order order);
        void UpdateClient(Client client);
        void DeleteOrder(Order order);
        void UpdateClientCar(ClientCar clientCar);
        void AddClientCar(ClientCar clientCar);
        void DeleteClientCar(ClientCar clientCar);
        void UpdateCar(Car car);
        void AddCar(Car car);
        void DeleteCar(Car selectedCar);
        void UpdateMaster(Master master);
        List<OrderWork> GetAllSalaries(DateTime startDate, DateTime endDate);
        void AddMaster(Master master);
        void DeleteMaster(Master selectedMaster);
        void AddActivity(Activity activity);
        void UpdateActivity(Activity oldActivity);
        Order GetOrderById(Guid id);
        List<Activity> GetAllActivities();
        List<SparePart> GetAllSpareParts();
        void UpdateSparePart(SparePart sparePart);
        void AddSparePart(SparePart sparePart);
        void DeleteSparePart(SparePart selectedSparePart);
        void UpdateWorkTemplate(WorkTemplate workTemplate);
        void AddWorkTemplate(WorkTemplate workTemplate);
        void DeleteWorkTemplate(WorkTemplate selectedWork);
        List<SparePartsFolder> GetAllSparePartsFolders();
        void AddSparePartsFolder(SparePartsFolder folder);
        void UpdateSparePartsFolder(SparePartsFolder folder);
        void AddCars(List<Car> cars);
        List<OrderWork> GetAllSalariesByMaster(DateTime startDate, DateTime endDate, Guid masterGuid);
        void DeleteSparePartsFolder(SparePartsFolder sparePartFolder);
        void AddWorkTemplateWork(WorkTemplateWork tWork);
    }
}
