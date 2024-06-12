using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damage;
    [SerializeField] bool isMelee;
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
        if (collision.gameObject.layer == 7)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage(damage, transform.position, isMelee);
            }
            ProjectileFinish();
        }
        else if (collision.gameObject.layer == 3)
        {
            ProjectileFinish();
        }
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
