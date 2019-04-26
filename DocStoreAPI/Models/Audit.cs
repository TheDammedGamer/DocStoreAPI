using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class Audit
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }

}
