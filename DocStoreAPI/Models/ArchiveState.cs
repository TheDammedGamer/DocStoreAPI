using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class ArchiveState
    {
        public bool Is { get; protected set; }
        public string By { get; protected set; }
        public DateTime At { get; protected set; }

        public ArchiveState()
        {
            this.Is = false;
        }

        public void Archive(string userName)
        {
            this.Is = true;
            this.By = userName;
            this.At = DateTime.UtcNow;
        }
        public void UnArchive(string userName)
        {
            this.Is = false;
            this.By = string.Empty;
            this.At = DateTime.MinValue;
        }
    }
}
