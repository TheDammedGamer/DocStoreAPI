using DocStoreAPI.Models;
using DocStoreAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DocStoreAPI.Shared
{
    public static class GenericExtensions
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }        
    }
}
