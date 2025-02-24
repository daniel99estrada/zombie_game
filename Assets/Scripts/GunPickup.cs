using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunPickup : MonoBehaviour
{
    private IFirable gun;

    private void Awake()
    {
        gun = GetComponent<IFirable>(); 
    }

    private void OnEnable()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        GunManager.Instance.SetCurrentGun(gun);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        GunManager.Instance.SetCurrentGun(null);
    }
}
