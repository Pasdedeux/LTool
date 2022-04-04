using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Button), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the Button Component.
    ///   Extend this class to write a custom editor for a component derived from Button.
    /// </summary>
    public class ButtonEditor : SelectableEditor
    {
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_BtnVolumeProperty;
        SerializedProperty m_BtnAudioProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_BtnVolumeProperty= serializedObject.FindProperty("m_BtnVolume");
            m_BtnAudioProperty = serializedObject.FindProperty("m_BtnAudio");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_BtnVolumeProperty);
            EditorGUILayout.PropertyField(m_BtnAudioProperty);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_OnClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
