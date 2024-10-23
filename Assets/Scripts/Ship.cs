using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform[] containerSlots;
    private GameObject[] containers;
    [SerializeField] private float shipTime = 5f;
    private RaycastHit hit;
    private bool isShipFull = false;

    private int currentSlotIndex = 0;
    public string ShipType { get; private set; }
    private void Awake()
    {
        ValidateComponents();
        ShipType = gameObject.tag.Replace("Ship", "");
    }

    private void ValidateComponents()
    {
        if (containerSlots == null || containerSlots.Length == 0)
        {
            Debug.LogError($"No container slots assigned to ship {gameObject.name}! Assign Slots in the inspector.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (currentSlotIndex == containerSlots.Length && !isShipFull)
        {
            isShipFull = true;
            StartCoroutine(GetIsFilledContainer());
        }
        
    }

    private IEnumerator GetIsFilledContainer()
    {
        yield return new WaitForSeconds(1f);
        ShipAllContainers();
    }


    public Transform GetNextAvailableSlot()
    {
        if (currentSlotIndex < containerSlots.Length)
        {
            return containerSlots[currentSlotIndex++]; 
        }
        return null;
    }
    
    private void ShipAllContainers()
    {
        transform.DOMoveX(transform.position.x + 10, shipTime).OnComplete(() =>
        {
            containerSlots = null;
            Debug.Log($"All containers shipped on {gameObject.name}");
            Destroy(gameObject);
        });
    }
}
