using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class UpdateState
    {
        public string By { get; protected set; }
        public DateTime At { get; protected set; }

        public UpdateState()
        {

        }
        public UpdateState(string userName)
        {
            this.By = userName;
            this.At = DateTime.UtcNow;
        }

        public void Update(string userName)
        {
            this.By = userName;
            this.At = DateTime.UtcNow;
        }
    }
}
