using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinotaurVisual : EnemyVisual
{
    public event Action OnPrepareAttack;
    protected override void EnemyOnStateChanged(object sender, Enemy.OnStateChangedEventArgs e)
    {
        if(e.state == Enemy.State.Hurt)
        {
            canLookAtPlayer = true;
            enemyAnimator.Play(animationNames.HurtSecondLayer, 1, 0f);
            return;
        }
        base.EnemyOnStateChanged(sender, e);
    }
    public void PrepareAttack()
    {
        OnPrepareAttack?.Invoke();
    }
}
