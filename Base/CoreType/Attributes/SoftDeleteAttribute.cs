using System;

namespace CoreType.Attributes
{
    /// <summary>
    /// <example><code>[SoftDelete(nameof(Status), (int)Types.Status.Deleted)]</code> on top of the database model you want to enable soft deletion.</example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SoftDeleteAttribute : Attribute
    {
        public string PropertyName { get; }
        public object ValueToBeAssigned { get; }

        public SoftDeleteAttribute(string propertyName, object valueToBeAssigned)
        {
            PropertyName = propertyName;
            ValueToBeAssigned = valueToBeAssigned;
        }
    }
}