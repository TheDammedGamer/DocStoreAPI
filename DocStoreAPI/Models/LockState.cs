using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class LockState
    {
        public bool Is { get; protected set; }
        public string By { get; protected set; }
        public DateTime? At { get; protected set; }
        public DateTime? Expiration { get; protected set; }

        public LockState()
        {
            Is = false;
        }

        public void LockDocument(string userName, TimeSpan span)
        {
            this.Is = true;
            var tempDate = DateTime.UtcNow;
            this.At = tempDate;
            this.Expiration = tempDate.Add(span);
            this.By = userName;
        }
        public bool UnLockDocument(string userName)
        {
            if (this.By == userName)
            {
                // Success
                this.Is = false;
                this.At = DateTime.MinValue;
                this.Expiration = DateTime.MinValue;
                this.By = String.Empty;
                return true;
            }
            else
            {
                // Fail
                return false;
            }
            
        }
    }
}
