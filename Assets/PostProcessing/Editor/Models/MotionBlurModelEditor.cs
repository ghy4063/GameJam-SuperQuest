using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = MotionBlurModel.Settings;

    [PostProcessingModelEditor(typeof(MotionBlurModel))]
    public class MotionBlurModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_ShutterAngle;
        SerializedProperty m_SampleCount;
        SerializedProperty m_FrameBlending;

        GraphDrawer m_GraphDrawer;

        class GraphDrawer
        {
            const float k_Height = 32f;

            Texture m_BlendingIcon;

            GUIStyle m_LowerCenterStyle;
            GUIStyle m_MiddleCenterStyle;

            Color m_ColorDark;
            Color m_ColorGray;

            Vector3[] m_RectVertices = new Vector3[4];

            public GraphDrawer()
            {
				this.m_BlendingIcon = EditorResources.Load<Texture>("UI/MotionBlendingIcon.png");

				this.m_LowerCenterStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.LowerCenter };
				this.m_MiddleCenterStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };

                if (EditorGUIUtility.isProSkin)
                {
					this.m_ColorDark = new Color(0.18f, 0.18f, 0.18f);
					this.m_ColorGray = new Color(0.43f, 0.43f, 0.43f);
                }
                else
                {
					this.m_ColorDark = new Color(0.64f, 0.64f, 0.64f);
					this.m_ColorGray = new Color(0.92f, 0.92f, 0.92f);
                }
            }

            public void DrawShutterGraph(float angle)
            {
                var center = GUILayoutUtility.GetRect(128, k_Height).center;

                // Parameters used to make transitions smooth.
                var zeroWhenOff = Mathf.Min(1f, angle * 0.1f);
                var zeroWhenFull = Mathf.Min(1f, (360f - angle) * 0.02f);

                // Shutter angle graph
                var discCenter = center - new Vector2(k_Height * 2.4f, 0f);
                // - exposure duration indicator
                this.DrawDisc(discCenter, k_Height * Mathf.Lerp(0.5f, 0.38f, zeroWhenFull), this.m_ColorGray);
                // - shutter disc
                this.DrawDisc(discCenter, k_Height * 0.16f * zeroWhenFull, this.m_ColorDark);
                // - shutter blade
                this.DrawArc(discCenter, k_Height * 0.5f, 360f - angle, this.m_ColorDark);
                // - shutter axis
                this.DrawDisc(discCenter, zeroWhenOff, this.m_ColorGray);

                // Shutter label (off/full)
                var labelSize = new Vector2(k_Height, k_Height);
                var labelOrigin = discCenter - labelSize * 0.5f;
                var labelRect = new Rect(labelOrigin, labelSize);

                if (Mathf.Approximately(angle, 0f))
                    GUI.Label(labelRect, "Off", this.m_MiddleCenterStyle);
                else if (Mathf.Approximately(angle, 360f))
                    GUI.Label(labelRect, "Full", this.m_MiddleCenterStyle);

                // Exposure time bar graph
                var outerBarSize = new Vector2(4.75f, 0.5f) * k_Height;
                var innerBarSize = outerBarSize;
                innerBarSize.x *= angle / 360f;

                var barCenter = center + new Vector2(k_Height * 0.9f, 0f);
                var barOrigin = barCenter - outerBarSize * 0.5f;

                this.DrawRect(barOrigin, outerBarSize, this.m_ColorDark);
                this.DrawRect(barOrigin, innerBarSize, this.m_ColorGray);

                var barText = "Exposure time = " + (angle / 3.6f).ToString("0") + "% of ΔT";
                GUI.Label(new Rect(barOrigin, outerBarSize), barText, this.m_MiddleCenterStyle);
            }

            public void DrawBlendingGraph(float strength)
            {
                var center = GUILayoutUtility.GetRect(128, k_Height).center;

                var iconSize = new Vector2(k_Height, k_Height);
                var iconStride = new Vector2(k_Height * 0.9f, 0f);
                var iconOrigin = center - iconSize * 0.5f - iconStride * 2f;

                for (var i = 0; i < 5; i++)
                {
                    var weight = this.BlendingWeight(strength, i / 60f);
                    var rect = new Rect(iconOrigin + iconStride * i, iconSize);

                    var color = this.m_ColorGray;
                    color.a = weight;

                    GUI.color = color;
                    GUI.Label(rect, this.m_BlendingIcon);

                    GUI.color = Color.white;
                    GUI.Label(rect, (weight * 100).ToString("0") + "%", this.m_LowerCenterStyle);
                }
                // EditorGUIUtility.isProSkin
            }

            // Weight function for multi frame blending
            float BlendingWeight(float strength, float time)
            {
                if (strength > 0f || Mathf.Approximately(time, 0f))
                    return Mathf.Exp(-time * Mathf.Lerp(80f, 10f, strength));

                return 0;
            }

            // Draw a solid disc in the graph rect.
            void DrawDisc(Vector2 center, float radius, Color fill)
            {
                Handles.color = fill;
                Handles.DrawSolidDisc(center, Vector3.forward, radius);
            }

            // Draw an arc in the graph rect.
            void DrawArc(Vector2 center, float radius, float angle, Color fill)
            {
                var start = new Vector2(
                        -Mathf.Cos(Mathf.Deg2Rad * angle / 2f),
                        Mathf.Sin(Mathf.Deg2Rad * angle / 2f)
                        );

                Handles.color = fill;
                Handles.DrawSolidArc(center, Vector3.forward, start, angle, radius);
            }

            // Draw a rectangle in the graph rect.
            void DrawRect(Vector2 origin, Vector2 size, Color color)
            {
                var p0 = origin;
                var p1 = origin + size;

				this.m_RectVertices[0] = p0;
				this.m_RectVertices[1] = new Vector2(p1.x, p0.y);
				this.m_RectVertices[2] = p1;
				this.m_RectVertices[3] = new Vector2(p0.x, p1.y);

                Handles.color = Color.white;
                Handles.DrawSolidRectangleWithOutline(this.m_RectVertices, color, Color.clear);
            }
        }

        public override void OnEnable()
        {
			this.m_ShutterAngle = this.FindSetting((Settings x) => x.shutterAngle);
			this.m_SampleCount = this.FindSetting((Settings x) => x.sampleCount);
			this.m_FrameBlending = this.FindSetting((Settings x) => x.frameBlending);
        }

        public override void OnInspectorGUI()
        {
            if (this.m_GraphDrawer == null)
				this.m_GraphDrawer = new GraphDrawer();

            EditorGUILayout.LabelField("Shutter Speed Simulation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
			this.m_GraphDrawer.DrawShutterGraph(this.m_ShutterAngle.floatValue);
            EditorGUILayout.PropertyField(this.m_ShutterAngle);
            EditorGUILayout.PropertyField(this.m_SampleCount);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Multiple Frame Blending", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            var fbValue = this.m_FrameBlending.floatValue;
			this.m_GraphDrawer.DrawBlendingGraph(fbValue);
            EditorGUILayout.PropertyField(this.m_FrameBlending);

            if (fbValue > 0f)
                EditorGUILayout.HelpBox("Multi-Frame Blending lowers precision of the final picture for optimization purposes.", MessageType.Info);


            EditorGUI.indentLevel--;
        }
    }
}
