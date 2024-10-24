using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class Car : MonoBehaviour, IInteractable 
{
    [SerializeField] private float reversalSpeed = 1f;
    
    private SplineAnimate splineAnimate;
    private Path path;
    private bool isResetting;
    internal bool hasCollided;
    private bool wasHitByOtherCar;
    
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
        if (hasCollided && !isResetting && !wasHitByOtherCar)
        {
            StartCoroutine(ResetToStart());
        }
    }
    
    public void Interact()
    {
        if (!wasHitByOtherCar && !splineAnimate.IsPlaying)
        {
            Debug.Log($"Interacting with car: {gameObject.name}");
            PlaySplineAnimation();
        }
    }
    
    private void PlaySplineAnimation()
    {
        if (splineAnimate != null && !splineAnimate.IsPlaying && !hasCollided && !wasHitByOtherCar)
        {
            splineAnimate.Play();
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Car otherCar = collision.gameObject.GetComponent<Car>();
            if (otherCar != null)
            {
                // Check which car is moving
                bool thisCarMoving = splineAnimate.IsPlaying;
                bool otherCarMoving = otherCar.splineAnimate.IsPlaying;
                
                if (thisCarMoving && !otherCarMoving)
                {
                    // This car hit a stationary car
                    hasCollided = true;
                    otherCar.wasHitByOtherCar = true;
                }
                else if (!thisCarMoving && otherCarMoving)
                {
                    // This car was hit by a moving car
                    wasHitByOtherCar = true;
                    otherCar.hasCollided = true;
                }
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (splineAnimate.IsPlaying)
            {
                hasCollided = true;
            }
        }
    }
    
    public void StopMovementPermanently()
    {
        if (splineAnimate != null)
        {
            splineAnimate.Pause();
            wasHitByOtherCar = true;
            hasCollided = false;
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
        splineAnimate.Pause();
        isResetting = false;
        hasCollided = false;
    }
    
    public void ResetCollisionState()
    {
        hasCollided = false;
        wasHitByOtherCar = false;
    }
}


