using UnityEngine;

public interface IContainer
{
    bool IsFilled { get; }
    string CarType { get; }
    void HandleCarOnEntry(GameObject car);
    event System.Action<IContainer> OnContainerFilled;
}





