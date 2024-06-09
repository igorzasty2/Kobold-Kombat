using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] protected float distanceHitboxFromEnemy;
    [SerializeField] protected float radiusOfHitbox;
    protected override void Attack()
    {
        Vector2 hitboxPos = transform.position + (playerTransform.position - transform.position).normalized * distanceHitboxFromEnemy;
        if (Physics2D.OverlapCircle(hitboxPos, radiusOfHitbox, playerLayerMask) != null)
        {
            Debug.Log($"Gracz zaatakowany! przez {gameObject.name}");
        }
    }
}
