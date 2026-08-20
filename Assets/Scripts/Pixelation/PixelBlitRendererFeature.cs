using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelBlitRendererFeature : ScriptableRendererFeature
{
    private PixelateCamera pixelCamera;
    private PixelDownscalePass downscalePass;
    private PixelUpscalePass upscalePass;
    private Material blitMaterial;

    public override void Create()
    {
        var shader = Shader.Find("Blit/CameraPixelBlit");
        if (shader != null)
            blitMaterial = CoreUtils.CreateEngineMaterial(shader);

        downscalePass = new PixelDownscalePass(RenderPassEvent.BeforeRendering);
        upscalePass = new PixelUpscalePass(RenderPassEvent.BeforeRenderingPostProcessing, blitMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        downscalePass.SetResolution(pixelCamera.ReferenceResolution);
        renderer.EnqueuePass(downscalePass);
        
        // Pass null for upscaleHandle to let RenderGraph dynamically bind resourceData.cameraColor
        upscalePass.UpdatePixelOffset(pixelCamera.SubPixelOffset);
        renderer.EnqueuePass(upscalePass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(blitMaterial);
        upscalePass = null;
        downscalePass = null;
    }
}