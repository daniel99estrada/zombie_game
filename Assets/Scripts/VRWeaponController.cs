using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRWeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource gunSound;
    [SerializeField] private float fireRate = 0.1f;
    
    [Header("XR Settings")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    [SerializeField] private InputHelpers.Button triggerButton = InputHelpers.Button.Trigger;
    
    private bool canFire = true;
    private float nextFireTime;
    
    private void Awake()
    {
        // Ensure the weapon can be grabbed properly
        if (grabInteractable == null)
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            
        SetupInteractions();
    }
    
    private void SetupInteractions()
    {
        grabInteractable.activated.AddListener(OnTriggerPull);
        grabInteractable.deactivated.AddListener(OnTriggerRelease);
    }
    
    private void OnTriggerPull(ActivateEventArgs args)
    {
        if (Time.time >= nextFireTime && canFire)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    private void OnTriggerRelease(DeactivateEventArgs args)
    {
        // Reset firing state if needed
        canFire = true;
    }
    
    private void Fire()
    {
        // Create projectile or raycast for hit detection
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit))
        {
            // Handle hit effects here
            Debug.DrawLine(muzzlePoint.position, hit.point, Color.red, 1f);
            Debug.Log("Fired a round");
        }
        
        // Visual and audio feedback
        if (muzzleFlash != null)
            muzzleFlash.Play();
            
        if (gunSound != null)
            gunSound.Play();
            
        // Add recoil effect if desired
        ApplyRecoil();
    }
    
    private void ApplyRecoil()
    {
        // Add subtle rotation and position offset
        transform.localPosition -= Vector3.forward * 0.05f;
        transform.localRotation *= Quaternion.Euler(-2f, 0f, 0f);
    }
    
    private void OnDestroy()
    {
        // Clean up event listeners
        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(OnTriggerPull);
            grabInteractable.deactivated.RemoveListener(OnTriggerRelease);
        }
    }
}