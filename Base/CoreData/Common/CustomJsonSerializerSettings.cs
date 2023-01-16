using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreData.Common
{
    public static class CustomJsonSerializerSettings
    {
        public static readonly JsonSerializerSettings Default = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        public static readonly JsonSerializerSettings MVC = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static readonly JsonSerializerSettings Logging = new JsonSerializerSettings
        {
            ContractResolver = new CustomJsonContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        public static readonly JsonSerializerSettings CamelCase = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        public static readonly JsonSerializerSettings TypeInfo = new JsonSerializerSettings
        {
            ContractResolver = new TypeNameContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        public static readonly JsonSerializerSettings IgnoreAttributes = new JsonSerializerSettings
        {
            ContractResolver = new IgnoredAttributesContractResolver(),
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
    }
}