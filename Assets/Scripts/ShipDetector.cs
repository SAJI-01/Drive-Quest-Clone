using UnityEngine;

public class ShipDetector : MonoBehaviour, IShipDetector
{
    private string detectedShipType;

    public bool DetectShip(Vector3 position, Vector3 direction, float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit, distance))
        {
            detectedShipType = hit.collider.gameObject.tag.Replace("Ship", "");
            Debug.Log($"Detected ship!!: <color=red>{detectedShipType}</color>");
            
            return true;
        }
        detectedShipType = string.Empty;
        return false;
    }

    public string GetDetectedShipType() => detectedShipType;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);
    }
}