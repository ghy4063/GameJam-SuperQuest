using UnityEngine;

namespace UnityStandardAssets.Water
{
	public enum WaterQuality
    {
        High = 2,
        Medium = 1,
        Low = 0,
    }

    [ExecuteInEditMode]
    public class WaterBase : MonoBehaviour
    {
        public Material sharedMaterial;
        public WaterQuality waterQuality = WaterQuality.High;
        public bool edgeBlend = true;


        public void UpdateShader()
        {
            if (this.waterQuality > WaterQuality.Medium)
            {
				this.sharedMaterial.shader.maximumLOD = 501;
            }
            else if (this.waterQuality > WaterQuality.Low)
            {
				this.sharedMaterial.shader.maximumLOD = 301;
            }
            else
            {
				this.sharedMaterial.shader.maximumLOD = 201;
            }

            // If the system does not support depth textures (ie. NaCl), turn off edge bleeding,
            // as the shader will render everything as transparent if the depth texture is not valid.
            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
            {
				this.edgeBlend = false;
            }

            if (this.edgeBlend)
            {
                Shader.EnableKeyword("WATER_EDGEBLEND_ON");
                Shader.DisableKeyword("WATER_EDGEBLEND_OFF");
                // just to make sure (some peeps might forget to add a water tile to the patches)
                if (Camera.main)
                {
                    Camera.main.depthTextureMode |= DepthTextureMode.Depth;
                }
            }
            else
            {
                Shader.EnableKeyword("WATER_EDGEBLEND_OFF");
                Shader.DisableKeyword("WATER_EDGEBLEND_ON");
            }
        }


        public void WaterTileBeingRendered(Transform tr, Camera currentCam)
        {
            if (currentCam && this.edgeBlend)
            {
                currentCam.depthTextureMode |= DepthTextureMode.Depth;
            }
        }


        public void Update()
        {
            if (this.sharedMaterial)
            {
                this.UpdateShader();
            }
        }
    }
}