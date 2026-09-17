using JetBrains.Annotations;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace ShaderNightVision
{
    
    
    public static class AssetBundleUtil
    {
        
        
        public static AssetBundle assetBundle;
        
        [CanBeNull]
        public static AssetBundle LoadAssetBundle(string assetBundleName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            string resourceName = $"{nameof(ShaderNightVision)}.EmbeddedResources.{assetBundleName}";

            
            
            byte[] bundleBytes;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Plugin.Logger.LogError($"Assetbundle \"{assetBundleName}\" not found ({resourceName})");
                    return null;
                }
                bundleBytes = new byte[stream.Length];
                stream.Read(bundleBytes, 0, bundleBytes.Length);
            }
            
            AssetBundle loadedAssetBundle = AssetBundle.LoadFromMemory(bundleBytes);

            if (loadedAssetBundle == null)
            {
                Plugin.Logger.LogError("Unable to load Assetbundle");
                return null;
            }
            
            if (assetBundle != null)
            {
                assetBundle.Unload(false);
            }
            
            assetBundle = loadedAssetBundle;
            
            foreach (string name in  assetBundle.GetAllAssetNames())
            {
                Plugin.Logger.LogDebug(name);
            }

            Plugin.Logger.LogInfo($"Assetbundle loaded with {assetBundle.name.Length} asset(s)");

         
            return loadedAssetBundle;
            
        }
        
        [CanBeNull]
        public static T LoadAsset<T>(string assetName)  where T : Object
        {
            if (assetBundle == null)
            {
                Plugin.Logger.LogError("Assetbundle not found");
                return null;
            }
            
            T asset = assetBundle.LoadAsset<T>(assetName);

            if (asset == null)
            {
                Plugin.Logger.LogError($"Asset \"{assetName}\" not found");
                return null;
            }

            Plugin.Logger.LogInfo($"Asset \"{assetName}\" loaded");
            
            return asset;
        }
        
        
        
    }
}
