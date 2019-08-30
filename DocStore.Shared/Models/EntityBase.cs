using System;

namespace DocStore.Shared
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
