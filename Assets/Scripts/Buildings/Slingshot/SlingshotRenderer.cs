using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class SlingshotRenderer : MonoBehaviour
{
    private Vector3[] linePositions;
    
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform payload;
    
    [SerializeField] private int splineResolution = 10;
    [FormerlySerializedAs("spline")]
    [SerializeField] private SplineContainer splineContainer;

    private void Update()
    {
        if (linePositions == null || linePositions.Length != splineResolution)
        {
            linePositions = new Vector3[splineResolution];
            lineRenderer.positionCount = splineResolution;
        }

        var knotIndex = 0;
        BezierKnot payloadKnot = default;
        foreach (var knot in splineContainer.Spline.Knots)
        {
            if (knotIndex++ != 1)
                continue;
            
            payloadKnot = knot;
            break;
        }

        payloadKnot.Position = transform.InverseTransformPoint(payload.position);
        splineContainer.Spline.SetKnot(1, payloadKnot);
            
        for (int i = 0; i < splineResolution; i++)
        {
            linePositions[i] = splineContainer.EvaluatePosition((float)i / (splineResolution - 1));
        }
        
        lineRenderer.SetPositions(linePositions);
    }
}
