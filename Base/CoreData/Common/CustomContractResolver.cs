using System.Linq;
using System.Reflection;
using CoreData.Common.Converters;
using CoreType.Attributes;
using CoreType.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreData.Common
{
    public class CustomJsonContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (member is PropertyInfo)
            {
                var loggingAttributes = member
                    .GetCustomAttributes<BaseLoggingAttribute>(true)
                    .ToList();

                if (loggingAttributes.Any())
                {
                    if (loggingAttributes.Any(attr => attr.LoggingBehaviour == LoggingBehaviour.IgnoreAll))
                    {
                        property.ShouldSerialize = _ => false;
                        property.ShouldDeserialize = _ => false;
                    }
                    else if (loggingAttributes.Any(attr => attr.LoggingBehaviour == LoggingBehaviour.IgnoreSerialization))
                    {
                        property.ShouldSerialize = _ => false;
                    }
                    else if (loggingAttributes.Any(attr => attr.LoggingBehaviour == LoggingBehaviour.Hash))
                    {
                        property.Converter = new SensitiveDataConverter();
                    }
                    else if (loggingAttributes.Any(attr => attr.LoggingBehaviour == LoggingBehaviour.Mask))
                    {
                        var maskedLoggingAttribute = loggingAttributes
                            .Where(attr => attr.LoggingBehaviour == LoggingBehaviour.Mask)
                            .OfType<MaskedLoggingAttribute>()
                            .FirstOrDefault();

                        if (maskedLoggingAttribute != null)
                            // if (maskedLoggingAttribute.MaskerDelegate != null)
                            //     property.Converter = new MaskedDataConverter(maskedLoggingAttribute.MaskerDelegate);
                            // else
                            property.Converter = new MaskedDataConverter(maskedLoggingAttribute.RegexPattern, maskedLoggingAttribute.MaskingChar);
                    }
                }
            }

            return property;
        }
    }
}