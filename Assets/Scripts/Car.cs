using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class Car : MonoBehaviour, IInteractable
{
    private SplineAnimate SplineAnimate { get; set; }
    private Path path;
    private bool isResetting;
    private static bool isCarsCollisionOccurred;
    [SerializeField] private float reversalSpeed = 1f;

    private void Start()
    {
        SplineAnimate = GetComponent<SplineAnimate>();
        path = FindObjectOfType<Path>();
        if (path == null) Debug.LogError("Path component not found in the scene.");
    }

    private void Update()
    {
        if (isCarsCollisionOccurred && !isResetting) StartCoroutine(ResetToStart());
    }

    public void Interact()
    {
        Debug.Log($"Interacting with car: <color=green>{gameObject.name}</color>");
        PlaySplineAnimation();
    }

    private void PlaySplineAnimation()
    {
        if (SplineAnimate != null && !SplineAnimate.IsPlaying && !isCarsCollisionOccurred) SplineAnimate.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") || collision.gameObject.CompareTag("Obstacle"))
            isCarsCollisionOccurred = true;
    }

    public IEnumerator ResetToStart()
    {
        isResetting = true;
        gameObject.SetActive(true);

        SplineAnimate.Pause();

        var startTime = Time.time;
        var splineTimeLenght = SplineAnimate.NormalizedTime;

        while (SplineAnimate.NormalizedTime > 0)
        {
            var distanceCovered = (Time.time - startTime) * reversalSpeed;
            var reverseCoveredDistance = distanceCovered / splineTimeLenght;
            SplineAnimate.NormalizedTime = Mathf.Lerp(splineTimeLenght, 0, reverseCoveredDistance);
            yield return null;
        }

        SplineAnimate.NormalizedTime = 0f;
        yield return new WaitForSeconds(0.5f);

        isResetting = false;
        isCarsCollisionOccurred = false;
    }

    public static void ResetGlobalCollisionState()
    {
        isCarsCollisionOccurred = false;
    }
}