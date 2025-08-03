using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class PixelateCamera : MonoBehaviour
{
    private Camera thisCamera;
    private Camera upscaleCamera;

    private RTHandle pixelatedHandle;
    private int zoomLevel;

    [SerializeField] private Vector2Int referenceResolution = new Vector2Int(320, 180);
    [SerializeField] private float pixelsPerUnit = 16;
    
    [SerializeField] private bool subPixel;
    [SerializeField] private bool useTruePosition;

    public float UnitsPerPixel => thisCamera.orthographicSize * 2 / referenceResolution.y;
    public Vector2 SubPixelOffset { get; private set; }

    public RTHandle PixelatedHandle => pixelatedHandle;
    public Camera UpscaleCamera => upscaleCamera;
    public Camera PixelCamera => thisCamera;
    public static PixelateCamera Instance { get; private set; }

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnCameraRender;

        Instance = this;
        RTHandles.Initialize(referenceResolution.x, referenceResolution.y);
        UpdateRenderTexture();
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnCameraRender;

        if (upscaleCamera)
        {
            if (Application.isEditor)
                DestroyImmediate(upscaleCamera.gameObject);
            else
                Destroy(upscaleCamera.gameObject);

            upscaleCamera = null;
        }

        thisCamera.ResetWorldToCameraMatrix();
    }

    private void CreateUpscaleCamera()
    {
        var upscaleCameraName = gameObject.name + " Upscaled";
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        var existingUpscaleCamera = rootGameObjects.FirstOrDefault(x => x.name == upscaleCameraName);

        upscaleCamera = existingUpscaleCamera
            ? existingUpscaleCamera.GetComponent<Camera>()
            : new GameObject(upscaleCameraName).AddComponent<Camera>();
        var additional = existingUpscaleCamera
            ? upscaleCamera.GetComponent<UniversalAdditionalCameraData>()
            : upscaleCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();

        additional.renderShadows = false;
        additional.requiresDepthOption = CameraOverrideOption.Off;

        upscaleCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
        upscaleCamera.depth = thisCamera.depth + 1;
        upscaleCamera.clearFlags = CameraClearFlags.Nothing;
        upscaleCamera.cullingMask = 0;
    }

    private void UpdateRenderTexture()
    {
        var isHDR = ((UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline).supportsHDR;

        var textureDescriptor =
            new RenderTextureDescriptor(referenceResolution.x, referenceResolution.y,
                isHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);

        if (PixelatedHandle == null)
            pixelatedHandle = RTHandles.Alloc(textureDescriptor, FilterMode.Point, TextureWrapMode.Clamp,
                name: "_PixelatedTexture");
        else
            RenderingUtils.ReAllocateIfNeeded(ref pixelatedHandle, textureDescriptor, FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelatedTexture");

        thisCamera.targetTexture = pixelatedHandle.rt;
    }

    private void OnCameraRender(ScriptableRenderContext srp, Camera cam)
    {
        if (cam != thisCamera)
            return;

        if (upscaleCamera == null)
            CreateUpscaleCamera();

        thisCamera.orthographicSize = referenceResolution.y / pixelsPerUnit / 2f;
        
        UpdateRenderTexture();
        SnapToPixels();
    }

    private void UpdateCameraProperties()
    {
        var width = Screen.width;
        var height = Screen.height;

        // zoom level (PPU scale)
        int verticalZoom = height / referenceResolution.y;
        int horizontalZoom = width / referenceResolution.x;
        zoomLevel = Math.Max(1, Math.Min(verticalZoom, horizontalZoom));

        var pixelRect = new Rect();

        pixelRect.width = zoomLevel * referenceResolution.x;
        pixelRect.height = zoomLevel * referenceResolution.y;
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

        var invPos = Matrix4x4.TRS(pixelPerfectPos, Quaternion.identity, Vector3.one).inverse;
        var invRot = Matrix4x4.Rotate(transform.rotation).inverse;
        var scaleMatrix = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, -1.0f));

        thisCamera.worldToCameraMatrix = scaleMatrix * invRot * invPos;

        if (subPixel)
            SubPixelOffset = Vector2.one / 2 - (Vector2)thisCamera.WorldToViewportPoint(transform.position);
        else
            SubPixelOffset = Vector2.zero;
    }
}
