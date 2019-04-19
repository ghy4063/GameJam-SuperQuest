using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = AmbientOcclusionModel.Settings;

    [PostProcessingModelEditor(typeof(AmbientOcclusionModel))]
    public class AmbientOcclusionModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_Intensity;
        SerializedProperty m_Radius;
        SerializedProperty m_SampleCount;
        SerializedProperty m_Downsampling;
        SerializedProperty m_ForceForwardCompatibility;
        SerializedProperty m_AmbientOnly;
        SerializedProperty m_HighPrecision;

        public override void OnEnable()
        {
			this.m_Intensity = this.FindSetting((Settings x) => x.intensity);
			this.m_Radius = this.FindSetting((Settings x) => x.radius);
			this.m_SampleCount = this.FindSetting((Settings x) => x.sampleCount);
			this.m_Downsampling = this.FindSetting((Settings x) => x.downsampling);
			this.m_ForceForwardCompatibility = this.FindSetting((Settings x) => x.forceForwardCompatibility);
			this.m_AmbientOnly = this.FindSetting((Settings x) => x.ambientOnly);
			this.m_HighPrecision = this.FindSetting((Settings x) => x.highPrecision);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.m_Intensity);
            EditorGUILayout.PropertyField(this.m_Radius);
            EditorGUILayout.PropertyField(this.m_SampleCount);
            EditorGUILayout.PropertyField(this.m_Downsampling);
            EditorGUILayout.PropertyField(this.m_ForceForwardCompatibility);
            EditorGUILayout.PropertyField(this.m_HighPrecision, EditorGUIHelper.GetContent("High Precision (Forward)"));

            using (new EditorGUI.DisabledGroupScope(this.m_ForceForwardCompatibility.boolValue))
                EditorGUILayout.PropertyField(this.m_AmbientOnly, EditorGUIHelper.GetContent("Ambient Only (Deferred + HDR)"));
        }
    }
}
