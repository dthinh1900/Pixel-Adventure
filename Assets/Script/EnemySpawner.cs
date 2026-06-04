using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemySpawnData spawnData;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        foreach (EnemySpawnInfo enemy in spawnData.enemies)
        {
            Instantiate(
                enemy.enemyPrefab,
                enemy.position,
                Quaternion.identity
            );
        }
    }
}