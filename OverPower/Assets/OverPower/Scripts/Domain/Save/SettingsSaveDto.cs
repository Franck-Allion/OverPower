using System;

namespace OverPower.Domain.Save
{
    /// <summary>
    /// Minimal immutable save DTO containing persisted user settings.
    /// </summary>
    public sealed class SettingsSaveDto
    {
        /// <summary>
        /// Gets the stable technical locale identifier (e.g., "en", "fr").
        /// </summary>
        public string Locale { get; }

        /// <summary>
        /// Creates a new SettingsSaveDto with explicit validation.
        /// </summary>
        public SettingsSaveDto(string locale)
        {
            if (locale == null)
            {
                throw new ArgumentNullException(nameof(locale), "Locale cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(locale))
            {
                throw new ArgumentException("Locale cannot be empty or whitespace.", nameof(locale));
            }

            // Reject unsupported values as pure Domain constraints (FR/EN supported).
            if (locale != "en" && locale != "fr")
            {
                throw new ArgumentException($"Locale '{locale}' is not supported. Supported locales are 'en' and 'fr'.", nameof(locale));
            }

            Locale = locale;
        }
    }
}
