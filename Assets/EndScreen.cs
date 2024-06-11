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
        gameManager.FinishLevel();
        EndScreenUI.SetActive(true);
        StatsScores.text = Mathf.Round((Time.time - gameManager.floorTime) * 100f) * 0.01f + "s\n" + Mathf.Round(gameManager.runTime*100f)*0.01f + "s\n" + gameManager.killedEnemies + "\n" + gameManager.damageDealt + "\n" + gameManager.damageTaken;
        FinishText.text = "Floor " + gameManager.currentFloor;
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
            Destroy(gameManager);
        }
    }
    public void Menu()
    {
        StartCoroutine(Anim(0));
    }
    public IEnumerator Anim(int scene)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
    }
    public void Continue()
    {
        Time.timeScale = 1f;
        StartCoroutine(Anim(1));
    }
}
