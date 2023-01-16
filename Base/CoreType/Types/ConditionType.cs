namespace CoreType.Types
{
    /// <summary>
    /// <remarks>Supported methods by client;
    /// <code>Equals	equals	Text, Number, Date
    /// Not Equals	notEqual	Text, Number, Date
    /// Contains	contains	Text
    /// Not Contains	notContains	Text
    /// Starts With	startsWith	Text
    /// Ends With	endsWith	Text
    /// Less Than	lessThan	Number, Date
    /// Less Than or Equal	lessThanOrEqual	Number
    /// Greater Than	greaterThan	Number, Date
    /// Greater Than or Equal	greaterThanOrEqual	Number
    /// In Range	inRange	Number, Date
    /// Empty*	empty	Text, Number, Date
    /// </code> 
    /// </remarks>
    /// </summary>
    public enum ConditionType
    {
        Equals,
        NotEquals,
        Contains,
        NotContains,
        StartsWith,
        EndsWith,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        InRange
    };
}