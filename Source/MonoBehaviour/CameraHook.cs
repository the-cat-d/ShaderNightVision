using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderNightVision.MonoBehaviour
{
   public class CameraHook : UnityEngine.MonoBehaviour
    {
        private Camera BaseCamera;
        private Camera CockpitRendererCam;
        
        private RenderTexture FinalRenderTexture;
        
        private CommandBuffer BlitCmd;
        
        private static readonly int _globalTexture = Shader.PropertyToID( "_CustomBGTex");

    

        void Awake()
        {
            BaseCamera = GetComponent<Camera>();
            if (BaseCamera == null) return;
            
            
            FinalRenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            FinalRenderTexture.filterMode = FilterMode.Bilinear;
            FinalRenderTexture.wrapMode = TextureWrapMode.Clamp;
            FinalRenderTexture.Create();

            BlitCmd = new CommandBuffer { name = "ShaderNV_GrabPass" };
            Shader.SetGlobalTexture(_globalTexture, FinalRenderTexture);

            
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            
            if (CockpitRendererCam == null)
            {
                GameObject camObj = GameObject.Find("cockpitRenderer");
                if (camObj != null)
                {
                    CockpitRendererCam = camObj.GetComponent<Camera>();
                }
            }

            
            Camera currentTarget = (CockpitRendererCam != null &&  CameraStateManager.cameraMode == CameraMode.cockpit) ? CockpitRendererCam : BaseCamera;

            if (camera != currentTarget) return;

            BlitCmd.Clear();
            
            
            if (currentTarget.activeTexture != null)
            {
                BlitCmd.Blit(currentTarget.activeTexture, FinalRenderTexture);
            }
            else if (BaseCamera.activeTexture != null)
            {
                BlitCmd.Blit(BaseCamera.activeTexture, FinalRenderTexture);
            }
            else
            {
                // Fallback
                RenderTargetIdentifier activeBuffer = BuiltinRenderTextureType.CurrentActive;
                BlitCmd.Blit(activeBuffer, FinalRenderTexture);
            }
            
            BlitCmd.SetGlobalTexture(_globalTexture, FinalRenderTexture);
            context.ExecuteCommandBuffer(BlitCmd);
            context.Submit();
        }

        void OnDestroy()
        {
            
            Plugin.logger.LogDebug("Camera hook destroyed");
            
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;

            BlitCmd?.Release();
            if (FinalRenderTexture != null)
            {
                FinalRenderTexture.Release();
                Destroy(FinalRenderTexture);
            }
        }
    }

}
