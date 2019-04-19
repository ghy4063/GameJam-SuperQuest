using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = DepthOfFieldModel.Settings;

    [PostProcessingModelEditor(typeof(DepthOfFieldModel))]
    public class DepthOfFieldModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_FocusDistance;
        SerializedProperty m_Aperture;
        SerializedProperty m_FocalLength;
        SerializedProperty m_UseCameraFov;
        SerializedProperty m_KernelSize;

        public override void OnEnable()
        {
			this.m_FocusDistance = this.FindSetting((Settings x) => x.focusDistance);
			this.m_Aperture = this.FindSetting((Settings x) => x.aperture);
			this.m_FocalLength = this.FindSetting((Settings x) => x.focalLength);
			this.m_UseCameraFov = this.FindSetting((Settings x) => x.useCameraFov);
			this.m_KernelSize = this.FindSetting((Settings x) => x.kernelSize);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.m_FocusDistance);
            EditorGUILayout.PropertyField(this.m_Aperture, EditorGUIHelper.GetContent("Aperture (f-stop)"));

            EditorGUILayout.PropertyField(this.m_UseCameraFov, EditorGUIHelper.GetContent("Use Camera FOV"));
            if (!this.m_UseCameraFov.boolValue)
                EditorGUILayout.PropertyField(this.m_FocalLength, EditorGUIHelper.GetContent("Focal Length (mm)"));

            EditorGUILayout.PropertyField(this.m_KernelSize);
        }
    }
}
