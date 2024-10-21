using UnityEngine;

public class HandleCar : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact();
        }
    }
}