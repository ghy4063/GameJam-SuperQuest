using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = GrainModel.Settings;

    [PostProcessingModelEditor(typeof(GrainModel))]
    public class GrainModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_Colored;
        SerializedProperty m_Intensity;
        SerializedProperty m_Size;
        SerializedProperty m_LuminanceContribution;

        public override void OnEnable()
        {
			this.m_Colored = this.FindSetting((Settings x) => x.colored);
			this.m_Intensity = this.FindSetting((Settings x) => x.intensity);
			this.m_Size = this.FindSetting((Settings x) => x.size);
			this.m_LuminanceContribution = this.FindSetting((Settings x) => x.luminanceContribution);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.m_Intensity);
            EditorGUILayout.PropertyField(this.m_LuminanceContribution);
            EditorGUILayout.PropertyField(this.m_Size);
            EditorGUILayout.PropertyField(this.m_Colored);
        }
    }
}
