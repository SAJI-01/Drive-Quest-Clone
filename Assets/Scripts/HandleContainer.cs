using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HandleContainer : MonoBehaviour
{
    [SerializeField] private Transform containerDoorSlider;
    [SerializeField] private float targetScaleY = 0f;
    [SerializeField] private float durationDoorSlider = 1f;
    [SerializeField] private Vector3 popAnimationScale = new Vector3(1.1f, 1.1f, 1.1f); 
    [SerializeField] private float popAnimationDuration = 0.2f; 

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            Destroy(other.gameObject);
            OpenContainerDoor();
        }
    }

    private void OpenContainerDoor()
    {
        Sequence doorSequence = DOTween.Sequence();
        doorSequence.Append(containerDoorSlider.DOScaleY(targetScaleY, durationDoorSlider).SetEase(Ease.Linear));
        doorSequence.Join(containerDoorSlider.DOMoveY(containerDoorSlider.position.y - targetScaleY * 0.5f, durationDoorSlider).SetEase(Ease.Linear)).OnComplete(() =>
        { 
            gameObject.transform.DOPunchScale(popAnimationScale, popAnimationDuration).SetEase(Ease.Linear);
        });

            
    }
}