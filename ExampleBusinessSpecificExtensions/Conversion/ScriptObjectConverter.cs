using Newtonsoft.Json;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;

namespace OLA.Conversion
{
    public class ScriptObjectConverter : JsonConverter
    {

        readonly string dateOnlyPattern = @"^\d\d\d\d-\d\d-\d\d$";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            if(value.GetType().Name == "ScriptArray")
            {
                writer.WriteStartArray();
                WriteList(writer, (ScriptArray)value);
                writer.WriteEndArray();
            }
            else if(value.GetType().Name == "ScriptObject")
            {
                writer.WriteStartObject();
                WriteObject(writer, (ScriptObject)value);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteValue(value);
            }
           
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ReadValue(reader);
        }

        private object ReadValue(JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return ReadObject(reader);
                case JsonToken.StartArray:
                    return ReadList(reader);
                default:
                    if (IsPrimitiveToken(reader.TokenType))
                    {
                        if(reader.TokenType == JsonToken.String)
                        {
                            //Either return string or if it matches  yyyy-mm-dd then return parsed date.
                            var matches = Regex.Match((string)reader.Value, dateOnlyPattern);
                            if (matches.Length > 0)
                            {
                                DateTime.TryParse(matches.Value, out var date);
                                return date;
                            }
                            else
                            {
                                return reader.Value;
                            }
                        }
                        return reader.Value;
                    }

                    throw new Exception($"Unexpected token when converting ExpandoObject: {reader.TokenType}");
            }
        }

        private void WriteList(JsonWriter writer, ScriptArray value)
        {
            foreach(var item in value)
            {
                if (item.GetType().Name == "ScriptObject")
                {
                    writer.WriteStartObject();
                    WriteObject(writer, (ScriptObject)item);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteValue(item);
                }
            }
        }

        private void WriteObject(JsonWriter writer, ScriptObject value)
        {
            foreach(var key in value.Keys)
            {
                writer.WritePropertyName(key);
                if (value[key].GetType().Name == "ScriptArray")
                {
                    writer.WriteStartArray();
                    WriteList(writer, (ScriptArray)value[key]);
                    writer.WriteEndArray();
                }
                else if (value[key].GetType().Name == "ScriptObject")
                {
                    writer.WriteStartObject();
                    WriteObject(writer, (ScriptObject)value[key]);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteValue(value[key]);
                }
            }
        }

        private object ReadList(JsonReader reader)
        {
            var list = new ScriptArray();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;
                    default:
                        object v = ReadValue(reader);

                        list.Add(v);
                        break;
                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw new Exception("Unexpected end when reading ScriptObject.");
        }

        private object ReadObject(JsonReader reader)
        {
            var scriptObject = new ScriptObject();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string propertyName = reader.Value.ToString();

                        if (!reader.Read())
                        {
                            throw new Exception("Unexpected end when reading ScriptObject.");
                        }

                        object v = ReadValue(reader);

                        scriptObject[propertyName] = v;
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        return scriptObject;
                }
            }

            throw new Exception("Unexpected end when reading ScriptObject.");
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ScriptObject));
        }

        public override bool CanWrite => true;

        internal static bool IsPrimitiveToken(JsonToken token)
        {
            return token switch
            {
                JsonToken.Integer or JsonToken.Float or JsonToken.String or JsonToken.Boolean or JsonToken.Undefined or JsonToken.Null or JsonToken.Date or JsonToken.Bytes => true,
                _ => false,
            };
        }
    }
}
