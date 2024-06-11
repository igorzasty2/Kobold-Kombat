using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageDisplay : MonoBehaviour
{
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] Canvas canvas;
    [SerializeField] Image healthBar;
    [SerializeField] TMP_Text healthText;

    public void ShowDamage(Vector3 position, int damage)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);

        GameObject damageTextInstance = Instantiate(damageTextPrefab, screenPosition, Quaternion.identity, canvas.transform);
        
        TMP_Text damageText = damageTextInstance.GetComponent<TMP_Text>();
        damageText.text = damage.ToString();

        StartCoroutine(AnimateDamageText(damageTextInstance));
    }
    private IEnumerator AnimateDamageText(GameObject damageTextInstance)
    {
        TMP_Text damageText = damageTextInstance.GetComponent<TMP_Text>();
        Color originalColor = damageText.color;

        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 originalPosition = damageTextInstance.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            damageTextInstance.transform.position = originalPosition + Vector3.up * progress * 50;

            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, progress));

            yield return null;
        }

        Destroy(damageTextInstance);
    }
    public void UpdateHealth(int health, int maxHealth)
    {
        healthBar.fillAmount = (float)health/maxHealth;
        healthText.text = health.ToString()+"/"+maxHealth.ToString();
    }
}