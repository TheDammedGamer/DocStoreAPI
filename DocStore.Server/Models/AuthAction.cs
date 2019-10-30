﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.Server.Models
{
    public enum AuthActions
    {
        Create = 1,
        Return = 2,
        Update = 3,
        Delete = 4,
        Archive = 5,
        Supervisor = 6,
    }
}
