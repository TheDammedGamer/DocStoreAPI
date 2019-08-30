using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models
{
    public abstract class EntityBase
    {
        public int Id { get; protected set; }

        public void SetID(int Id)
        {
            this.Id = Id;
        }
    }
}
