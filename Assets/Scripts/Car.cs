using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class Car : MonoBehaviour, IInteractable
{
    public SplineAnimate SplineAnimate { get; private set; }
    private Path path;
    private bool isResetting = false;
    private static bool globalCollisionOccurred = false;

    private void Start()
    {
        SplineAnimate = GetComponent<SplineAnimate>();
        path = FindObjectOfType<Path>();
        if (path == null)
        {
            Debug.LogError("Path component not found in the scene.");
        }
    }

    private void Update()
    {
        if (globalCollisionOccurred && !isResetting)
        {
            StartCoroutine(ResetToStart());
        }
    }

    public void Interact()
    {
        Debug.Log($"Interacting with car: {gameObject.name}");
        PlaySplineAnimation();
    }

    private void PlaySplineAnimation()
    {
        if (SplineAnimate != null && !SplineAnimate.isPlaying && !globalCollisionOccurred)
        {
            SplineAnimate.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") || collision.gameObject.CompareTag("Obstacle"))
        {
            globalCollisionOccurred = true;
        }
    }

    public IEnumerator ResetToStart()
    {
        isResetting = true;
        SplineAnimate.Pause();

        float startTime = Time.time;
        float journeyLength = SplineAnimate.NormalizedTime;
        float speed = 1f; // Adjust this value to control the speed of reversal

        while (SplineAnimate.NormalizedTime > 0)
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;
            SplineAnimate.NormalizedTime = Mathf.Lerp(journeyLength, 0, fractionOfJourney);
            yield return null;
        }

        SplineAnimate.NormalizedTime = 0f;
        yield return new WaitForSeconds(0.5f);

        isResetting = false;
        globalCollisionOccurred = false;
    }

    public static void ResetGlobalCollisionState()
    {
        globalCollisionOccurred = false;
    }
}