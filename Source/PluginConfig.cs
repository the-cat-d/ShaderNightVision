using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderNightVision
{
    public static class PluginConfig
    {


      
        public static ConfigEntry<KeyCode> nvGainIncreaseKey;
        public static ConfigEntry<KeyCode> nvGainDecreaseKey;
      

        
        
        public static NVGProfile currentProfile;
        
        public static void BindAll(ConfigFile config)
        {
            
            
            nvGainIncreaseKey = config.Bind("Keybinds - Keybinds","Gain Increase Keybind",KeyCode.Equals);
            nvGainDecreaseKey = config.Bind("Keybinds - Keybinds","Gain Decrease Keybind",KeyCode.Minus);
            
            
            currentProfile = new NVGProfile(config,NVTubeType.Double,
                new Color(0.22f, 1, 0.22f),
                40f,
                8f,
                4,
                0.02f,
                3f,
                24f,
                800f,
                1080,
                1.5f,
                new Vector2(0,0)

                
                );
            
            



        }


      
    }
}
