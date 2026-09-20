using System;
using System.Text.RegularExpressions;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>
    /// Presentation utility to format custom semantic brace tokens (e.g., {health}) into TextMeshPro Rich Text sprite tags.
    /// </summary>
    public static class UIRichTextFormatter
    {
        private static readonly Regex TokenRegex = new Regex(@"\{([a-zA-Z0-9_\-]+)\}", RegexOptions.Compiled);

        /// <summary>
        /// Converts semantic brace tokens inside a string (e.g., {health}) to TMP sprite tags (e.g., &lt;sprite name="health"&gt;).
        /// Unknown tokens are left untouched and unchanged.
        /// </summary>
        public static string Format(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text ?? string.Empty;
            }

            return TokenRegex.Replace(text, match =>
            {
                string tokenName = match.Groups[1].Value.ToLower();

                // Only replace the four explicitly supported semantic tokens of the design system.
                if (tokenName == "attack" || tokenName == "health" || tokenName == "gold" || tokenName == "mana")
                {
                    return $"<sprite name=\"{tokenName}\">";
                }

                // Leave unknown tokens unchanged.
                return match.Value;
            });
        }
    }
}
