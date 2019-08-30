using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models.Stor
{
    public class StorConfigConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IStorConfig);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var stor = default(IStorConfig);
            switch (jsonObject["StorType"].Value<String>())
            {
                case "FileShareStor":
                    stor = new FileShareStorConfig();
                    break;
                case "AzureBlobStor":
                    stor = new AzureBlobStorConfig();
                    break;
            }
            serializer.Populate(jsonObject.CreateReader(), stor);
            return stor;
        }
    }
}
