using ShaderNightVision.MonoBehaviour;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderNightVision
{
    
   
    
    public static class NVGHandler
    {
        private static NVTubeType  lastTubeType;
        
        private static GameObject  nvgCanvas;
        public static GameObject  nvgGameObject;
        public static Material  nvgMaterial;
        
        // Shader property names
        
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

        public static void InitializeCanvas([CanBeNull] CombatHUD hudBackup = null)
        {
            Plugin.logger.LogInfo("Initializing NVGCanvas");
            
            CombatHUD hud = CombatHUD.i ?? hudBackup;
            
            if (hud is null || Camera.main is null) return;
            
            Camera.main.gameObject.AddComponent<CameraHook>();
            
            nvgCanvas = new GameObject("NVGCanvas",typeof(Canvas))
            {
                transform =
                {
                    parent = hud.transform.parent,
                    localPosition = hud.transform.localPosition,
                },
            };

            nvgCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            
            nvgCanvas.transform.SetAsFirstSibling();
            
            InitializeNVG();

        }

        public static void InitializeNVG(NVTubeType? tubeOverride = null)
        {
            
            Plugin.logger.LogInfo("Initializing NVG");
            
            if (Camera.main is null) return;


           
            
            
            NVTubeType currentTubeType = tubeOverride ?? PluginConfig.currentProfile.TubeType.Value;
            
            
            Plugin.logger.LogInfo($"t: {currentTubeType} o: {tubeOverride}");

            if (lastTubeType == currentTubeType)
            {
                return;
            }
            
            if (nvgGameObject) Object.Destroy(nvgGameObject);

            switch (currentTubeType)
            {
                case NVTubeType.Mono:
                    nvgGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1M"));
                    break;
                case NVTubeType.Double:
                    nvgGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1"));
                    break;
                case NVTubeType.Quad:
                    nvgGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG3"));
                    break;
                case NVTubeType.Fullscreen:
                    nvgGameObject = new GameObject("NVGF");
                    Image nvgImage = nvgGameObject.AddComponent<Image>();
                    nvgImage.material = AssetBundleUtil.GetAsset<Material>("UINVG");
                    nvgMaterial = nvgImage.material;
                    RectTransform rect = nvgGameObject.GetComponent<RectTransform>();
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    
                    rect.sizeDelta = Vector2.zero;
                    
                    break;
                    
                default:
                    nvgGameObject = Object.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1"));
                    break;
            }
            
            nvgGameObject.transform.SetParent(nvgCanvas.transform,false);
            nvgGameObject.transform.localPosition = Vector3.zero;
            nvgGameObject.transform.SetAsFirstSibling();
            nvgGameObject.SetActive(NightVision.i != null && NightVision.i.nightVisSelected);

            
            
            Transform mask = nvgGameObject.transform.Find("NVGMask");
            if (mask) nvgMaterial = mask.GetComponent<Image>().material;

            Plugin.logger.LogInfo(nvgMaterial);
            lastTubeType = currentTubeType;
            
            
            
            ChangeGain(false);


            UpdateNVGMaterial();
        }

        
        // ---
        
        
        public static void UpdateNVGMaterial()
        {
            if (nvgMaterial is null) return;
            
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
        
        // Property Changes
        #region

        public static int currentPhase;
        
        private static void ChangeGain(bool direction)
        {
            int gainPhases = PluginConfig.currentProfile.NvGainPhases.Value;
            float gainMin = PluginConfig.currentProfile.NvGainMin.Value;
            float gainMax = PluginConfig.currentProfile.NvGainMax.Value;
            
            currentPhase += direction ? -1 : 1;
            currentPhase = Mathf.Clamp(currentPhase, 0, gainPhases);
            
            float intensity = Mathf.Lerp(gainMin, gainMax, (float)currentPhase / gainPhases);
            
            nvgMaterial.SetFloat(_nvIntensity, intensity);
            UpdateNoiseIntensity();
            
        }

       
        
        public static void UpdateColor()
        {
            nvgMaterial.SetColor(_nvColor, PluginConfig.currentProfile.NvColor.Value);
        }
        
        public static void ToggleNoise()
        {
            
            float enabled = PluginConfig.currentProfile.NoiseEnabled.Value? 1 : 0;
            nvgMaterial.SetFloat(_noiseEnabled, enabled);
        }
        
        public static void TogglePixelation()
        {
            float enabled = PluginConfig.currentProfile.NvPixelate.Value ? 1 : 0;
            nvgMaterial.SetFloat(_pixelate, enabled);
        }
        
        public static void UpdateResolution()
        {
            nvgMaterial.SetFloat(_nvResolution, PluginConfig.currentProfile.NvResolution.Value);
            
        }
        
        public static void UpdateNoiseIntensity()
        {
            float baseIntensity = PluginConfig.currentProfile.NoiseIntensity.Value; 
            
            float matNoiseIntensity = Mathf.Lerp(baseIntensity, baseIntensity * PluginConfig.currentProfile.NoiseIntensityGainMultiplier.Value, currentPhase /  (float)PluginConfig.currentProfile.NvGainPhases.Value);
            
            nvgMaterial.SetFloat(NVGHandler._noiseIntensity, matNoiseIntensity);
            
        }

        public static void UpdateNoiseScale()
        {
            nvgMaterial.SetFloat(_noiseScale, PluginConfig.currentProfile.NoiseScale.Value);
            
        }
        
        public static void UpdateNoiseFPS()
        {
            nvgMaterial.SetFloat(_noiseFPS, PluginConfig.currentProfile.NoiseFPS.Value);
        }

        public static void UpdateNoiseBlending()
        {
            
            nvgMaterial.SetFloat(_noiseBlending, (int)PluginConfig.currentProfile.NoiseBlending.Value);
        }
        
        public static void ToggleDistortion()
        {
            float enabled = PluginConfig.currentProfile.DistortionEnabled.Value ? 1 : 0;
            nvgMaterial.SetFloat(_distortionEnabled, enabled);
        }

        
        public static void UpdateDistortionStrength()
        {
            nvgMaterial.SetFloat(_distortionStrength, PluginConfig.currentProfile.DistortionStrength.Value);
        }
        
        public static void UpdateDistortionPower()
        {
            nvgMaterial.SetFloat(_distortionPower, PluginConfig.currentProfile.DistortionPower.Value);
        }

        
        public static void UpdateScale()
        {
            if (PluginConfig.currentProfile.TubeType.Value == NVTubeType.Fullscreen) return;   
            
            float scale = PluginConfig.currentProfile.NvScale.Value;
            
            nvgGameObject.transform.localScale = new Vector3(scale,scale);
        }
        
        public static void UpdatePosition()
        {
            if (PluginConfig.currentProfile.TubeType.Value == NVTubeType.Fullscreen) return;
            
            nvgGameObject.transform.localPosition = PluginConfig.currentProfile.NvPosition.Value;
        }
        
        #endregion
        

        private static void ProcessControls()
        {
            if (nvgGameObject)
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
