using Autoservice.DAL.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class SparePart : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public SparePart()
        {
            Id = Guid.NewGuid();
        }
    }
}
