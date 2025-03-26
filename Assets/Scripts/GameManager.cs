using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Slider healthSlider;

    private int score = 0;
    private PlayerHealth playerHealth;
    private WeaponController weaponController;

    public int CurrentScore => score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        weaponController = Object.FindFirstObjectByType<WeaponController>();
        UpdateUI();

        // Cacher le curseur au début du jeu
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void UpdateWave(int waveNumber)
    {
        waveText.text = $"Wave: {waveNumber}";
    }

    private void Update()
    {
        // Update health and ammo every frame as they can change frequently
        if (playerHealth != null)
        {
            healthText.text = $"Health: {playerHealth.CurrentHealth}";
            healthSlider.value = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        }

        if (weaponController != null)
        {
            ammoText.text = $"Ammo: {weaponController.CurrentAmmo} / {weaponController.MaxAmmo}";
        }
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
    }
}
