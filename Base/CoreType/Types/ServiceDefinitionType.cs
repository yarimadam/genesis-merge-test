using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoreType.Types
{
    public class ServiceDefinitionsRef
    {
        public List<NamespaceTypeRef> Namespaces { get; set; }
        public Dictionary<string, PropertyTypeWithRef> Models { get; set; }
    }

    public class ServiceDefinitions
    {
        public List<NamespaceType> Namespaces { get; set; }
        public Dictionary<string, PropertyType> Models { get; set; }
    }

    public class NamespaceType
    {
        public string Namespace { get; set; }
        public List<ControllerType> Controllers { get; set; }
    }

    public class NamespaceTypeRef
    {
        public string Namespace { get; set; }
        public List<ControllerTypeRef> Controllers { get; set; }
    }

    public class ControllerTypeRef
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string AQN { get; set; }
        public List<MethodTypeRef> Methods { get; set; }
    }

    public class ControllerType
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string AQN { get; set; }
        public List<MethodType> Methods { get; set; }
    }

    public class MethodTypeRef
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public PropertyTypeBase ReturnType { get; set; }
        public List<PropertyTypeBase> Parameters { get; set; }
    }

    public class MethodType
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public PropertyType ReturnType { get; set; }
        public List<PropertyType> Parameters { get; set; }
    }

    public class PropertyType
    {
        public string Name { get; set; }
        public string AQN { get; set; }
        public string Type { get; set; }
        public List<PropertyType> Properties { get; set; }
    }

    public class PropertyTypeWithRef
    {
        public string Name { get; set; }
        public string AQN { get; set; }
        public string Type { get; set; }

        [JsonIgnore]
        public int RemainingDepth { get; set; }

        public List<PropertyTypeBase> Properties { get; set; }
    }

    public class PropertyTypeBase
    {
        public string Name { get; set; }

        [JsonIgnore]
        public string Type { get; set; }

        public string AQN { get; set; }
    }
}