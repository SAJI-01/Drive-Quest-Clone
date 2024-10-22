using UnityEngine;

public interface IContainerLoader
{
    bool LoadContainer(Container container, Transform destination);
}