using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int pointsValue = 10;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Canvas healthCanvas;

    private float currentHealth;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        
        // Setup health bar
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Make health bar face camera
        if (healthCanvas != null)
        {
            healthCanvas.worldCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Make health bar face camera
        if (healthCanvas != null)
        {
            healthCanvas.transform.LookAt(Camera.main.transform);
            healthCanvas.transform.Rotate(0, 180, 0);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Add points
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(pointsValue);
        }

        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public void ScaleHealth(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
}
