using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
	using Settings = ScreenSpaceReflectionModel.Settings;

	[PostProcessingModelEditor(typeof(ScreenSpaceReflectionModel))]
    public class ScreenSpaceReflectionModelEditor : PostProcessingModelEditor
    {
        struct IntensitySettings
        {
            public SerializedProperty reflectionMultiplier;
            public SerializedProperty fadeDistance;
            public SerializedProperty fresnelFade;
            public SerializedProperty fresnelFadePower;
        }

        struct ReflectionSettings
        {
            public SerializedProperty blendType;
            public SerializedProperty reflectionQuality;
            public SerializedProperty maxDistance;
            public SerializedProperty iterationCount;
            public SerializedProperty stepSize;
            public SerializedProperty widthModifier;
            public SerializedProperty reflectionBlur;
            public SerializedProperty reflectBackfaces;
        }

        struct ScreenEdgeMask
        {
            public SerializedProperty intensity;
        }

        IntensitySettings m_Intensity;
        ReflectionSettings m_Reflection;
        ScreenEdgeMask m_ScreenEdgeMask;

        public override void OnEnable()
        {
			this.m_Intensity = new IntensitySettings
            {
                reflectionMultiplier = this.FindSetting((Settings x) => x.intensity.reflectionMultiplier),
                fadeDistance = this.FindSetting((Settings x) => x.intensity.fadeDistance),
                fresnelFade = this.FindSetting((Settings x) => x.intensity.fresnelFade),
                fresnelFadePower = this.FindSetting((Settings x) => x.intensity.fresnelFadePower)
            };

			this.m_Reflection = new ReflectionSettings
            {
                blendType = this.FindSetting((Settings x) => x.reflection.blendType),
                reflectionQuality = this.FindSetting((Settings x) => x.reflection.reflectionQuality),
                maxDistance = this.FindSetting((Settings x) => x.reflection.maxDistance),
                iterationCount = this.FindSetting((Settings x) => x.reflection.iterationCount),
                stepSize = this.FindSetting((Settings x) => x.reflection.stepSize),
                widthModifier = this.FindSetting((Settings x) => x.reflection.widthModifier),
                reflectionBlur = this.FindSetting((Settings x) => x.reflection.reflectionBlur),
                reflectBackfaces = this.FindSetting((Settings x) => x.reflection.reflectBackfaces)
            };

			this.m_ScreenEdgeMask = new ScreenEdgeMask
            {
                intensity = this.FindSetting((Settings x) => x.screenEdgeMask.intensity)
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This effect only works with the deferred rendering path.", MessageType.Info);

            EditorGUILayout.LabelField("Reflection", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_Reflection.blendType);
            EditorGUILayout.PropertyField(this.m_Reflection.reflectionQuality);
            EditorGUILayout.PropertyField(this.m_Reflection.maxDistance);
            EditorGUILayout.PropertyField(this.m_Reflection.iterationCount);
            EditorGUILayout.PropertyField(this.m_Reflection.stepSize);
            EditorGUILayout.PropertyField(this.m_Reflection.widthModifier);
            EditorGUILayout.PropertyField(this.m_Reflection.reflectionBlur);
            EditorGUILayout.PropertyField(this.m_Reflection.reflectBackfaces);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Intensity", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_Intensity.reflectionMultiplier);
            EditorGUILayout.PropertyField(this.m_Intensity.fadeDistance);
            EditorGUILayout.PropertyField(this.m_Intensity.fresnelFade);
            EditorGUILayout.PropertyField(this.m_Intensity.fresnelFadePower);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Screen Edge Mask", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_ScreenEdgeMask.intensity);
            EditorGUI.indentLevel--;
        }
    }
}
