
using System;
using UnityEngine;

public class shiplast : MonoBehaviour
{
    [SerializeField] private GameObject[] enableObjects;
    private void OnDestroy()
    {
        foreach (var obj in enableObjects)
        {
            obj.SetActive(true);
        }
    }
}