using HarmonyLib;
using System;

namespace ShaderNightVision
{
    public class NVPatches
    {

        
        
        [HarmonyPatch(typeof(CombatHUD), "Awake")]
        class HUDPatch
        {
            [HarmonyPrefix]
            public static void Init(CombatHUD __instance)
            {

                try
                {
                    NVGHandler.Initialize(__instance);

                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e.ToString());
                }
            }
        }

        class  NVGPatches
        {
            [HarmonyPatch(typeof(NightVision), "NightVis_OnSwitchCam")]
            class NVGInitPatch
            {
                [HarmonyPostfix]
                static void Init(NightVision __instance)
                {
                    NVGHandler.NVGGameObject.SetActive(NightVision.i.nightVisSelected);
                }
            }
            
            [HarmonyPatch(typeof(NightVision), "Toggle")]
            class NVGTogglePatch
            {
                [HarmonyPostfix]
                public static void Toggle(NightVision __instance)
                {
                    if (NVGHandler.NVGGameObject is null) return;
                    Plugin.Logger.LogDebug($"setting nv to {NightVision.i.nightVisSelected}");
                    NVGHandler.NVGGameObject.SetActive(NightVision.i.nightVisSelected);
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
        
    }
}
