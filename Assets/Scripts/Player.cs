using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    
    [SerializeField]
    private int Health = 300;
    // Start is called before the first frame update
    private void OnAttack(IDamageable Target)
    {
        Debug.Log("Attacking");
    }
    
    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        PlayerDamageVFX.Instance.DoDamageEffect();
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
