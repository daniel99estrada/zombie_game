using UnityEngine;

public enum EnemyAnimationState
{
    Idle,
    Walk,
    Attack,
    Death
}

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private EnemyAnimationState currentState;

    private const string Is_Walking = "IsWalking";
    private const string Death = "Death";
    public Enemy enemy;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        enemy.OnDeath += HandleDeath;
    }

    public void SetAnimation(EnemyAnimationState state)
    {
        switch (state)
        {
            case EnemyAnimationState.Idle:
                animator.SetBool(Is_Walking, false);
                break;

            case EnemyAnimationState.Walk:
                animator.SetBool(Is_Walking, true);
                break;

            case EnemyAnimationState.Attack:
                animator.SetTrigger("Attack");
                break;

            case EnemyAnimationState.Death:
                animator.SetTrigger(Death);
                break;
        }
    }

    private void HandleDeath()
    {
        SetAnimation(EnemyAnimationState.Death);
    }
}
