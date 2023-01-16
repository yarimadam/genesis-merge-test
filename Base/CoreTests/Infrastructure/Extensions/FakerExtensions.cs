using System;
using System.Globalization;
using System.Text;
using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;

namespace CoreTests.Infrastructure.Extensions
{
    public static class FakerExtensions
    {
        public static T RandomByType<T>(this Faker faker,
            bool isNullable = false,
            int? maxLength = null,
            int? minValue = null, int? maxValue = null)
        {
            object returnValue = Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Int32 => minValue != null && maxValue != null
                    ? faker.Random.Int((int) minValue, (int) maxValue)
                    : minValue != null
                        ? faker.Random.Int((int) minValue)
                        : maxValue != null
                            ? faker.Random.Int(max: (int) maxValue)
                            : faker.Random.Int(),
                TypeCode.Decimal => minValue != null && maxValue != null
                    ? faker.Random.Decimal((int) minValue, (int) maxValue)
                    : minValue != null
                        ? faker.Random.Decimal((int) minValue)
                        : maxValue != null
                            ? faker.Random.Decimal(max: (int) maxValue)
                            : faker.Random.Decimal(),
                TypeCode.Boolean => faker.Random.Bool(),
                TypeCode.Char => faker.Random.Char(),
                TypeCode.SByte => faker.Random.SByte(),
                TypeCode.Byte => faker.Random.Byte(),
                TypeCode.Int16 => minValue != null && maxValue != null
                    ? faker.Random.Short((short) minValue, (short) maxValue)
                    : minValue != null
                        ? faker.Random.Short((short) minValue)
                        : maxValue != null
                            ? faker.Random.Short(max: (short) maxValue)
                            : faker.Random.Short(),
                TypeCode.UInt16 => minValue != null && maxValue != null
                    ? faker.Random.UShort((ushort) minValue, (ushort) maxValue)
                    : minValue != null
                        ? faker.Random.UShort((ushort) minValue)
                        : maxValue != null
                            ? faker.Random.UShort(max: (ushort) maxValue)
                            : faker.Random.UShort(),
                TypeCode.UInt32 => minValue != null && maxValue != null
                    ? faker.Random.UInt((uint) minValue, (uint) maxValue)
                    : minValue != null
                        ? faker.Random.UInt((uint) minValue)
                        : maxValue != null
                            ? faker.Random.UInt(max: (uint) maxValue)
                            : faker.Random.UInt(),
                TypeCode.Int64 => minValue != null && maxValue != null
                    ? faker.Random.Long((long) minValue, (long) maxValue)
                    : minValue != null
                        ? faker.Random.Long((long) minValue)
                        : maxValue != null
                            ? faker.Random.Long(max: (long) maxValue)
                            : faker.Random.Long(),
                TypeCode.UInt64 => minValue != null && maxValue != null
                    ? faker.Random.ULong((ulong) minValue, (ulong) maxValue)
                    : minValue != null
                        ? faker.Random.ULong((ulong) minValue)
                        : maxValue != null
                            ? faker.Random.ULong(max: (ulong) maxValue)
                            : faker.Random.ULong(),
                TypeCode.Single => minValue != null && maxValue != null
                    ? faker.Random.Float((float) minValue, (float) maxValue)
                    : minValue != null
                        ? faker.Random.Float((float) minValue)
                        : maxValue != null
                            ? faker.Random.Float(max: (float) maxValue)
                            : faker.Random.Float(),
                TypeCode.Double => minValue != null && maxValue != null
                    ? faker.Random.Double((double) minValue, (double) maxValue)
                    : minValue != null
                        ? faker.Random.Double((double) minValue)
                        : maxValue != null
                            ? faker.Random.Double(max: (double) maxValue)
                            : faker.Random.Double(),
                TypeCode.DateTime => faker.Random.Float() > 0.2
                    ? faker.Date.Recent()
                    : faker.Date.Past(),
                TypeCode.String => faker.Lorem.Text(maxLength: maxLength, asByteLength: true),
                _ => Activator.CreateInstance<T>()
            };

            if (isNullable)
                returnValue = returnValue.OrNull(faker, 0.4f);

            return (T) returnValue;
        }

        public static string Text(this Lorem lorem, int? minLength = null, int? maxLength = null, bool asByteLength = false)
        {
            const char ellipsisChar = '.';
            const int avgSentenceLength = 60;
            const string separator = " ";

            var encoding = Encoding.UTF8;

            int GetLength(string text) =>
                text == null
                    ? 0
                    : asByteLength
                        ? encoding.GetByteCount(text)
                        : text.Length;

            string Substring(string text)
            {
                var length = GetLength(text);

                if (length > maxLength)
                {
                    bool ellipsis = maxLength > 3;

                    length = (int) (ellipsis
                        ? maxLength - 3
                        : maxLength);

                    text = asByteLength
                        ? text.OfMaxBytes(length)
                        : text.Substring(0, length);

                    if (ellipsis)
                        text += new string(ellipsisChar, 3);

                    var currentLength = GetLength(text);
                    if (currentLength < minLength)
                        text = new string(ellipsisChar, (int) (minLength - currentLength)) + text;
                }

                return text;
            }

            var minSentence = minLength / avgSentenceLength ?? 1;
            var maxSentence = maxLength / avgSentenceLength ?? 1;

            var returnValue = lorem.Sentences(lorem.Random.Number(minSentence, maxSentence), separator);

            if (minLength != null)
                while (GetLength(returnValue) < minLength)
                    returnValue += lorem.Sentence();

            if (maxLength != null)
                returnValue = Substring(returnValue);

            return returnValue;
        }

        public static string OfMaxBytes(this string input, int maxBytes)
        {
            if (maxBytes == 0 || string.IsNullOrEmpty(input))
                return string.Empty;

            var encoding = Encoding.UTF8;
            if (encoding.GetByteCount(input) <= maxBytes)
                return input;

            var sb = new StringBuilder();
            var bytes = 0;
            var enumerator = StringInfo.GetTextElementEnumerator(input);
            while (enumerator.MoveNext())
            {
                var textElement = enumerator.GetTextElement();
                bytes += encoding.GetByteCount(textElement);
                if (bytes <= maxBytes)
                    sb.Append(textElement);
                else
                    break;
            }

            return sb.ToString();
        }
    }
}