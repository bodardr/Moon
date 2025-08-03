using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelBlitPass : ScriptableRenderPass
{
    private static readonly ProfilingSampler m_ProfilingScope = new ProfilingSampler("Pixel Upscale Pass");
    private static readonly int offsetID = Shader.PropertyToID("_Offset");

    private Vector2 pixelOffset;
    private RTHandle source;
    private RTHandle pixelatedHandle;
    private Material blitMaterial;

    public PixelBlitPass(RenderPassEvent evt, Material blitMaterial)
    {
        renderPassEvent = evt;
        this.blitMaterial = blitMaterial;
    }

    public void Setup(RTHandle pixelHandle, RTHandle upscaleHandle, Vector2 pixelOffset)
    {
        source = upscaleHandle;
        pixelatedHandle = pixelHandle;
        this.pixelOffset = pixelOffset;
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (PixelateCamera.Instance == null || renderingData.cameraData.camera != PixelateCamera.Instance.UpscaleCamera)
            return;

        blitMaterial.SetVector(offsetID, pixelOffset);

        var cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingScope))
        {
            CoreUtils.SetRenderTarget(cmd, pixelatedHandle,
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                ClearFlag.None, Color.clear);
            Blit(cmd, pixelatedHandle, source, blitMaterial);
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
    }
}
