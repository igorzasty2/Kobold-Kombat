using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoulBossVisual : EnemyVisual
{
    private const string DASH = "GhoulBossDash";
    private const string ATTACK_AFTER_DASH = "GhoulBossAttackAfterDash";

    public event Action OnAttackAfterDashAnimationFinished;
    protected override void EnemyOnStateChanged(object sender, Enemy.OnStateChangedEventArgs e)
    {
        if(e is GhoulBoss.GhoulOnStateChangedEventArgs)
        {
            GhoulBoss.GhoulOnStateChangedEventArgs eventArgs = e as GhoulBoss.GhoulOnStateChangedEventArgs;
            switch(eventArgs.ghoulBossState)
            {
                case GhoulBoss.GhoulBossState.Dash:
                    canLookAtPlayer = true;
                    enemyAnimator.Play(DASH);
                    break;
                case GhoulBoss.GhoulBossState.AttackAfterDash:
                    canLookAtPlayer = true;
                    enemyAnimator.Play(ATTACK_AFTER_DASH);
                    break;
                case GhoulBoss.GhoulBossState.Hurt:
                    canLookAtPlayer = true;
                    enemyAnimator.Play(animationNames.HurtSecondLayer, 1, 0f);
                    break;
            }
            return;
        }
        base.EnemyOnStateChanged(sender, e);
    }
    public void AttackAfterDashAnimationFinished()
    {
        OnAttackAfterDashAnimationFinished?.Invoke();
    }

}
