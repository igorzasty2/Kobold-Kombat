using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] protected float detectionRange;
    [SerializeField] protected float focusRange;
    [SerializeField] protected float attackRange;
    [SerializeField] LayerMask wallLayerMask;
    [SerializeField] protected LayerMask playerLayerMask;
    [SerializeField] protected float attackCooldownMax;
    [SerializeField] protected EnemyVisual enemyVisual;
    [SerializeField] float knockBackForce;
    [SerializeField] protected float health;
    [SerializeField] float damage;
    [SerializeField] bool canNotBeStunned;
    [SerializeField] float timeToDestroyObjectAfterDeath;
    protected Transform playerTransform;
    protected Rigidbody2D enemyRigidbody;
    protected float attackCooldown;
    bool isAttackAnimationFinished;
    bool isHurtAnimationFinished;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    protected State state;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
        public OnStateChangedEventArgs(State state)
        {
            this.state = state;
        }
    }
    public enum State
    {
        Idle,
        Chasing,
        Cooldown,
        Attacking,
        Hurt,
        Dead
    }
    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        state = State.Idle;
    }
    protected virtual void Start()
    {
        playerTransform = PlayerControl.Instance.GetPlayerTransform();
        enemyVisual.OnEnemyAttack += Attack;
        enemyVisual.OnEnemyAttackAnimationFinished += EnemyAttackAnimationFinished;
        enemyVisual.OnEnemyHurtAnimationFinished += EnemyAttackHurtAnimationFinished;
        enemyVisual.OnEnemyDeadAnimationFinished += EnemyDeadAnimationFinished;
    }
    protected virtual void Update()
    {
        SetEnemyState();
        if (state == State.Chasing)
        {
            MoveToPlayer();
        }
    }
    protected virtual void SetEnemyState()
    {
        State previousState = state;
        switch (state)
        {
            case State.Idle:
                if (DetectPlayer())
                {
                    state = State.Chasing;
                }
                break;
            case State.Chasing:
                if (!IsPlayerInRange(focusRange))
                {
                    state = State.Idle;
                }
                else if (IsPlayerInRange(attackRange) && CanLookAtPlayer())
                {
                    state = State.Cooldown;
                    attackCooldown = attackCooldownMax;
                }
                break;
            case State.Cooldown:
                if(IsPlayerInRange(attackRange))
                {
                    if(!CanLookAtPlayer())
                    {
                        state = State.Chasing;
                    }
                    else
                    {
                        if (attackCooldown < attackCooldownMax)
                        {
                            attackCooldown += Time.deltaTime;
                        }
                        else
                        {
                            attackCooldown = 0f;
                            isAttackAnimationFinished = false;
                            state = State.Attacking;
                        }
                    }
                }
                else
                {
                    if (!IsPlayerInRange(focusRange))
                    {
                        state = State.Idle;
                    }
                    else
                    {
                        state = State.Chasing;
                    }
                }
                break;
            case State.Attacking:
                if(isAttackAnimationFinished)
                {
                    if (!IsPlayerInRange(attackRange))
                    {
                        if (!IsPlayerInRange(focusRange))
                        {
                            state = State.Idle;
                        }
                        else
                        {
                            state = State.Chasing;
                        }
                    }
                    else
                    {
                        state = State.Cooldown;
                    }
                }
                break;
            case State.Hurt:
                if(isHurtAnimationFinished)
                {
                        if (IsPlayerInRange(attackRange))
                        {
                            state = State.Chasing;
                        }
                        else if (IsPlayerInRange(focusRange))
                        {
                            state = State.Chasing;
                        }
                        else
                        {
                            state = State.Idle;
                        }
                }
                break;
        }
        if(previousState != state)
        {
            EnemyStateChanged(new OnStateChangedEventArgs(state));
            if(state != State.Chasing)
            {
                enemyRigidbody.velocity = Vector2.zero;
            }
        }
    }
    protected void EnemyStateChanged(OnStateChangedEventArgs e)
    {
        OnStateChanged?.Invoke(this, e);
    }
    private bool DetectPlayer()
    {
        if (IsPlayerInRange(detectionRange))
        {
            return CanLookAtPlayer();
        }
        return false;
    }
    protected bool CanLookAtPlayer()
    {
        Vector2 direction = playerTransform.position - transform.position;
        float distanceToPlayer = direction.magnitude;
        return !Physics2D.Raycast(transform.position, direction, distanceToPlayer, wallLayerMask);
    }
    protected bool IsPlayerInRange(float range)
    {
        if (playerTransform == null)
            return false;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= range;
    }
    private void MoveToPlayer()
    {
        Vector2 velocity = playerTransform.position - transform.position;
        enemyRigidbody.velocity = velocity.normalized * movementSpeed;
    }
    protected virtual void Attack()
    {
        Debug.Log("Attack");
    }
    private void EnemyAttackAnimationFinished()
    {
        isAttackAnimationFinished = true;
    }
    private void EnemyAttackHurtAnimationFinished()
    {
        isHurtAnimationFinished = true;
    }
    private void EnemyDeadAnimationFinished()
    {
        Destroy(gameObject, timeToDestroyObjectAfterDeath);
    }
    protected virtual void EnemyHit(float damage)
    {
        if (state != State.Dead)
        {
            enemyRigidbody.velocity = Vector2.zero;
            health -= damage;
            if(health > 0)
            {
                if (canNotBeStunned)
                {
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs(State.Hurt));
                }
                else
                {
                    state = State.Hurt;
                    isHurtAnimationFinished = false;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs(state));
                }
            }
            else
            {
                state = State.Dead;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs(state));
            }
            if(!canNotBeStunned)
            {
                KnockBack();
            }
        }
    }
    private void KnockBack()
    {
        Vector2 forceVector = (transform.position - playerTransform.position).normalized * knockBackForce;
        enemyRigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }
    public  Vector2 GetLookingDirecton()
    {
        return playerTransform.position - transform.position;
    }
}
