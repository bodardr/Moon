using UnityEngine;
namespace Utils
{
    public static class LayerMaskUtility
    {
        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            return (1 << layer & layerMask.value) != 0;
        }
        
        public static bool IsInLayerMask(this int layer, LayerMask layerMask)
        {
            return (1 << layer & layerMask.value) != 0;
        }
    }
}
