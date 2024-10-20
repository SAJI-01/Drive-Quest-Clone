using UnityEngine;
using UnityEngine.Splines;
using System.Linq;

public class Path : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int segmentCount = 100;

    private Car[] allCars;

    private void Start()
    {
        InitializeComponents();
        DrawFullSplinePath();
        allCars = FindObjectsOfType<Car>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Add a key to manually reset all cars (for testing)
        {
            ResetAllCars();
        }
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
        foreach (var car in allCars)
        {
            car.StartCoroutine(car.GetComponent<Car>().ResetToStart());
        }
        Car.ResetGlobalCollisionState();
    }
}