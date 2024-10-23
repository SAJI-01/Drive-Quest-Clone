using UnityEngine;

public interface IDockManager
{
    void InitializeDocks(int count, float gap, Vector3 scale);
    Transform GetAvailableDock();
}