using BepInEx.Configuration;
using UnityEngine;

namespace ShaderNightVision
{
    public class NVGProfile
    {
        public ConfigEntry<Color> nvColor;
        
        public ConfigEntry<float> nvGainMax;
        public ConfigEntry<float> nvGainMin;
        public ConfigEntry<int> nvGainPhases;


        public ConfigEntry<bool> nvPixelate;
        public ConfigEntry<int> nvResolution;
        
        public ConfigEntry<bool> noiseEnabled;
        public ConfigEntry<float> noiseIntensityGainMultiplier;
        public ConfigEntry<float> noiseIntensity;
        public ConfigEntry<float> noiseFPS;
        public ConfigEntry<float> noiseScale;
        public ConfigEntry<NoiseBlendingMode> noiseBlending;
        
        public ConfigEntry<bool> distortionEnabled;
        public ConfigEntry<float> distortionStrength;
        public ConfigEntry<float> distortionPower;
        
        public ConfigEntry<float> nvScale;
        public ConfigEntry<Vector2> nvPosition;

        
        public ConfigEntry<NVTubeType> tubeType;
        
        private readonly string profileName = "Shader";
        
        public NVGProfile(ConfigFile config, NVTubeType newTubeType,Color NVColor,float NVGainMax,float NVGainMin,int NVGainPhases,bool NoiseEnabled,float NoiseIntensity,float NoiseIntensityGainMultiplier,float NoiseFPS,float NoiseScale,NoiseBlendingMode NoiseBlending, int NVResolution,bool NVPixelate,bool DistortionEnabled,float DistortionPower, float DistortioStrength, float GogglesScale, Vector2 GogglesPosition)
        {
            
            
            // General
            
            tubeType = config.Bind($"Night Vision - {profileName} - General","Tube Type",newTubeType);
            tubeType.SettingChanged += (sender, args) =>
            {
               
                
                
                

                if (SceneSingleton<CombatHUD>.i)
                {
                    
                    NVGHandler.NVGMaterial = null;
                    GameObject.Destroy(NVGHandler.NVGGameObject);
                    NVGHandler.Initialize(CombatHUD.i);
                    
                    
                }

            };
            
            nvColor = config.Bind($"Night Vision - {profileName} - General","Night Vision Color",NVColor);
            nvColor.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateColor();
                
            };
            
            nvGainMax = config.Bind($"Night Vision - {profileName} - General","Gain Max",NVGainMax);
            nvGainMin = config.Bind($"Night Vision - {profileName} - General","Gain Min",NVGainMin);
            nvGainPhases = config.Bind($"Night Vision - {profileName} - General","Gain Increments",NVGainPhases);
            
            nvScale = config.Bind($"Night Vision - {profileName} - General","Goggle Scale",GogglesScale);
            nvScale.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null) return;
                NVGHandler.UpdateScale();
                
                
            };
            nvPosition = config.Bind($"Night Vision - {profileName} - General","Goggle Position",GogglesPosition);
            nvPosition.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null) return;
                NVGHandler.UpdatePosition();
                
            };
            
           
            // Noise
            
            noiseEnabled = config.Bind($"Night Vision - {profileName} - Noise","Noise Enabled",NoiseEnabled,new ConfigDescription("",null, new ConfigurationManagerAttributes {Order = 1}));            
            noiseEnabled.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.ToggleNoise();
                 
            };
            
            noiseIntensityGainMultiplier = config.Bind($"Night Vision - {profileName} - Noise","Noise Intensity Gain Multiplier",NoiseIntensityGainMultiplier,"Multiplier that increases noise when gain is increased");            
            
            noiseIntensity = config.Bind($"Night Vision - {profileName} - Noise","Noise Intensity",NoiseIntensity);            
            noiseIntensity.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateNoiseIntensity();
                
            };
            
            noiseFPS = config.Bind($"Night Vision - {profileName} - Noise","Noise FPS",NoiseFPS);
            noiseFPS.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateNoiseFPS();
                
            };
            
            noiseScale = config.Bind($"Night Vision - {profileName} - Noise","Noise Scale",NoiseScale,"Scales the size of each noise pixel");
            noiseScale.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateNoiseScale();
                
            };
            
            noiseBlending = config.Bind($"Night Vision - {profileName} - Noise","Noise Blending",NoiseBlending,"Blending between noise and the render");            
            noiseBlending.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateNoiseBlending();
                 
            };
            
            // Distortion
            
            distortionEnabled = config.Bind($"Night Vision - {profileName} - Distortion","Distortion Enabled",DistortionEnabled);            
            distortionEnabled.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.ToggleDistortion();
                 
            };
            
            distortionPower = config.Bind($"Night Vision - {profileName} - Distortion","Distortion Power",DistortionPower,"Distortion border fall off");            
            distortionPower.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateDistortionPower();
                 
            };
            
            distortionStrength = config.Bind($"Night Vision - {profileName} - Distortion","Distortion Strength",DistortioStrength);            
            distortionStrength.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateDistortionStrength();
                 
            };
            
            // Pixelation
            
            nvPixelate = config.Bind($"Night Vision - {profileName} - Pixelation","Pixelation Enabled",NVPixelate);            
            nvPixelate.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.TogglePixelation();
                 
            };
            
            nvResolution = config.Bind($"Night Vision - {profileName} - Pixelation","Pixelation Resolution",NVResolution);            
            nvResolution.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateResolution();
                 
            };

            
          
            
        }
        
        
    }
}
