using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class AccessControlEntity : EntityBase
    {
        public string GroupName { get; set; }
        public string BusinessArea { get; set; }
        public bool Create { get; set; }
        public bool Return { get; set; }
        public bool Update { get; set; }
        public bool Archvie { get; set; }
        public bool Delete { get; set; }

        public AccessControlEntity()
        {

        }

        public AccessControlEntity(string groupName, string businessArea, bool create, bool @return, bool update, bool archvie, bool delete)
        {
            GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
            BusinessArea = businessArea ?? throw new ArgumentNullException(nameof(businessArea));
            Create = create;
            Return = @return;
            Update = update;
            Archvie = archvie;
            Delete = delete;
        }
    }
}
