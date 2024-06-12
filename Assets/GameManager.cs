using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static int currentFloor = 1;
    public float floorTime;
    public float runTime = 0;
    public static int killedEnemies = 0;
    public static int damageDealt = 0;
    public static int damageTaken = 0;
    public static int health = 0;
    private void Awake()
    {
        floorTime = Time.time;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void FinishLevel()
    {
        runTime += Time.time - floorTime;
    }
    public void NextLevel()
    {
        currentFloor += 1;
        floorTime = Time.time;
    }
}