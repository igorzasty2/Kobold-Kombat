using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    [SerializeField] GameObject endScreenObject;
    [SerializeField] GameObject wrapper;
    private bool triggered = false;
    void Update()
    {
        if (!triggered && playerInRange && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C)))
        {
            EndScreen endScreen = endScreenObject.GetComponent<EndScreen>();
            if (endScreen != null)
            {
                triggered = true;
                endScreen.FinishLevel(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            wrapper.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            wrapper.SetActive(false);
        }
    }
}
