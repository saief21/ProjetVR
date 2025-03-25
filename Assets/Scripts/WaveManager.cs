using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private int enemiesPerWave = 5;
    [SerializeField] private float timeBetweenSpawns = 1f;
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("Wave Settings")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private float enemyHealthMultiplier = 1.1f;
    
    private Transform playerTransform;
    private int enemiesAlive;
    private bool isSpawning;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        while (true)
        {
            currentWave++;
            Debug.Log($"Starting Wave {currentWave}");
            
            // Spawn enemies
            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // Wait until all enemies are dead
            while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            // Wait before next wave
            Debug.Log($"Wave {currentWave} completed!");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemy()
    {
        // Get random position around player
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Create enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.tag = "Enemy";

        // Scale enemy health with wave number
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.ScaleHealth(Mathf.Pow(enemyHealthMultiplier, currentWave - 1));
        }

        // Make enemy look at player
        enemy.transform.LookAt(playerTransform);
    }
}
