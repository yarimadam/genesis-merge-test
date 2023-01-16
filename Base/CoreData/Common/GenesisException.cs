using System;
using CoreData.CacheManager;
using Serilog;

namespace CoreData.Common
{
    public class GenesisException : Exception
    {
        // TODO Update hardcoded messages for all usages then localize
        public GenesisException(string message)
            : base(message)
        {
        }

        public GenesisException(LocalizedMessage localizedMessage, params object[] args)
            : base(LocalizeException(localizedMessage, args, true))
        {
        }

        public GenesisException(string message, params object[] args)
            : base(LocalizeException(message, args))
        {
        }

        public GenesisException(string message, Exception innerException, params object[] args)
            : base(LocalizeException(message, args), innerException)
        {
        }

        private static string LocalizeException(string message, object[] args, bool isAlreadResolved = false)
        {
            try
            {
                if (!isAlreadResolved)
                    message = DistributedCache.GetParam(message);

                return string.Format(message, args);
            }
            catch (Exception e)
            {
                Log.Error(e, "LocalizeException");
                return string.Format(message, args);
            }
        }
    }
}