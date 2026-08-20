using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class PixelUpscalePass : ScriptableRenderPass
{
    private static readonly ProfilingSampler m_ProfilingScope = new ProfilingSampler("Pixel Upscale Pass");

    private Vector2 pixelOffset;
    private Material blitMaterial;

    private class PassData
    {
        public Material material;
        public TextureHandle source;
    }

    public PixelUpscalePass(RenderPassEvent evt, Material blitMaterial)
    {
        renderPassEvent = evt;
        this.blitMaterial = blitMaterial;
        profilingSampler = m_ProfilingScope;
        this.requiresIntermediateTexture = true;
    }

    public void UpdatePixelOffset(Vector2 pixelOffset)
    {
        this.pixelOffset = pixelOffset;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (blitMaterial == null)
            return;

        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();

        //This should be the pixelated handle, so the RT with reduced resolution.
        var srcHandle = resourceData;
        var cameraTargetDescriptor = cameraData.cameraTargetDescriptor;

        using (var builder =
            renderGraph.AddRasterRenderPass<PassData>("Pixel Upscale Pass", out var passData, m_ProfilingScope))
        {
            passData.material = blitMaterial;
            passData.source = srcHandle.cameraColor;

            builder.UseTexture(passData.source);
            builder.AllowPassCulling(false);

            //Here we create the upscaled RT, our screen texture we'll upscale our pixels on.
            var dstDesc = renderGraph.GetTextureDesc(resourceData.cameraColor);
            dstDesc.name = "_PixelUpscaleTexture";

            //We retrieve the output resolution from the camera target descriptor.
            dstDesc.width = cameraTargetDescriptor.width;
            dstDesc.height = cameraTargetDescriptor.height;
            dstDesc.clearBuffer = false;
            var outputTexture = renderGraph.CreateTexture(dstDesc);

            builder.SetRenderAttachment(outputTexture, 0);
            var offset = -pixelOffset;
            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, offset.x, offset.y),
                    data.material, 0);
            });

            resourceData.cameraColor = outputTexture;
        }
    }
}
