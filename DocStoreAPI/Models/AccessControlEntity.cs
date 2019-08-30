using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models
{
    public class AccessControlEntity : EntityBase
    {
        public string Group { get; set; }
        public string BusinessArea { get; set; }
        public bool Create { get; set; }
        public bool Return { get; set; }
        public bool Update { get; set; }
        public bool Archive { get; set; }
        public bool Delete { get; set; }
        public bool Supervisor { get; set; }

        public AccessControlEntity()
        {

        }

        public AccessControlEntity(string group, string businessArea, bool create, bool returnPerm, bool update, bool archive, bool delete, bool supervisor)
        {
            Group = group ?? throw new ArgumentNullException(nameof(group));
            BusinessArea = businessArea ?? throw new ArgumentNullException(nameof(businessArea));
            Create = create;
            Return = returnPerm;
            Update = update;
            Archive = archive;
            Delete = delete;
            Supervisor = supervisor;
        }
    }
}
