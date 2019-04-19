using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using VignetteMode = VignetteModel.Mode;
    using Settings = VignetteModel.Settings;

    [PostProcessingModelEditor(typeof(VignetteModel))]
    public class VignetteModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_Mode;
        SerializedProperty m_Color;
        SerializedProperty m_Center;
        SerializedProperty m_Intensity;
        SerializedProperty m_Smoothness;
        SerializedProperty m_Roundness;
        SerializedProperty m_Mask;
        SerializedProperty m_Opacity;
        SerializedProperty m_Rounded;

        public override void OnEnable()
        {
			this.m_Mode = this.FindSetting((Settings x) => x.mode);
			this.m_Color = this.FindSetting((Settings x) => x.color);
			this.m_Center = this.FindSetting((Settings x) => x.center);
			this.m_Intensity = this.FindSetting((Settings x) => x.intensity);
			this.m_Smoothness = this.FindSetting((Settings x) => x.smoothness);
			this.m_Roundness = this.FindSetting((Settings x) => x.roundness);
			this.m_Mask = this.FindSetting((Settings x) => x.mask);
			this.m_Opacity = this.FindSetting((Settings x) => x.opacity);
			this.m_Rounded = this.FindSetting((Settings x) => x.rounded);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.m_Mode);
            EditorGUILayout.PropertyField(this.m_Color);

            if (this.m_Mode.intValue < (int)VignetteMode.Masked)
            {
                EditorGUILayout.PropertyField(this.m_Center);
                EditorGUILayout.PropertyField(this.m_Intensity);
                EditorGUILayout.PropertyField(this.m_Smoothness);
                EditorGUILayout.PropertyField(this.m_Roundness);
                EditorGUILayout.PropertyField(this.m_Rounded);
            }
            else
            {
                var mask = (this.target as VignetteModel).settings.mask;

                // Checks import settings on the mask, offers to fix them if invalid
                if (mask != null)
                {
                    var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mask)) as TextureImporter;

                    if (importer != null) // Fails when using an internal texture
                    {
#if UNITY_5_5_OR_NEWER
                        var valid = importer.anisoLevel == 0
                            && importer.mipmapEnabled == false
                            //&& importer.alphaUsage == TextureImporterAlphaUsage.FromGrayScale
                            && importer.alphaSource == TextureImporterAlphaSource.FromGrayScale
                            && importer.textureCompression == TextureImporterCompression.Uncompressed
                            && importer.wrapMode == TextureWrapMode.Clamp;
#else
                        bool valid = importer.anisoLevel == 0
                            && importer.mipmapEnabled == false
                            && importer.grayscaleToAlpha == true
                            && importer.textureFormat == TextureImporterFormat.Alpha8
                            && importer.wrapMode == TextureWrapMode.Clamp;
#endif

                        if (!valid)
                        {
                            EditorGUILayout.HelpBox("Invalid mask import settings.", MessageType.Warning);

                            GUILayout.Space(-32);
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Fix", GUILayout.Width(60)))
                                {
                                    this.SetMaskImportSettings(importer);
                                    AssetDatabase.Refresh();
                                }
                                GUILayout.Space(8);
                            }
                            GUILayout.Space(11);
                        }
                    }
                }

                EditorGUILayout.PropertyField(this.m_Mask);
                EditorGUILayout.PropertyField(this.m_Opacity);
            }
        }

        void SetMaskImportSettings(TextureImporter importer)
        {
#if UNITY_5_5_OR_NEWER
            importer.textureType = TextureImporterType.SingleChannel;
            //importer.alphaUsage = TextureImporterAlphaUsage.FromGrayScale;
            importer.alphaSource = TextureImporterAlphaSource.FromGrayScale;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
#else
            importer.textureType = TextureImporterType.Advanced;
            importer.grayscaleToAlpha = true;
            importer.textureFormat = TextureImporterFormat.Alpha8;
#endif

            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }
    }
}
