using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models
{
    public class LockState : IEquatable<LockState>
    {
        public bool Is { get; protected set; }
        public string By { get; protected set; }
        public DateTime At { get; protected set; }
        public DateTime Expiration { get; protected set; }

        public LockState()
        {
            Is = false;
        }

        public bool LockDocument(string userName, TimeSpan span)
        {
            if (this.Is)
            {
                return false;
            }
            else
            {
                this.Is = true;
                var tempDate = DateTime.UtcNow;
                this.At = tempDate;
                this.Expiration = tempDate.Add(span);
                this.By = userName;
                return true;
            }
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
        public bool Equals(LockState other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null))
                return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other))
                return true;
            if (Is)
            {
                return At.Equals(other.At) && Expiration.Equals(other.Expiration) && By.Equals(other.By);
            }
            else
            {
                if (other.Is)
                    return false;
                else
                    return true;
            }
        }
    }

    
}
