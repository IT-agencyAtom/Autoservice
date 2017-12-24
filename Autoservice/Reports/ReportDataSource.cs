using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace Autoservice.Reports
{
    [DataObject]
    public class ReportDataSource
    {
        public ReportDataSource()
        {
            new ViewModelLocator();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Master> GetAllMasters()
        {
            var cargoService = ServiceLocator.Current.GetInstance<IGeneralService>();
            return cargoService.GetAllMasters();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OrderWork> GetAllSalaries(DateTime startDate, DateTime endDate, string masterId)
        {
            if (startDate.Date == endDate.Date)
                endDate = endDate.AddDays(1);

            var cargoService = ServiceLocator.Current.GetInstance<IGeneralService>();
            //Если отчет с учетом контрагента
            if (masterId != null)
            {
                var masterGuid = Guid.Parse(masterId);
                var allHauls = cargoService.GetAllSalariesByMaster(startDate, endDate, masterGuid).OrderBy(h => h.Order.StartDate).ToList();
                return allHauls;
            }
            return cargoService.GetAllSalaries(startDate, endDate).OrderBy(h => h.Order.StartDate).ToList();
        }
    }
}
