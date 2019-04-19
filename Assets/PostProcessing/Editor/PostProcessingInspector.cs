using UnityEngine;
using UnityEngine.PostProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEditor.PostProcessing
{
    //[CanEditMultipleObjects]
    [CustomEditor(typeof(PostProcessingProfile))]
    public class PostProcessingInspector : Editor
    {
        static GUIContent s_PreviewTitle = new GUIContent("Monitors");

        PostProcessingProfile m_ConcreteTarget
        {
            get { return this.target as PostProcessingProfile; }
        }

        int m_CurrentMonitorID
        {
            get { return this.m_ConcreteTarget.monitors.currentMonitorID; }
            set { this.m_ConcreteTarget.monitors.currentMonitorID = value; }
        }

        List<PostProcessingMonitor> m_Monitors;
        GUIContent[] m_MonitorNames;
        Dictionary<PostProcessingModelEditor, PostProcessingModel> m_CustomEditors = new Dictionary<PostProcessingModelEditor, PostProcessingModel>();

        public bool IsInteractivePreviewOpened { get; private set; }

        void OnEnable()
        {
            if (this.target == null)
                return;

            // Aggregate custom post-fx editors
            var assembly = Assembly.GetAssembly(typeof(PostProcessingInspector));

            var editorTypes = assembly.GetTypes()
                .Where(x => x.IsDefined(typeof(PostProcessingModelEditorAttribute), false));

            var customEditors = new Dictionary<Type, PostProcessingModelEditor>();
            foreach (var editor in editorTypes)
            {
                var attr = (PostProcessingModelEditorAttribute)editor.GetCustomAttributes(typeof(PostProcessingModelEditorAttribute), false)[0];
                var effectType = attr.type;
                var alwaysEnabled = attr.alwaysEnabled;

                var editorInst = (PostProcessingModelEditor)Activator.CreateInstance(editor);
                editorInst.alwaysEnabled = alwaysEnabled;
                editorInst.profile = this.target as PostProcessingProfile;
                editorInst.inspector = this;
                customEditors.Add(effectType, editorInst);
            }

            // ... and corresponding models
            var baseType = this.target.GetType();
            var property = this.serializedObject.GetIterator();

            while (property.Next(true))
            {
                if (!property.hasChildren)
                    continue;

                var type = baseType;
                var srcObject = ReflectionUtils.GetFieldValueFromPath(this.serializedObject.targetObject, ref type, property.propertyPath);

                if (srcObject == null)
                    continue;

                PostProcessingModelEditor editor;
                if (customEditors.TryGetValue(type, out editor))
                {
                    var effect = (PostProcessingModel)srcObject;

                    if (editor.alwaysEnabled)
                        effect.enabled = editor.alwaysEnabled;

					this.m_CustomEditors.Add(editor, effect);
                    editor.target = effect;
                    editor.serializedProperty = property.Copy();
                    editor.OnPreEnable();
                }
            }

			// Prepare monitors
			this.m_Monitors = new List<PostProcessingMonitor>();

            var monitors = new List<PostProcessingMonitor>
            {
                new HistogramMonitor(),
                new WaveformMonitor(),
                new ParadeMonitor(),
                new VectorscopeMonitor()
            };

            var monitorNames = new List<GUIContent>();

            foreach (var monitor in monitors)
            {
                if (monitor.IsSupported())
                {
                    monitor.Init(this.m_ConcreteTarget.monitors, this);
					this.m_Monitors.Add(monitor);
                    monitorNames.Add(monitor.GetMonitorTitle());
                }
            }

			this.m_MonitorNames = monitorNames.ToArray();

            if (this.m_Monitors.Count > 0)
                this.m_ConcreteTarget.monitors.onFrameEndEditorOnly = this.OnFrameEnd;
        }

        void OnDisable()
        {
            if (this.m_CustomEditors != null)
            {
                foreach (var editor in this.m_CustomEditors.Keys)
                    editor.OnDisable();

				this.m_CustomEditors.Clear();
            }

            if (this.m_Monitors != null)
            {
                foreach (var monitor in this.m_Monitors)
                    monitor.Dispose();

				this.m_Monitors.Clear();
            }

            if (this.m_ConcreteTarget != null)
                this.m_ConcreteTarget.monitors.onFrameEndEditorOnly = null;
        }

        void OnFrameEnd(RenderTexture source)
        {
            if (!this.IsInteractivePreviewOpened)
                return;

            if (this.m_CurrentMonitorID < this.m_Monitors.Count)
				this.m_Monitors[this.m_CurrentMonitorID].OnFrameData(source);

            this.IsInteractivePreviewOpened = false;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            // Handles undo/redo events first (before they get used by the editors' widgets)
            var e = Event.current;
            if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed")
            {
                foreach (var editor in this.m_CustomEditors)
                    editor.Value.OnValidate();
            }

            if (!this.m_ConcreteTarget.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.None))
                EditorGUILayout.HelpBox("A debug view is currently enabled. Changes done to an effect might not be visible.", MessageType.Info);

            foreach (var editor in this.m_CustomEditors)
            {
                EditorGUI.BeginChangeCheck();

                editor.Key.OnGUI();

                if (EditorGUI.EndChangeCheck())
                    editor.Value.OnValidate();
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        public override GUIContent GetPreviewTitle()
        {
            return s_PreviewTitle;
        }

        public override bool HasPreviewGUI()
        {
            return GraphicsUtils.supportsDX11 && this.m_Monitors.Count > 0;
        }

        public override void OnPreviewSettings()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (this.m_CurrentMonitorID < this.m_Monitors.Count)
					this.m_Monitors[this.m_CurrentMonitorID].OnMonitorSettings();

                GUILayout.Space(5);
                this.m_CurrentMonitorID = EditorGUILayout.Popup(this.m_CurrentMonitorID, this.m_MonitorNames, FxStyles.preDropdown, GUILayout.MaxWidth(100f));
            }
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            this.IsInteractivePreviewOpened = true;

            if (this.m_CurrentMonitorID < this.m_Monitors.Count)
				this.m_Monitors[this.m_CurrentMonitorID].OnMonitorGUI(r);
        }
    }
}
