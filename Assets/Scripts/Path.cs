using UnityEngine;
using UnityEngine.Splines;

public class Path : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int segmentCount = 100;
    [SerializeField] private Transform movingObject;

    private void Start()
    {
        InitializeComponents();
        DrawFullSplinePath();
    }

    private void Update()
    {
        if (AreComponentsValid())
        {
            UpdateLineRendererWithMovingObject();
        }
    }

    private void InitializeComponents()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    private bool AreComponentsValid()
    {
        return splineContainer != null && lineRenderer != null && movingObject != null;
    }

    private void DrawFullSplinePath()
    {
        if (!AreComponentsValid()) return;

        lineRenderer.positionCount = segmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            float t = GetNormalizedIndex(i);
            Vector3 position = EvaluateSplinePosition(t);
            lineRenderer.SetPosition(i, position);
        }
    }

    private void UpdateLineRendererWithMovingObject()
    {
        float closestT = FindClosestTOnSpline(movingObject.position);
        int erasedPointCount = Mathf.FloorToInt(closestT * segmentCount);

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 position = (i <= erasedPointCount) 
                ? movingObject.position 
                : EvaluateSplinePosition(GetNormalizedIndex(i));
            
            lineRenderer.SetPosition(i, position);
        }
    }

    public Vector3 EvaluateSplinePosition(float t)
    {
        Vector3 localPosition = splineContainer.Spline.EvaluatePosition(t);
        return splineContainer.transform.TransformPoint(localPosition);
    }

    private float FindClosestTOnSpline(Vector3 targetPosition)
    {
        float closestT = 0f;
        float minDistance = float.MaxValue;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = GetNormalizedIndex(i);
            Vector3 position = EvaluateSplinePosition(t);
            float distance = Vector3.Distance(position, targetPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestT = t;
            }
        }

        return closestT;
    }

    private float GetNormalizedIndex(int index)
    {
        return index / (float)(segmentCount - 1);
    }

    public float GetPreviousKnotPosition(float currentPosition)
    {
        Spline spline = splineContainer.Spline;
        int knotCount = spline.Count;

        for (int i = 1; i < knotCount; i++)
        {
            float knotT = (float)i / (knotCount - 1);
            if (knotT > currentPosition)
            {
                return (float)(i - 1) / (knotCount - 1);
            }
        }

        return (float)(knotCount - 2) / (knotCount - 1);
    }
}