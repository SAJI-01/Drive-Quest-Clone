using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class HandleContainer : MonoBehaviour
{
    [SerializeField] private Transform containerDoorSlider;
    [SerializeField] private Vector3 targetScale;
    [SerializeField] private float targetPositionZ = 0.5f;
    [SerializeField] private float durationDoorSlider = 1f;
    [SerializeField] private Vector3 popAnimationScale; 
    [SerializeField] private float popAnimationDuration = 0.2f;
    
    public Loader loader;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            gameObject.GetComponent<Collider>().enabled = false;
            other.gameObject.SetActive(false);
            CloseContainerDoor();
        }
    }

    private void CloseContainerDoor()
    {
        containerDoorSlider.DOScale(new Vector3(targetScale.x, targetScale.y, targetScale.z), durationDoorSlider).SetEase(Ease.Linear).OnComplete(PopupAnimation);
        
    }
    
    private void PopupAnimation()
    {
        gameObject.transform.DOPunchScale(popAnimationScale, popAnimationDuration).SetEase(Ease.Linear).OnComplete(OnContainerFilled);
    }
    
    private void OnContainerFilled()
    {
        loader.onContainerFilled.Invoke();
    }
}