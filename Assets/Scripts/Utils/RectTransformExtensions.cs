using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

public static class RectTransformExtensions
{
    private static readonly Vector3[] targetWorldCorners = new Vector3[4];

    public static Vector3 NormalizedPointToLocal(this RectTransform rectTransform, Vector2 normalizedPosition)
    {
        return Rect.NormalizedToPoint(rectTransform.rect, normalizedPosition);
    }

    public static Vector3 NormalizedPointToWorld(this RectTransform rectTransform, Vector2 normalizedPosition)
    {
        return rectTransform.localToWorldMatrix.MultiplyPoint(NormalizedPointToLocal(rectTransform,
            normalizedPosition));
    }

    public static Vector3 PivotToWorld(this RectTransform rectTransform)
    {
        return rectTransform.position;
    }

    public static Vector3 PivotToLocal(this RectTransform rectTransform)
    {
        return rectTransform.localPosition;
    }

    public static void FitToTarget(this RectTransform rectTransform, RectTransform target, bool conformPosition,
        bool conformSize)
    {
        target.GetWorldCorners(targetWorldCorners);

        var size = target.rect.size * Vector2.Max(Vector2.one * Vector2.kEpsilon,
            new Vector2(target.lossyScale.x / rectTransform.lossyScale.x,
                target.lossyScale.y / rectTransform.lossyScale.y));

        if (conformPosition)
        {
            var pos = target.TransformPoint(Rect.NormalizedToPoint(target.rect, target.pivot));
            pos.z = rectTransform.position.z;
            rectTransform.position = pos;
        }

        if (!conformSize)
            return;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    public static Vector2 GetOffScreenAnchoredPosition(this RectTransform rectTransform, RectTransform rootCanvas,
        Direction direction, float padding = 0)
    {
        var isAtEndOfScreen = direction is Direction.Right or Direction.Up;
        var normalizedPoint = isAtEndOfScreen ? Vector2.one : Vector2.zero;

        var offPoint = WorldToAnchoredPos(rootCanvas.NormalizedPointToWorld(normalizedPoint), rectTransform);

        var anchoredPos = rectTransform.anchoredPosition;
        offPoint = direction switch
        {
            Direction.Up or Direction.Down => new Vector2(
                anchoredPos.x,
                offPoint.y + rectTransform.rect.size.y *
                (isAtEndOfScreen ? rectTransform.pivot.y : -(1 - rectTransform.pivot.y))),
            Direction.Left or Direction.Right => new Vector2(
                offPoint.x +
                rectTransform.rect.size.x * (isAtEndOfScreen ? rectTransform.pivot.x : -(1 - rectTransform.pivot.x)),
                anchoredPos.y)
        };

        return offPoint;
    }

    private static Vector2 WorldToAnchoredPos(Vector3 worldPoint, RectTransform rectTransform)
    {
        if (rectTransform.parent == null)
            return worldPoint;

        var anchorPoint = rectTransform.anchorMin;

        if (rectTransform.anchorMin != rectTransform.anchorMax)
            anchorPoint = (rectTransform.anchorMax + rectTransform.anchorMin) * 0.5f;

        return rectTransform.parent.InverseTransformPoint(worldPoint) -
            ((RectTransform)rectTransform.parent).NormalizedPointToLocal(anchorPoint);
    }

    public static Vector2 GetSizeToTarget(this RectTransform rectTransform, RectTransform target)
    {
        return target.rect.size * Vector2.Max(Vector2.one * Vector2.kEpsilon,
            new Vector2(target.lossyScale.x / (rectTransform.lossyScale.x / rectTransform.localScale.x),
                target.lossyScale.y / (rectTransform.lossyScale.x / rectTransform.localScale.x)));
    }

    public static Vector2 NormalizedPointInBounds(Vector2 position, Bounds bounds)
    {
        return new Vector2(Mathf.InverseLerp(bounds.min.x, bounds.max.x, position.x),
            Mathf.InverseLerp(bounds.min.y, bounds.max.y, position.y));
    }

    public static Vector2 NormalizedPointInBoundsNoClamp(Vector2 position, Bounds bounds)
    {
        return new Vector2(InverseLerpNoClamp(bounds.min.x, bounds.max.x, position.x),
            InverseLerpNoClamp(bounds.min.y, bounds.max.y, position.y));
    }

    public static float InverseLerpNoClamp(float a, float b, float value)
    {
        return a != (double)b ? (float)((value - (double)a) / (b - (double)a)) : 0.0f;
    }
}
