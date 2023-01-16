using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Serilog;

namespace CoreData.Common.Converters
{
    public class MaskedDataConverter : JsonConverter
    {
        private readonly string _maskingChar;
        private readonly Regex _regexPattern;

        //private Func<string, string> MaskerDelegate { get; }

        public MaskedDataConverter(string regexPattern, string maskingChar)
        {
            _maskingChar = maskingChar;
            _regexPattern = new Regex(regexPattern);
        }

//        public MaskedDataConverter(Func<string, string> maskerDelegate)
//        {
//            MaskerDelegate = maskerDelegate;
//        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string maskedValue = null;
            string stringValue = value?.ToString();

            if (stringValue != null)
            {
                try
                {
                    // Find matches globally in given string. 
                    var matches = _regexPattern.Matches(stringValue);

                    if (matches.Any())
                    {
                        maskedValue = "";
                        var lastIndex = 0;
                        foreach (Match match in matches)
                            // Every match should include at least one regex group (except original group) to proceed.
                            if (match.Groups.Count >= 2)
                                for (var i = 1; i < match.Groups.Count; i++)
                                {
                                    // Save positions of current group that define the boundaries to be masked. 
                                    var groupInd = match.Groups[i].Index;
                                    var groupLength = match.Groups[i].Length;

                                    // Gets unprocessed chars without masking until group boundary then masks between group boundaries.  
                                    maskedValue += stringValue.Substring(lastIndex, groupInd - lastIndex)
                                                   + string.Join("", Enumerable.Range(0, groupLength).Select(x => _maskingChar));

                                    // Mark last processed index to prevent losing data when no matching.
                                    lastIndex = groupInd + groupLength;
                                }

                        // Appends rest of the unprocessed string. 
                        maskedValue += stringValue.Substring(lastIndex, stringValue.Length - lastIndex);
                    }
                    else
                        // Fully masks the string to prevent any security leaks if regex pattern is not matched.
                        maskedValue = GetFullyMaskedValue(stringValue);
                }
                catch (Exception e)
                {
                    // Fully masks the string to prevent any security leaks if there are any errors.
                    maskedValue = GetFullyMaskedValue(stringValue);

                    Log.Fatal(e, "MaskedDataConverter.WriteJson");
                }
            }

            serializer.Serialize(writer, maskedValue);
        }

        private string GetFullyMaskedValue(string stringValue)
        {
            // Regex.Replace(stringValue, @"[\s\S]", _maskingChar);
            return string.Join("", Enumerable.Range(0, stringValue.Length).Select(x => _maskingChar));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }
    }
}