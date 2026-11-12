using HarmonyLib;
using System;

namespace ShaderNightVision
{
    public static class NVPatches
    {

        private static class NVGPatches
        {
            [HarmonyPatch(typeof(NightVision), "NightVis_OnSwitchCam")]
            class NVGInitPatch
            {
                [HarmonyPostfix]
                static void NVGCamSwitch(NightVision __instance)
                {
                    Plugin.logger.LogWarning(NVGHandler.nvgGameObject);
                    if (!NVGHandler.nvgGameObject)
                    {
                        NVGHandler.InitializeNVGObject(force: true);
                        return;
                    };
                    NVGHandler.nvgGameObject.SetActive(NightVision.i.nightVisSelected);
                }
            }
            
            [HarmonyPatch(typeof(NightVision), "Toggle")]
            class NVGTogglePatch
            {
                [HarmonyPostfix]
                public static void NVGToggle(NightVision __instance)
                {
                    if (!NVGHandler.nvgGameObject)
                    {
                        NVGHandler.InitializeNVGObject(force: true);
                        return;
                    }
                    Plugin.logger.LogDebug($"setting nv to {NightVision.i.nightVisSelected}");
                    NVGHandler.nvgGameObject.SetActive(NightVision.i.nightVisSelected);
                }
            }
            
            [HarmonyPatch(typeof(NightVision), "Update")]
            class NVGUpdatePatch
            {
                [HarmonyPrefix]
                public static bool NVGUpdate(NightVision __instance)
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
            public static void NVGHUDInit(CombatHUD __instance)
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
            public static void BindProfile(Aircraft __instance)
            {
                
                if (__instance != SceneSingleton<CombatHUD>.i.aircraft || !PluginConfig.aircraftProfiles.Value) return;
                
                
                PluginConfig.BindAircraftProfile(__instance.definition.unitName);
            }
        }
        
        [HarmonyPatch(typeof(CameraStateManager), "SwitchState")]
        class CameraStateSwitchPatch
        {
            [HarmonyPostfix]
            public static void OnCamStateSwitch(CameraStateManager __instance)
            {

                if (CameraStateManager.cameraMode == CameraMode.cockpit)
                {
                    NVGHandler.InitializeNVGObject();
                }
                else if (PluginConfig.fullscreenOnCamSwitch.Value)
                {
                    NVGHandler.InitializeNVGObject(NVTubeType.Fullscreen);
                }

            }
        }

    }
}
