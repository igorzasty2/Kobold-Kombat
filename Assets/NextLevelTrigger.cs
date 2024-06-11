using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    [SerializeField] GameObject endScreenObject;

    void Update()
    {
        if (playerInRange && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C)))
        {
            EndScreen endScreen = endScreenObject.GetComponent<EndScreen>();
            if (endScreen != null)
            {
                endScreen.FinishLevel(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
