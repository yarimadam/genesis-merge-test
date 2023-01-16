using System;
using System.Linq.Expressions;
using CoreType.Types;

namespace CoreType.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PersistenceConverterAttribute : Attribute
    {
        public readonly PersistenceOptions PersistenceOptions;
        public readonly Expression<Func<string, string>> ConvertToProviderExpression;
        public readonly Expression<Func<string, string>> ConvertFromProviderExpression;

        public PersistenceConverterAttribute(PersistenceOptions persistenceOptions)
        {
            PersistenceOptions = persistenceOptions;
        }

        public PersistenceConverterAttribute(Expression<Func<string, string>> convertToProviderExpression, Expression<Func<string, string>> convertFromProviderExpression)
        {
            ConvertToProviderExpression = convertToProviderExpression ?? throw new ArgumentNullException(nameof(ConvertToProviderExpression));
            ConvertFromProviderExpression = convertFromProviderExpression ?? throw new ArgumentNullException(nameof(ConvertFromProviderExpression));
        }
    }
}