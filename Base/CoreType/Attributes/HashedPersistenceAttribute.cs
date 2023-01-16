using CoreType.Types;

namespace CoreType.Attributes
{
    public class HashedPersistenceAttribute : PersistenceConverterAttribute
    {
        public HashedPersistenceAttribute() : base(PersistenceOptions.Hash)
        {
        }
    }
}