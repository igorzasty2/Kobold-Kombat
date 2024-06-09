using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagBoss : Mag
{
    [SerializeField] private float specialAttackTimerMax;
    private float specialAttackTimer;
    bool isSpecialAttackReady;
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        if (specialAttackTimer < specialAttackTimerMax)
        {
            if (!isSpecialAttackReady)
            {
                specialAttackTimer += Time.deltaTime;
            }
        }
        else
        {
            if (state != State.Attacking)
            {
                isSpecialAttackReady = true;
                specialAttackTimer = 0f;
            }
        }
    }
    protected override void MagOnStateChanged(object sender, OnStateChangedEventArgs e)
    {
        if (e.state == State.Hurt || state == State.Dead)
        {
            isSpecialAttackReady = false;
        }
        base.MagOnStateChanged(sender, e);
    }
    protected override void Attack()
    {
        if (isSpecialAttackReady)
        {
            float fullAngle = 360;
            float angle = 30;
            Vector2 dir = Vector2.up;
            for (int i = 0; i < fullAngle / angle; i++)
            {
                Vector2 dirSpellShot = GetRotatedVector(dir, i * angle);
                float angleSpellShot = Mathf.Atan2(dirSpellShot.y, dirSpellShot.x);
                spellShots.Add(Instantiate(spellShot, (Vector2)transform.position + dirSpellShot.normalized * distanceSpellShotFromEnemy, Quaternion.Euler(0, 0, angleSpellShot * Mathf.Rad2Deg)));
            }
            foreach (GameObject element in spellShots)
            {
                Projectile projectile = element.GetComponent<Projectile>();
                projectile.Disable();
            }
            return;
        }
        base.Attack();
    }
    protected override void EnemyVisualOnEnemyAttackAnimationFinished()
    {
        if(isSpecialAttackReady)
        {
            isSpecialAttackReady = false;
        }
        base.EnemyVisualOnEnemyAttackAnimationFinished();
    }
}
