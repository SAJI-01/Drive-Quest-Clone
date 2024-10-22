using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform[] containerSlots;

    private int currentSlotIndex = 0;

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

    public string ShipType { get; private set; }

    public Transform GetNextAvailableSlot()
    {
        if (currentSlotIndex < containerSlots.Length)
            return containerSlots[currentSlotIndex++];
        return null;
    }
}
