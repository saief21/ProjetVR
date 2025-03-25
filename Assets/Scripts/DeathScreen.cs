using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        // S'assurer que l'écran de mort est caché au début
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }

        // S'abonner à l'événement de mort du joueur
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.onPlayerDeath.AddListener(ShowDeathScreen);
        }
    }

    public void ShowDeathScreen()
    {
        // Activer le panneau
        deathScreenPanel.SetActive(true);

        // Récupérer et afficher le score
        int currentScore = GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0;
        finalScoreText.text = $"Score Final : {currentScore}";

        // Gérer le high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        highScoreText.text = $"Meilleur Score : {highScore}";

        // Désactiver le contrôle du joueur
        DisablePlayerControl();
        
        // Montrer le curseur
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void DisablePlayerControl()
    {
        // Désactiver le PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Désactiver le WeaponController
        WeaponController weaponController = FindObjectOfType<WeaponController>();
        if (weaponController != null)
        {
            weaponController.enabled = false;
        }
    }

    public void RestartGame()
    {
        // Recharger la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
