using System;
using System.Linq.Expressions;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    [CustomEditor(typeof(PostProcessingBehaviour))]
    public class PostProcessingBehaviourEditor : Editor
    {
        SerializedProperty m_Profile;

        public void OnEnable()
        {
			this.m_Profile = this.FindSetting((PostProcessingBehaviour x) => x.profile);
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.m_Profile);

            this.serializedObject.ApplyModifiedProperties();
        }

        SerializedProperty FindSetting<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            return this.serializedObject.FindProperty(ReflectionUtils.GetFieldPath(expr));
        }
    }
}
