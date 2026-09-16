using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace OverPower.Unity.Localization
{
    public class UnityLocaleService
    {
        public IReadOnlyList<Locale> AvailableLocales => LocalizationSettings.AvailableLocales.Locales;

        public Locale CurrentLocale => LocalizationSettings.SelectedLocale;

        public async Task SetLocaleAsync(string localeCode)
        {
            Locale targetLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (targetLocale == null)
            {
                Debug.LogWarning($"[LocaleService] Locale code '{localeCode}' is not supported.");
                return;
            }

            Debug.Log($"[LocaleService] Switching locale to '{localeCode}'...");
            LocalizationSettings.SelectedLocale = targetLocale;

            var initializationOperation = LocalizationSettings.InitializationOperation;
            while (!initializationOperation.IsDone)
            {
                await Task.Yield();
            }
            Debug.Log($"[LocaleService] Successfully switched to locale '{localeCode}'.");
        }
    }
}
