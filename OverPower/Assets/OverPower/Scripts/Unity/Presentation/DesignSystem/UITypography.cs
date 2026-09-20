using TMPro;
using UnityEngine;

namespace OverPower.Unity.Presentation.DesignSystem
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class UITypography : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private TypographyStyle _style = TypographyStyle.Body;
        [SerializeField] private UISemanticColor _color = UISemanticColor.TextPrimary;

        private void OnEnable() => Apply();

        public void Apply()
        {
            var text = GetComponent<TMP_Text>();
            _config.ApplyTypography(text, _style);
            text.color = _config.GetColor(_color);
        }
    }
}
