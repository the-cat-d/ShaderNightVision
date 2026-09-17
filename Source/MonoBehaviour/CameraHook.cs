using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderNightVision.MonoBehaviour
{
   public class CameraHook : UnityEngine.MonoBehaviour
    {
        private Camera baseCamera;
        private Camera cockpitRendererCam;
        
        private RenderTexture finalRenderTexture;
        
        private CommandBuffer blitCmd;


        private readonly string globalTextureName = "_CustomBGTex";

        void Awake()
        {
            baseCamera = GetComponent<Camera>();
            if (baseCamera == null) return;
            
            
            finalRenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            finalRenderTexture.filterMode = FilterMode.Bilinear;
            finalRenderTexture.wrapMode = TextureWrapMode.Clamp;
            finalRenderTexture.Create();

            blitCmd = new CommandBuffer { name = "ShaderNV_GrabPass" };
            Shader.SetGlobalTexture(globalTextureName, finalRenderTexture);

            
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
           
            if (cockpitRendererCam == null)
            {
                GameObject camObj = GameObject.Find("cockpitRenderer");
                if (camObj != null)
                {
                    cockpitRendererCam = camObj.GetComponent<Camera>();
                }
            }

            
            Camera currentTarget = (cockpitRendererCam != null) ? cockpitRendererCam : baseCamera;

            if (camera != currentTarget) return;

            blitCmd.Clear();
            
            
            if (currentTarget.activeTexture != null)
            {
                blitCmd.Blit(currentTarget.activeTexture, finalRenderTexture);
            }
            else if (baseCamera.activeTexture != null)
            {
                blitCmd.Blit(baseCamera.activeTexture, finalRenderTexture);
            }
            else
            {
                // Fallback
                RenderTargetIdentifier activeBuffer = BuiltinRenderTextureType.CurrentActive;
                blitCmd.Blit(activeBuffer, finalRenderTexture);
            }

            
            blitCmd.SetGlobalTexture(globalTextureName, finalRenderTexture);
            context.ExecuteCommandBuffer(blitCmd);
            context.Submit();
        }

        void OnDestroy()
        {
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;

            blitCmd?.Release();
            if (finalRenderTexture != null)
            {
                finalRenderTexture.Release();
                Destroy(finalRenderTexture);
            }
        }
    }

}
