using UnityEngine;
using System;

public class EnemyVisual : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] protected AnimationNames animationNames;
    SpriteRenderer enemySpriteRenderer;
    protected Animator enemyAnimator;
    
    protected bool canLookAtPlayer;
    
    public event Action OnEnemyAttack;
    public event Action OnEnemyAttackAnimationFinished;
    public event Action OnEnemyHurtAnimationFinished;
    public event Action OnEnemyDeadAnimationFinished;
    private void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        canLookAtPlayer = false;
    }
    private void Start()
    {
        enemy.OnStateChanged += EnemyOnStateChanged;
    }
    private void Update()
    {
        if(canLookAtPlayer)
        {
            enemySpriteRenderer.flipX = enemy.GetLookingDirecton().x < 0;
        }
    }
    protected virtual void EnemyOnStateChanged(object sender, Enemy.OnStateChangedEventArgs e)
    {
        switch (e.state)
        {
            case Enemy.State.Idle:
                canLookAtPlayer = false;
                enemyAnimator.Play(animationNames.Idle);
                break;
            case Enemy.State.Chasing:
                canLookAtPlayer = true;
                enemyAnimator.Play(animationNames.Chasing);
                break;
            case Enemy.State.Cooldown:
                canLookAtPlayer = true;
                enemyAnimator.Play(animationNames.Cooldown);
                break;
            case Enemy.State.Attacking:
                canLookAtPlayer = true;
                enemyAnimator.Play(animationNames.Attacking);
                break;
            case Enemy.State.Hurt:
                canLookAtPlayer = false;
                enemyAnimator.Play(animationNames.Hurt, 0, 0f);
                enemyAnimator.Play(animationNames.HurtSecondLayer, 1, 0f);
                break;
            case Enemy.State.Dead:
                canLookAtPlayer = false;
                enemyAnimator.Play(animationNames.Dead);
                enemyAnimator.Play(animationNames.HurtSecondLayer, 1, 0f);
                break;
        }
    }
    public void HurtAnimationFinished()
    {
        OnEnemyHurtAnimationFinished?.Invoke();
    }
    public void AttackAnimationFinished()
    {
        OnEnemyAttackAnimationFinished?.Invoke();
    }
    public void DeadAnimationFinished()
    {
        OnEnemyDeadAnimationFinished?.Invoke();
    }
    public void EnemyAttack()
    {
        OnEnemyAttack?.Invoke();
    }
}
