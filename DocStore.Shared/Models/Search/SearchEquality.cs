using System;
using System.Collections.Generic;
using System.Text;

namespace DocStore.Shared.Models.Search
{
    public class SearchEquality : ISearchEntry
    {
        public string Type { get => "Equality"; }
        public string Key { get; set; }
        public bool Invert { get; set; }
        public string Value { get; set; }
    }
}
