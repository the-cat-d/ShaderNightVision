using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ShaderNightVision;

[BepInPlugin(PluginInfo.PluginGUID, PluginInfo.PluginName, PluginInfo.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource logger;

    private static Harmony harmony;
    
    private void Awake()
    {
        // Plugin startup logic
        logger = base.Logger;
        logger.LogInfo($"Plugin {PluginInfo.PluginName} is loading");

        harmony = new Harmony(PluginInfo.PluginGUID);
        harmony.PatchAll();
        
        AssetBundleUtil.LoadAssetBundle("shadernv");
        
        PluginConfig.BindAll(Config);
        
    }

    private void Update()
    {
        NVGHandler.NVGUpdate();
        
        
    }

}
