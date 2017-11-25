﻿using Autoservice.DAL.Common.Implementation;
using Autoservice.Screens.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class SparePart : IEntity,ITreeViewNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Cargo { get; set; }
        public int Number { get; set; }


        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public SparePartsFolder Parent { get; set; }

        public SparePart()
        {
            Id = Guid.NewGuid();
        }
    }
}
