using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models.Stor
{
    public class StorConfig
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StorConfigConverter))]
        public List<IStorConfig> Stors { get; set; }

        public StorConfig ()
        {

        }
    }

    public static class StorConfigFactory
    {
        public static StorConfig GetStorConfig(string Path)
        {
            string jsonStorConfig = File.ReadAllText(Path);
            StorConfig stor = (StorConfig)JsonConvert.DeserializeObject(jsonStorConfig, typeof(StorConfig));
            return stor;
        }
    }
}
