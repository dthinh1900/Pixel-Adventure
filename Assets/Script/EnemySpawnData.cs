using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public Vector2 position;
}

[CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Game/Enemy Spawn Data")]
public class EnemySpawnData : ScriptableObject
{
    public List<EnemySpawnInfo> enemies;
}