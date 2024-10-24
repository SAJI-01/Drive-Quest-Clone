using UnityEngine;

public class ContainerLoader : MonoBehaviour, IContainerLoader
{
    [SerializeField] private float loadingSpeed = 5f;

    public bool LoadContainer(Container container, Transform destination,bool isDockedContainer =false)
    {
        if (container == null || destination == null)
            return false;

        StartCoroutine(LoadingProcess(container, destination,isDockedContainer));
        return true;
    }

    private System.Collections.IEnumerator LoadingProcess(Container container, Transform destination ,bool isDockedContainer)
    {
        Vector3 startPos = container.transform.position;
        Vector3 endPos = destination.position;
        Vector3 startRot = container.transform.localRotation.eulerAngles;
        Vector3 endRot = Vector3.zero;
        float length = Vector3.Distance(startPos, endPos);
        float startTime = Time.time;
        container.transform.GetChild(0).GetComponent<Obstacle>().DisableAllColliders();
        

        while (container.transform.position != endPos)
        {
            float distanceCovered = (Time.time - startTime) * loadingSpeed; 
            float coveredSoFar = distanceCovered / length;
            
            container.transform.position = Vector3.Lerp(startPos, endPos, coveredSoFar);
            if(container.transform.localRotation != Quaternion.Euler(endRot))
            {
                container.transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRot, endRot, coveredSoFar));
            }
            yield return null;
        }
        container.transform.position = endPos;
        container.transform.localRotation = Quaternion.Euler(endRot);
        
        if(!isDockedContainer)
        {
            container.transform.SetParent(destination);
        }
        
    }
}