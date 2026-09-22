using UnityEngine;
using OverPower.Application;
using OverPower.Unity.Bootstrap;
using OverPower.Domain.Run;
using OverPower.Unity.Presentation.DesignSystem;

namespace OverPower.Unity.Presentation.MainMenu
{
    /// <summary>
    /// Screen-level presenter orchestrating the main menu actions and wiring events.
    /// </summary>
    public sealed class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private UIButton playButton;
        [SerializeField] private UIButton languageButton;
        [SerializeField] private UIButton exitButton;

        private void Awake()
        {
            UnityEngine.Application.runInBackground = true;
        }

        private void Start()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] Play button reference is missing!");
            }

            if (languageButton != null)
            {
                languageButton.onClick.AddListener(OnLanguageClicked);
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] Language button reference is missing!");
            }

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClicked);
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] Exit button reference is missing!");
            }
        }

        private async void OnPlayClicked()
        {
            IGameFlowController gameFlow = GameBootstrap.GetGameFlow();
            if (gameFlow != null)
            {
                if (playButton != null) playButton.interactable = false;

                try
                {
                    var systemRandom = new System.Random();
                    byte[] buffer = new byte[8];
                    systemRandom.NextBytes(buffer);
                    ulong seedValue = System.BitConverter.ToUInt64(buffer, 0);
                    RunSeed seed = new RunSeed(seedValue);
                    await gameFlow.StartNewRunAsync(seed);
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
            var localeService = GameBootstrap.IsInitialized
                ? GameBootstrap.GetLocaleService()
                : new OverPower.Unity.Localization.UnityLocaleService();

            if (localeService != null)
            {
                string currentCode = localeService.CurrentLocale != null ? localeService.CurrentLocale.Identifier.Code : "en";
                string nextLocale = currentCode == "fr" ? "en" : "fr";
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
        }

        private void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
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
            if (exitButton != null)
            {
                exitButton.onClick.RemoveListener(OnExitClicked);
            }
        }
    }
}
