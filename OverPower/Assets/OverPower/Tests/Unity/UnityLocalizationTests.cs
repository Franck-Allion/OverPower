using NUnit.Framework;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class UnityLocalizationTests
    {
        [SetUp]
        public void SetUp()
        {
            // Force synchronous initialization of the Unity Localization system
            var op = LocalizationSettings.InitializationOperation;
            op.WaitForCompletion();
        }

        [Test]
        public void Localization_EnglishAndFrench_LocalesAreConfigured()
        {
            var provider = LocalizationSettings.AvailableLocales;
            Assert.IsNotNull(provider, "AvailableLocales provider must be configured.");

            var enLocale = provider.Locales.FirstOrDefault(l => l.Identifier.Code == "en");
            var frLocale = provider.Locales.FirstOrDefault(l => l.Identifier.Code == "fr");

            Assert.IsNotNull(enLocale, "English locale 'en' must be configured.");
            Assert.IsNotNull(frLocale, "French locale 'fr' must be configured.");
        }

        [Test]
        public void Localization_CoreKeys_ExistAndAreTranslated()
        {
            var stringDatabase = LocalizationSettings.StringDatabase;
            Assert.IsNotNull(stringDatabase, "StringDatabase must be configured.");

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
                var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(l => l.Identifier.Code == localeCode);
                Assert.IsNotNull(locale, $"Locale '{localeCode}' must be available.");

                foreach (var key in requiredKeys)
                {
                    string value = stringDatabase.GetLocalizedString("UI.Core", key, locale);

                    Assert.IsFalse(string.IsNullOrEmpty(value),
                        $"Key '{key}' in locale '{localeCode}' must have a non-empty translation.");
                }
            }
        }
    }
}
