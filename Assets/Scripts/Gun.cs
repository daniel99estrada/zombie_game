using UnityEngine;

public class Gun : MonoBehaviour, IFirable
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource gunshotSound;
    [SerializeField] private GameObject bulletPrefab;

    private float nextFireTime = 0f;

    public Transform MuzzlePoint => muzzlePoint;
    public float FireRate => fireRate;
    public int DamageAmount => damageAmount;

    public delegate void GunFiredEvent();
    public event GunFiredEvent OnGunFired;

    public void Fire()
    {   
        if (Time.time >= nextFireTime)
        {   
            nextFireTime = Time.time + fireRate;

            if (muzzleFlash != null)
            {
                ParticleSystem muzzleEffect = Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
                muzzleEffect.Play();
            }

            if (gunshotSound != null)
                gunshotSound.Play();

            GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

            if (bullet.TryGetComponent<Bullet>(out Bullet bulletScript))
            {
                bulletScript.Initialize(muzzlePoint.forward, damageAmount);
            }

            OnGunFired?.Invoke();
        }
    }
}