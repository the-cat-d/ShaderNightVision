using HarmonyLib;

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
                
                NVGHandler.Initialize(__instance);
                
            }
        }

        class  NVGPatches
        {
            [HarmonyPatch(typeof(NightVision), "Toggle")]
            class NVGTogglePatch
            {
                [HarmonyPostfix]
                public static void Toggle(NightVision __instance)
                {
                    if (NVGHandler.NVGGameObject is null) return;
                    
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
