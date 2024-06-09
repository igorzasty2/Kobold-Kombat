using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    [SerializeField] ProjectileAnimationNames projectileAnimationNames;
    [SerializeField] Projectile projectile;
    Animator projectileAnimator;
    public event Action OnDestroyAnimationFinished;
    private void Awake()
    {
        projectileAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        projectile.OnDestroy += ProjectileOnDestroy;
    }

    private void ProjectileOnDestroy()
    {
        projectileAnimator.Play(projectileAnimationNames.Destroyed);
    }
    public void DestroyAnimationFinished()
    {
        OnDestroyAnimationFinished?.Invoke();
    }
}
