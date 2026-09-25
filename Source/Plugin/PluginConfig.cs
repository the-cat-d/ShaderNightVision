using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System.Reflection;
using UnityEngine;

namespace ShaderNightVision
{
    public static class PluginConfig
    {


      
        public static ConfigEntry<KeyCode> nvGainIncreaseKey;
        public static ConfigEntry<KeyCode> nvGainDecreaseKey;
      
        public static ConfigEntry<bool> aircraftProfiles;
        
        
        private static NVGProfile defaultProfile;
        private static NVGProfile aircraftProfile;
        
        public static NVGProfile currentProfile;
        
        private static ConfigFile config;
        
        
        public static void BindAll(ConfigFile newConfig)
        {
            config = newConfig;
            
            
            nvGainIncreaseKey = config.Bind("Night Vision - Keybinds","Gain Increase Keybind",KeyCode.Equals);
            nvGainDecreaseKey = config.Bind("Night Vision - Keybinds","Gain Decrease Keybind",KeyCode.Minus);


            aircraftProfiles = config.Bind("Night Vision - Profiles", "Enable Aircraft Specific Profiles", false);
            aircraftProfiles.SettingChanged += (sender, args) =>
            {
                if (aircraftProfiles.Value)
                {
                    ShowAircraftProfile();
                }
                else
                {
                    ShowDefaultProfile();

                }
                
                
            };

            if (aircraftProfiles.Value)
            {
                ShowAircraftProfile();


            }
            else
            {
                
                ShowDefaultProfile();
            }
            
            


        }

        private static void ShowAircraftProfile()
        {
            string t;
                    
            if (CombatHUD.i == null)
            {
                t = "T/A-30 Compass";
            }
            else
            {
                t = CombatHUD.i.aircraft.definition.unitName;
            }
                    
            BindAircraftProfile(t);
        }

        private static void ShowDefaultProfile()
        {
            if (defaultProfile is null)
            {
                defaultProfile = new NVGProfile(config);
                defaultProfile.ToggleEntries(true);
            }
            else
            {
                defaultProfile.ToggleEntries(true);
            }
                
            aircraftProfile?.ToggleEntries(false);
            
            currentProfile = defaultProfile;
            NVGHandler.UpdateNVGMaterial();
            

        }
        
        public static void BindAircraftProfile(string aircraftName)
        {
            
            
            defaultProfile?.ToggleEntries(false);
            aircraftProfile?.ToggleEntries(false);
            
            Plugin.logger.LogInfo(aircraftName);
            

            aircraftProfile = NVGProfile.CreateAircraftProfile(config,aircraftName);
            currentProfile = aircraftProfile;
     
            aircraftProfile.ToggleEntries(true);
            NVGHandler.UpdateNVGMaterial();
            
        }

       
        
        private static BaseUnityPlugin configManager;
        
        private static MethodInfo rebuildListMethod;
        
        public static void RebuildList()
        {
            if (rebuildListMethod == null)
            {
                if (!Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.configurationmanager", out var pluginInfo))
                {
                    return;
                }

                configManager = pluginInfo.Instance;
            
                if (configManager == null) return;
            
                rebuildListMethod = configManager.GetType().GetMethod("BuildSettingList",BindingFlags.Public | BindingFlags.Instance);
            
                
            }
            
            rebuildListMethod?.Invoke(configManager, null);
        } 

      
    }
}
