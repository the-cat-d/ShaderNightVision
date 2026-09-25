using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderNightVision
{
    public class NVGProfile
    {
        private readonly string ProfileName;

        private readonly List<ConfigurationManagerAttributes> Attributes = new List<ConfigurationManagerAttributes>();


        private readonly ConfigFile Config;

        
        public readonly ConfigEntry<Color> NvColor;

        public readonly ConfigEntry<float> NvGainMax;
        public readonly ConfigEntry<float> NvGainMin;
        public readonly ConfigEntry<int> NvGainPhases;


        public readonly ConfigEntry<bool> NvPixelate;
        public readonly ConfigEntry<int> NvResolution;

        public readonly ConfigEntry<bool> NoiseEnabled;
        public readonly ConfigEntry<float> NoiseIntensityGainMultiplier;
        public readonly ConfigEntry<float> NoiseIntensity;
        public readonly ConfigEntry<float> NoiseFPS;
        public readonly ConfigEntry<float> NoiseScale;
        public readonly ConfigEntry<NoiseBlendingMode> NoiseBlending;

        public readonly ConfigEntry<bool> DistortionEnabled;
        public readonly ConfigEntry<float> DistortionStrength;
        public readonly ConfigEntry<float> DistortionPower;

        public readonly ConfigEntry<float> NvScale;
        public readonly ConfigEntry<Vector2> NvPosition;


        public readonly ConfigEntry<NVTubeType> TubeType;
        
      
        
        public NVGProfile(ConfigFile newConfig, string profileName = "")
        {

            Config = newConfig;
            
            
            if (profileName.Length > 0)
            {
                ProfileName = $" ({profileName})";
            }



            
            
            Plugin.logger.LogDebug($"Creating profile {this.ProfileName}");
            
            // General

            TubeType = Bind($"Night Vision{this.ProfileName} - Shader - General", "Tube Type", NVTubeType.Double,"The shape of the NVG mask",1);
            TubeType.SettingChanged += (_, _) =>
            {

                if (!SceneSingleton<CombatHUD>.i || CameraStateManager.cameraMode != CameraMode.cockpit) return;
    

                NVGHandler.nvgMaterial = null;
                Object.Destroy(NVGHandler.nvgGameObject);
                NVGHandler.InitializeNVG();

            };

            NvColor = Bind($"Night Vision{this.ProfileName} - Shader - General", "Night Vision Color", new Color(0,1,0),"The color of the NVGs");
            NvColor.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateColor();

            };

            NvGainMax = Bind($"Night Vision{this.ProfileName} - Shader - General", "Gain Max", 40f,"The maximum brightness the NVGs can output");
            NvGainMin = Bind($"Night Vision{this.ProfileName} - Shader - General", "Gain Min", 10f,"The minimum brightness the NVGs can output");
            NvGainPhases = Bind($"Night Vision{this.ProfileName} - Shader - General", "Gain Phases", 4,"The number of steps to take to get to the maximum gain");
            NvGainPhases.SettingChanged += (_, _) =>
            {
                NVGHandler.currentPhase = 0;
            };
            
            NvScale = Bind($"Night Vision{this.ProfileName} - Shader - General", "Goggle Scale", 1.5f,"Size of the mask");
            NvScale.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null) return;

                NVGHandler.UpdateScale();


            };
            NvPosition =Bind($"Night Vision{this.ProfileName} - Shader - General", "Goggle Position", new Vector2(0,0),"Offset of the mask");
            NvPosition.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null) return;

                NVGHandler.UpdatePosition();

            };


            // Noise

            NoiseEnabled = Bind($"Night Vision{this.ProfileName} - Shader - Noise", "Noise Enabled", true,"Enables noise.");
            NoiseEnabled.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.ToggleNoise();

            };

            NoiseIntensityGainMultiplier = Bind($"Night Vision{this.ProfileName} - Shader - Noise", "Noise Intensity Gain Multiplier", 3f,"Multiplication factor that increases noise when the gain is increased");
            NoiseIntensity = Bind($"Night Vision{this.ProfileName} - Shader - Noise", "Noise Intensity", 0.02f);
            NoiseIntensity.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateNoiseIntensity();

            };

            NoiseFPS = Bind($"Night Vision{this.ProfileName} - Shader - Noise", "Noise FPS", 24f,"How frequently the noise updates.");
            NoiseFPS.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateNoiseFPS();

            };

            NoiseScale = Bind($"Night Vision{this.ProfileName} - Shader - Noise", "Noise Resolution", 800f,"Resolution of the noise relative to the mask."); // "Scales the size of each noise pixel");
            NoiseScale.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateNoiseScale();

            };

            NoiseBlending = Bind($"Night Vision{this.ProfileName} - Shader - Noise", "Noise Blending", NoiseBlendingMode.Subtractive,"How the noise will blend between the actual image itself");
            NoiseBlending.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateNoiseBlending();

            };

            // Distortion

            DistortionEnabled = Bind($"Night Vision{this.ProfileName} - Shader - Distortion", "Distortion Enabled", true,"Enables pincushion distortion for NVGs");
            DistortionEnabled.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.ToggleDistortion();

            };

            DistortionPower = Bind($"Night Vision{this.ProfileName} - Shader - Distortion", "Distortion Power", 1.3f,"The fall of the distortion's border.");
            DistortionPower.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateDistortionPower();

            };

            DistortionStrength = Bind($"Night Vision{this.ProfileName} - Shader - Distortion", "Distortion Strength", 0.1f,"The strength of the distortion effect.");
            DistortionStrength.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateDistortionStrength();

            };

            // Pixelation

            NvPixelate = Bind($"Night Vision{this.ProfileName} - Shader - Pixelation", "Pixelation Enabled", false,"Enables a pixelation effect for NVGs");
            NvPixelate.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.TogglePixelation();

            };

            NvResolution = Bind($"Night Vision{this.ProfileName} - Shader - Pixelation", "Pixelation Resolution", 1080,"The resolution of the pixelation Effect");
            NvResolution.SettingChanged += (_, _) =>
            {
                if (NVGHandler.nvgGameObject is null && NVGHandler.nvgMaterial is null) return;

                NVGHandler.UpdateResolution();

            };

            
            PluginConfig.RebuildList();
        }


        private ConfigEntry<T> Bind<T>(string section,string key, T value, string description = "", int? order = null)
        {
            ConfigurationManagerAttributes a = new ConfigurationManagerAttributes() { Browsable = false };

            if (order.HasValue) a.Order = order.Value;
            
            Attributes.Add(a);
            
            return Config.Bind(section, key, value, new ConfigDescription(description + $"\nDefault: {value}",null,a));
        }
        
        public void ToggleEntries(bool val)
        {
            
            foreach (ConfigurationManagerAttributes attr in Attributes) 
            {
              
                attr.Browsable = val;
                
            }
            
            PluginConfig.RebuildList();
        }

        // ---- Static ----

        
        private static Dictionary<string,NVGProfile> aircraftProfiles = new Dictionary<string, NVGProfile>();
        
        public static NVGProfile CreateAircraftProfile(ConfigFile config, string profileName)
        {
            if (aircraftProfiles.TryGetValue(profileName, out NVGProfile profile))
            {
                return profile;
            }
            
            profile = new NVGProfile(config, profileName);
            aircraftProfiles.Add(profileName, profile);
            
            return profile;
            
        }
        
    }
}
