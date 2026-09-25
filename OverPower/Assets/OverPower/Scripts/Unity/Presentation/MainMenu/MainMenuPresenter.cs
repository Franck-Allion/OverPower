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
        [SerializeField] private UIButton settingsButton;
        [SerializeField] private UIButton exitButton;
        [SerializeField] private UISettingsModal settingsModal;
        [SerializeField] private UITransition mainPanelTransition;

        public UIButton PlayButton => playButton;
        public UIButton SettingsButton => settingsButton;
        public UIButton ExitButton => exitButton;
        public UISettingsModal SettingsModal => settingsModal;
        public UITransition MainPanelTransition => mainPanelTransition;

        private void Awake()
        {
            UnityEngine.Application.runInBackground = true;
            GameBootstrap.EnsureInitialized();
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

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClicked);
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] Exit button reference is missing!");
            }

            if (mainPanelTransition != null)
            {
                mainPanelTransition.Show();
            }
        }

        private void OnSettingsClicked()
        {
            if (settingsModal != null)
            {
                settingsModal.Open();
            }
        }

        private async void OnPlayClicked()
        {
            IGameFlowController gameFlow = GameBootstrap.GetGameFlow();
            if (gameFlow != null)
            {
                if (playButton != null) playButton.interactable = false;
                if (settingsButton != null) settingsButton.interactable = false;
                if (exitButton != null) exitButton.interactable = false;

                if (mainPanelTransition != null)
                {
                    mainPanelTransition.Hide();
                    await System.Threading.Tasks.Task.Delay(200);
                }
                else
                {
                    await System.Threading.Tasks.Task.Delay(180);
                }

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
                    if (settingsButton != null) settingsButton.interactable = true;
                    if (exitButton != null) exitButton.interactable = true;
                    if (mainPanelTransition != null) mainPanelTransition.Show();
                }
            }
            else
            {
                Debug.LogError("[MainMenuPresenter] GameFlow controller is not available.");
            }
        }

        private async void OnExitClicked()
        {
            if (playButton != null) playButton.interactable = false;
            if (settingsButton != null) settingsButton.interactable = false;
            if (exitButton != null) exitButton.interactable = false;

            if (mainPanelTransition != null)
            {
                mainPanelTransition.Hide();
            }

            await System.Threading.Tasks.Task.Delay(200);

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
            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(OnSettingsClicked);
            }
            if (exitButton != null)
            {
                exitButton.onClick.RemoveListener(OnExitClicked);
            }
        }
    }
}
