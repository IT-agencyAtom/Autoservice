using Autoservice.DAL.Common.Implementation;
using Autoservice.Dialogs.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class Work : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }        

        public Work()
        {
            Id = Guid.NewGuid();          
        }
        public Work(WorkModel work)
        {
            Id = work.Id;
            Name = work.Name;
            Price = work.Price;
        }
    }
}
