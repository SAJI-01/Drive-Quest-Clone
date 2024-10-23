using DG.Tweening;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform[] containerSlots;
    [SerializeField] private float shipTime = 5f;

    private int currentSlotIndex = 0;
    public string ShipType { get; private set; }
    public bool IsFullyLoaded => currentSlotIndex >= containerSlots.Length;

    private void Awake()
    {
        ValidateComponents();
        ShipType = gameObject.tag.Replace("Ship", "");
    }

    private void ValidateComponents()
    {
        if (containerSlots == null || containerSlots.Length == 0)
        {
            Debug.LogError($"No container slots assigned to ship {gameObject.name}!");
            enabled = false;
        }
    }

    private void Update()
    {
        if (IsFullyLoaded)
        {
            StartCoroutine(DepartShip());
        }
    }

    public Transform GetNextAvailableSlot()
    {
        if (currentSlotIndex < containerSlots.Length)
        {
            return containerSlots[currentSlotIndex++];
        }
        return null;
    }

    private System.Collections.IEnumerator DepartShip()
    {
        yield return new WaitForSeconds(1f);
        transform.DOMoveX(transform.position.x + 20, shipTime)
            .OnComplete(() => Destroy(gameObject));
    }
}