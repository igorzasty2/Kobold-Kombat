using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] LayerMask wallLayerMask;
    [SerializeField] float speed;
    [SerializeField] ProjectileVisual projectileVisual;
    Vector2 startPosition;
    Vector2 lookingDirection;
    bool crossedMaxDistance;
    Rigidbody2D projectileRigidBody;
    Collider2D projectileCollider;
    public event Action OnDestroy;
    private void Awake()
    {
        projectileRigidBody = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
        startPosition = transform.position;
        crossedMaxDistance = false;
        projectileVisual.OnDestroyAnimationFinished += ProjectileVisualOnDestroyAnimationFinished;
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
        if(Vector2.Distance(transform.position, startPosition) > 7  && !crossedMaxDistance)
        {
            crossedMaxDistance = true;
            OnDestroy?.Invoke();
            projectileCollider.enabled = false;
            projectileRigidBody.velocity = Vector2.zero;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LayerMask colliderLayerMask = GetLayerMask(collision.gameObject.layer);
        if (colliderLayerMask == playerLayerMask || colliderLayerMask == wallLayerMask)
        {
            OnDestroy?.Invoke();
            projectileCollider.enabled = false;
            projectileRigidBody.velocity = Vector2.zero;
        }
    }
    private LayerMask GetLayerMask(int layer)
    {
        return LayerMask.GetMask(LayerMask.LayerToName(layer));
    }
    private void ProjectileVisualOnDestroyAnimationFinished()
    {
        Destroy(gameObject);
    }
}
