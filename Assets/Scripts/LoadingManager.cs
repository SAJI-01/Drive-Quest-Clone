using System.Collections;
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
    [SerializeField] private float waitTimeBeforeLoading = 1f;
    [SerializeField] private float shipMoveSpeed = 2f;

    private Queue<Ship> shipQueue = new Queue<Ship>();
    private Dictionary<string, List<Container>> dockedContainers = new Dictionary<string, List<Container>>();
    private bool isProcessingShipMovement = false;

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
        var ships = FindObjectsOfType<Ship>()
            .OrderBy(s => Vector3.Distance(shipSpawnPoint.position, s.transform.position))
            .ToList();

        foreach (var ship in ships)
        {
            shipQueue.Enqueue(ship);
            PositionShipInQueue(ship, shipQueue.Count - 1);
        }
    }

    private void PositionShipInQueue(Ship ship, int queuePosition)
    {
        Vector3 targetPosition = shipSpawnPoint.position - Vector3.right * (shipSpacing * queuePosition);
        ship.transform.position = targetPosition;
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
    }

    private void LoadOntoShip(IContainer container, Ship ship)
    {
        Transform slot = ship.GetNextAvailableSlot();
        if (slot != null)
        {
            containerLoader.LoadContainer(container as Container, slot);
            if (ship.IsFullyLoaded)
            {
                ProcessFullyLoadedShip();
            }
        }
        else
        {
            LoadOntoDock(container);
        }
    }

    private void ProcessFullyLoadedShip()
    {
        if (!isProcessingShipMovement)
        {
            isProcessingShipMovement = true;
            Ship departingShip = shipQueue.Dequeue();
            StartCoroutine(ShipDepartureSequence(departingShip));
        }
    }

    private IEnumerator ShipDepartureSequence(Ship departingShip)
    {
        yield return new WaitForSeconds(1.5f);
        Vector3 departurePosition = departingShip.transform.position + Vector3.right * shipSpacing * 2;
        departingShip.transform.DOMove(departurePosition, shipMoveSpeed)
            .OnComplete(() => Destroy(departingShip.gameObject));
        
        if (shipQueue.Count > 0)
        {
            Ship nextShip = shipQueue.Peek(); // Peek the next ship in the queue
            yield return StartCoroutine(MoveNextShipToLoadingPosition(nextShip));
        }

        isProcessingShipMovement = false;
    }

    private IEnumerator MoveNextShipToLoadingPosition(Ship ship)
    {
        // Move ship to the loading/spawn position
        ship.transform.DOMove(shipSpawnPoint.position, shipMoveSpeed);
        yield return new WaitForSeconds(shipMoveSpeed);

        // Wait a moment before starting to load containers
        yield return new WaitForSeconds(waitTimeBeforeLoading);

        // Load any matching containers from the dock
        yield return StartCoroutine(LoadDockedContainers(ship));
    }

    private IEnumerator LoadDockedContainers(Ship ship)
    {
        string shipType = ship.ShipType;

        if (dockedContainers.ContainsKey(shipType))
        {
            var containersToLoad = dockedContainers[shipType].ToList();
            foreach (var container in containersToLoad)
            {
                if (ship.IsFullyLoaded) break;

                Transform slot = ship.GetNextAvailableSlot();
                if (slot != null)
                {
                    containerLoader.LoadContainer(container, slot);
                    dockedContainers[shipType].Remove(container);
                }

                yield return new WaitForSeconds(0.5f);
            }

            if (ship.IsFullyLoaded)
            {
                ProcessFullyLoadedShip();
            }
        }
    }

    private void LoadOntoDock(IContainer container)
    {
        Transform availableDock = dockManager.GetAvailableDock();
        if (availableDock != null)
        {
            containerLoader.LoadContainer(container as Container, availableDock,true);
            
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

    private void OnDestroy()
    {
        var containers = FindObjectsOfType<Container>();
        foreach (var container in containers)
            container.OnContainerFilled -= HandleFilledContainer;
    }
}