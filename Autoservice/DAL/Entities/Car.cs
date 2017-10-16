using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Car: IEntity
    {
        public Guid Id { get; set; }        
        public string RegistrationNumber { get; set;}
        public string Brand { get; set; }
        public string Model { get; set; }
        public CarType Type { get; set; }
        public int Mileage { get; set; }

        public Car()
        {
            Id = Guid.NewGuid();
        }
        public enum CarType
        {
            Automobile,Motocycle
        }
        public override string ToString()
        {
            return string.Format("{0} {1}",Brand,Model);
        }
    }
}
