using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhoulBoss : MeleeEnemy
{
    [SerializeField] private float dashMaxDistance;
    [SerializeField] private float specialAttackTimerMax;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float staggerableTimerMax;
    [SerializeField] private float staggerableWhileHittingTimerMax;

    private float specialAttackTimer;
    private bool specialAttackFinished;
    private Vector2 dashPosition;
    private bool canDashSecondTime;
    private bool hitPlayer;
    private GhoulBossState ghoulBossState;
    private GhoulAttackVariant attackVariant;
    private static readonly System.Random random = new System.Random();
    private bool unstaggerable;
    private float staggerableTimer;

    public class GhoulOnStateChangedEventArgs : OnStateChangedEventArgs
    {
        public GhoulBossState ghoulBossState;
        public GhoulOnStateChangedEventArgs(State state, GhoulBossState ghoulBossState) : base(state)
        {
            this.ghoulBossState = ghoulBossState;
        }
    }

    private enum GhoulAttackVariant
    {
        Basic,
        OneDashAttack,
        TwoDashAttack
    }

    public enum GhoulBossState
    {
        None,
        Dash,
        AttackAfterDash,
        Hurt
    }

    protected override void Start()
    {
        base.Start();
        GhoulBossVisual ghoulBossVisual = enemyVisual as GhoulBossVisual;
        if (ghoulBossVisual != null)
        {
            ghoulBossVisual.OnAttackAfterDashAnimationFinished += AttackAfterDashFinished;
        }
        attackVariant = GhoulAttackVariant.Basic;
        unstaggerable = true;
    }

    protected override void Update()
    {
        base.Update();
        if(unstaggerable == false)
        {
            if(staggerableTimer < staggerableTimerMax)
            {
                staggerableTimer += Time.deltaTime;
            }
            else
            {
                if(staggerableTimer < staggerableWhileHittingTimerMax && state == State.Hurt)
                {
                    staggerableTimer += Time.deltaTime;
                }
                else
                {
                    unstaggerable = true;
                    staggerableTimer = 0f;
                }
            }
        }
        if (specialAttackTimer < specialAttackTimerMax)
        {
            specialAttackTimer += Time.deltaTime;
        }
        if (attackVariant == GhoulAttackVariant.OneDashAttack || attackVariant == GhoulAttackVariant.TwoDashAttack)
        {
            if (ghoulBossState == GhoulBossState.Dash)
            {
                DashTowardsThePlayer();
            }
        }
    }

    protected override void SetEnemyState()
    {
        if (state == State.Cooldown && specialAttackTimer >= specialAttackTimerMax)
        {
            if (IsPlayerInRange(attackRange))
            {
                if (!CanLookAtPlayer())
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
                        PerformSpecialAttack();
                    }
                }
            }
            else
            {
                UpdateStateBasedOnPlayerRange();
            }
            return;
        }

        if (state == State.Attacking && attackVariant != GhoulAttackVariant.Basic)
        {
            if (specialAttackFinished)
            {
                unstaggerable = false;
                staggerableTimer = 0f;
                ResetAttackState();
            }
            return;
        }

        base.SetEnemyState();
    }

    private void PerformSpecialAttack()
    {
        hitPlayer = false;
        attackCooldown = 0f;
        specialAttackTimer = 0f;
        specialAttackFinished = false;
        attackVariant = (GhoulAttackVariant)random.Next(1, 3);
        if (attackVariant == GhoulAttackVariant.TwoDashAttack)
        {
            canDashSecondTime = true;
            hitPlayer = false;
        }
        state = State.Attacking;
        ghoulBossState = GhoulBossState.Dash;
        EnemyStateChanged(new GhoulOnStateChangedEventArgs(state, ghoulBossState));
        SetDashPosition();
        DashTowardsThePlayer();
    }

    private void UpdateStateBasedOnPlayerRange()
    {
        if (!IsPlayerInRange(focusRange))
        {
            state = State.Idle;
        }
        else
        {
            state = State.Chasing;
        }
        EnemyStateChanged(new OnStateChangedEventArgs(state));
    }

    private void ResetAttackState()
    {
        attackVariant = GhoulAttackVariant.Basic;
        ghoulBossState = GhoulBossState.None;
        canDashSecondTime = false;
        UpdateStateBasedOnPlayerRange();
    }

    private void SetDashPosition()
    {
        Vector2 vectorToPlayer = playerTransform.position - transform.position;
        dashPosition = vectorToPlayer.magnitude <= dashMaxDistance
            ? playerTransform.position
            : (Vector2)transform.position + vectorToPlayer.normalized * dashMaxDistance;
    }

    private void DashTowardsThePlayer()
    {
        if (Vector3.Distance(transform.position, dashPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashPosition, dashSpeed * Time.deltaTime);
        }
        else
        {
            ghoulBossState = GhoulBossState.AttackAfterDash;
            EnemyStateChanged(new GhoulOnStateChangedEventArgs(state, ghoulBossState));
        }
    }

    private void AttackAfterDashFinished()
    {
        if (attackVariant == GhoulAttackVariant.OneDashAttack)
        {
            specialAttackFinished = true;
        }
        else if (attackVariant == GhoulAttackVariant.TwoDashAttack)
        {
            HandleTwoDashAttack();
        }
    }

    private void HandleTwoDashAttack()
    {
        if (canDashSecondTime)
        {
            if (!hitPlayer)
            {
                SetDashPosition();
                state = State.Attacking;
                ghoulBossState = GhoulBossState.Dash;
                EnemyStateChanged(new GhoulOnStateChangedEventArgs(state, ghoulBossState));
            }
            else
            {
                hitPlayer = false;
                specialAttackFinished = true;
            }
        }
        else
        {
            specialAttackFinished = true;
        }
        canDashSecondTime = false;
    }

    public override void Damage(int damage, Vector3 origin, bool isMelee)
    {
        if(unstaggerable)
        {
            if(health - damage > 0)
            {
                GameManager.damageDealt += damage;
                health -= damage;
                if (damageDisplay != null)
                {
                    damageDisplay.ShowDamage(transform.position, damage);
                }
                EnemyStateChanged(new GhoulOnStateChangedEventArgs(state, GhoulBossState.Hurt));
                return;
            }
        }
        canDashSecondTime = false;
        attackVariant = GhoulAttackVariant.Basic;
        ghoulBossState = GhoulBossState.None;
        base.Damage(damage, origin, isMelee);
    }
    protected override void Attack()
    {
        Vector2 hitboxPos = transform.position + (playerTransform.position - transform.position).normalized * distanceHitboxFromEnemy;
        if (Physics2D.OverlapCircle(hitboxPos, radiusOfHitbox, playerLayerMask) != null)
        {
            if(attackVariant == GhoulAttackVariant.TwoDashAttack)
            {
                hitPlayer = true;
            }
        }
    }
}
