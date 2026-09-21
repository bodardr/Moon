using System;
using System.Collections.Generic;
using Bodardr.Utility.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class Carousel : UIBehaviour, IBeginDragHandler, IEndDragHandler
{
    public enum ScrollingDirection
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum StartingElement
    {
        First = 0,
        Middle,
        Last,
        Custom,
        None
    }

    [SerializeField] private StartingElement startingElement;

    [SerializeField] [ShowIfEnum(nameof(startingElement), (int)StartingElement.Custom)]
    private int customStartingElement;

    [Tooltip("This enables wrapping back to the start or finish when calling Previous() and Next()")] [SerializeField]
    private bool wrap;

    [FormerlySerializedAs("rebuildChildListOnEnable")] [ShowIf(nameof(wrap))] [SerializeField]
    private bool allowRebuildChildList;

    [Range(1, 25)] [SerializeField] private float centeringSpeed = 2;

    [SerializeField] private bool canSwipeToNext;

    [ShowIf(nameof(canSwipeToNext))] [SerializeField]
    private float swipeToNextDeltaThreshold;

    private readonly List<ICarouselListener[]> carouselListeners = new();

    private readonly List<RectTransform> rectChildren = new();

    private OnChildrenUpdatedCallback contentChangedCallback;
    private RectTransform contentRectTransform;

    private int currentIndex;

    private ScrollingDirection direction;
    private bool initialized;
    private bool isLayoutDirty;
    private bool isWrapping;
    private bool shouldNotifyCarouselListeners;

    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private RectTransform rectTransform;
    private Canvas rootCanvas;
    private ScrollRect scrollRect;

    public int CurrentIndex
    {
        get => currentIndex;
        set
        {
            var newIndex = initialized ? Mathf.Clamp(value, 0, rectChildren.Count - 1) : value;
            currentIndex = newIndex;
            OnCurrentIndexChanged?.Invoke(newIndex);
        }
    }

    public IReadOnlyList<RectTransform> RectChildren => rectChildren;

    public ScrollRect ScrollRect
    {
        get => scrollRect ??= GetComponent<ScrollRect>();
        private set => scrollRect = value;
    }

    public bool IsDragging { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        rectTransform = (RectTransform)transform;
        contentRectTransform = (RectTransform)ScrollRect.content.transform;
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

        layoutGroup = ScrollRect.content.GetComponent<HorizontalOrVerticalLayoutGroup>();
        contentChangedCallback = layoutGroup.gameObject.AddComponent<OnChildrenUpdatedCallback>();
        contentChangedCallback.OnContentUpdated += LayoutOnContentUpdated;

        direction = layoutGroup is HorizontalLayoutGroup ? ScrollingDirection.Horizontal : ScrollingDirection.Vertical;
    }

    protected override void Start()
    {
        base.Start();

        Initialize();
        isLayoutDirty = false;
    }

    private void Update()
    {
        if (isLayoutDirty)
            RebuildChildList();

        var delta =
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            Pointer.current.delta.value[(int)direction];
#else
            Mouse.current.delta.value[(int)direction];
#endif

        if (IsDragging)
        {
            var foundIndex = GetClosestTargetIndex();
            
            if (foundIndex != CurrentIndex)
                CurrentIndex = foundIndex;
        }

        if (!IsDragging && rectChildren.Count >= 1)
        {
            var targetPos = GetWorldPositionFromTargetIndex(CurrentIndex);
            var newPosition =
                LerpUtility.ExpDecayLerp(contentRectTransform.position, targetPos, centeringSpeed, Time.deltaTime);

            contentRectTransform.position = newPosition;
        }

        if (wrap)
        {
            var dragSign = Mathf.Abs(delta) > 0.01f ? (int)Mathf.Sign(delta) : 0;

            if (dragSign != 0 && ShouldWrap(dragSign))
                WrapCarousel(dragSign);
        }

        shouldNotifyCarouselListeners = true;
    }

    private void LateUpdate()
    {
        if (!shouldNotifyCarouselListeners)
            return;

        shouldNotifyCarouselListeners = false;

        for (var i = 0; i < rectChildren.Count; i++)
        {
            var listeners = carouselListeners[i];

            if (listeners == null || listeners.Length < 1)
                continue;

            var normalizedPos = GetCarouselPositionNormalized(rectChildren[i]);
            foreach (var listener in listeners)
                listener.OnCarouselUpdated(normalizedPos);
        }

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (allowRebuildChildList)
            isLayoutDirty = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (layoutGroup == null)
            return;

        var callback = layoutGroup.GetComponent<OnChildrenUpdatedCallback>();
        if (callback != null)
            callback.OnContentUpdated -= LayoutOnContentUpdated;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;

        var scrollDelta = (eventData.delta[(int)direction] +
            eventData.scrollDelta[(int)direction]) * ScrollRect.scrollSensitivity;

        var newCurrentIndex = GetClosestTargetIndex();
        if (canSwipeToNext)
        {
            var isScrollingNext = Mathf.Abs(scrollDelta / Time.deltaTime) * (96f / Screen.dpi) >
                swipeToNextDeltaThreshold;
            if (isScrollingNext)
            {
                newCurrentIndex += (direction == ScrollingDirection.Horizontal ? -1 : 1) *
                    (layoutGroup.reverseArrangement ? -1 : 1) * (int)Mathf.Sign(scrollDelta);
                if (wrap)
                {
                    newCurrentIndex = newCurrentIndex < 0
                        ? rectChildren.Count + newCurrentIndex
                        : newCurrentIndex % rectChildren.Count;

                    var currentSiblingIndex = rectChildren[CurrentIndex].GetSiblingIndex();

                    if (currentSiblingIndex == 0 || currentSiblingIndex == rectChildren.Count - 1)
                        WrapCarousel((int)Mathf.Sign(scrollDelta), false);
                }
            }
        }

        CurrentIndex = newCurrentIndex;
    }

    public event Action<int> OnCurrentIndexChanged;
    public event Action OnCarouselUpdated;

    private void Initialize()
    {
        initialized = true;
        RebuildChildList();

        if (rectChildren.Count <= 1)
            return;

        if (startingElement != StartingElement.None)
            CurrentIndex = startingElement switch
            {
                StartingElement.Custom => customStartingElement,
                StartingElement.Last => rectChildren.Count - 1,
                StartingElement.Middle => (rectChildren.Count - 1) / 2,
                StartingElement.First or _ => 0
            };

        CurrentIndex = Mathf.Clamp(CurrentIndex, 0, rectChildren.Count - 1);
        CenterToCurrentIndexInstant();
    }

    private bool ShouldWrap(int dragSign)
    {
        var isHorizontal = direction is ScrollingDirection.Horizontal;
        var isPositive = dragSign > 0;
        var furthestElement =
            (RectTransform)contentRectTransform.GetChild((isHorizontal && isPositive) || (!isHorizontal && !isPositive)
                ? 0
                : contentRectTransform.childCount - 1);

        var checkStart = dragSign > 0;
        var viewport = ScrollRect.viewport ?? (RectTransform)ScrollRect.transform;
        var delta =
            (furthestElement.NormalizedPointToWorld(checkStart ? Vector2.zero : Vector2.one)[(int)direction] -
                viewport.NormalizedPointToWorld(checkStart ? Vector2.zero : Vector2.one)[(int)direction]) *
            furthestElement.lossyScale[(int)direction];

        return (checkStart ? delta : -delta) > layoutGroup.spacing;
    }

    private void WrapCarousel(int draggingSign, bool performDistanceChecks = true)
    {
        var directionIndex = (int)direction;

        var isHorizontal = direction is ScrollingDirection.Horizontal;
        var isPositive = draggingSign > 0;

        var wrappedElement =
            (RectTransform)contentRectTransform.GetChild((isHorizontal && isPositive) || (!isHorizontal && !isPositive)
                ? contentRectTransform.childCount - 1
                : 0);

        if (wrappedElement == rectChildren[CurrentIndex])
            return;

        if (performDistanceChecks)
        {
            var wrappedPoint =
                wrappedElement.NormalizedPointToWorld(isPositive ? Vector2.zero : Vector2.one)[directionIndex];
            var viewport = ScrollRect.viewport ?? (RectTransform)ScrollRect.transform;
            var viewportPoint =
                viewport.NormalizedPointToWorld(isPositive ? Vector2.one : Vector2.zero)[directionIndex];

            if ((isPositive && wrappedPoint < viewportPoint) || (!isPositive && wrappedPoint > viewportPoint))
                return;
        }

        isWrapping = true;

        if ((isHorizontal && isPositive) || (!isHorizontal && !isPositive))
            wrappedElement.SetAsFirstSibling();
        else
            wrappedElement.SetAsLastSibling();

        var wrappedElementSize = wrappedElement.rect.size[directionIndex] + layoutGroup.spacing;
        var directionMultiplier = Vector3.zero;
        directionMultiplier[directionIndex] = (isHorizontal && isPositive) || (!isHorizontal && !isPositive) ? 1 : -1;
        contentRectTransform.localPosition -= directionMultiplier * wrappedElementSize;

        //This should reset the initial position set by the scroll rect, allowing the offset to be taken into account.
        ScrollRect.OnBeginDrag(new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left,
            position =
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                Pointer.current.position.value,
#else
                Mouse.current.position.value,
#endif

            pointerPressRaycast = new RaycastResult
            {
                module = RaycasterManager.GetRaycasters()[0]
            }
        });

        isWrapping = false;
    }

    private void LayoutOnContentUpdated()
    {
        if ((!wrap || allowRebuildChildList) && !isWrapping)
            isLayoutDirty = true;
    }

    private void RebuildChildList()
    {
        if (!gameObject.activeInHierarchy)
            return;

        rectChildren.Clear();

        var toIgnoreList = ListPool<ILayoutIgnorer>.Get();
        for (var i = 0; i < contentRectTransform.childCount; i++)
        {
            var rect = contentRectTransform.GetChild(i) as RectTransform;
            if (rect == null || !rect.gameObject.activeInHierarchy)
                continue;

            rect.GetComponents(toIgnoreList);

            var ignore = false;
            foreach (var toIgnore in toIgnoreList)
            {
                if (!toIgnore.ignoreLayout)
                    continue;

                ignore = true;
                break;
            }

            if (ignore)
                continue;

            rectChildren.Add(rect);
        }

        carouselListeners.Clear();
        carouselListeners.Capacity = Mathf.Max(carouselListeners.Capacity, rectChildren.Count);

        for (var i = 0; i < rectChildren.Count; i++)
            carouselListeners.Insert(i, rectChildren[i].GetComponentsInChildren<ICarouselListener>());

        ListPool<ILayoutIgnorer>.Release(toIgnoreList);
        isLayoutDirty = false;

        if (rectChildren.Count > 0)
            currentIndex = Mathf.Clamp(currentIndex, 0, rectChildren.Count - 1);

        CenterToCurrentIndexInstant();
        OnCarouselUpdated?.Invoke();
    }

    public void CenterToCurrentIndexInstant()
    {
        if (contentRectTransform == null || ScrollRect == null)
            Awake();

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
        contentRectTransform.position = GetWorldPositionFromTargetIndex(CurrentIndex);
        ScrollRect.velocity = Vector2.zero;
    }

    [ContextMenu("Previous")]
    public void Previous()
    {
        if (wrap)
        {
            if (rectChildren[CurrentIndex].GetSiblingIndex() == 0)
                WrapCarousel(1);
            CurrentIndex = CurrentIndex < 1 ? rectChildren.Count - 1 : CurrentIndex - 1;
        }
        else
        {
            --CurrentIndex;
        }
    }

    [ContextMenu("Next")]
    public void Next()
    {
        if (wrap)
        {
            if (rectChildren[CurrentIndex].GetSiblingIndex() == rectChildren.Count - 1)
                WrapCarousel(-1);
            CurrentIndex = (CurrentIndex + 1) % rectChildren.Count;
        }
        else
        {
            ++CurrentIndex;
        }
    }

    private int GetClosestTargetIndex()
    {
        if (rectChildren.Count < 1)
            return 0;

        var bestIndex = 0;
        var shortestDist = float.MaxValue;

        var directionIndex = (int)direction;
        var currentPos = rectTransform.position[directionIndex];

        // todo : perform a binary search to optimize performance eventually.
        for (var i = 0; i < rectChildren.Count; i++)
        {
            var targetPosition = rectChildren[i].position[directionIndex];
            var dist = targetPosition - currentPos;

            if (Mathf.Abs(dist) < Mathf.Abs(shortestDist))
            {
                shortestDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    public Vector3 GetWorldPositionFromTargetIndex(int index)
    {
        if (index < 0 || index >= rectChildren.Count || rectChildren[index] == null)
            return Vector3.zero;

        return contentRectTransform.position +
            (rectTransform.position - rectChildren[index].position);
    }

    /// <summary>
    ///     Gets the normalized distance between this carousel child and the carousel's center.
    /// </summary>
    /// <param name="childTransform">The carousel's child RectTransform</param>
    /// <returns>Returns a range from -0.5,0.5 0 being perfectly at the center</returns>
    public float GetCarouselPositionNormalized(RectTransform childTransform)
    {
        var localChildPosition = rectTransform.InverseTransformPoint(childTransform.position);

        var directionIndex = (int)direction;
        var normalizedPoint = Rect.PointToNormalized(rectTransform.rect, localChildPosition)[directionIndex];
        var pivotPoint = rectTransform.pivot[directionIndex];
        var isInferiorToPivot = normalizedPoint < pivotPoint;
        return Mathf.InverseLerp(pivotPoint, isInferiorToPivot ? 0 : 1, normalizedPoint);
    }
}
