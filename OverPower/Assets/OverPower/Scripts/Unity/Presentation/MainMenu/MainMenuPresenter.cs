using UnityEngine;
using UnityEngine.UI;
using OverPower.Application;
using OverPower.Unity.Bootstrap;

namespace OverPower.Unity.Presentation.MainMenu
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private Button playButton;

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

        private void OnDestroy()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayClicked);
            }
        }
    }
}
