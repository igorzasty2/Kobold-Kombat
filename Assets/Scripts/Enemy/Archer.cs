using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Archer : Enemy
{
    [SerializeField] protected float distanceProjectileFromEnemy;
    [SerializeField] protected GameObject projectile;
    [SerializeField] Ray ray;
    public event Action OnArcherStartedAiming;
    public event Action OnArcherStoppedAiming;
    public event Action OnArcherReadyToAttack;
    public event Action OnArcherStartAttacking;
    bool isAiming;
    protected override void Start()
    {
        base.Start();
        OnStateChanged += ArcherOnStateChanged;
        enemyVisual.OnEnemyAttackAnimationFinished += ArcherAttackAnimationFinished;
        ArcherVisual archerVisual = enemyVisual as ArcherVisual;
        archerVisual.OnAimingAtPlayer += ArcherVisualOnAimingAtPlayer;
        archerVisual.OnReadyToAttack += ArcherVisualOnReadyToAttack;
        archerVisual.OnStartAttacking += ArcherAttackAnimationStarted;
    }
    protected override void Update()
    {
        base.Update();
        ray.UpdateLineRenderer(transform.position, playerTransform.position);
    }

    private void ArcherOnStateChanged(object sender, OnStateChangedEventArgs e)
    {
        if(e.state == State.Hurt || state == State.Dead && isAiming)
        {
            ArcherAttackAnimationFinished();
        }
    }

    protected virtual void ArcherVisualOnReadyToAttack()
    {
        OnArcherReadyToAttack?.Invoke();
    }

    protected virtual void ArcherVisualOnAimingAtPlayer()
    {
        isAiming = true;
        OnArcherStartedAiming?.Invoke();
    }

    protected virtual void ArcherAttackAnimationFinished()
    {
        isAiming = false;
        OnArcherStoppedAiming?.Invoke();
    }
    protected virtual void ArcherAttackAnimationStarted()
    {
        OnArcherStartAttacking?.Invoke();
    }
    protected override void Attack()
    {
        Vector3 lookDirection = ray.GetDirection();
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x);
        Instantiate(projectile, transform.position + lookDirection.normalized * distanceProjectileFromEnemy, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg));
    }
}
