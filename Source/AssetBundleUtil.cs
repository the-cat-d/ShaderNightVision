using JetBrains.Annotations;
using System.Collections.Generic;
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
                    Plugin.logger.LogError($"Assetbundle \"{assetBundleName}\" not found ({resourceName})");
                    return null;
                }
                bundleBytes = new byte[stream.Length];
                stream.Read(bundleBytes, 0, bundleBytes.Length);
            }
            
            AssetBundle loadedAssetBundle = AssetBundle.LoadFromMemory(bundleBytes);

            if (loadedAssetBundle == null)
            {
                Plugin.logger.LogError("Unable to load Assetbundle");
                return null;
            }
            
            if (assetBundle != null)
            {
                assetBundle.Unload(false);
            }
            
            assetBundle = loadedAssetBundle;
            
            foreach (string name in  assetBundle.GetAllAssetNames())
            {
                Plugin.logger.LogDebug(name);
            }

            Plugin.logger.LogInfo($"Assetbundle loaded with {assetBundle.name.Length} asset(s)");

         
            return loadedAssetBundle;
            
        }

        private static Dictionary<string, Object> cachedAssets = new Dictionary<string, Object>();
        
        [CanBeNull]
        public static T GetAsset<T>(string assetName)  where T : Object
        {
            if (assetBundle == null)
            {
                Plugin.logger.LogError("Assetbundle not found");
                return null;
            }
            
            T asset;

            if (cachedAssets.TryGetValue(assetName, out Object cachedAsset))
            {
                Plugin.logger.LogInfo($"Asset \"{assetName}\" found in cache");
                asset = (T)cachedAsset;
                
            }
            else
            { 
                asset = assetBundle.LoadAsset<T>(assetName);
                cachedAssets.Add(assetName, asset);
                Plugin.logger.LogInfo($"Asset \"{assetName}\" cached");
            }
            
            if (asset == null)
            {
                Plugin.logger.LogError($"Asset \"{assetName}\" not found");
                return null;
            }

            Plugin.logger.LogInfo($"Asset \"{assetName}\" found");
            
            return asset;
        }
        
        
        
    }
}
