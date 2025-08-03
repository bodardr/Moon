using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelBlitRendererFeature : ScriptableRendererFeature
{
    private PixelateCamera pixelCamera;
    private PixelBlitPass upscalePass;
    private Material blitMaterial;

    public override void Create()
    {
        blitMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Blit/CameraPixelBlit"));
        upscalePass = new PixelBlitPass(RenderPassEvent.BeforeRenderingPostProcessing, blitMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(upscalePass);
    }
    
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (pixelCamera == null)
            pixelCamera = PixelateCamera.Instance;

        if (pixelCamera == null)
            return;
        
        upscalePass.Setup(pixelCamera.PixelatedHandle,renderer.cameraColorTargetHandle, pixelCamera.SubPixelOffset);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(blitMaterial);

        upscalePass?.Dispose();
        upscalePass = null;
    }
}