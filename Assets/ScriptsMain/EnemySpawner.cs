using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    private void Start()
    {
        PlayerDetector.OnPlayerDetected += SpawnEnemies;
    }

    private void SpawnEnemies()
    {
        // Spawn de enemigos usando los spawners
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}

