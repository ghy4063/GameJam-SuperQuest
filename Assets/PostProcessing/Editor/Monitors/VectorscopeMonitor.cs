using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    public class VectorscopeMonitor : PostProcessingMonitor
    {
        static GUIContent s_MonitorTitle = new GUIContent("Vectorscope");

        ComputeShader m_ComputeShader;
        ComputeBuffer m_Buffer;
        Material m_Material;
        RenderTexture m_VectorscopeTexture;
        Rect m_MonitorAreaRect;

        public VectorscopeMonitor()
        {
			this.m_ComputeShader = EditorResources.Load<ComputeShader>("Monitors/VectorscopeCompute.compute");
        }

        public override void Dispose()
        {
            GraphicsUtils.Destroy(this.m_Material);
            GraphicsUtils.Destroy(this.m_VectorscopeTexture);

            if (this.m_Buffer != null)
				this.m_Buffer.Release();

			this.m_Material = null;
			this.m_VectorscopeTexture = null;
			this.m_Buffer = null;
        }

        public override bool IsSupported()
        {
            return this.m_ComputeShader != null && GraphicsUtils.supportsDX11;
        }

        public override GUIContent GetMonitorTitle()
        {
            return s_MonitorTitle;
        }

        public override void OnMonitorSettings()
        {
            EditorGUI.BeginChangeCheck();

            var refreshOnPlay = this.m_MonitorSettings.refreshOnPlay;
            var exposure = this.m_MonitorSettings.vectorscopeExposure;
            var showBackground = this.m_MonitorSettings.vectorscopeShowBackground;

            refreshOnPlay = GUILayout.Toggle(refreshOnPlay, new GUIContent(FxStyles.playIcon, "Keep refreshing the vectorscope in play mode; this may impact performances."), FxStyles.preButton);
            exposure = GUILayout.HorizontalSlider(exposure, 0.05f, 0.3f, FxStyles.preSlider, FxStyles.preSliderThumb, GUILayout.Width(40f));
            showBackground = GUILayout.Toggle(showBackground, new GUIContent(FxStyles.checkerIcon, "Show an YUV background in the vectorscope."), FxStyles.preButton);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.m_BaseEditor.serializedObject.targetObject, "Vectorscope Settings Changed");
				this.m_MonitorSettings.refreshOnPlay = refreshOnPlay;
				this.m_MonitorSettings.vectorscopeExposure = exposure;
				this.m_MonitorSettings.vectorscopeShowBackground = showBackground;
                InternalEditorUtility.RepaintAllViews();
            }
        }

        public override void OnMonitorGUI(Rect r)
        {
            if (Event.current.type == EventType.Repaint)
            {
                // If m_MonitorAreaRect isn't set the preview was just opened so refresh the render to get the vectoscope data
                if (Mathf.Approximately(this.m_MonitorAreaRect.width, 0) && Mathf.Approximately(this.m_MonitorAreaRect.height, 0))
                    InternalEditorUtility.RepaintAllViews();

                // Sizing
                var size = 0f;

                if (r.width < r.height)
                {
                    size = this.m_VectorscopeTexture != null
                        ? Mathf.Min(this.m_VectorscopeTexture.width, r.width - 35f)
                        : r.width;
                }
                else
                {
                    size = this.m_VectorscopeTexture != null
                        ? Mathf.Min(this.m_VectorscopeTexture.height, r.height - 25f)
                        : r.height;
                }

				this.m_MonitorAreaRect = new Rect(
                        Mathf.Floor(r.x + r.width / 2f - size / 2f),
                        Mathf.Floor(r.y + r.height / 2f - size / 2f - 5f),
                        size, size
                        );

                if (this.m_VectorscopeTexture != null)
                {
					this.m_Material.SetFloat("_Exposure", this.m_MonitorSettings.vectorscopeExposure);

                    var oldActive = RenderTexture.active;
                    Graphics.Blit(null, this.m_VectorscopeTexture, this.m_Material, this.m_MonitorSettings.vectorscopeShowBackground ? 0 : 1);
                    RenderTexture.active = oldActive;

                    Graphics.DrawTexture(this.m_MonitorAreaRect, this.m_VectorscopeTexture);

                    var color = Color.white;
                    const float kTickSize = 10f;
                    const int kTickCount = 24;

                    var radius = this.m_MonitorAreaRect.width / 2f;
                    var midX = this.m_MonitorAreaRect.x + radius;
                    var midY = this.m_MonitorAreaRect.y + radius;
                    var center = new Vector2(midX, midY);

                    // Cross
                    color.a *= 0.5f;
                    Handles.color = color;
                    Handles.DrawLine(new Vector2(midX, this.m_MonitorAreaRect.y), new Vector2(midX, this.m_MonitorAreaRect.y + this.m_MonitorAreaRect.height));
                    Handles.DrawLine(new Vector2(this.m_MonitorAreaRect.x, midY), new Vector2(this.m_MonitorAreaRect.x + this.m_MonitorAreaRect.width, midY));

                    if (this.m_MonitorAreaRect.width > 100f)
                    {
                        color.a = 1f;

                        // Ticks
                        Handles.color = color;
                        for (var i = 0; i < kTickCount; i++)
                        {
                            var a = (float)i / (float)kTickCount;
                            var theta = a * (Mathf.PI * 2f);
                            var tx = Mathf.Cos(theta + (Mathf.PI / 2f));
                            var ty = Mathf.Sin(theta - (Mathf.PI / 2f));
                            var innerVec = center + new Vector2(tx, ty) * (radius - kTickSize);
                            var outerVec = center + new Vector2(tx, ty) * radius;
                            Handles.DrawAAPolyLine(3f, innerVec, outerVec);
                        }

                        // Labels (where saturation reaches 75%)
                        color.a = 1f;
                        var oldColor = GUI.color;
                        GUI.color = color * 2f;

                        var point = new Vector2(-0.254f, -0.750f) * radius + center;
                        var rect = new Rect(point.x - 10f, point.y - 10f, 20f, 20f);
                        GUI.Label(rect, "[R]", FxStyles.tickStyleCenter);

                        point = new Vector2(-0.497f, 0.629f) * radius + center;
                        rect = new Rect(point.x - 10f, point.y - 10f, 20f, 20f);
                        GUI.Label(rect, "[G]", FxStyles.tickStyleCenter);

                        point = new Vector2(0.750f, 0.122f) * radius + center;
                        rect = new Rect(point.x - 10f, point.y - 10f, 20f, 20f);
                        GUI.Label(rect, "[B]", FxStyles.tickStyleCenter);

                        point = new Vector2(-0.750f, -0.122f) * radius + center;
                        rect = new Rect(point.x - 10f, point.y - 10f, 20f, 20f);
                        GUI.Label(rect, "[Y]", FxStyles.tickStyleCenter);

                        point = new Vector2(0.254f, 0.750f) * radius + center;
                        rect = new Rect(point.x - 10f, point.y - 10f, 20f, 20f);
                        GUI.Label(rect, "[C]", FxStyles.tickStyleCenter);

                        point = new Vector2(0.497f, -0.629f) * radius + center;
                        rect = new Rect(point.x - 10f, point.y - 10f, 20f, 20f);
                        GUI.Label(rect, "[M]", FxStyles.tickStyleCenter);
                        GUI.color = oldColor;
                    }
                }
            }
        }

        public override void OnFrameData(RenderTexture source)
        {
            if (Application.isPlaying && !this.m_MonitorSettings.refreshOnPlay)
                return;

            if (Mathf.Approximately(this.m_MonitorAreaRect.width, 0) || Mathf.Approximately(this.m_MonitorAreaRect.height, 0))
                return;

            var ratio = (float)source.width / (float)source.height;
            var h = 384;
            var w = Mathf.FloorToInt(h * ratio);

            var rt = RenderTexture.GetTemporary(w, h, 0, source.format);
            Graphics.Blit(source, rt);
            this.ComputeVectorscope(rt);
			this.m_BaseEditor.Repaint();
            RenderTexture.ReleaseTemporary(rt);
        }

        void CreateBuffer(int width, int height)
        {
			this.m_Buffer = new ComputeBuffer(width * height, sizeof(uint));
        }

        void ComputeVectorscope(RenderTexture source)
        {
            if (this.m_Buffer == null)
            {
                this.CreateBuffer(source.width, source.height);
            }
            else if (this.m_Buffer.count != (source.width * source.height))
            {
				this.m_Buffer.Release();
                this.CreateBuffer(source.width, source.height);
            }

            var cs = this.m_ComputeShader;

            var kernel = cs.FindKernel("KVectorscopeClear");
            cs.SetBuffer(kernel, "_Vectorscope", this.m_Buffer);
            cs.SetVector("_Res", new Vector4(source.width, source.height, 0f, 0f));
            cs.Dispatch(kernel, Mathf.CeilToInt(source.width / 32f), Mathf.CeilToInt(source.height / 32f), 1);

            kernel = cs.FindKernel("KVectorscope");
            cs.SetBuffer(kernel, "_Vectorscope", this.m_Buffer);
            cs.SetTexture(kernel, "_Source", source);
            cs.SetInt("_IsLinear", GraphicsUtils.isLinearColorSpace ? 1 : 0);
            cs.SetVector("_Res", new Vector4(source.width, source.height, 0f, 0f));
            cs.Dispatch(kernel, Mathf.CeilToInt(source.width / 32f), Mathf.CeilToInt(source.height / 32f), 1);

            if (this.m_VectorscopeTexture == null || this.m_VectorscopeTexture.width != source.width || this.m_VectorscopeTexture.height != source.height)
            {
                GraphicsUtils.Destroy(this.m_VectorscopeTexture);
				this.m_VectorscopeTexture = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
                {
                    hideFlags = HideFlags.DontSave,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
            }

            if (this.m_Material == null)
				this.m_Material = new Material(Shader.Find("Hidden/Post FX/Monitors/Vectorscope Render")) { hideFlags = HideFlags.DontSave };

			this.m_Material.SetBuffer("_Vectorscope", this.m_Buffer);
			this.m_Material.SetVector("_Size", new Vector2(this.m_VectorscopeTexture.width, this.m_VectorscopeTexture.height));
        }
    }
}
