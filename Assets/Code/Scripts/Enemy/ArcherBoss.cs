using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBoss : Archer
{
    [SerializeField] private float specialAttackTimerMax;
    [SerializeField] private GameObject basicRayObject;
    [SerializeField] private float angleBeetwenRays;
    [SerializeField] private float rayLength;
    private int amountOfRays = 5;
    List<BasicRay> basicRays;
    private float specialAttackTimer;
    bool isSpecialAttackReady;
    protected override void Start()
    {
        base.Start();
        OnStateChanged += BossArcherOnStateChanged;
        basicRays = new List<BasicRay>();
        for(int i = 0; i < amountOfRays; i++)
        {
            GameObject rayObject = Instantiate(basicRayObject, transform);
            BasicRay basicRay = rayObject.GetComponent<BasicRay>();
            basicRay.SetLineVisibility(false);
            basicRays.Add(basicRay);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (specialAttackTimer < specialAttackTimerMax)
        {
            if(!isSpecialAttackReady)
            {
                specialAttackTimer += Time.deltaTime;
            }
        }
        else
        {
            if(state != State.Attacking)
            {
                isSpecialAttackReady = true;
                specialAttackTimer = 0f;
            }
        }
    }
    private void BossArcherOnStateChanged(object sender, OnStateChangedEventArgs e)
    {
        if (state == State.Hurt || state == State.Dead && isSpecialAttackReady)
        {
            foreach(BasicRay element in basicRays)
            {
                element.SetLineVisibility(false);
                ArcherAttackAnimationFinished();
            }
        }
    }
    protected override void ArcherVisualOnReadyToAttack()
    {
        if(isSpecialAttackReady)
        {
            return;
        }
        base.ArcherVisualOnReadyToAttack();
    }

    protected override void ArcherVisualOnAimingAtPlayer()
    {
        if (isSpecialAttackReady)
        {
            Vector2 dir = playerTransform.position - transform.position;
            dir = dir.normalized * rayLength;
            basicRays[0].SetLinePosition(transform.position, (Vector2)transform.position + dir);
            for(int i = 1; i < (basicRays.Count + 1)/ 2; i++)
            {
                basicRays[i].SetLinePosition(transform.position, transform.position + GetRotatedVector(basicRays[0].GetDirection(), i * angleBeetwenRays));
                basicRays[basicRays.Count - i].SetLinePosition(transform.position, transform.position + GetRotatedVector(basicRays[0].GetDirection(), - i * angleBeetwenRays));
            }
            foreach(BasicRay element in basicRays)
            {
                element.SetLineVisibility(true);
            }
            return;
        }
        base.ArcherVisualOnAimingAtPlayer();
    }

    protected override void ArcherAttackAnimationFinished()
    {
        if (isSpecialAttackReady)
        {
            isSpecialAttackReady = false;
            return;
        }
        base.ArcherAttackAnimationFinished();
    }
    protected override void ArcherAttackAnimationStarted()
    {
        if (isSpecialAttackReady)
        {
            return;
        }
        base.ArcherAttackAnimationStarted();
    }
    private Vector3 GetRotatedVector(Vector3 vector, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 rotatedVector = rotation * vector;
        return rotatedVector;
    }
    protected override void Attack()
    {
        if(isSpecialAttackReady)
        {
            foreach (BasicRay element in basicRays)
            {
                element.SetLineVisibility(false);
                Vector2 lookDirection = element.GetDirection();
                float angle = Mathf.Atan2(lookDirection.y, lookDirection.x);
                Instantiate(projectile, (Vector2)transform.position + lookDirection.normalized * distanceProjectileFromEnemy, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg));
            }
            return;
        }
        base.Attack();
    }
}
