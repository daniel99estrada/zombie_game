using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyState DefaultState;
    [SerializeField] private float IdleLocationRadius = 4f;
    [SerializeField] private float IdleMoveSpeedMultiplier = 0.5f;
    [SerializeField] private float updateSpeed = 0.1f;
    [SerializeField] private int WaypointIndex = 0;

    public EnemyAnimationController EnemyAnimationController;
    public EnemyLineOfSightChecker LineOfSightChecker;

    public Transform Player;
    public NavMeshTriangulation Triangulation;
    public float Health = 100f;

    private EnemyState _state;
    public EnemyState State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    private NavMeshAgent agent;
    private Coroutine FollowCoroutine;
    private Enemy Enemy;
    private Vector3[] Waypoints = new Vector3[4];

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public event StateChangeEvent OnStateChange;

    private void OnDisable() => State = DefaultState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        Enemy = GetComponent<Enemy>();

        OnStateChange += HandleStateChange;
        LineOfSightChecker.OnGainSight += _ => State = EnemyState.Chase;
        LineOfSightChecker.OnLoseSight += _ => State = DefaultState;
        Enemy.OnDeath += () => State = EnemyState.Dead;
    }

    public void Spawn()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out NavMeshHit hit, 2f, agent.areaMask))
                Waypoints[i] = hit.position;
            else
                Debug.LogError("Unable to find position for NavMesh near triangulation vertex");
        }
        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
    }

    private void Update()
        => EnemyAnimationController.SetAnimation(agent.velocity.magnitude > 0.01f ? EnemyAnimationState.Walk : EnemyAnimationState.Idle);

    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (FollowCoroutine != null) StopCoroutine(FollowCoroutine);

        if (oldState == EnemyState.Idle)
            agent.speed /= IdleMoveSpeedMultiplier;

        FollowCoroutine = newState switch
        {
            EnemyState.Idle => StartCoroutine(DoIdleMotion()),
            EnemyState.Patrol => StartCoroutine(DoPatrolMotion()),
            EnemyState.Chase => StartCoroutine(FollowTarget()),
            _ => null
        };
    }

    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);

        yield return new WaitUntil(() => agent.enabled && agent.isOnNavMesh);
        agent.SetDestination(Waypoints[WaypointIndex]);

        while (true)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                WaypointIndex = (WaypointIndex + 1) % Waypoints.Length;
                agent.SetDestination(Waypoints[WaypointIndex]);
            }
            yield return wait;
        }
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);
        agent.speed *= IdleMoveSpeedMultiplier;

        while (true)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                yield return wait;
                continue;
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
                if (NavMesh.SamplePosition(agent.transform.position + new Vector3(point.x, 0, point.y), out NavMeshHit hit, 2f, agent.areaMask))
                    agent.SetDestination(hit.position);
            }

            yield return wait;
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);

        while (enabled)
        {
            agent.SetDestination(Player.position);
            yield return wait;
        }
    }
}
