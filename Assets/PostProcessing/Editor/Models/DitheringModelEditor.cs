using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    [PostProcessingModelEditor(typeof(DitheringModel))]
    public class DitheringModelEditor : PostProcessingModelEditor
    {
        public override void OnInspectorGUI()
        {
            if (this.profile.grain.enabled && this.target.enabled)
                EditorGUILayout.HelpBox("Grain is enabled, you probably don't need dithering !", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("Nothing to configure !", MessageType.Info);
        }
    }
}
