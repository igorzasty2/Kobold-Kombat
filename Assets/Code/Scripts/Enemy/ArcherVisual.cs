using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ArcherVisual : EnemyVisual
{
    public event Action OnAimingAtPlayer;
    public event Action OnReadyToAttack;
    public event Action OnStartAttacking;
    public void AimingAtPlayer()
    {
        OnAimingAtPlayer?.Invoke();
    }
    public void ReadyToAttack()
    {
        OnReadyToAttack?.Invoke();
    }
    public void StartAttacking()
    {
        OnStartAttacking?.Invoke();
    }
    protected override void EnemyOnStateChanged(object sender, Enemy.OnStateChangedEventArgs e)
    {
        base.EnemyOnStateChanged(sender, e);
    }
}
