using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class Car : MonoBehaviour, IInteractable
{
    private SplineAnimate splineAnimate;
    private Path path;
    private float lastKnotPosition;
    private bool isReversing = false;
    private float reverseCooldown = 1f; // Cooldown time in seconds
    private float lastReverseTime = -1f;

    private void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        path = FindObjectOfType<Path>();
        if (path == null)
        {
            Debug.LogError("Path component not found in the scene.");
        }
    }

    public void Interact()
    {
        Debug.Log($"Interacting with car: {gameObject.name}");
        PlaySplineAnimation();
    }

    private void PlaySplineAnimation()
    {
        if (splineAnimate != null)
        {
            splineAnimate.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Car") || collision.gameObject.CompareTag("Obstacle")) && !isReversing)
        {
            StartCoroutine(ReverseToLastKnot());
        }
    }

    private IEnumerator ReverseToLastKnot()
    {
        if (splineAnimate != null && path != null && Time.time - lastReverseTime > reverseCooldown)
        {
            isReversing = true;
            lastReverseTime = Time.time;

            // Pause the animation
            splineAnimate.Pause();

            // Get the previous knot position
            lastKnotPosition = path.GetPreviousKnotPosition(splineAnimate.NormalizedTime);

            // Move to the previous knot position
            float startTime = Time.time;
            float journeyLength = Mathf.Abs(splineAnimate.NormalizedTime - lastKnotPosition);
            Vector3 startPosition = transform.position;
            Vector3 endPosition = path.EvaluateSplinePosition(lastKnotPosition);

            while (Time.time - startTime < 0.5f) // 0.5 seconds for the reverse movement
            {
                float fractionOfJourney = (Time.time - startTime) / 0.5f;
                transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
                yield return null;
            }

            // Set the final position and update the spline animate component
            transform.position = endPosition;
            splineAnimate.NormalizedTime = lastKnotPosition;

            // Wait for a short duration before allowing movement again
            yield return new WaitForSeconds(0.5f);

            isReversing = false;
        }
    }
}

