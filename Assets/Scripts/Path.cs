using UnityEngine;
using UnityEngine.Splines;

public class Path : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int segmentCount = 100;

    private Transform car;

    private void Start()
    {
        InitializeComponents();
        DrawFullSplinePath();
    }

    private void Update()
    {
        if (AreComponentsValid()) UpdateLineRendererWithMovingObject();
    }

    private bool AreComponentsValid()
    {
        return splineContainer != null && lineRenderer != null && car != null;
    }

    private void UpdateLineRendererWithMovingObject()
    {
        var closestT = FindClosestTOnSpline(car.position);
        var erasedPointCount = Mathf.FloorToInt(closestT * segmentCount);

        for (var i = 0; i < segmentCount; i++)
        {
            var position = i <= erasedPointCount
                ? car.position
                : EvaluateSplinePosition(GetNormalizedIndex(i));

            lineRenderer.SetPosition(i, position);
        }
    }

    private float FindClosestTOnSpline(Vector3 targetPosition)
    {
        var closestT = 0f;
        var minDistance = float.MaxValue;

        for (var i = 0; i < segmentCount; i++)
        {
            var t = GetNormalizedIndex(i);
            var position = EvaluateSplinePosition(t);
            var distance = Vector3.Distance(position, targetPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestT = t;
            }
        }

        return closestT;
    }

    private void InitializeComponents()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (car == null) car = GetComponent<Car>().transform;
    }

    private void DrawFullSplinePath()
    {
        if (splineContainer == null || lineRenderer == null) return;

        lineRenderer.positionCount = segmentCount;
        for (var i = 0; i < segmentCount; i++)
        {
            var t = GetNormalizedIndex(i);
            var position = EvaluateSplinePosition(t);
            lineRenderer.SetPosition(i, position);
        }
    }

    private Vector3 EvaluateSplinePosition(float t)
    {
        Vector3 localPosition = splineContainer.Spline.EvaluatePosition(t);
        return splineContainer.transform.TransformPoint(localPosition);
    }

    private float GetNormalizedIndex(int index)
    {
        return index / (float)(segmentCount - 1);
    }
}