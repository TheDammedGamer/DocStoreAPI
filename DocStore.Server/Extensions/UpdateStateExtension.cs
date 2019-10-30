using DocStore.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.Server.Extensions
{
    public static class UpdateStateExtension
    {
        public static void ServerUpdate(this UpdateState state, string user, DateTime time)
        {
            state.At = time;
            state.By = user;
        }
    }
}
