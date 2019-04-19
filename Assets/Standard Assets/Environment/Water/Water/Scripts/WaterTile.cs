using UnityEngine;

namespace UnityStandardAssets.Water
{
	[ExecuteInEditMode]
    public class WaterTile : MonoBehaviour
    {
        public PlanarReflection reflection;
        public WaterBase waterBase;


        public void Start()
        {
            this.AcquireComponents();
        }


        void AcquireComponents()
        {
            if (!this.reflection)
            {
                if (this.transform.parent)
                {
					this.reflection = this.transform.parent.GetComponent<PlanarReflection>();
                }
                else
                {
					this.reflection = this.transform.GetComponent<PlanarReflection>();
                }
            }

            if (!this.waterBase)
            {
                if (this.transform.parent)
                {
					this.waterBase = this.transform.parent.GetComponent<WaterBase>();
                }
                else
                {
					this.waterBase = this.transform.GetComponent<WaterBase>();
                }
            }
        }


#if UNITY_EDITOR
        public void Update()
        {
            this.AcquireComponents();
        }
#endif


        public void OnWillRenderObject()
        {
            if (this.reflection)
            {
				this.reflection.WaterTileBeingRendered(this.transform, Camera.current);
            }
            if (this.waterBase)
            {
				this.waterBase.WaterTileBeingRendered(this.transform, Camera.current);
            }
        }
    }
}