using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = BloomModel.Settings;

    [PostProcessingModelEditor(typeof(BloomModel))]
    public class BloomModelEditor : PostProcessingModelEditor
    {
        struct BloomSettings
        {
            public SerializedProperty intensity;
            public SerializedProperty threshold;
            public SerializedProperty softKnee;
            public SerializedProperty radius;
            public SerializedProperty antiFlicker;
        }

        struct LensDirtSettings
        {
            public SerializedProperty texture;
            public SerializedProperty intensity;
        }

        BloomSettings m_Bloom;
        LensDirtSettings m_LensDirt;

        public override void OnEnable()
        {
			this.m_Bloom = new BloomSettings
            {
                intensity = this.FindSetting((Settings x) => x.bloom.intensity),
                threshold = this.FindSetting((Settings x) => x.bloom.threshold),
                softKnee = this.FindSetting((Settings x) => x.bloom.softKnee),
                radius = this.FindSetting((Settings x) => x.bloom.radius),
                antiFlicker = this.FindSetting((Settings x) => x.bloom.antiFlicker)
            };

			this.m_LensDirt = new LensDirtSettings
            {
                texture = this.FindSetting((Settings x) => x.lensDirt.texture),
                intensity = this.FindSetting((Settings x) => x.lensDirt.intensity)
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            this.PrepareGraph();
            this.DrawGraph();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.m_Bloom.intensity);
            EditorGUILayout.PropertyField(this.m_Bloom.threshold, EditorGUIHelper.GetContent("Threshold (Gamma)"));
            EditorGUILayout.PropertyField(this.m_Bloom.softKnee);
            EditorGUILayout.PropertyField(this.m_Bloom.radius);
            EditorGUILayout.PropertyField(this.m_Bloom.antiFlicker);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dirt", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_LensDirt.texture);
            EditorGUILayout.PropertyField(this.m_LensDirt.intensity);
            EditorGUI.indentLevel--;
        }

        #region Graph

        float m_GraphThreshold;
        float m_GraphKnee;
        float m_GraphIntensity;

        // Number of vertices in curve
        const int k_CurveResolution = 48;

        // Vertex buffers
        Vector3[] m_RectVertices = new Vector3[4];
        Vector3[] m_LineVertices = new Vector3[2];
        Vector3[] m_CurveVertices = new Vector3[k_CurveResolution];

        Rect m_RectGraph;
        float m_RangeX;
        float m_RangeY;

        float ResponseFunction(float x)
        {
            var rq = Mathf.Clamp(x - this.m_GraphThreshold + this.m_GraphKnee, 0, this.m_GraphKnee * 2);
            rq = rq * rq * 0.25f / this.m_GraphKnee;
            return Mathf.Max(rq, x - this.m_GraphThreshold) * this.m_GraphIntensity;
        }

        // Transform a point into the graph rect
        Vector3 PointInRect(float x, float y)
        {
            x = Mathf.Lerp(this.m_RectGraph.x, this.m_RectGraph.xMax, x / this.m_RangeX);
            y = Mathf.Lerp(this.m_RectGraph.yMax, this.m_RectGraph.y, y / this.m_RangeY);
            return new Vector3(x, y, 0);
        }

        // Draw a line in the graph rect
        void DrawLine(float x1, float y1, float x2, float y2, float grayscale)
        {
			this.m_LineVertices[0] = this.PointInRect(x1, y1);
			this.m_LineVertices[1] = this.PointInRect(x2, y2);
            Handles.color = Color.white * grayscale;
            Handles.DrawAAPolyLine(2.0f, this.m_LineVertices);
        }

        // Draw a rect in the graph rect
        void DrawRect(float x1, float y1, float x2, float y2, float fill, float line)
        {
			this.m_RectVertices[0] = this.PointInRect(x1, y1);
			this.m_RectVertices[1] = this.PointInRect(x2, y1);
			this.m_RectVertices[2] = this.PointInRect(x2, y2);
			this.m_RectVertices[3] = this.PointInRect(x1, y2);

            Handles.DrawSolidRectangleWithOutline(
				this.m_RectVertices,
                fill < 0 ? Color.clear : Color.white * fill,
                line < 0 ? Color.clear : Color.white * line
                );
        }

        // Update internal state with a given bloom instance
        public void PrepareGraph()
        {
            var bloom = (BloomModel) this.target;
			this.m_RangeX = 5f;
			this.m_RangeY = 2f;

			this.m_GraphThreshold = bloom.settings.bloom.thresholdLinear;
			this.m_GraphKnee = bloom.settings.bloom.softKnee * this.m_GraphThreshold + 1e-5f;

			// Intensity is capped to prevent sampling errors
			this.m_GraphIntensity = Mathf.Min(bloom.settings.bloom.intensity, 10f);
        }

        // Draw the graph at the current position
        public void DrawGraph()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUI.indentLevel * 15f);
				this.m_RectGraph = GUILayoutUtility.GetRect(128, 80);
            }

            // Background
            this.DrawRect(0, 0, this.m_RangeX, this.m_RangeY, 0.1f, 0.4f);

            // Soft-knee range
            this.DrawRect(this.m_GraphThreshold - this.m_GraphKnee, 0, this.m_GraphThreshold + this.m_GraphKnee, this.m_RangeY, 0.25f, -1);

            // Horizontal lines
            for (var i = 1; i < this.m_RangeY; i++)
                this.DrawLine(0, i, this.m_RangeX, i, 0.4f);

            // Vertical lines
            for (var i = 1; i < this.m_RangeX; i++)
                this.DrawLine(i, 0, i, this.m_RangeY, 0.4f);

            // Label
            Handles.Label(
                this.PointInRect(0, this.m_RangeY) + Vector3.right,
                "Brightness Response (linear)", EditorStyles.miniLabel
                );

            // Threshold line
            this.DrawLine(this.m_GraphThreshold, 0, this.m_GraphThreshold, this.m_RangeY, 0.6f);

            // Response curve
            var vcount = 0;
            while (vcount < k_CurveResolution)
            {
                var x = this.m_RangeX * vcount / (k_CurveResolution - 1);
                var y = this.ResponseFunction(x);
                if (y < this.m_RangeY)
                {
					this.m_CurveVertices[vcount++] = this.PointInRect(x, y);
                }
                else
                {
                    if (vcount > 1)
                    {
                        // Extend the last segment to the top edge of the rect.
                        var v1 = this.m_CurveVertices[vcount - 2];
                        var v2 = this.m_CurveVertices[vcount - 1];
                        var clip = (this.m_RectGraph.y - v1.y) / (v2.y - v1.y);
						this.m_CurveVertices[vcount - 1] = v1 + (v2 - v1) * clip;
                    }
                    break;
                }
            }

            if (vcount > 1)
            {
                Handles.color = Color.white * 0.9f;
                Handles.DrawAAPolyLine(2.0f, vcount, this.m_CurveVertices);
            }
        }

        #endregion
    }
}
