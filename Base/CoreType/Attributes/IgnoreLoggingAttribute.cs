using CoreType.Types;

namespace CoreType.Attributes
{
    public class IgnoreLoggingAttribute : BaseLoggingAttribute
    {
        public IgnoreLoggingAttribute()
        {
            LoggingBehaviour = LoggingBehaviour.IgnoreSerialization;
        }
    }
}