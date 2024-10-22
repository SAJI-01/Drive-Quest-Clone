using UnityEngine;

public interface IShipDetector
{
    bool DetectShip(Vector3 position, Vector3 direction, float distance);
    string GetDetectedShipType();
}