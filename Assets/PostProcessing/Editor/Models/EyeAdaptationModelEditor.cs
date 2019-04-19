using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
	using Settings = EyeAdaptationModel.Settings;

	[PostProcessingModelEditor(typeof(EyeAdaptationModel))]
    public class EyeAdaptationModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_LowPercent;
        SerializedProperty m_HighPercent;
        SerializedProperty m_MinLuminance;
        SerializedProperty m_MaxLuminance;
        SerializedProperty m_KeyValue;
        SerializedProperty m_DynamicKeyValue;
        SerializedProperty m_AdaptationType;
        SerializedProperty m_SpeedUp;
        SerializedProperty m_SpeedDown;
        SerializedProperty m_LogMin;
        SerializedProperty m_LogMax;

        public override void OnEnable()
        {
			this.m_LowPercent = this.FindSetting((Settings x) => x.lowPercent);
			this.m_HighPercent = this.FindSetting((Settings x) => x.highPercent);
			this.m_MinLuminance = this.FindSetting((Settings x) => x.minLuminance);
			this.m_MaxLuminance = this.FindSetting((Settings x) => x.maxLuminance);
			this.m_KeyValue = this.FindSetting((Settings x) => x.keyValue);
			this.m_DynamicKeyValue = this.FindSetting((Settings x) => x.dynamicKeyValue);
			this.m_AdaptationType = this.FindSetting((Settings x) => x.adaptationType);
			this.m_SpeedUp = this.FindSetting((Settings x) => x.speedUp);
			this.m_SpeedDown = this.FindSetting((Settings x) => x.speedDown);
			this.m_LogMin = this.FindSetting((Settings x) => x.logMin);
			this.m_LogMax = this.FindSetting((Settings x) => x.logMax);
        }

        public override void OnInspectorGUI()
        {
            if (!GraphicsUtils.supportsDX11)
                EditorGUILayout.HelpBox("This effect requires support for compute shaders. Enabling it won't do anything on unsupported platforms.", MessageType.Warning);

            EditorGUILayout.LabelField("Luminosity range", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_LogMin, EditorGUIHelper.GetContent("Minimum (EV)"));
            EditorGUILayout.PropertyField(this.m_LogMax, EditorGUIHelper.GetContent("Maximum (EV)"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Auto exposure", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var low = this.m_LowPercent.floatValue;
            var high = this.m_HighPercent.floatValue;

            EditorGUILayout.MinMaxSlider(EditorGUIHelper.GetContent("Histogram filtering|These values are the lower and upper percentages of the histogram that will be used to find a stable average luminance. Values outside of this range will be discarded and won't contribute to the average luminance."), ref low, ref high, 1f, 99f);

			this.m_LowPercent.floatValue = low;
			this.m_HighPercent.floatValue = high;

            EditorGUILayout.PropertyField(this.m_MinLuminance, EditorGUIHelper.GetContent("Minimum (EV)"));
            EditorGUILayout.PropertyField(this.m_MaxLuminance, EditorGUIHelper.GetContent("Maximum (EV)"));
            EditorGUILayout.PropertyField(this.m_DynamicKeyValue);

            if (!this.m_DynamicKeyValue.boolValue)
                EditorGUILayout.PropertyField(this.m_KeyValue);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Adaptation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(this.m_AdaptationType, EditorGUIHelper.GetContent("Type"));

            if (this.m_AdaptationType.intValue == (int)EyeAdaptationModel.EyeAdaptationType.Progressive)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_SpeedUp);
                EditorGUILayout.PropertyField(this.m_SpeedDown);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }
    }
}
