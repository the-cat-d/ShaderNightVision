using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace ShaderNightVision;

[BepInPlugin(PluginInfo.PluginGUID, PluginInfo.PluginName, PluginInfo.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private static Harmony _harmony;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.PluginName} is loading");

        _harmony = new Harmony(PluginInfo.PluginGUID);
        _harmony.PatchAll();
        
        AssetBundleUtil.LoadAssetBundle("shadernv");
        
        PluginConfig.BindAll(Config);
        
    }

    private void Update()
    {
        NVGHandler.NVGUpdate();
    }

}
