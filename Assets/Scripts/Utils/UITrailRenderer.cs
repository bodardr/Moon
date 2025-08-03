using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public class UITrailRenderer : Graphic
{
    private float initialWidth;

    private Canvas rootCanvas;
    private Mesh trailMesh;

    [SerializeField]
    private TrailRenderer trail;

    protected override void Awake()
    {
        base.Awake();

        rootCanvas = transform.root.GetComponentInChildren<Canvas>();
        initialWidth = trail.widthMultiplier;

        trailMesh = new Mesh();
        canvasRenderer.SetMaterial(trail.sharedMaterial, null);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        canvasRenderer.cull = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        canvasRenderer.cull = true;
    }

    private void Update()
    {
        if (trailMesh == null)
            trailMesh = new Mesh();

        trail.BakeMesh(trailMesh, Camera.main);

        var posMult = rootCanvas.transform.localScale;

        #if UNITY_EDITOR
        if (Application.isPlaying)
        #endif
            trail.widthMultiplier = initialWidth * posMult.magnitude;

        posMult.x = Mathf.Approximately(posMult.x, 0) ? 1 : 1 / posMult.x;
        posMult.y = Mathf.Approximately(posMult.y, 0) ? 1 : 1 / posMult.y;
        posMult.z = Mathf.Approximately(posMult.z, 0) ? 1 : 1 / posMult.z;


        Vector3[] vertices = trailMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.Scale(vertices[i], posMult);
        }

        trailMesh.vertices = vertices;
        canvasRenderer.SetMesh(trailMesh);
    }
}
