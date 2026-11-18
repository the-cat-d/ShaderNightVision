using JetBrains.Annotations;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ShaderNightVision
{
    public enum NVTubeType 
    {
        [Description("Mono Tube")]
        Mono,
        [Description("Dobule Tube")]
        Double,
        [Description("Quad Tube (GNPVG)")]
        Quad,
        Fullscreen,
        Custom,
            
    }

    public enum NoiseBlendingMode
    {
        Subtractive = 1,
        Additive = 0,
        Multiply = 2,
    }

    public class NVTubes
    {
        [CanBeNull]
        public static Sprite GetCustomTubeSprite()
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"CustomNVG.png");

            if (!File.Exists(filePath))
            {
                Plugin.logger.LogWarning($"Custom NVG file not found @{filePath}");
                return null;
                
            }
            
            byte[] fileData = File.ReadAllBytes(filePath);
            
            Texture2D tex = new Texture2D(2, 2);

            if (!tex.LoadImage(fileData))
            {
                Plugin.logger.LogError($"Custom NVG file could not be loaded @{filePath}");
                
                return null;
            }       
            
            tex.filterMode = FilterMode.Bilinear;
            
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new (0.5f, 0.5f));
        }
    }
    
}
