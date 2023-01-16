using System;
using CoreType.Types;

namespace CoreType.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class BaseLoggingAttribute : Attribute
    {
        public LoggingBehaviour LoggingBehaviour { get; protected set; } = LoggingBehaviour.IgnoreSerialization;
    }
}