using UnityEngine;

[RequireComponent(typeof(ShipDetector))]
[RequireComponent(typeof(ContainerLoader))]
[RequireComponent(typeof(DockManager))]
public class LoadingSystemInitializer : MonoBehaviour
{
    private void Awake()
    {
        // This empty MonoBehaviour ensures all required components are added
        // when LoadingManager is added to a GameObject
    }
}