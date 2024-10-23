using UnityEngine;


public class LoadingManager : MonoBehaviour
{
    [Header("Required Components")] 
    [SerializeField] private ShipDetector shipDetector;
    [SerializeField] private ContainerLoader containerLoader;
    [SerializeField] private DockManager dockManager;

    [Header("Configuration")] 
    [SerializeField] private int dockCount = 5;
    [SerializeField] private float gapBetweenDocks = 2f;
    [SerializeField] private float shipDetectionDistance = 3f;

    private void Awake()
    {
        ValidateComponents();
        InitializeSystem();
        
        
        
    }

    private void InitializeSystem()
    {
        var containers = FindObjectsOfType<Container>();
        foreach (var container in containers) container.OnContainerFilled += HandleFilledContainer;
        dockManager.InitializeDocks(dockCount, gapBetweenDocks);
    }

    private void ValidateComponents()
    {
        shipDetector ??= GetComponent<ShipDetector>();
        containerLoader ??= GetComponent<ContainerLoader>();
        dockManager ??= GetComponent<DockManager>();

        if (shipDetector == null || containerLoader == null || dockManager == null)
        {
            Debug.LogError("Load the Missing Components!");
            enabled = false;
        }
    }

    private void HandleFilledContainer(IContainer container)
    {
        if (!container.IsFilled) return;
        var shipDetected = shipDetector.DetectShip(
            transform.position,
            transform.forward,
            shipDetectionDistance
        );

        if (shipDetected)
        {
            var shipType = shipDetector.GetDetectedShipType();
            if (shipType == container.CarType)
                LoadOntoShip(container);
            else
                LoadOntoDock(container);
        }
        else
        {
            LoadOntoDock(container);
        }
    }

    private void LoadOntoShip(IContainer container)
    {
        Ship ship = FindObjectOfType<Ship>(); // Consider caching this
        Transform slot = ship.GetNextAvailableSlot();

        if (slot != null)
            containerLoader.LoadContainer(container as Container, slot);
        else
            LoadOntoDock(container);
    }

    private void LoadOntoDock(IContainer container)
    {
        Transform availableDock = dockManager.GetAvailableDock();
        if (availableDock != null)
            containerLoader.LoadContainer(container as Container, availableDock); 
        else
            Debug.LogWarning("No available docks to load container!");
    }

    private void OnDestroy()
    {
        var containers = FindObjectsOfType<Container>();
        foreach (var container in containers) container.OnContainerFilled -= HandleFilledContainer;
    }
}