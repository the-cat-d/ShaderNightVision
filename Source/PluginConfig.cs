using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderNightVision
{
    public static class PluginConfig
    {
        public static ConfigEntry<bool> nvEnabled;

        
      
        public static ConfigEntry<KeyCode> nvGainIncreaseKey;
        public static ConfigEntry<KeyCode> nvGainDecreaseKey;
      
        
        
        
        
        // public static Dictionary<NVTubeType,NVGProfile> nvProfiles = new Dictionary<NVTubeType,NVGProfile>();
        
        public static NVGProfile currentProfile;
        
        public static void BindAll(ConfigFile config)
        {
            
            
            nvGainIncreaseKey = config.Bind("Keybinds - Keybinds","Gain Increase Keybind",KeyCode.Equals);
            nvGainDecreaseKey = config.Bind("Keybinds - Keybinds","Gain Decrease Keybind",KeyCode.Minus);
            
            
            nvEnabled = config.Bind("Night Vision - Main","Night Vision Enabled",true);
            
            
           
            
            // nvProfiles[NVTubeType.Mono] = new NVGProfile(config,NVTubeType.Mono,
            //     new Color(0.76f, 1, 0.64f),
            //     24f,
            //     5f,
            //     3,
            //     0.3f,
            //     1.5f,
            //     12f,
            //     400f,
            //     1080,
            //     1.5f,
            //     new Vector2(0,0)
            //
            //     
            //     );
            
            currentProfile = new NVGProfile(config,NVTubeType.Double,
                new Color(0.22f, 1, 0.22f),
                40f,
                8f,
                4,
                0.02f,
                24f,
                3f,
                800f,
                1080,
                1.5f,
                new Vector2(0,0)

                
                );
            
            // nvProfiles[NVTubeType.Quad] = new NVGProfile(config,NVTubeType.Quad,
            //     new Color(0.56f, 0.87f, 0.93f),
            //     35f,
            //     10f,
            //     6,
            //     0.0008f,
            //     5f,
            //     1.5f,
            //     1000f,
            //     480000,
            //     1.5f,
            //     new Vector2(0,0)
            //
            //     
            //     );
            //


            // UpdateCurrentProfile();



        }


        // private static void UpdateCurrentProfile()
        // {
        //
        //     currentProfile = nvProfiles[nvTube.Value];
        //
        // }
        
    }
}
