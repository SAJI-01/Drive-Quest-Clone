using UnityEngine;

public class ContainerLoader : MonoBehaviour, IContainerLoader
{
    [SerializeField] private float loadingSpeed = 5f;

    public bool LoadContainer(Container container, Transform destination)
    {
        if (container == null || destination == null)
            return false;

        StartCoroutine(LoadingProcess(container, destination));
        return true;
    }

    private System.Collections.IEnumerator LoadingProcess(Container container, Transform destination)
    {
        Vector3 startPos = container.transform.position;
        Vector3 endPos = destination.position;
        float length = Vector3.Distance(startPos, endPos);
        float startTime = Time.time;

        while (container.transform.position != endPos)
        {
            float distanceCovered = (Time.time - startTime) * loadingSpeed; 
            float coveredSoFar = distanceCovered / length;
            
            container.transform.position = Vector3.Lerp(startPos, endPos, coveredSoFar);
            container.transform.SetParent(destination);
            yield return null;
        }
    }
}