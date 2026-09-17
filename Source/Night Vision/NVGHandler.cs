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
        private static readonly int _nvResolution = Shader.PropertyToID("_Resolution");

        private static readonly int _noiseIntensity = Shader.PropertyToID("_NoiseIntensity");
        private static readonly int _noiseFPS = Shader.PropertyToID("_NoiseFPS");
        private static readonly int _noiseScale = Shader.PropertyToID("_NoiseScale");
        
        
        public static void Initialize([CanBeNull] CombatHUD _hud = null)
        {
            // if (NVGGameObject != null) return;
            
            Plugin.Logger.LogInfo("Initializing NVG");
            
            CombatHUD HUD = CombatHUD.i ?? _hud;
            
            Camera.main.gameObject.AddComponent<CameraHook>();

            
            switch (PluginConfig.currentProfile.tubeType.Value)
            {
                case NVTubeType.Mono:
                    NVGGameObject = GameObject.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1M"));
                    break;
                case NVTubeType.Double:
                    NVGGameObject = GameObject.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1"));
                    break;
                case NVTubeType.Quad:
                    NVGGameObject = GameObject.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG3"));
                    break;
                default:
                    NVGGameObject = GameObject.Instantiate(AssetBundleUtil.GetAsset<GameObject>("NVG1"));
                    break;
            }
            Plugin.Logger.LogInfo(HUD is null); 
            
            NVGGameObject.transform.SetParent(HUD.transform,true);
            NVGGameObject.transform.localPosition = Vector3.zero;
            NVGGameObject.transform.SetAsFirstSibling();
            NVGGameObject.SetActive(NightVision.i != null && NightVision.i.nightVisSelected);

            
            GameObject mask = NVGGameObject.transform.Find("NVGMask").gameObject;
            NVGMaterial = mask.GetComponent<Image>().material;

            currentPhase = 0;
          
            
            ChangeGain(false);
            UpdateScale();
            UpdatePosition();
            UpdateColor();
            UpdateResolution();
            UpdateNoiseIntensity();
            UpdateNoiseFPS();
            UpdateNoiseScale();
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
            Plugin.Logger.LogInfo(NVGHandler.NVGMaterial.GetFloat(NVGHandler._nvIntensity));
        }

        public static void UpdateColor()
        {
            NVGMaterial.SetColor(_nvColor, PluginConfig.currentProfile.nvColor.Value);
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
            Plugin.Logger.LogInfo(NVGHandler.NVGMaterial.GetFloat(_noiseScale));
        }
        
        public static void UpdateNoiseFPS()
        {
            NVGMaterial.SetFloat(_noiseFPS, PluginConfig.currentProfile.noiseFPS.Value);
        }

        public static void UpdateScale()
        {
            float scale = PluginConfig.currentProfile.nvScale.Value;
            
            NVGGameObject.transform.localScale = new Vector3(scale,scale);
        }
        
        public static void UpdatePosition()
        {
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
