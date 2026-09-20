using UnityEditor;

namespace OverPower.Unity.Presentation.DesignSystem.Editor
{
    [CustomEditor(typeof(UIButton))]
    [CanEditMultipleObjects]
    public sealed class UIButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            foreach (string field in new[] { "_config", "_family", "_visual", "_surface", "_border",
                "_focusMark", "_disabledMark", "_visualOpacity", "_label", "_icon", "_accessibleLabel" })
                EditorGUILayout.PropertyField(serializedObject.FindProperty(field));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
