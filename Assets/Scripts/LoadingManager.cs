using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

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
    [SerializeField] private Vector3 dockScale;
    [SerializeField] private Transform shipSpawnPoint;
    [SerializeField] private float shipSpacing = 10f;

    private Queue<Ship> shipQueue = new Queue<Ship>();
    private Dictionary<string, List<Container>> dockedContainers = new Dictionary<string, List<Container>>();

    private void Awake()
    {
        ValidateComponents();
        InitializeSystem();
    }

    private void InitializeSystem()
    {
        var containers = FindObjectsOfType<Container>();
        foreach (var container in containers)
            container.OnContainerFilled += HandleFilledContainer;

        dockManager.InitializeDocks(dockCount, gapBetweenDocks, dockScale);
        InitializeShipQueue();
    }

    private void InitializeShipQueue()
    {
        var ships = FindObjectsOfType<Ship>().OrderBy(s => Vector3.Distance(shipSpawnPoint.position, s.transform.position)).ToList();

        foreach (var ship in ships)
        {
            shipQueue.Enqueue(ship);
            PositionShipInQueue(ship, shipQueue.Count - 1);
        }
    }

    private void PositionShipInQueue(Ship ship, int queuePosition)
    {
        Vector3 targetPosition = shipSpawnPoint.position + Vector3.right * (shipSpacing * queuePosition);
        ship.transform.position = targetPosition;
    }

    private void ValidateComponents()
    {
        shipDetector ??= GetComponent<ShipDetector>();
        containerLoader ??= GetComponent<ContainerLoader>();
        dockManager ??= GetComponent<DockManager>();

        if (shipDetector == null || containerLoader == null || dockManager == null)
        {
            Debug.LogError("Missing required components!");
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

        if (shipDetected && shipQueue.Count > 0)
        {
            var currentShip = shipQueue.Peek();
            var shipType = currentShip.ShipType;

            if (shipType == container.CarType)
            {
                LoadOntoShip(container, currentShip);
            }
            else
            {
                LoadOntoDock(container);
            }
        }
        else
        {
            LoadOntoDock(container);
        }

        CheckDockedContainersForLoading();
    }

    private void LoadOntoShip(IContainer container, Ship ship)
    {
        Transform slot = ship.GetNextAvailableSlot();
        if (slot != null)
        {
            containerLoader.LoadContainer(container as Container, slot);
            if (ship.IsFullyLoaded)
            {
                shipQueue.Dequeue();
                StartCoroutine(MoveShipQueueForward());
            }
        }
        else
        {
            LoadOntoDock(container);
        }
    }

    private void LoadOntoDock(IContainer container)
    {
        Transform availableDock = dockManager.GetAvailableDock();
        if (availableDock != null)
        {
            containerLoader.LoadContainer(container as Container, availableDock);
            
            // Store container reference by type
            string containerType = container.CarType;
            if (!dockedContainers.ContainsKey(containerType))
            {
                dockedContainers[containerType] = new List<Container>();
            }
            dockedContainers[containerType].Add(container as Container);
        }
        else
        {
            Debug.LogWarning("No available docks to load container!");
        }
    }

    private void CheckDockedContainersForLoading()
    {
        if (shipQueue.Count == 0) return;

        Ship currentShip = shipQueue.Peek();
        string shipType = currentShip.ShipType;

        if (dockedContainers.ContainsKey(shipType))
        {
            var containersToLoad = dockedContainers[shipType].ToList();
            foreach (var container in containersToLoad)
            {
                if (currentShip.IsFullyLoaded) break;

                Transform slot = currentShip.GetNextAvailableSlot();
                if (slot != null)
                {
                    containerLoader.LoadContainer(container, slot);
                    dockedContainers[shipType].Remove(container);
                }
            }

            if (currentShip.IsFullyLoaded)
            {
                shipQueue.Dequeue();
                StartCoroutine(MoveShipQueueForward());
            }
        }
    }

    private System.Collections.IEnumerator MoveShipQueueForward()
    {
        yield return new WaitForSeconds(1f); // Wait for current ship to start moving

        int queuePosition = 0;
        foreach (var ship in shipQueue)
        {
            Vector3 newPosition = shipSpawnPoint.position + Vector3.right * (shipSpacing * queuePosition);
            ship.transform.DOMove(newPosition, 2f);
            queuePosition++;
        }
    }

    private void OnDestroy()
    {
        var containers = FindObjectsOfType<Container>();
        foreach (var container in containers)
            container.OnContainerFilled -= HandleFilledContainer;
    }
}
