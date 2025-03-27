using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 20f;  // Distance de spawn autour du joueur
    [SerializeField] private float minSpawnDistance = 10f;  // Distance minimum du joueur
    
    [Header("Wave Settings")]
    [SerializeField] private int baseEnemiesPerWave = 5;  // Nombre d'ennemis de base par vague
    [SerializeField] private float timeBetweenWaves = 5f;  // Temps entre les vagues
    [SerializeField] private float enemyHealthMultiplier = 1.2f;  // Multiplicateur de santé par vague
    
    private Transform player;
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool isSpawning = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        while (true)
        {
            if (!isSpawning && enemiesAlive == 0)
            {
                currentWave++;
                yield return StartCoroutine(SpawnWave());
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        int enemiesThisWave = baseEnemiesPerWave + (currentWave - 1) * 2;  // Augmente de 2 par vague

        Debug.Log($"Wave {currentWave} starting! Spawning {enemiesThisWave} enemies");

        for (int i = 0; i < enemiesThisWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);  // Délai entre chaque spawn
        }

        isSpawning = false;
    }

    private void SpawnEnemy()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, spawnRadius);
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Ajuster la position au sol
        if (Physics.Raycast(spawnPos + Vector3.up * 10, Vector3.down, out RaycastHit hit))
        {
            spawnPos = hit.point;
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.LookRotation((player.position - spawnPos).normalized));
        
        // Augmenter la santé en fonction de la vague
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            float newMaxHealth = enemyHealth.MaxHealth * Mathf.Pow(enemyHealthMultiplier, currentWave - 1);
            enemyHealth.SetMaxHealth(newMaxHealth);
        }

        enemiesAlive++;
        enemy.GetComponent<EnemyHealth>().OnEnemyDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        enemiesAlive--;
    }
}
