using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] protected float distanceHitboxFromEnemy;
    [SerializeField] protected float radiusOfHitbox;
    [SerializeField] protected int damage;
    protected override void Attack()
    {
        Vector2 hitboxPos = transform.position + (playerTransform.position - transform.position).normalized * distanceHitboxFromEnemy;
        Collider2D collider = Physics2D.OverlapCircle(hitboxPos, radiusOfHitbox, playerLayerMask);
        if (collider != null)
        {
            PlayerControl player = collider.GetComponent<PlayerControl>();
            player.Damage(damage);
        }
    }
}
