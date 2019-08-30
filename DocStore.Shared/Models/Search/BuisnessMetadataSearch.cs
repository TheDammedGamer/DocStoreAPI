using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DocStore.Shared.Models.Search
{
    public class BuisnessMetadataSearch
    {
        //can only search by one business area at a time as it would require multiple auths.
        //maybe add this later
        public string BusinessArea { get; set; }
        //We only want 5 for now
        [MaxLength(5)]
        public List<ISearchEntry> Search { get; set; }
        public SearchJoin Joiner { get; set; }
    }
}
