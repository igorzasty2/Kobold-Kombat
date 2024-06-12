using System.Collections.Generic;
using UnityEngine;

public class Mag : Enemy
{
    [SerializeField] protected float distanceSpellShotFromEnemy;
    [SerializeField] float angleBetweenSpellShots;
    [SerializeField] protected GameObject spellShot;
    protected List<GameObject> spellShots;

    private int amountOfSpellShots = 5;
    protected override void Start()
    {
        base.Start();
        spellShots = new List<GameObject>();
        enemyVisual.OnEnemyAttackAnimationFinished += EnemyVisualOnEnemyAttackAnimationFinished;
        OnStateChanged += MagOnStateChanged;
    }

    protected virtual void MagOnStateChanged(object sender, OnStateChangedEventArgs e)
    {
        if(e.state == State.Hurt || state == State.Dead)
        {
            foreach(GameObject element in spellShots)
            {
                Destroy(element);
            }
            spellShots.Clear();
        }
    }

    protected override void Attack()
    {
        Vector3 lookDirection = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x);
        spellShots.Add(Instantiate(spellShot, transform.position + lookDirection.normalized * distanceSpellShotFromEnemy, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg)));
        for(int i = 1; i <= (amountOfSpellShots - 1)/2; i++)
        {
            Vector3 rotatedVector = GetRotatedVector(lookDirection, i * angleBetweenSpellShots);
            spellShots.Add(Instantiate(spellShot, transform.position + rotatedVector.normalized * distanceSpellShotFromEnemy, Quaternion.Euler(0, 0, Mathf.Atan2(rotatedVector.y, rotatedVector.x) * Mathf.Rad2Deg)));
            rotatedVector = GetRotatedVector(lookDirection, - (i * angleBetweenSpellShots));
            spellShots.Add(Instantiate(spellShot, transform.position + rotatedVector.normalized * distanceSpellShotFromEnemy, Quaternion.Euler(0, 0, Mathf.Atan2(rotatedVector.y, rotatedVector.x) * Mathf.Rad2Deg)));
        }
        foreach(GameObject element in spellShots)
        {
            Projectile projectile = element.GetComponent<Projectile>();
            projectile.Disable();
        }
    }

    protected virtual void EnemyVisualOnEnemyAttackAnimationFinished()
    {
        foreach (GameObject element in spellShots)
        {
            Projectile projectile = element.GetComponent<Projectile>();
            projectile.Enable();
        }
        spellShots.Clear();
    }
    protected Vector3 GetRotatedVector(Vector3 vector, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 rotatedVector = rotation * vector;
        return rotatedVector;
    }
}
