using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : MeleeEnemy
{
    [SerializeField] float dashSpeed;
    protected override void Start()
    {
        base.Start();
        MinotaurVisual minotaurVisual;
        minotaurVisual = enemyVisual as MinotaurVisual;
        minotaurVisual.OnPrepareAttack += DashToPlayerBeforeAttack;
    }
    private void DashToPlayerBeforeAttack()
    {
        enemyRigidbody.velocity = (playerTransform.position - transform.position).normalized * dashSpeed;
    }
}
