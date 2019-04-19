using System;

namespace UnityEngine.PostProcessing
{
    [Serializable]
    public class DitheringModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            public static Settings defaultSettings
            {
                get { return new Settings(); }
            }
        }

        [SerializeField]
        Settings m_Settings = Settings.defaultSettings;
        public Settings settings
        {
            get { return this.m_Settings; }
            set { this.m_Settings = value; }
        }

        public override void Reset()
        {
			this.m_Settings = Settings.defaultSettings;
        }
    }
}
