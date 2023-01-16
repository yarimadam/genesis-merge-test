using System;
using System.Linq;
using CoreData.CacheManager;

namespace CoreData.Common
{
    public class LocalizedMessage
    {
        private readonly string _keyCode;

        public LocalizedMessage(string keyCode)
        {
            _keyCode = keyCode;
        }

        public string GetKeyCode() => _keyCode;

        public static implicit operator string(LocalizedMessage localizedMessage) => localizedMessage.ToString();

        public static implicit operator LocalizedMessage(string keyCode) => new LocalizedMessage(keyCode);

        public string ToDefaultLang()
        {
            if (_keyCode == null)
                throw new ArgumentNullException(nameof(_keyCode));

            if (Constants.DEFAULT_LANGUAGE == null)
                throw new ArgumentNullException(nameof(Constants.DEFAULT_LANGUAGE));

            return DistributedCache.GetParam(_keyCode, Constants.DEFAULT_LANGUAGE) ?? string.Empty;
        }

        public string ToSpecificLang(string lang)
        {
            if (_keyCode == null)
                throw new ArgumentNullException(nameof(_keyCode));

            if (lang == null)
                throw new ArgumentNullException(nameof(lang));

            return DistributedCache.GetParam(_keyCode, lang) ?? string.Empty;
        }

        public string ToString(object arg, params object[] args)
        {
            args ??= new object[] { };

            args = args.Append(arg).ToArray();

            return string.Format(ToString(), args);
        }

        public override string ToString()
        {
            if (_keyCode == null)
                throw new ArgumentNullException(nameof(_keyCode));

            return DistributedCache.GetParam(_keyCode) ?? string.Empty;
        }
    }
}