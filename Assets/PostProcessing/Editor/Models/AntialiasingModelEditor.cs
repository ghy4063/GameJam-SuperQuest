using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Method = AntialiasingModel.Method;
    using Settings = AntialiasingModel.Settings;

    [PostProcessingModelEditor(typeof(AntialiasingModel))]
    public class AntialiasingModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_Method;

        SerializedProperty m_FxaaPreset;

        SerializedProperty m_TaaJitterSpread;
        SerializedProperty m_TaaSharpen;
        SerializedProperty m_TaaStationaryBlending;
        SerializedProperty m_TaaMotionBlending;

        static string[] s_MethodNames =
        {
            "Fast Approximate Anti-aliasing",
            "Temporal Anti-aliasing"
        };

        public override void OnEnable()
        {
			this.m_Method = this.FindSetting((Settings x) => x.method);

			this.m_FxaaPreset = this.FindSetting((Settings x) => x.fxaaSettings.preset);

			this.m_TaaJitterSpread = this.FindSetting((Settings x) => x.taaSettings.jitterSpread);
			this.m_TaaSharpen = this.FindSetting((Settings x) => x.taaSettings.sharpen);
			this.m_TaaStationaryBlending = this.FindSetting((Settings x) => x.taaSettings.stationaryBlending);
			this.m_TaaMotionBlending = this.FindSetting((Settings x) => x.taaSettings.motionBlending);
        }

        public override void OnInspectorGUI()
        {
			this.m_Method.intValue = EditorGUILayout.Popup("Method", this.m_Method.intValue, s_MethodNames);

            if (this.m_Method.intValue == (int)Method.Fxaa)
            {
                EditorGUILayout.PropertyField(this.m_FxaaPreset);
            }
            else if (this.m_Method.intValue == (int)Method.Taa)
            {
                if (QualitySettings.antiAliasing > 1)
                    EditorGUILayout.HelpBox("Temporal Anti-Aliasing doesn't work correctly when MSAA is enabled.", MessageType.Warning);

                EditorGUILayout.LabelField("Jitter", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_TaaJitterSpread, EditorGUIHelper.GetContent("Spread"));
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Blending", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_TaaStationaryBlending, EditorGUIHelper.GetContent("Stationary"));
                EditorGUILayout.PropertyField(this.m_TaaMotionBlending, EditorGUIHelper.GetContent("Motion"));
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(this.m_TaaSharpen);
            }
        }
    }
}
