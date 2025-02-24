using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{   
    public SphereCollider Collider;
    public EnemyAnimationController EnemyAnimationController;
    public int Damage = 10;
    public float AttackDelay = 0.5f;
    public delegate void AttackEvent();
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
            if (AttackCoroutine == null) // Fixed: Ensure coroutine starts only if not already running
            {
                AttackCoroutine = StartCoroutine(Attack(damageable));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {   
        if (AttackCoroutine != null)
        {
            StopCoroutine(AttackCoroutine);
            AttackCoroutine = null;
        }
    }

    private IEnumerator Attack(IDamageable damageable)
    {
        WaitForSeconds Wait = new WaitForSeconds(AttackDelay);

        OnAttack?.Invoke();
        
        damageable.TakeDamage(Damage);

        EnemyAnimationController.SetAnimation(EnemyAnimationState.Attack);

        yield return Wait;
 
        AttackCoroutine = null;
    }
}
