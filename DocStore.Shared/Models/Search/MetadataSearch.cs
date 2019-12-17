using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DocStore.Shared.Models.Search
{
    public class MetadataSearch
    {
        public string BusinessArea { get; set; }
        [MaxLength(5)]
        public List<ISearchEntry> Search { get; set; }
        public SearchJoin Joiner { get; set; }
    }
}
