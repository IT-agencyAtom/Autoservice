using Autoservice.DAL.Common.Implementation;
using Autoservice.Screens.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.DAL.Entities
{
    public class SparePartsFolder : IEntity, ITreeViewNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<SparePartsFolder> Folders { get; set; }
        public List<SparePart> SpareParts { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public SparePartsFolder Parent { get; set; }

        public SparePartsFolder()
        {
            Id = Guid.NewGuid();
        }
        [NotMapped]
        public List<ITreeViewNode> Children
        {
            get
            {
                var list = new List<ITreeViewNode>();
                list.AddRange(Folders);
                list.AddRange(SpareParts);
                return list;
            }
        }
    }
}
