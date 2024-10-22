using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections;


public class Container : MonoBehaviour, IContainer
{
    [Header("Container Configuration")]
    [SerializeField] private Transform containerDoorSlider;
    [SerializeField] private Vector3 targetScale = new Vector3(1f, 1f, 0.1f);
    [SerializeField] private float durationDoorSlider = 1f;
    [SerializeField] private Vector3 popAnimationScale = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private float popAnimationDuration = 0.2f;
    [SerializeField] private VehicleType acceptedCarType;

    public bool IsFilled { get; private set; }
    public string CarType { get; private set; }

    public event System.Action<IContainer> OnContainerFilled;

    private void OnTriggerEnter(Collider collideCar)
    {
        var incomingCarType = GetCarTypeFromTag(collideCar.tag);
        if (TagManager.IsValidCarTag(collideCar.tag))
        {
            if (incomingCarType == acceptedCarType)
            {
                HandleCarOnEntry(collideCar.gameObject);
            }
            else
            {
                Debug.Log($"Wrong car type entered container. Accepted Car: {acceptedCarType}, Got: {incomingCarType}");
            }
        }
    }

    private VehicleType GetCarTypeFromTag(string tag)
    {
        string colorPart = tag.Replace("Car", "");
        if (System.Enum.TryParse(colorPart, out VehicleType vehicleType))
        {
            return vehicleType;
        }
        Debug.LogError($"Invalid car tag : {tag}");
        return VehicleType.Red; 
    }


    public void HandleCarOnEntry(GameObject car)
    {
        if (IsFilled) return;
        CarType = car.tag.Replace("Car", "");
        car.SetActive(false);
        StartCoroutine(ContainerFillingAction());
    }

    private IEnumerator ContainerFillingAction()
    {
        GetComponent<Collider>().enabled = false;
        containerDoorSlider.DOScale(targetScale, durationDoorSlider).SetEase(Ease.Linear);
        yield return new WaitForSeconds(durationDoorSlider);
        transform.DOPunchScale(popAnimationScale, popAnimationDuration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(popAnimationDuration);
        IsFilled = true;
        OnContainerFilled?.Invoke(this);
    }


}

