using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign your "enemy" prefab in the Inspector
    public float spawnInterval = 2f; // Time in seconds between spawns
    public int maxEnemies = 10;      // Maximum number of enemies allowed at once
    public float spawnRadius = 8f;   // Distance from the player to spawn enemies

    public Transform player;         // Assign your player here in the Inspector (or auto-find)
    private float timer = 0f;

    void Awake()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Count current enemies in the scene
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        timer += Time.deltaTime;
        if (timer >= spawnInterval && currentEnemies < maxEnemies)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Spawn at a random position around the player within the radius
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(spawnOffset.x, spawnOffset.y, 0f);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
