using System;
using UnityEngine;

public class Obstacle : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"Hit obstacle: <color=red>{gameObject.name}</color>");
    }

    private void OnCollisionEnter(Collision other)
    {
        other.gameObject.GetComponent<Car>().ResetToStart();
    }
}