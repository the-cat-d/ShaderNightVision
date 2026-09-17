using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace ShaderNightVision;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private static Harmony _harmony;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loading");

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll();
        
        AssetBundleUtil.LoadAssetBundle("shadernv");
        
        PluginConfig.BindAll(Config);

    }

    private void Update()
    {
        NVGHandler.NVGUpdate();
    }

}
