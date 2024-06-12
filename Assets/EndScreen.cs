using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] Animator transition;
    [SerializeField] GameObject EndScreenUI;
    [SerializeField] TMP_Text FinishText;
    [SerializeField] TMP_Text StatsScores;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject continueButton;
    public void FinishLevel(bool isSuccess)
    {
        Time.timeScale = 0.0f;
        gameManager.FinishLevel();
        EndScreenUI.SetActive(true);
        StatsScores.text = Mathf.Round((Time.time - gameManager.floorTime) * 100f) * 0.01f + "s\n" + Mathf.Round(GameManager.runTime*100f)*0.01f + "s\n" + GameManager.killedEnemies + "\n" + GameManager.damageDealt + "\n" + GameManager.damageTaken;
        FinishText.text = "Floor " + GameManager.currentFloor;
        if (isSuccess)
        {
            FinishText.text += " Finished";
            continueButton.SetActive(true);
            quitButton.SetActive(false);
        }
        else
        {
            FinishText.text += " Failed";
            continueButton.SetActive(false);
            quitButton.SetActive(true);
            GameManager.runTime = 0.0f;
            GameManager.currentFloor = 1;
            GameManager.damageDealt = 0;
            GameManager.damageTaken = 0;
            GameManager.killedEnemies = 0;
        }
    }
    public void Menu()
    {
        Destroy(gameManager);
        StartCoroutine(Anim(0));
    }
    public IEnumerator Anim(int scene)
    {
        Time.timeScale = 1f;
        gameManager.NextLevel();
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
    }
    public void Continue()
    {
        StartCoroutine(Anim((GameManager.currentFloor%2)+1));
    }
}
