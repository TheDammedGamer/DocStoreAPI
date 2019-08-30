using System;
using System.Collections.Generic;
using System.Text;

namespace DocStore.Shared.Models.Search
{
    public class SearchPresent : ISearchEntry
    {
        public string Type { get => "Present"; }
        public string Key { get; set; }
        public bool Invert { get; set; }
    }
}
