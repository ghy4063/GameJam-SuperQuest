using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Water
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(WaterBase))]
    public class PlanarReflection : MonoBehaviour
    {
        public LayerMask reflectionMask;
        public bool reflectSkybox = false;
        public Color clearColor = Color.grey;
        public String reflectionSampler = "_ReflectionTex";
        public float clipPlaneOffset = 0.07F;


        Vector3 m_Oldpos;
        Camera m_ReflectionCamera;
        Material m_SharedMaterial;
        Dictionary<Camera, bool> m_HelperCameras;


        public void Start()
        {
			this.m_SharedMaterial = ((WaterBase) this.gameObject.GetComponent(typeof(WaterBase))).sharedMaterial;
        }


        Camera CreateReflectionCameraFor(Camera cam)
        {
            var reflName = this.gameObject.name + "Reflection" + cam.name;
            var go = GameObject.Find(reflName);

            if (!go)
            {
                go = new GameObject(reflName, typeof(Camera));
            }
            if (!go.GetComponent(typeof(Camera)))
            {
                go.AddComponent(typeof(Camera));
            }
            var reflectCamera = go.GetComponent<Camera>();

            reflectCamera.backgroundColor = this.clearColor;
            reflectCamera.clearFlags = this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;

            this.SetStandardCameraParameter(reflectCamera, this.reflectionMask);

            if (!reflectCamera.targetTexture)
            {
                reflectCamera.targetTexture = this.CreateTextureFor(cam);
            }

            return reflectCamera;
        }


        void SetStandardCameraParameter(Camera cam, LayerMask mask)
        {
            cam.cullingMask = mask & ~(1 << LayerMask.NameToLayer("Water"));
            cam.backgroundColor = Color.black;
            cam.enabled = false;
        }


        RenderTexture CreateTextureFor(Camera cam)
        {
            var rt = new RenderTexture(Mathf.FloorToInt(cam.pixelWidth * 0.5F),
                Mathf.FloorToInt(cam.pixelHeight * 0.5F), 24);
            rt.hideFlags = HideFlags.DontSave;
            return rt;
        }


        public void RenderHelpCameras(Camera currentCam)
        {
            if (null == this.m_HelperCameras)
            {
				this.m_HelperCameras = new Dictionary<Camera, bool>();
            }

            if (!this.m_HelperCameras.ContainsKey(currentCam))
            {
				this.m_HelperCameras.Add(currentCam, false);
            }
            if (this.m_HelperCameras[currentCam])
            {
                return;
            }

            if (!this.m_ReflectionCamera)
            {
				this.m_ReflectionCamera = this.CreateReflectionCameraFor(currentCam);
            }

            this.RenderReflectionFor(currentCam, this.m_ReflectionCamera);

			this.m_HelperCameras[currentCam] = true;
        }


        public void LateUpdate()
        {
            if (null != this.m_HelperCameras)
            {
				this.m_HelperCameras.Clear();
            }
        }


        public void WaterTileBeingRendered(Transform tr, Camera currentCam)
        {
            this.RenderHelpCameras(currentCam);

            if (this.m_ReflectionCamera && this.m_SharedMaterial)
            {
				this.m_SharedMaterial.SetTexture(this.reflectionSampler, this.m_ReflectionCamera.targetTexture);
            }
        }


        public void OnEnable()
        {
            Shader.EnableKeyword("WATER_REFLECTIVE");
            Shader.DisableKeyword("WATER_SIMPLE");
        }


        public void OnDisable()
        {
            Shader.EnableKeyword("WATER_SIMPLE");
            Shader.DisableKeyword("WATER_REFLECTIVE");
        }


        void RenderReflectionFor(Camera cam, Camera reflectCamera)
        {
            if (!reflectCamera)
            {
                return;
            }

            if (this.m_SharedMaterial && !this.m_SharedMaterial.HasProperty(this.reflectionSampler))
            {
                return;
            }

            reflectCamera.cullingMask = this.reflectionMask & ~(1 << LayerMask.NameToLayer("Water"));

            this.SaneCameraSettings(reflectCamera);

            reflectCamera.backgroundColor = this.clearColor;
            reflectCamera.clearFlags = this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
            if (this.reflectSkybox)
            {
                if (cam.gameObject.GetComponent(typeof(Skybox)))
                {
                    var sb = (Skybox)reflectCamera.gameObject.GetComponent(typeof(Skybox));
                    if (!sb)
                    {
                        sb = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
                    }
                    sb.material = ((Skybox)cam.GetComponent(typeof(Skybox))).material;
                }
            }

            GL.invertCulling = true;

            var reflectiveSurface = this.transform; //waterHeight;

            var eulerA = cam.transform.eulerAngles;

            reflectCamera.transform.eulerAngles = new Vector3(-eulerA.x, eulerA.y, eulerA.z);
            reflectCamera.transform.position = cam.transform.position;

            var pos = reflectiveSurface.transform.position;
            pos.y = reflectiveSurface.position.y;
            var normal = reflectiveSurface.transform.up;
            var d = -Vector3.Dot(normal, pos) - this.clipPlaneOffset;
            var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            var reflection = Matrix4x4.zero;
            reflection = CalculateReflectionMatrix(reflection, reflectionPlane);
			this.m_Oldpos = cam.transform.position;
            var newpos = reflection.MultiplyPoint(this.m_Oldpos);

            reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            var clipPlane = this.CameraSpacePlane(reflectCamera, pos, normal, 1.0f);

            var projection = cam.projectionMatrix;
            projection = CalculateObliqueMatrix(projection, clipPlane);
            reflectCamera.projectionMatrix = projection;

            reflectCamera.transform.position = newpos;
            var euler = cam.transform.eulerAngles;
            reflectCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

            reflectCamera.Render();

            GL.invertCulling = false;
        }


        void SaneCameraSettings(Camera helperCam)
        {
            helperCam.depthTextureMode = DepthTextureMode.None;
            helperCam.backgroundColor = Color.black;
            helperCam.clearFlags = CameraClearFlags.SolidColor;
            helperCam.renderingPath = RenderingPath.Forward;
        }


        static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
        {
            var q = projection.inverse * new Vector4(
                Sgn(clipPlane.x),
                Sgn(clipPlane.y),
                1.0F,
                1.0F
                );
            var c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
            // third row = clip plane - fourth row
            projection[2] = c.x - projection[3];
            projection[6] = c.y - projection[7];
            projection[10] = c.z - projection[11];
            projection[14] = c.w - projection[15];

            return projection;
        }


        static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1.0F - 2.0F * plane[0] * plane[0]);
            reflectionMat.m01 = (- 2.0F * plane[0] * plane[1]);
            reflectionMat.m02 = (- 2.0F * plane[0] * plane[2]);
            reflectionMat.m03 = (- 2.0F * plane[3] * plane[0]);

            reflectionMat.m10 = (- 2.0F * plane[1] * plane[0]);
            reflectionMat.m11 = (1.0F - 2.0F * plane[1] * plane[1]);
            reflectionMat.m12 = (- 2.0F * plane[1] * plane[2]);
            reflectionMat.m13 = (- 2.0F * plane[3] * plane[1]);

            reflectionMat.m20 = (- 2.0F * plane[2] * plane[0]);
            reflectionMat.m21 = (- 2.0F * plane[2] * plane[1]);
            reflectionMat.m22 = (1.0F - 2.0F * plane[2] * plane[2]);
            reflectionMat.m23 = (- 2.0F * plane[3] * plane[2]);

            reflectionMat.m30 = 0.0F;
            reflectionMat.m31 = 0.0F;
            reflectionMat.m32 = 0.0F;
            reflectionMat.m33 = 1.0F;

            return reflectionMat;
        }


        static float Sgn(float a)
        {
            if (a > 0.0F)
            {
                return 1.0F;
            }
            if (a < 0.0F)
            {
                return -1.0F;
            }
            return 0.0F;
        }


        Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            var offsetPos = pos + normal * this.clipPlaneOffset;
            var m = cam.worldToCameraMatrix;
            var cpos = m.MultiplyPoint(offsetPos);
            var cnormal = m.MultiplyVector(normal).normalized * sideSign;

            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }
    }
}