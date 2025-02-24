using UnityEngine;

public class GunManager : MonoBehaviour
{
    private IFirable currentGun;
    public static GunManager Instance {get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentGun(IFirable gun)
    {
        currentGun = gun;
    }

    public void FireCurrentGun()
    {
        if (currentGun != null)
        {
            currentGun.Fire();
        }
    }
}
