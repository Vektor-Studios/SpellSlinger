using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign your "enemy" prefab in the Inspector
    public int maxEnemies = 10;      // Maximum number of enemies allowed at once
    public int roundNumber = 0;      // Current round number
    public float spawnRadius = 8f;   // Distance from the player to spawn enemies

    public Transform player;
    public GameObject playerObj;
    void Awake()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Count current enemies in the scene
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemies == 0)
        {
            NewRound();
            Debug.Log("Round " + roundNumber + " started with " + maxEnemies + " enemies.");

        }
    }

    void SpawnEnemy()
    {
        // Spawn at a random position around the player within the radius
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(spawnOffset.x, spawnOffset.y, 0f);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    void NewRound()
    {
        maxEnemies += 2 * roundNumber; // Increase max enemies each round
        roundNumber++;
        var i = 0;
        while (i < maxEnemies)
        {
            SpawnEnemy();
            i++;
        }
    }
}
