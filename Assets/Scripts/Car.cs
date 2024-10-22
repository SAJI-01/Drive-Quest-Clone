using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class Car : MonoBehaviour, IInteractable
{
    [SerializeField] private float reversalSpeed = 1f;
    
    private SplineAnimate splineAnimate;
    private Path path;
    private bool isResetting;
    private static bool isCarsCollisionOccurred;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        if (splineAnimate == null)
        {
            Debug.LogError($"SplineAnimate component missing on {gameObject.name}!");
            enabled = false;
            return;
        }

        path = FindObjectOfType<Path>();
        if (path == null)
        {
            Debug.LogError("Path component not found in the scene!");
            enabled = false;
        }
    }

    private void Update()
    {
        if (isCarsCollisionOccurred && !isResetting)
        {
            StartCoroutine(ResetToStart());
        }
    }

    public void Interact()
    {
        Debug.Log($"Interacting with car: <color=green>{gameObject.name}</color>");
        PlaySplineAnimation();
    }

    private void PlaySplineAnimation()
    {
        if (splineAnimate != null && !splineAnimate.IsPlaying && !isCarsCollisionOccurred)
        {
            splineAnimate.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") || collision.gameObject.CompareTag("Obstacle"))
        {
            isCarsCollisionOccurred = true;
        }
    }

    public IEnumerator ResetToStart()
    {
        isResetting = true;
        gameObject.SetActive(true);

        splineAnimate.Pause();

        float startTime = Time.time;
        float splineTimeLength = splineAnimate.NormalizedTime;

        while (splineAnimate.NormalizedTime > 0)
        {
            float distanceCovered = (Time.time - startTime) * reversalSpeed;
            float reverseCoveredDistance = distanceCovered / splineTimeLength;
            splineAnimate.NormalizedTime = Mathf.Lerp(splineTimeLength, 0, reverseCoveredDistance);
            yield return null;
        }

        splineAnimate.NormalizedTime = 0f;
        yield return new WaitForSeconds(0.5f);

        isResetting = false;
        isCarsCollisionOccurred = false;
    }

    public static void ResetGlobalCollisionState()
    {
        isCarsCollisionOccurred = false;
    }
}