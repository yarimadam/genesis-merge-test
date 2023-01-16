using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreData.Common
{
    public static class NameReaderExtensions
    {
        private const string expressionCannotBeNullMessage = "The expression cannot be null.";
        private const string invalidExpressionMessage = "Invalid expression.";

        public static List<string> GetMemberNames<T>(params Expression<Func<T, object>>[] expressions)
        {
            var resultType = typeof(T);
            List<string> memberNames = new List<string>();

            foreach (var cExpression in expressions)
            {
                if (cExpression.Body is NewExpression newExpression)
                    memberNames.AddRange(GetMemberNames(newExpression));
                else
                    memberNames.Add(GetMemberName(cExpression.Body));
            }

            return memberNames
                .Select(memberName => $"{resultType.Name}.{memberName}")
                .ToList();
        }

        public static List<string> GetMemberNames(NewExpression newExpression)
        {
            return newExpression.Arguments
                .Select(x => string.Join(".", x.ToString().Split(".").Skip(1)))
                .ToList();
        }

        public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            List<string> memberNames = new List<string>();
            foreach (var cExpression in expressions)
            {
                memberNames.Add(GetMemberName(cExpression.Body));
            }

            return memberNames;
        }

        public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }

        public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression)
        {
            return GetMemberName(expression.Body);
        }

        public static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(expressionCannotBeNullMessage);
            }

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpressionInfo = ((MemberExpression) expression).Member;
                return memberExpressionInfo.DeclaringType.Name + "." + memberExpressionInfo.Name;
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression) expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression) expression;
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException(invalidExpressionMessage);
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression) unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression) unaryExpression.Operand).Member.Name;
        }
    }
}