using UnityEngine;
using UnityEngine.InputSystem;

namespace OverPower.Unity.Input
{
    public class GameInputReader : MonoBehaviour, IGameInput
    {
        [SerializeField] private InputActionAsset inputActionAsset;

        private InputActionMap _uiMap;
        private InputActionMap _gameplayMap;

        private InputAction _uiNavigate;
        private InputAction _uiPoint;
        private InputAction _uiClick;
        private InputAction _uiSubmit;
        private InputAction _uiCancel;
        private InputAction _uiOpenDetails;

        private InputAction _gpNavigate;
        private InputAction _gpPoint;
        private InputAction _gpClick;
        private InputAction _gpSubmit;
        private InputAction _gpCancel;
        private InputAction _gpEndTurn;
        private InputAction _gpOpenDetails;

        private void Awake()
        {
            if (inputActionAsset == null)
            {
                inputActionAsset = Resources.Load<InputActionAsset>("Input/OverPowerInputActions");
                if (inputActionAsset == null)
                {
                    Debug.LogError("[GameInputReader] InputActionAsset is not assigned and could not be loaded from Resources!");
                    return;
                }
            }

            // Create a local instance of the asset to prevent mutating the source asset at runtime
            inputActionAsset = Instantiate(inputActionAsset);

            // Initialize UI Map
            _uiMap = inputActionAsset.FindActionMap("UI", true);
            _uiNavigate = _uiMap.FindAction("Navigate", true);
            _uiPoint = _uiMap.FindAction("Point", true);
            _uiClick = _uiMap.FindAction("Click", true);
            _uiSubmit = _uiMap.FindAction("Submit", true);
            _uiCancel = _uiMap.FindAction("Cancel", true);
            _uiOpenDetails = _uiMap.FindAction("OpenDetails", true);

            // Initialize Gameplay Map
            _gameplayMap = inputActionAsset.FindActionMap("Gameplay", true);
            _gpNavigate = _gameplayMap.FindAction("Navigate", true);
            _gpPoint = _gameplayMap.FindAction("Point", true);
            _gpClick = _gameplayMap.FindAction("Click", true);
            _gpSubmit = _gameplayMap.FindAction("Submit", true);
            _gpCancel = _gameplayMap.FindAction("Cancel", true);
            _gpEndTurn = _gameplayMap.FindAction("EndTurn", true);
            _gpOpenDetails = _gameplayMap.FindAction("OpenDetails", true);

            // Default: Enable UI Input Map
            EnableUIInput();
            Debug.Log("[GameInputReader] Initialized and UI action map enabled.");
        }

        public Vector2 Navigate => _gameplayMap.enabled ? _gpNavigate.ReadValue<Vector2>() : (_uiMap.enabled ? _uiNavigate.ReadValue<Vector2>() : Vector2.zero);
        public Vector2 Point => _gameplayMap.enabled ? _gpPoint.ReadValue<Vector2>() : (_uiMap.enabled ? _uiPoint.ReadValue<Vector2>() : Vector2.zero);

        public bool IsClickPressed => _gameplayMap.enabled ? _gpClick.IsPressed() : (_uiMap.enabled ? _uiClick.IsPressed() : false);
        public bool WasSubmitPressedThisFrame => _gameplayMap.enabled ? _gpSubmit.WasPressedThisFrame() : (_uiMap.enabled ? _uiSubmit.WasPressedThisFrame() : false);
        public bool WasCancelPressedThisFrame => _gameplayMap.enabled ? _gpCancel.WasPressedThisFrame() : (_uiMap.enabled ? _uiCancel.WasPressedThisFrame() : false);
        public bool WasEndTurnPressedThisFrame => _gameplayMap.enabled ? _gpEndTurn.WasPressedThisFrame() : false;
        public bool WasOpenDetailsPressedThisFrame => _gameplayMap.enabled ? _gpOpenDetails.WasPressedThisFrame() : (_uiMap.enabled ? _uiOpenDetails.WasPressedThisFrame() : false);

        public void EnableGameplayInput()
        {
            _uiMap.Disable();
            _gameplayMap.Enable();
            Debug.Log("[GameInputReader] Gameplay input map enabled.");
        }

        public void EnableUIInput()
        {
            _gameplayMap.Disable();
            _uiMap.Enable();
            Debug.Log("[GameInputReader] UI input map enabled.");
        }

        public void DisableAllInput()
        {
            _uiMap.Disable();
            _gameplayMap.Disable();
            Debug.Log("[GameInputReader] All input maps disabled.");
        }

        private void OnDestroy()
        {
            DisableAllInput();
            if (inputActionAsset != null)
            {
                Destroy(inputActionAsset);
            }
        }
    }
}
