using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreData.Common
{
    public static class LinqExtensions
    {
        public static Expression<Func<TSource, TSource>> DynamicFields<TSource>(IList<string> excludedFields, IList<string> basicTypes = null)
        {
            var resultType = typeof(TSource);
            var source = Expression.Parameter(resultType, "p");

            var result = DynamicFieldsRecursive<TSource>(excludedFields, source, null, basicTypes);
            return Expression.Lambda<Func<TSource, TSource>>(result, source);
        }

        public static MemberInitExpression DynamicFieldsRecursive<TSource>(IList<string> excludedFields, ParameterExpression source, string propertyName, IList<string> basicTypes = null)
        {
            return DynamicFieldsRecursive(typeof(TSource), excludedFields, source, propertyName, basicTypes);
        }

        public static MemberInitExpression DynamicFieldsRecursive(Type returnType, IList<string> excludedFields, ParameterExpression source, string propertyName, IList<string> basicTypes = null)
        {
            Expression pe = source;

            if (!string.IsNullOrEmpty(propertyName))
            {
                var propertyInfo = source.Type.GetProperty(propertyName);
                pe = Expression.Property(source, propertyInfo ?? throw new ArgumentException(nameof(propertyName)));
            }

            var assignments = returnType.GetProperties()
                .Where(x =>
                {
                    if (string.IsNullOrEmpty(propertyName))
                        return !excludedFields.Contains($"{returnType.Name}.{x.Name}");
                    return !excludedFields.Contains($"{propertyName}.{x.Name}");
                })
                .GroupBy(x => x.Name)
                .Select(infos => infos.First())
                .Select(property =>
                {
                    if (property == null) throw new ArgumentException(nameof(property.Name));
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var isSameType = propertyType == returnType;
                    var isPrimitiveType = propertyType.IsValueType
                                          || propertyType.IsPrimitive
                                          || isSameType || propertyType == typeof(string)
                                          || typeof(IEnumerable).IsAssignableFrom(propertyType);

                    var excludedSubFields = !isPrimitiveType
                        ? excludedFields
                            .Where(excludedField => excludedField.StartsWith($"{returnType.Name}.{property.Name}."))
                            .Select(excludedField =>
                            {
                                var propertyPrefix = $"{returnType.Name}.";
                                var index = excludedField.IndexOf(propertyPrefix, StringComparison.Ordinal);

                                return excludedField.Substring(index + propertyPrefix.Length);
                            })
                            .ToList()
                        : null;

                    if (isPrimitiveType
                        || (basicTypes != null
                            && basicTypes.Contains(property.Name)
                            && !excludedSubFields.Any(excludedField => excludedField.StartsWith($"{property.Name}."))))
                        return Expression.Bind(property, Expression.Property(pe, property));

                    return Expression.Bind(property, DynamicFieldsRecursive(property.PropertyType, excludedSubFields, source, property.Name, basicTypes));
                })
                .ToList();

            return Expression.MemberInit(Expression.New(returnType), assignments);
        }
    }
}