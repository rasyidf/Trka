namespace Terka.Tokenizers
{
    internal static class Regexes
    {
        public static string digital_numeral = @"\d{1,4}";

        public static string roman_numeral = @"(?=[MCDLXVI]+)M{0,4}(?:CM|CD|D?C{0,3})(?:XC|XL|L?X{0,3})(?:IX|IV|V?I{0,3})";

        public static string[] english_word_numeral_list = {
    "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
    "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen", "twenty"
};
    }
}