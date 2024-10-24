using UnityEngine;
using UnityEngine.Splines;

public class Obstacle : MonoBehaviour, IInteractable 
{
    public void Interact()
    {
        Debug.Log($"Hit obstacle: {gameObject.name}");
    }
    
    public void DisableAllColliders()
    {
        foreach (var collider in GetComponents<Collider>())
        {
            collider.enabled = false;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Car car = other.gameObject.GetComponent<Car>();
        if (car != null && car.GetComponent<SplineAnimate>().IsPlaying)
        {
            car.hasCollided = true;
        }
    }
}
