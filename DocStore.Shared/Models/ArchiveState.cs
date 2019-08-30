using System;
using System.Collections.Generic;
using System.Text;

namespace DocStore.Shared.Models
{
    public class ArchiveState : IEquatable<ArchiveState>
    {
        public bool Is { get; protected set; }
        public string By { get; protected set; }
        public DateTime? At { get; protected set; }

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

        public bool Equals(ArchiveState other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null))
                return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other))
                return true;

            if (!Is)
            {
                if (!other.Is)
                    return true;
                else
                    return false;
            }
            else
            {
                return By.Equals(other.By) && At.Equals(other.At);
            }

        }
    }
}
