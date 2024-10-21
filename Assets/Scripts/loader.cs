using System;
using UnityEngine;
using UnityEngine.Events;

public class loader : MonoBehaviour
{
    public static UnityEvent onContainerFilled = new UnityEvent();
    
    private void Start()
    {
        onContainerFilled.AddListener(OnContainerFilled);
    }
    
    private void OnContainerFilled()
    {
        Debug.Log("Container filled");
    }

    private void Update()
    {
            //find what ship using raycast
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                if(hit.collider.gameObject.CompareTag("RedShip"))
                {
                    Debug.Log("Ship found");
                    //TODO: Add code to handle the ship
                }
            }
    }
    
    //TODO:   Add code to loader containers on ground and ship
}