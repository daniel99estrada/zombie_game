using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRInputController : MonoBehaviour
{
    [SerializeField] private InputActionReference triggerAction;
    
    private void Awake()
    {
        SetupInputActions();
    }
    
    private void SetupInputActions()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed += OnTriggerPressed;
            triggerAction.action.canceled += OnTriggerReleased;
        }
    }
    
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        GunManager.Instance.FireCurrentGun();
    }
    
    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Not Implemented Yet");
    }
    
    
    private void OnDestroy()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed -= OnTriggerPressed;
            triggerAction.action.canceled -= OnTriggerReleased;
        }
    }
}