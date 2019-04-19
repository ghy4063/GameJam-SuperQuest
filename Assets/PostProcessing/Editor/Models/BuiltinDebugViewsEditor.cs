using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Mode = BuiltinDebugViewsModel.Mode;
    using Settings = BuiltinDebugViewsModel.Settings;

    [PostProcessingModelEditor(typeof(BuiltinDebugViewsModel), alwaysEnabled: true)]
    public class BuiltinDebugViewsEditor : PostProcessingModelEditor
    {
        struct DepthSettings
        {
            public SerializedProperty scale;
        }

        struct MotionVectorsSettings
        {
            public SerializedProperty sourceOpacity;
            public SerializedProperty motionImageOpacity;
            public SerializedProperty motionImageAmplitude;
            public SerializedProperty motionVectorsOpacity;
            public SerializedProperty motionVectorsResolution;
            public SerializedProperty motionVectorsAmplitude;
        }

        SerializedProperty m_Mode;
        DepthSettings m_Depth;
        MotionVectorsSettings m_MotionVectors;

        public override void OnEnable()
        {
			this.m_Mode = this.FindSetting((Settings x) => x.mode);

			this.m_Depth = new DepthSettings
            {
                scale = this.FindSetting((Settings x) => x.depth.scale)
            };

			this.m_MotionVectors = new MotionVectorsSettings
            {
                sourceOpacity = this.FindSetting((Settings x) => x.motionVectors.sourceOpacity),
                motionImageOpacity = this.FindSetting((Settings x) => x.motionVectors.motionImageOpacity),
                motionImageAmplitude = this.FindSetting((Settings x) => x.motionVectors.motionImageAmplitude),
                motionVectorsOpacity = this.FindSetting((Settings x) => x.motionVectors.motionVectorsOpacity),
                motionVectorsResolution = this.FindSetting((Settings x) => x.motionVectors.motionVectorsResolution),
                motionVectorsAmplitude = this.FindSetting((Settings x) => x.motionVectors.motionVectorsAmplitude),
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.m_Mode);

            var mode = this.m_Mode.intValue;

            if (mode == (int)Mode.Depth)
            {
                EditorGUILayout.PropertyField(this.m_Depth.scale);
            }
            else if (mode == (int)Mode.MotionVectors)
            {
                EditorGUILayout.HelpBox("Switch to play mode to see motion vectors.", MessageType.Info);

                EditorGUILayout.LabelField("Source Image", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_MotionVectors.sourceOpacity, EditorGUIHelper.GetContent("Opacity"));
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Motion Vectors (overlay)", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                if (this.m_MotionVectors.motionImageOpacity.floatValue > 0f)
                    EditorGUILayout.HelpBox("Please keep opacity to 0 if you're subject to motion sickness.", MessageType.Warning);

                EditorGUILayout.PropertyField(this.m_MotionVectors.motionImageOpacity, EditorGUIHelper.GetContent("Opacity"));
                EditorGUILayout.PropertyField(this.m_MotionVectors.motionImageAmplitude, EditorGUIHelper.GetContent("Amplitude"));
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Motion Vectors (arrows)", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_MotionVectors.motionVectorsOpacity, EditorGUIHelper.GetContent("Opacity"));
                EditorGUILayout.PropertyField(this.m_MotionVectors.motionVectorsResolution, EditorGUIHelper.GetContent("Resolution"));
                EditorGUILayout.PropertyField(this.m_MotionVectors.motionVectorsAmplitude, EditorGUIHelper.GetContent("Amplitude"));
                EditorGUI.indentLevel--;
            }
            else
            {
                this.CheckActiveEffect(mode == (int)Mode.AmbientOcclusion && !this.profile.ambientOcclusion.enabled, "Ambient Occlusion");
                this.CheckActiveEffect(mode == (int)Mode.FocusPlane && !this.profile.depthOfField.enabled, "Depth Of Field");
                this.CheckActiveEffect(mode == (int)Mode.EyeAdaptation && !this.profile.eyeAdaptation.enabled, "Eye Adaptation");
                this.CheckActiveEffect((mode == (int)Mode.LogLut || mode == (int)Mode.PreGradingLog) && !this.profile.colorGrading.enabled, "Color Grading");
                this.CheckActiveEffect(mode == (int)Mode.UserLut && !this.profile.userLut.enabled, "User Lut");
            }
        }

        void CheckActiveEffect(bool expr, string name)
        {
            if (expr)
                EditorGUILayout.HelpBox(string.Format("{0} isn't enabled, the debug view won't work.", name), MessageType.Warning);
        }
    }
}
