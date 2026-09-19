using ShaderNightVision.MonoBehaviour;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderNightVision
{
    
   
    
    public static class NVGHandler
    {
        public static GameObject  NVGGameObject;
        public static Material  NVGMaterial;
        
        
        private static readonly int _nvIntensity = Shader.PropertyToID("_NVIntensity");
        private static readonly int _nvColor = Shader.PropertyToID("_ColorTint");
        
        private static readonly int _pixelate = Shader.PropertyToID("_Pixelate");
        private static readonly int _nvResolution = Shader.PropertyToID("_Resolution");
        
        
        private static readonly int _noiseEnabled = Shader.PropertyToID("_Noise");
        private static readonly int _noiseIntensity = Shader.PropertyToID("_NoiseIntensity");
        private static readonly int _noiseFPS = Shader.PropertyToID("_NoiseFPS");
        private static readonly int _noiseScale = Shader.PropertyToID("_NoiseScale");
        private static readonly int _noiseBlending = Shader.PropertyToID("_NoiseBlendingMode");
        
        private static readonly int _distortionEnabled = Shader.PropertyToID("_Distortion");
        private static readonly int _distortionStrength = Shader.PropertyToID("_DistortionStrength");
        private static readonly int _distortionPower = Shader.PropertyToID("_DistortionPower");
        
        public static void Initialize([CanBeNull] CombatHUD _hud = null)
        {
            // if (NVGGameObject != null) return;
            
            Plugin.Logger.LogInfo("Initializing NVG");
            
            CombatHUD HUD = CombatHUD.i ?? _hud;
            
            if (HUD is null || Camera.main is null) return;
            
            Camera.main.gameObject.AddComponent<CameraHook>();

            
            switch (PluginConfig.currentProfile.tubeType.Value)
            {
                case NVTubeType.Mono:
                    NVGGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1M"));
                    break;
                case NVTubeType.Double:
                    NVGGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1"));
                    break;
                case NVTubeType.Quad:
                    NVGGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG3"));
                    break;
                case NVTubeType.Fullscreen:
                    NVGGameObject = new GameObject("NVGF");
                    Image nvgImage = NVGGameObject.AddComponent<Image>();
                    nvgImage.material = AssetBundleUtil.GetAsset<Material>("UINVG");
                    NVGMaterial = nvgImage.material;
                    RectTransform rect = NVGGameObject.GetComponent<RectTransform>();
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    
                    rect.sizeDelta = Vector2.zero;
                    
                    break;
                    
                default:
                    NVGGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1"));
                    break;
            }
            Plugin.Logger.LogInfo(HUD is null); 
            
            NVGGameObject.transform.SetParent(HUD.transform,false);
            NVGGameObject.transform.localPosition = Vector3.zero;
            NVGGameObject.transform.SetAsFirstSibling();
            NVGGameObject.SetActive(NightVision.i != null && NightVision.i.nightVisSelected);

            
            
            Transform mask = NVGGameObject.transform.Find("NVGMask");
            if (mask) NVGMaterial = mask.GetComponent<Image>().material;

            currentPhase = 0;
            
            ChangeGain(false);
            
            UpdateScale();
            UpdatePosition();
            
            UpdateColor();

            UpdateNoiseIntensity();
            UpdateNoiseFPS();
            UpdateNoiseScale();
            ToggleNoise();
            UpdateNoiseBlending();
            
            TogglePixelation();
            UpdateResolution();


            ToggleDistortion();
            UpdateDistortionStrength();
            UpdateDistortionPower();
            
        }

        // ---

        private static int currentPhase = 0;
        
        private static void ChangeGain(bool direction)
        {
            int gainPhases = PluginConfig.currentProfile.nvGainPhases.Value;
            float gainMin = PluginConfig.currentProfile.nvGainMin.Value;
            float gainMax = PluginConfig.currentProfile.nvGainMax.Value;
            
            currentPhase += direction ? -1 : 1;
            currentPhase = Mathf.Clamp(currentPhase, 0, gainPhases);
            
            float intensity = Mathf.Lerp(gainMin, gainMax, (float)currentPhase / gainPhases);
            
            NVGMaterial.SetFloat(_nvIntensity, intensity);
            UpdateNoiseIntensity();
            
        }

        public static void UpdateColor()
        {
            NVGMaterial.SetColor(_nvColor, PluginConfig.currentProfile.nvColor.Value);
        }
        
        public static void ToggleNoise()
        {
            
            float enabled = PluginConfig.currentProfile.noiseEnabled.Value? 1 : 0;
            NVGMaterial.SetFloat(_noiseEnabled, enabled);
        }
        
        public static void TogglePixelation()
        {
            float enabled = PluginConfig.currentProfile.nvPixelate.Value ? 1 : 0;
            NVGMaterial.SetFloat(_pixelate, enabled);
        }
        
        public static void UpdateResolution()
        {
            NVGMaterial.SetFloat(_nvResolution, PluginConfig.currentProfile.nvResolution.Value);
            
        }
        
        public static void UpdateNoiseIntensity()
        {
            float baseIntensity = PluginConfig.currentProfile.noiseIntensity.Value; 
            
            float noiseIntensity = Mathf.Lerp(baseIntensity, baseIntensity * PluginConfig.currentProfile.noiseIntensityGainMultiplier.Value, currentPhase /  (float)PluginConfig.currentProfile.nvGainPhases.Value);
            
            NVGMaterial.SetFloat(_noiseIntensity, noiseIntensity);
            
        }

        public static void UpdateNoiseScale()
        {
            NVGMaterial.SetFloat(_noiseScale, PluginConfig.currentProfile.noiseScale.Value);
            
        }
        
        public static void UpdateNoiseFPS()
        {
            NVGMaterial.SetFloat(_noiseFPS, PluginConfig.currentProfile.noiseFPS.Value);
        }

        public static void UpdateNoiseBlending()
        {
            
            NVGMaterial.SetFloat(_noiseBlending, (int)PluginConfig.currentProfile.noiseBlending.Value);
        }
        
        public static void ToggleDistortion()
        {
            float enabled = PluginConfig.currentProfile.distortionEnabled.Value ? 1 : 0;
            NVGMaterial.SetFloat(_distortionEnabled, enabled);
        }

        
        public static void UpdateDistortionStrength()
        {
            NVGMaterial.SetFloat(_distortionStrength, PluginConfig.currentProfile.distortionStrength.Value);
        }
        
        public static void UpdateDistortionPower()
        {
            NVGMaterial.SetFloat(_distortionPower, PluginConfig.currentProfile.distortionPower.Value);
        }

        
        public static void UpdateScale()
        {
            if (PluginConfig.currentProfile.tubeType.Value == NVTubeType.Fullscreen) return;   
            
            float scale = PluginConfig.currentProfile.nvScale.Value;
            
            NVGGameObject.transform.localScale = new Vector3(scale,scale);
        }
        
        public static void UpdatePosition()
        {
            if (PluginConfig.currentProfile.tubeType.Value == NVTubeType.Fullscreen) return;
            
            NVGGameObject.transform.localPosition = PluginConfig.currentProfile.nvPosition.Value;
        }
        
        // ---

        private static void ProcessControls()
        {
            if (NVGGameObject)
            {
           
                if (Input.GetKeyDown(PluginConfig.nvGainIncreaseKey.Value))
                {
                    ChangeGain(false);
            
                } else if (Input.GetKeyDown(PluginConfig.nvGainDecreaseKey.Value))
                { 
                    ChangeGain(true);
            
                }

           
            }
        }


        public static void NVGUpdate()
        {
            ProcessControls();
        }
        
       
        
    }
    
   
}
