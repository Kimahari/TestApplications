using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewtonSoftJsonConverterTest {
    internal class Program {
        static async Task Main(string[] args) {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new TestConverter()
                }
            });
            var stream = new MemoryStream();
            {
                using var swr = new StreamWriter(stream, leaveOpen: true);
                using var jwr = new JsonTextWriter(swr);

                serializer.Serialize(jwr, new ResultObject());
            }
            stream.Position = 0;
            using var r = new StreamReader(stream, leaveOpen: true);
            Console.WriteLine(r.ReadToEnd());

            Console.WriteLine("Hello World!");
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class SkipMeAttribute : Attribute {

    }

    public class TestConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            var attr = objectType.GetCustomAttribute<SkipMeAttribute>();

            if (attr is null) return false;

            return true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value is null) return;

            writer.WriteStartObject();

            var type = value.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties) {
                var attr = property.GetCustomAttribute<SkipMeAttribute>();

                if (attr is not null) continue;

                var pValue = property.GetValue(value);

                if (pValue is null) continue;

                JToken token = JToken.FromObject(pValue, serializer);

                writer.WritePropertyName(property.Name);

                token.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        public override bool CanRead => false;
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }

    [SkipMe]
    public class ResultObject {
        public string Id { get; set; } = "tste";
        public string DisplayName { get; set; } = "tste";

        [SkipMe]
        public string Description { get; set; } = "tste";
        [SkipMe]
        public string CreatedBy { get; set; } = "tste";
        [SkipMe]
        public string UpdatedBy { get; set; } = "tste";

        public IList<InnerResultObject> Results { get; set; } = new List<InnerResultObject>() { new InnerResultObject() };
        public IList<InnerResultObject> Results2 { get; set; } = new List<InnerResultObject>() { new InnerResultObject() };
    }

    public class InnerResultObject {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DisplayName { get; set; } = Guid.NewGuid().ToString();

        [SkipMe]
        public string Description { get; set; } = Guid.NewGuid().ToString();

        [SkipMe]
        public string CreatedBy { get; set; } = Guid.NewGuid().ToString();

        [SkipMe]
        public string UpdatedBy { get; set; } = Guid.NewGuid().ToString();
    }
}
