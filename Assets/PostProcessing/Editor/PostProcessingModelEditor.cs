using UnityEngine;
using UnityEngine.PostProcessing;
using System;
using System.Linq.Expressions;

namespace UnityEditor.PostProcessing
{
    public class PostProcessingModelEditor
    {
        public PostProcessingModel target { get; internal set; }
        public SerializedProperty serializedProperty { get; internal set; }

        protected SerializedProperty m_SettingsProperty;
        protected SerializedProperty m_EnabledProperty;

        internal bool alwaysEnabled = false;
        internal PostProcessingProfile profile;
        internal PostProcessingInspector inspector;

        internal void OnPreEnable()
        {
			this.m_SettingsProperty = this.serializedProperty.FindPropertyRelative("m_Settings");
			this.m_EnabledProperty = this.serializedProperty.FindPropertyRelative("m_Enabled");

            this.OnEnable();
        }

        public virtual void OnEnable()
        {}

        public virtual void OnDisable()
        {}

        internal void OnGUI()
        {
            GUILayout.Space(5);

            var display = this.alwaysEnabled
                ? EditorGUIHelper.Header(this.serializedProperty.displayName, this.m_SettingsProperty, this.Reset)
                : EditorGUIHelper.Header(this.serializedProperty.displayName, this.m_SettingsProperty, this.m_EnabledProperty, this.Reset);

            if (display)
            {
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledGroupScope(!this.m_EnabledProperty.boolValue))
                {
                    this.OnInspectorGUI();
                }
                EditorGUI.indentLevel--;
            }
        }

        void Reset()
        {
            var obj = this.serializedProperty.serializedObject;
            Undo.RecordObject(obj.targetObject, "Reset");
            this.target.Reset();
            EditorUtility.SetDirty(obj.targetObject);
        }

        public virtual void OnInspectorGUI()
        {}

        public void Repaint()
        {
			this.inspector.Repaint();
        }

        protected SerializedProperty FindSetting<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            return this.m_SettingsProperty.FindPropertyRelative(ReflectionUtils.GetFieldPath(expr));
        }

        protected SerializedProperty FindSetting<T, TValue>(SerializedProperty prop, Expression<Func<T, TValue>> expr)
        {
            return prop.FindPropertyRelative(ReflectionUtils.GetFieldPath(expr));
        }
    }
}
