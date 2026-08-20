using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public class PixelDownscalePass : ScriptableRenderPass
{
    private Vector2Int targetResolution;

    public PixelDownscalePass(RenderPassEvent renderPassEvent)
    {
        this.renderPassEvent = renderPassEvent;
        profilingSampler = new ProfilingSampler(nameof(PixelDownscalePass));
    }

    public void SetResolution(Vector2Int resolution)
    {
        targetResolution = resolution;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (targetResolution.x <= 0 || targetResolution.y <= 0)
            return;
        
        var resourceData = frameData.Get<UniversalResourceData>();
        
        var colorDesc = renderGraph.GetTextureDesc(resourceData.cameraColor);
        colorDesc.width = targetResolution.x;
        colorDesc.height = targetResolution.y;
        colorDesc.filterMode = FilterMode.Point;
        
        var depthDesc = renderGraph.GetTextureDesc(resourceData.cameraDepth);
        depthDesc.width = targetResolution.x;
        depthDesc.height = targetResolution.y;
        
        var pixelColor = renderGraph.CreateTexture(colorDesc);
        var pixelDepth = renderGraph.CreateTexture(depthDesc);

        // 4. Redirect pipeline targets so all subsequent geometry passes render into them
        resourceData.cameraColor = pixelColor;
        resourceData.cameraDepth = pixelDepth;
    }
}
