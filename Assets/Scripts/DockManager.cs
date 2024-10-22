using System.Collections.Generic;
using UnityEngine;

public class DockManager : MonoBehaviour, IDockManager
{
    [Header("Required References")]
    [SerializeField] private Transform dockParent;
    [SerializeField] private GameObject dockPrefab;

    private List<Transform> availableDocks = new List<Transform>();

    private void Awake()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (dockParent == null)
        {
            dockParent = transform; // Use this object as parent if none assigned
            Debug.LogWarning("No dock parent assigned, using this GameObject as parent.");
        }

        if (dockPrefab == null)
        {
            Debug.LogError("Dock prefab is missing! Please assign it in the inspector.");
            enabled = false;
        }
    }

    public void InitializeDocks(int count, float gap)
    {
        float totalWidth = (count - 1) * gap;
        float startX = dockParent.position.x - totalWidth / 2;

        for (int i = 0; i < count; i++)
        {
            Vector3 position = new Vector3(
                startX + (i * gap),
                dockParent.position.y,
                dockParent.position.z
            );

            GameObject dock = Instantiate(dockPrefab, position, dockPrefab.transform.rotation, dockParent);
            dock.name = $"DockLoader_{i}";
            availableDocks.Add(dock.transform);
        }
    }

    public Transform GetAvailableDock()
    {
        if (availableDocks.Count > 0)
        {
            Transform dock = availableDocks[0];
            availableDocks.RemoveAt(0);
            return dock;
        }
        return null;
    }
}