using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models
{
    public class UpdateState : IEquatable<UpdateState>
    {
        public string By { get; internal set; }
        public DateTime At { get; internal set; }

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

        public bool Equals(UpdateState other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null))
                return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other))
                return true;

            //Should be enought to compare equity
            return By.Equals(other.By) && At.Equals(other.At);
        }
    }
}
