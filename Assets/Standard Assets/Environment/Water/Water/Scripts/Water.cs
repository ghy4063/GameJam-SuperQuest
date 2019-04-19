using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Water
{
    [ExecuteInEditMode] // Make water live-update even when not in play mode
    public class Water : MonoBehaviour
    {
        public enum WaterMode
        {
            Simple = 0,
            Reflective = 1,
            Refractive = 2,
        };


        public WaterMode waterMode = WaterMode.Refractive;
        public bool disablePixelLights = true;
        public int textureSize = 256;
        public float clipPlaneOffset = 0.07f;
        public LayerMask reflectLayers = -1;
        public LayerMask refractLayers = -1;


        private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
        private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
        private RenderTexture m_ReflectionTexture;
        private RenderTexture m_RefractionTexture;
        private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;
        private int m_OldReflectionTextureSize;
        private int m_OldRefractionTextureSize;
        private static bool s_InsideWater;


        // This is called when it's known that the object will be rendered by some
        // camera. We render reflections / refractions and do other updates here.
        // Because the script executes in edit mode, reflections for the scene view
        // camera will just work!
        public void OnWillRenderObject()
        {
            if (!this.enabled || !this.GetComponent<Renderer>() || !this.GetComponent<Renderer>().sharedMaterial ||
                !this.GetComponent<Renderer>().enabled)
            {
                return;
            }

            var cam = Camera.current;
            if (!cam)
            {
                return;
            }

            // Safeguard from recursive water reflections.
            if (s_InsideWater)
            {
                return;
            }
            s_InsideWater = true;

			// Actual water rendering mode depends on both the current setting AND
			// the hardware support. There's no point in rendering refraction textures
			// if they won't be visible in the end.
			this.m_HardwareWaterSupport = this.FindHardwareWaterSupport();
            var mode = this.GetWaterMode();

            Camera reflectionCamera, refractionCamera;
            this.CreateWaterObjects(cam, out reflectionCamera, out refractionCamera);

            // find out the reflection plane: position and normal in world space
            var pos = this.transform.position;
            var normal = this.transform.up;

            // Optionally disable pixel lights for reflection/refraction
            var oldPixelLightCount = QualitySettings.pixelLightCount;
            if (this.disablePixelLights)
            {
                QualitySettings.pixelLightCount = 0;
            }

            this.UpdateCameraModes(cam, reflectionCamera);
            this.UpdateCameraModes(cam, refractionCamera);

            // Render reflection if needed
            if (mode >= WaterMode.Reflective)
            {
                // Reflect camera around reflection plane
                var d = -Vector3.Dot(normal, pos) - this.clipPlaneOffset;
                var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

                var reflection = Matrix4x4.zero;
                CalculateReflectionMatrix(ref reflection, reflectionPlane);
                var oldpos = cam.transform.position;
                var newpos = reflection.MultiplyPoint(oldpos);
                reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

                // Setup oblique projection matrix so that near plane is our reflection
                // plane. This way we clip everything below/above it for free.
                var clipPlane = this.CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
                reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

				// Set custom culling matrix from the current camera
				reflectionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;

				reflectionCamera.cullingMask = ~(1 << 4) & this.reflectLayers.value; // never render water layer
                reflectionCamera.targetTexture = this.m_ReflectionTexture;
                var oldCulling = GL.invertCulling;
                GL.invertCulling = !oldCulling;
                reflectionCamera.transform.position = newpos;
                var euler = cam.transform.eulerAngles;
                reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
                reflectionCamera.Render();
                reflectionCamera.transform.position = oldpos;
                GL.invertCulling = oldCulling;
                this.GetComponent<Renderer>().sharedMaterial.SetTexture("_ReflectionTex", this.m_ReflectionTexture);
            }

            // Render refraction
            if (mode >= WaterMode.Refractive)
            {
                refractionCamera.worldToCameraMatrix = cam.worldToCameraMatrix;

                // Setup oblique projection matrix so that near plane is our reflection
                // plane. This way we clip everything below/above it for free.
                var clipPlane = this.CameraSpacePlane(refractionCamera, pos, normal, -1.0f);
                refractionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

				// Set custom culling matrix from the current camera
				refractionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;

				refractionCamera.cullingMask = ~(1 << 4) & this.refractLayers.value; // never render water layer
                refractionCamera.targetTexture = this.m_RefractionTexture;
                refractionCamera.transform.position = cam.transform.position;
                refractionCamera.transform.rotation = cam.transform.rotation;
                refractionCamera.Render();
                this.GetComponent<Renderer>().sharedMaterial.SetTexture("_RefractionTex", this.m_RefractionTexture);
            }

            // Restore pixel light count
            if (this.disablePixelLights)
            {
                QualitySettings.pixelLightCount = oldPixelLightCount;
            }

            // Setup shader keywords based on water mode
            switch (mode)
            {
                case WaterMode.Simple:
                    Shader.EnableKeyword("WATER_SIMPLE");
                    Shader.DisableKeyword("WATER_REFLECTIVE");
                    Shader.DisableKeyword("WATER_REFRACTIVE");
                    break;
                case WaterMode.Reflective:
                    Shader.DisableKeyword("WATER_SIMPLE");
                    Shader.EnableKeyword("WATER_REFLECTIVE");
                    Shader.DisableKeyword("WATER_REFRACTIVE");
                    break;
                case WaterMode.Refractive:
                    Shader.DisableKeyword("WATER_SIMPLE");
                    Shader.DisableKeyword("WATER_REFLECTIVE");
                    Shader.EnableKeyword("WATER_REFRACTIVE");
                    break;
            }

            s_InsideWater = false;
        }


        // Cleanup all the objects we possibly have created
        void OnDisable()
        {
            if (this.m_ReflectionTexture)
            {
                DestroyImmediate(this.m_ReflectionTexture);
				this.m_ReflectionTexture = null;
            }
            if (this.m_RefractionTexture)
            {
                DestroyImmediate(this.m_RefractionTexture);
				this.m_RefractionTexture = null;
            }
            foreach (var kvp in this.m_ReflectionCameras)
            {
                DestroyImmediate((kvp.Value).gameObject);
            }
			this.m_ReflectionCameras.Clear();
            foreach (var kvp in this.m_RefractionCameras)
            {
                DestroyImmediate((kvp.Value).gameObject);
            }
			this.m_RefractionCameras.Clear();
        }


        // This just sets up some matrices in the material; for really
        // old cards to make water texture scroll.
        void Update()
        {
            if (!this.GetComponent<Renderer>())
            {
                return;
            }
            var mat = this.GetComponent<Renderer>().sharedMaterial;
            if (!mat)
            {
                return;
            }

            var waveSpeed = mat.GetVector("WaveSpeed");
            var waveScale = mat.GetFloat("_WaveScale");
            var waveScale4 = new Vector4(waveScale, waveScale, waveScale * 0.4f, waveScale * 0.45f);

            // Time since level load, and do intermediate calculations with doubles
            var t = Time.timeSinceLevelLoad / 20.0;
            var offsetClamped = new Vector4(
                (float)Math.IEEERemainder(waveSpeed.x * waveScale4.x * t, 1.0),
                (float)Math.IEEERemainder(waveSpeed.y * waveScale4.y * t, 1.0),
                (float)Math.IEEERemainder(waveSpeed.z * waveScale4.z * t, 1.0),
                (float)Math.IEEERemainder(waveSpeed.w * waveScale4.w * t, 1.0)
                );

            mat.SetVector("_WaveOffset", offsetClamped);
            mat.SetVector("_WaveScale4", waveScale4);
        }

        void UpdateCameraModes(Camera src, Camera dest)
        {
            if (dest == null)
            {
                return;
            }
            // set water camera to clear the same way as current camera
            dest.clearFlags = src.clearFlags;
            dest.backgroundColor = src.backgroundColor;
            if (src.clearFlags == CameraClearFlags.Skybox)
            {
                var sky = src.GetComponent<Skybox>();
                var mysky = dest.GetComponent<Skybox>();
                if (!sky || !sky.material)
                {
                    mysky.enabled = false;
                }
                else
                {
                    mysky.enabled = true;
                    mysky.material = sky.material;
                }
            }
            // update other values to match current camera.
            // even if we are supplying custom camera&projection matrices,
            // some of values are used elsewhere (e.g. skybox uses far plane)
            dest.farClipPlane = src.farClipPlane;
            dest.nearClipPlane = src.nearClipPlane;
            dest.orthographic = src.orthographic;
            dest.fieldOfView = src.fieldOfView;
            dest.aspect = src.aspect;
            dest.orthographicSize = src.orthographicSize;
        }


        // On-demand create any objects we need for water
        void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
        {
            var mode = this.GetWaterMode();

            reflectionCamera = null;
            refractionCamera = null;

            if (mode >= WaterMode.Reflective)
            {
                // Reflection render texture
                if (!this.m_ReflectionTexture || this.m_OldReflectionTextureSize != this.textureSize)
                {
                    if (this.m_ReflectionTexture)
                    {
                        DestroyImmediate(this.m_ReflectionTexture);
                    }
					this.m_ReflectionTexture = new RenderTexture(this.textureSize, this.textureSize, 16);
					this.m_ReflectionTexture.name = "__WaterReflection" + this.GetInstanceID();
					this.m_ReflectionTexture.isPowerOfTwo = true;
					this.m_ReflectionTexture.hideFlags = HideFlags.DontSave;
					this.m_OldReflectionTextureSize = this.textureSize;
                }

				// Camera for reflection
				this.m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
                if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                {
                    var go = new GameObject("Water Refl Camera id" + this.GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
                    reflectionCamera = go.GetComponent<Camera>();
                    reflectionCamera.enabled = false;
                    reflectionCamera.transform.position = this.transform.position;
                    reflectionCamera.transform.rotation = this.transform.rotation;
                    reflectionCamera.gameObject.AddComponent<FlareLayer>();
                    go.hideFlags = HideFlags.HideAndDontSave;
					this.m_ReflectionCameras[currentCamera] = reflectionCamera;
                }
            }

            if (mode >= WaterMode.Refractive)
            {
                // Refraction render texture
                if (!this.m_RefractionTexture || this.m_OldRefractionTextureSize != this.textureSize)
                {
                    if (this.m_RefractionTexture)
                    {
                        DestroyImmediate(this.m_RefractionTexture);
                    }
					this.m_RefractionTexture = new RenderTexture(this.textureSize, this.textureSize, 16);
					this.m_RefractionTexture.name = "__WaterRefraction" + this.GetInstanceID();
					this.m_RefractionTexture.isPowerOfTwo = true;
					this.m_RefractionTexture.hideFlags = HideFlags.DontSave;
					this.m_OldRefractionTextureSize = this.textureSize;
                }

				// Camera for refraction
				this.m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
                if (!refractionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                {
                    var go =
                        new GameObject("Water Refr Camera id" + this.GetInstanceID() + " for " + currentCamera.GetInstanceID(),
                            typeof(Camera), typeof(Skybox));
                    refractionCamera = go.GetComponent<Camera>();
                    refractionCamera.enabled = false;
                    refractionCamera.transform.position = this.transform.position;
                    refractionCamera.transform.rotation = this.transform.rotation;
                    refractionCamera.gameObject.AddComponent<FlareLayer>();
                    go.hideFlags = HideFlags.HideAndDontSave;
					this.m_RefractionCameras[currentCamera] = refractionCamera;
                }
            }
        }

        WaterMode GetWaterMode()
        {
            if (this.m_HardwareWaterSupport < this.waterMode)
            {
                return this.m_HardwareWaterSupport;
            }
            return this.waterMode;
        }

        WaterMode FindHardwareWaterSupport()
        {
            if (!this.GetComponent<Renderer>())
            {
                return WaterMode.Simple;
            }

            var mat = this.GetComponent<Renderer>().sharedMaterial;
            if (!mat)
            {
                return WaterMode.Simple;
            }

            var mode = mat.GetTag("WATERMODE", false);
            if (mode == "Refractive")
            {
                return WaterMode.Refractive;
            }
            if (mode == "Reflective")
            {
                return WaterMode.Reflective;
            }

            return WaterMode.Simple;
        }

        // Given position/normal of the plane, calculates plane in camera space.
        Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            var offsetPos = pos + normal * this.clipPlaneOffset;
            var m = cam.worldToCameraMatrix;
            var cpos = m.MultiplyPoint(offsetPos);
            var cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        // Calculates reflection matrix around the given plane
        static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (- 2F * plane[0] * plane[1]);
            reflectionMat.m02 = (- 2F * plane[0] * plane[2]);
            reflectionMat.m03 = (- 2F * plane[3] * plane[0]);

            reflectionMat.m10 = (- 2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (- 2F * plane[1] * plane[2]);
            reflectionMat.m13 = (- 2F * plane[3] * plane[1]);

            reflectionMat.m20 = (- 2F * plane[2] * plane[0]);
            reflectionMat.m21 = (- 2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (- 2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }
    }
}
