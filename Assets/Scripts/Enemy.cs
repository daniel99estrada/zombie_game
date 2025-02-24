using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public class Enemy : PoolableObject, IDamageable
{   
    public AttackRadius AttackRadius;
    public EnemyMovement Movement;
    public NavMeshAgent Agent;
    public EnemyScriptableObject EnemyScriptableObject;
    public int Health = 100;
    public delegate void DeathEvent();
    public DeathEvent OnDeath;
    public delegate void OnAttackedEvent();
    public OnAttackedEvent OnAttacked;

    private Coroutine LookCoroutine;

    private void OnAttack(IDamageable Target)
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
    }

    private IEnumerator LookAt(Transform Target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * 2;
            yield return null;
        }
        transform.rotation = lookRotation;
    }

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (Agent != null) 
        {
            Agent.enabled = false;
        }
    }

    public virtual void SetupAgentFromConfiguration()
    {
        if (Agent == null || EnemyScriptableObject == null)
        {
            Debug.LogError("Agent or EnemyScriptableObject is null!");
            return;
        }

        Health = EnemyScriptableObject.Health;  
        AttackRadius.Collider.radius = EnemyScriptableObject.AttackRadius;
        AttackRadius.AttackDelay = EnemyScriptableObject.AttackDelay;
        AttackRadius.Damage = EnemyScriptableObject.Damage;

        Agent.updatePosition = true;
        Agent.updateRotation = true;
        Agent.updateUpAxis = true;
        
        Agent.acceleration = EnemyScriptableObject.Acceleration;
        Agent.angularSpeed = EnemyScriptableObject.AngularSpeed;
        Agent.areaMask = EnemyScriptableObject.AreaMask;
        Agent.avoidancePriority = EnemyScriptableObject.AvoidancePriority;
        Agent.baseOffset = EnemyScriptableObject.BaseOffset;
        Agent.height = EnemyScriptableObject.Height;
        Agent.obstacleAvoidanceType = EnemyScriptableObject.ObstacleAvoidanceType;
        Agent.radius = EnemyScriptableObject.Radius;
        Agent.speed = EnemyScriptableObject.Speed;
        Agent.stoppingDistance = EnemyScriptableObject.StoppingDistance; 
    }
    public void TakeDamage(int Damage)
    {
        Health -= Damage;
        OnAttacked?.Invoke();

        if (Health <= 0)
        {   
            OnDeath?.Invoke();
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
