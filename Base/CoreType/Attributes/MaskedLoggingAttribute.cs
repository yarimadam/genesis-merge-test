using System;
using System.Text.RegularExpressions;
using CoreType.Types;

namespace CoreType.Attributes
{
    public class MaskedLoggingAttribute : BaseLoggingAttribute
    {
        public readonly string RegexPattern;
        public readonly string MaskingChar;

        private static readonly Regex regexPatternCheck = new Regex(@"(?<!\\)\((?!\?:)[\s\S]*?(?<!\\)\)");

        public MaskedLoggingAttribute(string regexPattern = @"([\s\S]*)", string maskingChar = "*")
        {
            if (string.IsNullOrWhiteSpace(regexPattern))
                throw new ArgumentNullException(nameof(regexPattern));
            if (!regexPatternCheck.IsMatch(regexPattern))
                throw new ArgumentException($"{nameof(regexPattern)} must at least one RegEx group !");

            maskingChar ??= "";

            LoggingBehaviour = LoggingBehaviour.Mask;
            RegexPattern = regexPattern;
            MaskingChar = maskingChar;
        }
    }
}