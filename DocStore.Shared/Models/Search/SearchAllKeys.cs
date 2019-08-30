using System;
using System.Collections.Generic;
using System.Text;

namespace DocStore.Shared.Models.Search
{
    public class SearchAllKeys : ISearchEntry
    {
        public string Type { get => "AllKeys"; }
        public string Value { get; set; }
    }
}
