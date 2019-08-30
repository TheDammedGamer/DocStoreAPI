using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models
{
    public class Admins
    {
        public List<string> ADAdminGroupNames { get; set; }
        public List<string> AZAdminGroupNames { get; set; }

        public Admins()
        {

        }

        public Admins(List<string> aDAdminGroups, List<string> aZAdminGroups)
        {
            ADAdminGroupNames = aDAdminGroups ?? throw new ArgumentNullException(nameof(aDAdminGroups));
            AZAdminGroupNames = aZAdminGroups ?? throw new ArgumentNullException(nameof(aZAdminGroups));
        }
    }


}
