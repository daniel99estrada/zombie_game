using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{   
    [SerializeField]
    public EnemyState DefaultState;
    public EnemyAnimationController EnemyAnimationController;
    private EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {   
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }
    public EnemyLineOfSightChecker LineOfSightChecker;
    public delegate void StateChageEvent(EnemyState oldState, EnemyState newState);
    public StateChageEvent OnStateChange;
    public float IdleLocationRadius = 4f;
    public float IdleMoveSpeedMultiplier = 0.5f;
    public Transform Player;
    public UnityEngine.AI.NavMeshTriangulation Triangulation;
    public float updateSpeed = 0.1f;
    public float Health = 100f;

    private UnityEngine.AI.NavMeshAgent agent;

    private Coroutine FollowCoroutine;

    private Enemy Enemy; 

    [SerializeField]
    private int WaypointIndex = 0;
    private Vector3[] Waypoints = new Vector3[4];
    private void OnDisable()
    {
        _state = DefaultState;
    }

    void Awake() 
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        OnStateChange += HandleStateChange;
        
        LineOfSightChecker.OnGainSight += HandleGainSight;
        LineOfSightChecker.OnLoseSight += HandleLoseSight;

        Enemy = GetComponent<Enemy>();
        Enemy.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        State = EnemyState.Dead;
    }

    private void HandleGainSight(Player player)
    {
        State = EnemyState.Chase;
    }

    private void HandleLoseSight(Player player)
    {
        State = DefaultState;
    }

    public void Spawn()
    {   
        for (int i = 0; i < Waypoints.Length; i++)
        {
            NavMeshHit Hit;
            if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out Hit, 2f, agent.areaMask))
            {
                Waypoints[i] = Hit.position;
            }
            else
            {
                Debug.LogError($"Unable to find position for navmesh near triangulation vertrex");
            }
        }
        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
    }

    void Update()
    {   
        if (agent.velocity.magnitude > 0.01f)
        {
            EnemyAnimationController.SetAnimation(EnemyAnimationState.Walk);
        }
        else
        {
            EnemyAnimationController.SetAnimation(EnemyAnimationState.Idle);
        }
    }

    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {   
        if(FollowCoroutine != null)
        {
            StopCoroutine(FollowTarget());
        }

        if (oldState == EnemyState.Idle)
        {
            agent.speed /= IdleMoveSpeedMultiplier;
        }

        switch (newState)
        {
            case EnemyState.Idle:
                FollowCoroutine = StartCoroutine(DoIdleMotion());
                break;
            case EnemyState.Patrol:
                FollowCoroutine = StartCoroutine(DoPatrolMotion());
                break;
            case EnemyState.Chase:
                FollowCoroutine = StartCoroutine(FollowTarget());
                break;
            case EnemyState.Dead:
                break;
        }
    } 

    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds Wait = new WaitForSeconds(updateSpeed);

        yield return new WaitUntil(() => agent.enabled && agent.isOnNavMesh);
        agent.SetDestination(Waypoints[WaypointIndex]);
        while(true)
        {
            if (agent.enabled && agent.isOnNavMesh && agent.remainingDistance <= agent.stoppingDistance)
            {
                WaypointIndex++;
                if(WaypointIndex >= Waypoints.Length)
                {
                    WaypointIndex = 0;
                }
                agent.SetDestination(Waypoints[WaypointIndex]);
            }
            yield return Wait;
        }
    }
    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds Wait = new WaitForSeconds(updateSpeed);

        agent.speed *= IdleMoveSpeedMultiplier;

        while(true)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                yield return Wait;
            }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
                UnityEngine.AI.NavMeshHit hit;

                if(UnityEngine.AI.NavMesh.SamplePosition(agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, agent.areaMask))
                {
                    agent.SetDestination(hit.position);
                }
            }
            yield return Wait;

        }
    }
    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(updateSpeed);

        while(enabled)
        {
            agent.SetDestination(Player.transform.position);

            yield return Wait;
        }
    }
}
