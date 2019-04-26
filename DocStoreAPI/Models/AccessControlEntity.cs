using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class AccessControlEntity : EntityBase
    {
        public GroupEntity Group { get; set; }
        public string GroupName { get; set; }
        public BuisnessAreaEntity BuisnessAreaEntity { get; set; }
        public string BusinessArea { get; set; }
        public bool Create { get; set; }
        public bool Return { get; set; }
        public bool Update { get; set; }
        public bool Archive { get; set; }
        public bool Delete { get; set; }

        public AccessControlEntity()
        {

        }

        public AccessControlEntity(GroupEntity group, string businessArea, bool create, bool @return, bool update, bool archvie, bool delete)
        {
            Group = group;
            GroupName = group.Name ?? throw new ArgumentNullException(nameof(group.Name));
            BusinessArea = businessArea ?? throw new ArgumentNullException(nameof(businessArea));
            Create = create;
            Return = @return;
            Update = update;
            Archive = archvie;
            Delete = delete;
        }
    }
}
