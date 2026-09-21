using System.Diagnostics.CodeAnalysis;
using Bodardr.Utility.Runtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteAlways]
public class RectTransformFitter : MonoBehaviour, ILayoutElement, ILayoutIgnorer
{
    [SerializeField] private bool followTarget = true;
    [SerializeField] private bool forceUpdateWhenMoved = true;

    [Header("Target")] [SerializeField] private RectTransform target;
    [SerializeField] private bool useCustomTargetPivot = false;

    [ShowIf(nameof(useCustomTargetPivot))]
    [SerializeField]
    private Vector2 customTargetPivot = Vector2.zero;

    [Header("Offset")] [SerializeField] private Vector2 offset;

    [Header("Size")]
    [FormerlySerializedAs("adjustToSize")]
    [SerializeField]
    private bool adjustToTargetSize;

    [SerializeField] private Vector2 sizeOffset;
    private RectTransform rectTransform;
    private bool snappingDirty = false;
    private DrivenRectTransformTracker tracker;
    
    public RectTransform Target
    {
        get => target;
        set
        {
            target = value;
            snappingDirty = true;
        }
    }

    public Vector2 Offset
    {
        get => offset;
        set
        {
            offset = value;
            snappingDirty = true;
        }
    }

    public bool ForceUpdateWhenMoved
    {
        get => forceUpdateWhenMoved;
        set
        {
            forceUpdateWhenMoved = value;
            snappingDirty = true;
        }
    }

    public bool AdjustToTargetSize
    {
        get => adjustToTargetSize;
        set
        {
            adjustToTargetSize = value;
            snappingDirty = true;
        }
    }

    public Vector2 SizeOffset
    {
        get => sizeOffset;
        set
        {
            sizeOffset = value;
            snappingDirty = true;
        }
    }

    public bool UseCustomTargetPivot
    {
        get => useCustomTargetPivot;
        set
        {
            useCustomTargetPivot = value;
            snappingDirty = true;
        }
    }

    public Vector2 CustomTargetPivot
    {
        get => customTargetPivot;
        set
        {
            customTargetPivot = value;
            snappingDirty = true;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void LateUpdate()
    {
        if (!snappingDirty || !target)
            return;

        if (!rectTransform)
            Initialize();

        if (followTarget)
        {
            var pos =
                target.NormalizedPointToWorld(useCustomTargetPivot ? customTargetPivot : new Vector2(0.5f, 0.5f)) +
                (Vector3)Offset;
            pos.z = rectTransform.position.z;
            rectTransform.position = pos;
        }

        if (!adjustToTargetSize)
            return;

        var size = rectTransform.GetSizeToTarget(target);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + sizeOffset.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + sizeOffset.y);
    }

    [SuppressMessage("Domain reload", "UDR0005:Domain Reload Analyzer")]
    private void OnEnable()
    {
        Canvas.willRenderCanvases += CheckForUpdateSnapping;
        snappingDirty = true;
    }

    private void OnDisable()
    {
        Canvas.willRenderCanvases -= CheckForUpdateSnapping;
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        if (!rectTransform)
            Initialize();

        if (target && followTarget && target.IsChildOf(rectTransform))
        {
            Debug.LogWarning(
                $"<b>Snap To Rect Transform</b> : {target.name} cannot be followed since it is a child of {rectTransform.name}");
            target = null;
        }

        tracker.Clear();

        if (!target)
            return;

        if (forceUpdateWhenMoved && followTarget)
            tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPosition);

        if (adjustToTargetSize)
            tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
    }

    public float minWidth => 0;
    public float preferredWidth => 0;
    public float flexibleWidth => 0;
    public float minHeight => 0;
    public float preferredHeight => 0;
    public float flexibleHeight => 0;
    public int layoutPriority => 0;

    public void CalculateLayoutInputHorizontal()
    {
    }

    public void CalculateLayoutInputVertical()
    {
    }

    public bool ignoreLayout => true;

    private void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void CheckForUpdateSnapping()
    {
        if (target == null || target.hasChanged && !forceUpdateWhenMoved)
            return;

        snappingDirty = true;
    }
}
