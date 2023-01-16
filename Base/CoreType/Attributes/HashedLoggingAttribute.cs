using CoreType.Types;

namespace CoreType.Attributes
{
    public class HashedLoggingAttribute : BaseLoggingAttribute
    {
        public HashedLoggingAttribute()
        {
            LoggingBehaviour = LoggingBehaviour.Hash;
        }
    }
}