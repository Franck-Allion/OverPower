using UnityEngine;
using UnityEngine.UI;
using OverPower.Application;
using OverPower.Unity.Bootstrap;

namespace OverPower.Unity.Presentation.MainMenu
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button languageButton;

        private void Start()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
                Debug.Log("[MainMenuPresenter] Play button listener attached.");
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] Play button reference is missing!");
            }

            if (languageButton != null)
            {
                languageButton.onClick.AddListener(OnLanguageClicked);
                Debug.Log("[MainMenuPresenter] Language button listener attached.");
            }
        }

        private async void OnPlayClicked()
        {
            Debug.Log("[MainMenuPresenter] Play button clicked.");
            IGameFlowController gameFlow = GameBootstrap.GetGameFlow();
            if (gameFlow != null)
            {
                if (playButton != null) playButton.interactable = false;

                try
                {
                    await gameFlow.StartNewRunAsync();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[MainMenuPresenter] Error starting new run: {ex.Message}");
                    if (playButton != null) playButton.interactable = true;
                }
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] GameFlow controller is not available.");
            }
        }

        private async void OnLanguageClicked()
        {
            Debug.Log("[MainMenuPresenter] Language button clicked.");
            var localeService = GameBootstrap.GetLocaleService();
            if (localeService != null)
            {
                string nextLocale = localeService.CurrentLocale.Identifier.Code == "fr" ? "en" : "fr";
                if (languageButton != null) languageButton.interactable = false;

                try
                {
                    await localeService.SetLocaleAsync(nextLocale);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[MainMenuPresenter] Error switching language: {ex.Message}");
                }
                finally
                {
                    if (languageButton != null) languageButton.interactable = true;
                }
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] LocaleService is not available.");
            }
        }

        private void OnDestroy()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayClicked);
            }
            if (languageButton != null)
            {
                languageButton.onClick.RemoveListener(OnLanguageClicked);
            }
        }
    }
}
