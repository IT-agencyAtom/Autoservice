﻿using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Client: IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set;}
        public string Phone { get; set; }
        public int Discount { get; set; }
        public List<ClientCar> Cars { get; set; }

        public Client()
        {
            Id = Guid.NewGuid();
            Cars = new List<ClientCar>();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
