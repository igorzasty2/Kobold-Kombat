using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    LayerMask hitLayerMask;
    LayerMask wallLayerMask;
    [SerializeField] float speed;
    [SerializeField] int damage;
    Vector2 startPosition;
    Vector2 lookingDirection;
    bool crossedMaxDistance;
    Animator animator;
    Rigidbody2D projectileRigidBody;
    Collider2D projectileCollider;
    private void Awake()
    {
        projectileRigidBody = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        hitLayerMask = GetLayerMask(7);
        wallLayerMask = GetLayerMask(3);
        startPosition = transform.position;
        crossedMaxDistance = false;
        float angleInDegrees = transform.eulerAngles.z;
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        lookingDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        projectileRigidBody.velocity = lookingDirection.normalized * speed;
    }
    public void Enable()
    {
        projectileRigidBody.velocity = lookingDirection.normalized * speed;
        projectileCollider.enabled = true;
    }
    public void Disable()
    {
        projectileRigidBody.velocity = Vector2.zero;
        projectileCollider.enabled = false;
    }
    private void Update()
    {
        if (Vector2.Distance(transform.position, startPosition) > 7 && !crossedMaxDistance)
        {
            ProjectileFinish();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LayerMask colliderLayerMask = GetLayerMask(collision.gameObject.layer);
        if (colliderLayerMask == hitLayerMask)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage(damage);
            }
            ProjectileFinish();
        }
        else if (colliderLayerMask == wallLayerMask)
        {
            ProjectileFinish();
        }
    }
    private LayerMask GetLayerMask(int layer)
    {
        return LayerMask.GetMask(LayerMask.LayerToName(layer));
    }
    private void ProjectileFinish()
    {
        crossedMaxDistance = true;
        projectileCollider.enabled = false;
        projectileRigidBody.velocity = Vector2.zero;
        animator.SetTrigger("Destroy");
    }
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
