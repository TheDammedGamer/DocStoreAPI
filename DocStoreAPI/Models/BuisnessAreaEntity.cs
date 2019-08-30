using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocStore.Shared.Models;

namespace DocStore.API.Models
{
    public class BuisnessAreaEntity : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set;}

        public List<AccessControlEntity> RelevantAccessControlEntities { get; set; }

        public BuisnessAreaEntity ()
        {

        }

        public BuisnessAreaEntity (string name)
        {
            Name = name;
        }
    }
}
