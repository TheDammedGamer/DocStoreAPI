using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DocStore.Shared.Models;
using DocStore.Shared.Models.Search;

namespace DocStore.Server.Models
{
    public class SearchLogEntity : EntityBase
    {
        public string User { get; protected set; }
        public string TargetBuisnessArea { get; protected set; }
        public string SearchPerformed { get; protected set; }
        public DateTime LoggedAt { get; protected set; }

        public SearchLogEntity()
        {

        }

        public SearchLogEntity(string user, MetadataSearch search)
        {
            this.User = user; 
            this.TargetBuisnessArea = search.BusinessArea;
            this.LoggedAt = DateTime.UtcNow;
            this.SearchPerformed = JsonConvert.SerializeObject(search);
        }
    }
}
