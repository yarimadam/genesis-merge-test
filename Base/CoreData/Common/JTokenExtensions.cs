using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreData.Common
{
    public static class JTokenExtensions
    {
        static string ToCamelCaseString(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return char.ToUpperInvariant(str[0]) + str.Substring(1);
            }

            return str;
        }

        static JToken ToCamelCaseJToken(this JToken original)
        {
            switch (original.Type)
            {
                case JTokenType.Object:
                    return ((JObject) original).ToCamelCase();
                case JTokenType.Array:
                    return new JArray(((JArray) original).Select(x => x.ToCamelCaseJToken()));
                default:
                    return original.DeepClone();
            }
        }

        public static JObject ToCamelCase(this JObject original)
        {
            var newObj = new JObject();
            foreach (var property in original.Properties())
            {
                var newPropertyName = property.Name.ToCamelCaseString();
                newObj[newPropertyName] = property.Value.ToCamelCaseJToken();
            }

            return newObj;
        }

        public static string Encode(this JObject original)
        {
            var newObj = new JObject();
            foreach (var property in original.Properties())
            {
                newObj[property.Name] = property.Value.EncodeJToken();
            }

            return newObj.ToString(Formatting.None).Replace("{", "{{").Replace("}", "}}").Replace("@(", "{").Replace("@)", "}");
        }

        static JToken EncodeJToken(this JToken original)
        {
            switch (original.Type)
            {
                case JTokenType.Object:
                    return ((JObject) original).Encode();
                case JTokenType.Array:
                    return new JArray(((JArray) original).Select(x => x.EncodeJToken()));
                case JTokenType.String:
                    return original.ToString().Replace("{", "@(").Replace("}", "@)");
                default:
                    return original.DeepClone();
            }
        }
    }
}