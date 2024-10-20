using UnityEngine;
using UnityEngine.Splines;
using System.Linq;

public class Path : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int segmentCount = 100;

    [SerializeField] private Transform car;

    private void Start()
    {
        InitializeComponents();
        DrawFullSplinePath();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Add a key to manually reset all cars (for testing)
        {
            ResetAllCars();
        }
        
        if (AreComponentsValid())
        {
            UpdateLineRendererWithMovingObject();
        }
    }
    
    private bool AreComponentsValid()
    {
        return splineContainer != null && lineRenderer != null && car != null;
    }
    
    private void UpdateLineRendererWithMovingObject()
    {
        float closestT = FindClosestTOnSpline(car.position);
        int erasedPointCount = Mathf.FloorToInt(closestT * segmentCount);

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 position = (i <= erasedPointCount) 
                ? car.position
                : EvaluateSplinePosition(GetNormalizedIndex(i));
            
            lineRenderer.SetPosition(i, position);
        }
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
    
    private void InitializeComponents()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    private void DrawFullSplinePath()
    {
        if (splineContainer == null || lineRenderer == null) return;

        lineRenderer.positionCount = segmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            float t = GetNormalizedIndex(i);
            Vector3 position = EvaluateSplinePosition(t);
            lineRenderer.SetPosition(i, position);
        }
    }

    public Vector3 EvaluateSplinePosition(float t)
    {
        Vector3 localPosition = splineContainer.Spline.EvaluatePosition(t);
        return splineContainer.transform.TransformPoint(localPosition);
    }

    private float GetNormalizedIndex(int index)
    {
        return index / (float)(segmentCount - 1);
    }

    public void ResetAllCars()
    {
        StartCoroutine(car.GetComponent<Car>().ResetToStart());
        Car.ResetGlobalCollisionState();
    }
}