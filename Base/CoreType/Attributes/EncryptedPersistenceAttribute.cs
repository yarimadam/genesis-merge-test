using CoreType.Types;

namespace CoreType.Attributes
{
    public class EncryptedPersistenceAttribute : PersistenceConverterAttribute
    {
        public readonly string SymmetricKey;

        public EncryptedPersistenceAttribute(string symmetricKey = null) : base(PersistenceOptions.Encrypt)
        {
            SymmetricKey = symmetricKey;
        }
    }
}