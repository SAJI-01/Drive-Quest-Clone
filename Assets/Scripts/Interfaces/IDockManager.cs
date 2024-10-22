using UnityEngine;

public interface IDockManager
{
    void InitializeDocks(int count, float gap);
    Transform GetAvailableDock();
}