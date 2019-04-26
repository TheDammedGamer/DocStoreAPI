using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class GroupEntity : EntityBase
    {
        public string Name { get; set; }
        public bool IsAD { get; set; }
        public bool IsAzure { get; set; }

        public string ADName { get; set; }
        public string AzureName { get; set; }

        public List<AccessControlEntity> RelevantACEs { get; set; }
        //public Guid AzureID { get; set; } //Don't think I need

        public GroupEntity()
        {

        }
        public GroupEntity(string name, bool isAD, string refName)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            if (isAD)
            {
                this.IsAzure = false;
                this.IsAD = true;
                this.ADName = refName ?? throw new ArgumentNullException(nameof(refName));
            }
            else
            {
                this.IsAzure = true;
                this.IsAD = false;
                this.AzureName = refName ?? throw new ArgumentNullException(nameof(refName));
            }
        }

    }
}
