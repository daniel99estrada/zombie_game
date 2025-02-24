using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{   
    public SphereCollider Collider;
    public EnemyAnimationController EnemyAnimationController;
    private List<IDamageable> Damageables = new List<IDamageable>();
    public int Damage = 10;
    public float AttackDelay = 0.5f;
    public delegate void AttackEvent(IDamageable Target);
    public AttackEvent OnAttack;
    private Coroutine AttackCoroutine; // Fixed: Made AttackCoroutine private to avoid external modifications

    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
        
        Collider.isTrigger = true; 
    }

    
    private void OnTriggerEnter(Collider other)
    {   
        IDamageable damageable = other.GetComponent<IDamageable>(); // Fixed: Corrected variable name typo

        if (damageable != null)
        {
            Damageables.Add(damageable);

            if (AttackCoroutine == null) // Fixed: Ensure coroutine starts only if not already running
            {
                AttackCoroutine = StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerExit(Collider other) // Fixed: Changed to private for consistency
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            Damageables.Remove(damageable);
            if (Damageables.Count == 0 && AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine); // Fixed: Stop coroutine if no damageables remain
                AttackCoroutine = null;
            }
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds Wait = new WaitForSeconds(AttackDelay);

        while (Damageables.Count > 0)
        {
            IDamageable closestDamageable = null;
            float closestDistance = float.MaxValue;
            
            for (int i = 0; i < Damageables.Count; i++)
            {
                Transform damageableTransform = Damageables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTransform.position);
                
                if (distance < closestDistance)
                {
                    closestDamageable = Damageables[i]; // Fixed: Correctly assigned closest IDamageable
                    closestDistance = distance;
                } 
            }

            if (closestDamageable != null)
            {
                OnAttack?.Invoke(closestDamageable);
                closestDamageable.TakeDamage(Damage);
                
                EnemyAnimationController.SetAnimation(EnemyAnimationState.Attack); 
            }

            yield return Wait;

            Damageables.RemoveAll(DisabledDamageables);
        }

        AttackCoroutine = null;
    }

    private bool DisabledDamageables(IDamageable damageable)
    {
        return damageable == null || !damageable.GetTransform().gameObject.activeSelf; // Fixed: Corrected reference to damageable's transform
    }
}
