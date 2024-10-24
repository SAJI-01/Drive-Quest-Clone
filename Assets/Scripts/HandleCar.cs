using System;
using UnityEngine;

public class HandleCar : MonoBehaviour 
{

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Car>() != null)
        {
            //reset the start 
            StartCoroutine(collision.gameObject.GetComponent<Car>().ResetToStart());
        }
    }
}