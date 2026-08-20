using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class PixelateCamera : MonoBehaviour
{
    private Camera thisCamera;

    private RTHandle pixelatedHandle;
    private int zoomLevel;

    [SerializeField] private Vector2Int referenceResolution = new Vector2Int(320, 180);
    [SerializeField] private float pixelsPerUnit = 16;

    [SerializeField] private bool subPixel;
    [SerializeField] private bool useTruePosition;

    public float UnitsPerPixel => thisCamera.orthographicSize * 2 / referenceResolution.y;
    public Vector2 SubPixelOffset { get; private set; }

    public Vector2Int ReferenceResolution => referenceResolution;

    public RTHandle PixelatedHandle => pixelatedHandle;
    public Camera Camera => thisCamera;
    public static PixelateCamera Instance { get; private set; }

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Instance = this;
        RenderPipelineManager.beginCameraRendering += OnCameraRender;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnCameraRender;
        thisCamera.ResetWorldToCameraMatrix();
    }

    private void OnCameraRender(ScriptableRenderContext srp, Camera cam)
    {
        if (cam != thisCamera)
            return;

        thisCamera.orthographicSize = referenceResolution.y / pixelsPerUnit / 2f;

        SnapToPixels();
    }

    private void SnapToPixels()
    {
        var unitsPerPixel = UnitsPerPixel;

        var position = Quaternion.Inverse(transform.rotation) * transform.position;

        var roundedPos = new Vector3(
            Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel,
            Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel,
            position.z);

        var pixelPerfectPos = transform.rotation * roundedPos;

        var invPos = Matrix4x4.Translate(pixelPerfectPos).inverse;
        var invRot = Matrix4x4.Rotate(transform.rotation).inverse;
        var scaleMatrix = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, -1.0f));

        thisCamera.worldToCameraMatrix = scaleMatrix * invRot * invPos;

        if (subPixel)
            SubPixelOffset = Vector2.one / 2 - (Vector2)thisCamera.WorldToViewportPoint(transform.position);
        else
            SubPixelOffset = Vector2.zero;
    }
}
