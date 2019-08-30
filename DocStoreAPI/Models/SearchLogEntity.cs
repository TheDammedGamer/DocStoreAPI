using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DocStore.Shared.Models;
using DocStore.Shared.Models.Search;

namespace DocStore.API.Models
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

        public SearchLogEntity(string user, string buisnessArea, BuisnessMetadataSearch search)
        {
            this.User = user ?? throw new ArgumentNullException(nameof(user));
            this.TargetBuisnessArea = buisnessArea;
            this.LoggedAt = DateTime.UtcNow;
            this.SearchPerformed = JsonConvert.SerializeObject(search);
        }
    }
}
