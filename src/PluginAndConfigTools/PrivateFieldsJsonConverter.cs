using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.PipelinesCE.PluginAndConfigTools
{
    // TODO move this to utils
    /// <summary>
    /// Serializes private fields in root object
    /// 
    /// Inspired by https://stackoverflow.com/questions/30300740/how-to-configure-json-net-deserializer-to-track-missing-properties
    /// </summary>
    public class PrivateFieldsJsonConverter : JsonConverter
    {
        public List<FieldInfo> MissingFields { get; } = new List<FieldInfo>();
        public List<KeyValuePair<string, JToken>> ExtraFields { get; } = new List<KeyValuePair<string, JToken>>();

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            existingValue = existingValue ?? Activator.CreateInstance(objectType, true);

            JObject jObject = JObject.Load(reader);
            FieldInfo[] privateFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (FieldInfo field in privateFields)
            {
                JToken jToken = jObject[field.Name];

                // Missing field
                if (jToken == null)
                {
                    MissingFields.Add(field);
                    continue;
                }

                jObject.Remove(field.Name);
                object value = jToken.ToObject(field.FieldType);

                field.SetValue(existingValue, value);
            }

            foreach (KeyValuePair<string, JToken> pair in jObject)
            {
                // Extra field
                ExtraFields.Add(pair);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type objectType = value.GetType();
            FieldInfo[] privateFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            JsonSerializer secondarySerializer = new JsonSerializer();

            writer.WriteStartObject();

            foreach (FieldInfo field in privateFields)
            {
                writer.WritePropertyName(field.Name);
                object fieldValue = field.GetValue(value);
                // TODO what if value is serializable?
                // https://stackoverflow.com/questions/26129448/json-net-how-to-customize-serialization-to-insert-a-json-property
                // https://github.com/JamesNK/Newtonsoft.Json/issues/386
                secondarySerializer.Serialize(writer, fieldValue);
            }

            writer.WriteEndObject();
        }
    }
}
