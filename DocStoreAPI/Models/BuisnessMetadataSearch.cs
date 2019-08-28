using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
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

    public enum SearchJoin
    {
        Or,
        And
    }

    public interface ISearchEntry
    {
        string Type { get; }
    }

    public class SearchEquality : ISearchEntry
    {
        public string Type { get => "Equality"; }
        public string Key { get; set; }
        public bool Invert { get; set; }
        public string Value { get; set; }
    }

    public class SearchPresent : ISearchEntry
    {
        public string Type { get => "Present"; }
        public string Key { get; set; }
        public bool Invert { get; set; }
    }

    public class SearchAllKeys : ISearchEntry
    {
        public string Type { get => "AllKeys"; }
        public string Value { get; set; }
    }

}
