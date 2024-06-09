using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ray : BasicRay
{
    private const string IS_READY_TO_ATTACK = "IsReadyToAttack";
    [SerializeField] Archer archer;
    Animator rayAnimator;
    bool canTargeting;

    protected override void Awake()
    {
        rayAnimator = GetComponent<Animator>();
        base.Awake();
    }

    private void Start()
    {
        archer.OnArcherStoppedAiming += StopTargetingPlayer;
        archer.OnArcherStartedAiming += StartTargetingPlayer;
        archer.OnArcherReadyToAttack += StartFlashing;
        archer.OnArcherStartAttacking += ArcherOnArcherStartAttacking;
    }

    private void ArcherOnArcherStartAttacking()
    {
        canTargeting = true;
    }

    private void StartFlashing()
    {
        canTargeting = false;
        rayAnimator.SetBool(IS_READY_TO_ATTACK, true);
    }

    private void StartTargetingPlayer()
    {
        lineRenderer.enabled = true;
    }
    private void StopTargetingPlayer()
    {
        lineRenderer.enabled = false;
        rayAnimator.SetBool(IS_READY_TO_ATTACK, false);
    }

    public void UpdateLineRenderer(Vector2 startPos, Vector2 endPos)
    {
        if(canTargeting)
        {
            SetLinePosition(startPos, endPos);
        }
    }
}
