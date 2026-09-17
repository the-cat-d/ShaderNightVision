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
        
        
        public ConfigEntry<int> nvResolution;
        
        public ConfigEntry<float> noiseIntensityGainMultiplier;
        public ConfigEntry<float> noiseIntensity;
        public ConfigEntry<float> noiseFPS;
        public ConfigEntry<float> noiseScale;
        
        public ConfigEntry<float> nvScale;
        public ConfigEntry<Vector2> nvPosition;

        
        public ConfigEntry<NVTubeType> tubeType;
        
        private string profileName;
        
        public NVGProfile(ConfigFile config, NVTubeType newTubeType,Color NVColor,float NVGainMax,float NVGainMin,int NVGainPhases,float NoiseIntensity,float NoiseIntensityGainMultiplier,float NoiseFPS,float NoiseScale, int NVResolution, float GogglesScale, Vector2 GogglesPosition)
        {
            
            
            
            // if (tubeType is null)
            // {
                profileName = "Global Profile";    
            // }
            // else
            // {
            //     switch (tubeType)
            //     {
            //         case NVTubeType.Mono:
            //             profileName = "1st Generation";
            //             break;
            //         case NVTubeType.Double:
            //             profileName = "2nd Generation";
            //             break;
            //         case NVTubeType.Quad:
            //             profileName = "3rd Generation";
            //             break;
            //     }
            // }
            
            tubeType = config.Bind($"Night Vision - {profileName}","Tube Type",newTubeType);
            tubeType.SettingChanged += (sender, args) =>
            {
                // UpdateCurrentProfile();
                
                
                

                if (SceneSingleton<CombatHUD>.i)
                {
                    
                    NVGHandler.NVGMaterial = null;
                    GameObject.Destroy(NVGHandler.NVGGameObject);
                    NVGHandler.Initialize(CombatHUD.i);
                    
                    
                }

            };
            
            nvColor = config.Bind($"Night Vision - {profileName}","Night Vision Color",NVColor);
            nvColor.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateColor();
                
            };
            
            nvGainMax = config.Bind($"Night Vision - {profileName}","Gain Max",NVGainMax);
            nvGainMin = config.Bind($"Night Vision - {profileName}","Gain Min",NVGainMin);
            nvGainPhases = config.Bind($"Night Vision - {profileName}","Gain Increments",NVGainPhases);
            
            nvResolution = config.Bind($"Night Vision - {profileName}","Resolution",NVResolution);            
            nvResolution.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateResolution();
                 
            };
            
            noiseIntensityGainMultiplier = config.Bind($"Night Vision - {profileName}","Noise Intensity Gain Multiplier",NoiseIntensityGainMultiplier);            
            
            noiseIntensity = config.Bind($"Night Vision - {profileName}","Noise Intensity",NoiseIntensity);            
            noiseIntensity.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateNoiseIntensity();
                
            };
            
            noiseFPS = config.Bind($"Night Vision - {profileName}","Noise FPS",NoiseFPS);
            noiseFPS.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateNoiseFPS();
                
            };
            
            noiseScale = config.Bind($"Night Vision - {profileName}","Noise Scale",NoiseScale);
            noiseScale.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null && NVGHandler.NVGMaterial is null) return;
                
                NVGHandler.UpdateNoiseScale();
                
            };

            
            nvScale = config.Bind($"Night Vision - {profileName}","Goggle Scale",GogglesScale);
            nvScale.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null) return;
                NVGHandler.UpdateScale();
                
                
            };
            nvPosition = config.Bind($"Night Vision - {profileName}","Goggle Position",GogglesPosition);
            nvPosition.SettingChanged += (sender, args) =>
            {
                if (NVGHandler.NVGGameObject is null) return;
                NVGHandler.UpdatePosition();
                
            };
            
        }
        
        
    }
}
