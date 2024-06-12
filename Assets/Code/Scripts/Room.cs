using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] Transform center;
    [SerializeField] int numberOfWaves;
    [SerializeField] ListPatrols patrols;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] GameObject gateGameObject;
    [SerializeField] float timeBetweenWaves;
    [SerializeField] int percentOfChanceEmpty = 20;
    [SerializeField] int percentOfChanceRegular = 40;
    [SerializeField] int percentOfChanceArena = 40;
    float timerBetweenWaves;
    bool isArena;
    bool allEnemiesDefeated;
    bool playerCrossedArea;
    int currentWave;
    List<GameObject> wave;
    private void Start()
    {

        gateGameObject.SetActive(false);
        int randomNumber = UnityEngine.Random.Range(0, 100);
        if(randomNumber >= percentOfChanceEmpty && randomNumber < percentOfChanceEmpty + percentOfChanceRegular)
        {
            SpawnRandomPatrol();
        }
        else if(randomNumber >= percentOfChanceEmpty + percentOfChanceRegular && randomNumber < percentOfChanceEmpty + percentOfChanceRegular + percentOfChanceArena)
        {
            isArena = true;
        }
    }
    private void Update()
    {
        if(isArena && playerCrossedArea)
        {
            allEnemiesDefeated = AllEnemiesDefeated();
            if(allEnemiesDefeated)
            {
                if(timerBetweenWaves < timeBetweenWaves && currentWave != numberOfWaves - 1)
                {
                    timerBetweenWaves += Time.deltaTime;
                }
                else
                {
                    timerBetweenWaves = 0f;
                    currentWave++;
                    if (currentWave < numberOfWaves)
                    {
                        allEnemiesDefeated = false;
                        SpawnRandomPatrol();
                    }
                    else
                    {
                        isArena = false;
                        gateGameObject.SetActive(false);
                    }
                }
            }
        }
    }
    private void SpawnRandomPatrol()
    {
        int randomIndex = UnityEngine.Random.Range(0, patrols.listPatrols.Count);
        GameObject patrolObject = Instantiate(patrols.listPatrols[randomIndex], center.position, Quaternion.Euler(0, 0, 0));
        Patrol patrol = patrolObject.GetComponent<Patrol>();
        if (isArena)
        {
            wave = patrol.GetChildren();
        }
        patrol.UnSetChildObjectsParent();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LayerMask colliderLayerMask = GetLayerMask(collision.gameObject.layer);
        if(colliderLayerMask == playerLayerMask && isArena && !playerCrossedArea)
        {
            timerBetweenWaves = 0f;
            playerCrossedArea = true;
            currentWave = 0;
            gateGameObject.SetActive(true);
            allEnemiesDefeated = false;
            SpawnRandomPatrol();
        }
    }
    private LayerMask GetLayerMask(int layer)
    {
        return LayerMask.GetMask(LayerMask.LayerToName(layer));
    }
    public bool AllEnemiesDefeated()
    {
        foreach(GameObject enemy in wave)
        {
            if(enemy != null)
            {
                return false;
            }
        }
        return true;
    }
}
