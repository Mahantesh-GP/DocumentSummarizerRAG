
using System.Text.RegularExpressions;

namespace YourNamespace.Helpers
{
    public static class TextHighlighter
    {
        public static string HighlightTerms(string text, string query)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(query))
                return text;

            var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var term in terms.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var pattern = Regex.Escape(term);
                text = Regex.Replace(
                    text,
                    pattern,
                    match => $"<strong>{match.Value}</strong>",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
                );
            }

            return text;
        }
    }
}
