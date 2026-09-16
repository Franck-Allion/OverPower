using NUnit.Framework;
using System.Reflection;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UnityLocalizationTests
    {
        private LocalizationSettings GetActiveSettings()
        {
            var settings = UnityEditor.Localization.LocalizationEditorSettings.ActiveLocalizationSettings;
            Assert.IsNotNull(settings, "ActiveLocalizationSettings must be configured and not null.");
            return settings;
        }

        private ILocalesProvider GetLocalesProvider(LocalizationSettings settings)
        {
            var field = typeof(LocalizationSettings).GetField("m_AvailableLocales", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field, "m_AvailableLocales field must exist.");
            var provider = field.GetValue(settings) as ILocalesProvider;
            Assert.IsNotNull(provider, "ILocalesProvider must be assigned.");
            return provider;
        }

        [Test]
        public void Localization_EnglishAndFrench_LocalesAreConfigured()
        {
            var settings = GetActiveSettings();
            var provider = GetLocalesProvider(settings);

            var enLocale = provider.Locales.FirstOrDefault(l => l.Identifier.Code == "en");
            var frLocale = provider.Locales.FirstOrDefault(l => l.Identifier.Code == "fr");

            Assert.IsNotNull(enLocale, "English locale 'en' must be configured.");
            Assert.IsNotNull(frLocale, "French locale 'fr' must be configured.");
        }

        [Test]
        public void Localization_CoreKeys_ExistAndAreTranslated()
        {
            var settings = GetActiveSettings();
            var provider = GetLocalesProvider(settings);

            var collection = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollection("UI.Core");
            Assert.IsNotNull(collection, "UI.Core string table collection must exist.");

            var requiredKeys = new string[] {
                "ui.main_menu.title",
                "ui.main_menu.play",
                "ui.main_menu.settings",
                "ui.main_menu.language",
                "ui.main_menu.exit"
            };

            var localeCodes = new string[] { "en", "fr" };

            foreach (var localeCode in localeCodes)
            {
                var locale = provider.Locales.FirstOrDefault(l => l.Identifier.Code == localeCode);
                Assert.IsNotNull(locale, $"Locale '{localeCode}' must be available.");

                var table = collection.GetTable(locale.Identifier) as StringTable;
                Assert.IsNotNull(table, $"StringTable for locale '{localeCode}' must exist in UI.Core collection.");

                foreach (var key in requiredKeys)
                {
                    var entry = table.GetEntry(key);
                    Assert.IsNotNull(entry, $"Key '{key}' must exist in table '{localeCode}'.");
                    Assert.IsFalse(string.IsNullOrEmpty(entry.LocalizedValue),
                        $"Key '{key}' in table '{localeCode}' must have a non-empty localized value.");
                }
            }
        }
    }
}
