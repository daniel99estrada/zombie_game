using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;  // Bullet speed
    [SerializeField] private float lifetime = 5f;  // Bullet auto-destroy time
    [SerializeField] private int DamageAmount = 50; // Damage value

    private Vector3 moveDirection; // Reference to the impact particle effect
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody settings are correct
        rb.linearDamping = 0f; // No resistance
        rb.angularDamping = 0f; // No angular resistance
        rb.isKinematic = false; // Make sure it's not kinematic
    }

    public void Initialize(Vector3 direction, int damageAmount)
    {
        DamageAmount = damageAmount;
        moveDirection = direction.normalized; // Ensure the direction is normalized for consistent movement
        ApplyForce(); // Apply force when the bullet is initialized
        Invoke("DestroyBullet", lifetime); // Destroy after lifetime
    }

    private void ApplyForce()
    {
        // Apply an instant force to the Rigidbody in the direction of the moveDirection
        if (rb != null)
        {
            rb.AddForce(moveDirection * speed, ForceMode.Impulse); // Apply force to move the bullet
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target)) 
        {
            target.TakeDamage(DamageAmount);
            DestroyBullet();
        }   
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
