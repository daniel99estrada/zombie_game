using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;  // Bullet speed
    [SerializeField] private float lifetime = 5f;  // Bullet auto-destroy time
    [SerializeField] private int DamageAmount = 50; // Damage value
    [SerializeField] private ParticleSystem impactEffect; // Reference to impact effect

    private Vector3 moveDirection; // Reference to the impact particle effect

    public void Initialize(Vector3 direction, int damageAmount)
    {
        DamageAmount = damageAmount;
        moveDirection = direction;
        Invoke("DestroyBullet", lifetime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target)) 
        {
            target.TakeDamage(DamageAmount);

            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity).Play();
            }
            DestroyBullet();
        }   
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
