using System.Linq;
using CoreType.Types;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace CoreTests.Infrastructure.Extensions
{
    public static class FluentAssertionsExtensions
    {
        public static ResponseWrapperAssertions Should<T>(this ResponseWrapper<T> instance)
        {
            return new ResponseWrapperAssertions(instance);
        }
    }

    public class ResponseWrapperAssertions : ReferenceTypeAssertions<ResponseWrapper<dynamic>, ResponseWrapperAssertions>
    {
        protected override string Identifier => nameof(ResponseWrapper);

        public ResponseWrapperAssertions(ResponseWrapper<dynamic> instance) : base(instance)
        {
        }

        public AndConstraint<ResponseWrapperAssertions> BeValid(string because = "", params object[] becauseArgs)
        {
            DefaultValidAssertion(because, becauseArgs);

            return new AndConstraint<ResponseWrapperAssertions>(this);
        }

        public AndConstraint<ResponseWrapperAssertions> NotBeValid(string because = "", params object[] becauseArgs)
        {
            DefaultInvalidAssertion(because, becauseArgs);

            return new AndConstraint<ResponseWrapperAssertions>(this);
        }

        public AndConstraint<ResponseWrapperAssertions> BeValidWithData(string because = "", params object[] becauseArgs)
        {
            DefaultValidAssertion(because, becauseArgs)
                .Then
                .ForCondition(Subject.Data != null)
                .FailWith($"but found its {nameof(ResponseWrapper.Data)} property is <null>.");

            return new AndConstraint<ResponseWrapperAssertions>(this);
        }

        private Continuation DefaultValidAssertion(string because, object[] becauseArgs)
        {
            return Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .WithExpectation("Expected response to be valid{reason}, ")
                .ForCondition(Subject != null)
                .FailWith("but found {context} is <null>.")
                .Then
                // ReSharper disable once PossibleNullReferenceException
                .ForCondition(!Subject.Errors.Any())
                .FailWith("but found error(s) \n{0}\n",
                    () => string.Join("\n\n",
                        Subject.Errors
                            .Select(x =>
                                $"{(!string.IsNullOrEmpty(x.Code) ? $"(Code: {x.Code}) " : string.Empty)}\nMessage: {x.Message}\nStackTrace: {x.StackTrace}")))
                .Then
                .ForCondition(Subject.Success)
                .FailWith("but it was not successful.");
        }

        private Continuation DefaultInvalidAssertion(string because, object[] becauseArgs)
        {
            return Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .WithExpectation("Expected response not to be valid{reason}, ")
                .ForCondition(Subject != null)
                .FailWith("but found {context} is <null>.")
                .Then
                // ReSharper disable once PossibleNullReferenceException
                .ForCondition(Subject.Errors.Any())
                .FailWith("but not found any errors.")
                .Then
                .ForCondition(!Subject.Success)
                .FailWith("but it was successful.");
        }
    }
}