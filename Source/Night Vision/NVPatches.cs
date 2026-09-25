using HarmonyLib;
using System;

namespace ShaderNightVision
{
    public class NVPatches
    {
        
        class  NVGPatches
        {
            [HarmonyPatch(typeof(NightVision), "NightVis_OnSwitchCam")]
            class NVGInitPatch
            {
                [HarmonyPostfix]
                static void Init(NightVision __instance)
                {
                    NVGHandler.nvgGameObject.SetActive(NightVision.i.nightVisSelected);
                }
            }
            
            [HarmonyPatch(typeof(NightVision), "Toggle")]
            class NVGTogglePatch
            {
                [HarmonyPostfix]
                public static void Toggle(NightVision __instance)
                {
                    if (NVGHandler.nvgGameObject is null) return;
                    Plugin.logger.LogDebug($"setting nv to {NightVision.i.nightVisSelected}");
                    NVGHandler.nvgGameObject.SetActive(NightVision.i.nightVisSelected);
                }
            }
            
            [HarmonyPatch(typeof(NightVision), "Update")]
            class NVGUpdatePatch
            {
                [HarmonyPrefix]
                public static bool Update(NightVision __instance)
                {
                    if (GameManager.playerInput.GetButtonDown("Night Vis"))
                    {
                        NightVision.Toggle();
                    }
                    return false;
                }
            }
        }
        
        
        [HarmonyPatch(typeof(CombatHUD), "Awake")]
        class HUDPatch
        {
            [HarmonyPrefix]
            public static void Init(CombatHUD __instance)
            {

                try
                {
                    NVGHandler.InitializeCanvas(__instance);
                    
                }
                catch (Exception e)
                {
                    Plugin.logger.LogError(e.ToString());
                }
            }
        }
        
        [HarmonyPatch(typeof(Aircraft), "OnStartClient")]
        class AircraftInitPatch
        {
            [HarmonyPostfix]
            public static void Init(Aircraft __instance)
            {
                Plugin.logger.LogInfo(__instance);
                
                
                
                if (__instance != SceneSingleton<CombatHUD>.i.aircraft || !PluginConfig.aircraftProfiles.Value) return;
                
                Plugin.logger.LogInfo(__instance.definition.unitName);
                PluginConfig.BindAircraftProfile(__instance.definition.unitName);
            }
        }
        
        [HarmonyPatch(typeof(CameraStateManager), "SwitchState")]
        class CameraStateSwitchPatch
        {
            [HarmonyPostfix]
            public static void Init(CameraStateManager __instance)
            {
                Plugin.logger.LogInfo(CameraStateManager.cameraMode);

                if (CameraStateManager.cameraMode == CameraMode.cockpit)
                {
                    NVGHandler.InitializeNVG();
                }
                else
                {
                    NVGHandler.InitializeNVG(NVTubeType.Fullscreen);
                }

            }
        }

    }
}
