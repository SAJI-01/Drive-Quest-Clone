using UnityEngine;

public class Obstacle : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"Hit obstacle: <color=red>{gameObject.name}</color>");
    }
}