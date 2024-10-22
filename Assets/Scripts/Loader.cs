using System;
using UnityEngine;
using UnityEngine.Events;

public class Loader : MonoBehaviour
{
    public UnityEvent onContainerFilled;
    public Color[] colors;
    [Header("Dock Loaders")]
    public Transform dockLoader;
    public GameObject dockLoadersPrefab;
    [SerializeField] private int dockCount;
    [SerializeField] private float gapBtwDockLoaders;
    
    [Header("Ship Loaders")]
    public Transform[] shipLoaders;
    private bool isRedShip;
    private bool isGreenShip;
    private bool isBlueShip;
    private bool isYellowShip;
    private bool isPurpleShip;
    private bool isOrangeShip;
    private bool isPinkShip;

    private void Start()
    {
        SettingBools();
        DockLoadersInit();
        onContainerFilled.AddListener(OnContainerFilled);
    }

    private void DockLoadersInit()
    {
        float totalWidth = (dockCount - 1) * gapBtwDockLoaders;
        float startX = dockLoader.position.x - totalWidth / 2;

        for (int i = 0; i < dockCount; i++)
        {
            Vector3 position = new Vector3(startX + (i * gapBtwDockLoaders), 
                                         dockLoader.position.y, 
                                         dockLoader.position.z);

            var dockLoaderInstance = Instantiate(dockLoadersPrefab, position, dockLoadersPrefab.transform.rotation, dockLoader);
            dockLoaderInstance.name = "DockLoader" + i;
        }
    }

    private void SettingBools()
    {
        isRedShip = false;
        isGreenShip = false;
        isBlueShip = false;
        isYellowShip = false;
        isPurpleShip = false;
        isOrangeShip = false;
        isPinkShip = false;
    }

    private void OnContainerFilled()
    {
        if (isRedShip)
        {
            LoadOntoShip();
        }
    }

    private void LoadOntoShip()
    {
        
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3))
        {
            switch (hit.collider.gameObject.tag)
            {
                case "RedShip":
                    isRedShip = true;
                    break;
                case "GreenShip":
                    isGreenShip = true;
                    break;
                case "BlueShip":
                    isBlueShip = true;
                    break;
                case "YellowShip":
                    isYellowShip = true;
                    break;
                case "PurpleShip":
                    isPurpleShip = true;
                    break;
                case "OrangeShip":
                    isOrangeShip = true;
                    break;
                case "PinkShip":
                    isPinkShip = true;
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        dockLoader = null;
        onContainerFilled.RemoveListener(OnContainerFilled);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 3);
    }
}