using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class UpdateState
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
    }

    public static class UpdateStateExtensions
    {
        public static void ServerUpdate(this UpdateState state, string user, DateTime date)
        {
            state.At = date;
            state.By = user;
        }
    }
}
