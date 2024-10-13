using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController main;
    private void Awake()
    {
        main = this;
    }
    public float lastSpawn = 3f;
    public  float EnemySpawnRange = 10;
    public ObjectPool enemypool;

    public EnemyData[] enemiesSpawned;
    public EnemyData[] executivesSpawned;
    private void Update()
    {

        if (Level.main.state == Level.GameState.running && lastSpawn < Time.time)
        {
            lastSpawn += 5 * 5 / (5 + Level.main.currentRound);
            SpawnEnemy(enemiesSpawned[Mathf.FloorToInt(Random.value * enemiesSpawned.Length)]);
        }
    }
    #region Spawn Enemies

    public void SpawnWave(EnemyData data, int amount)
    {
        for (int I = 0; I < amount; I++)
        {
            SpawnEnemy(data);
        }
    }

    public void SpawnWave(EnemyData[] wave)
    {
        foreach (EnemyData name in wave)
        {
            SpawnEnemy(name);
        }
    }
    public void SpawnEnemy(EnemyData data)
    {
        if (enemypool.activeObjs.childCount < 5 + Level.main.currentRound)
            Enemy.FromData(data);
    }
    public void SpawnEnemiesInArea(Rect area, int count)
    {
        SpawnEnemiesInArea(area, count, executivesSpawned[Mathf.FloorToInt(Random.value*executivesSpawned.Length)]);
    }
    public void SpawnEnemiesInArea(Rect area, int count, EnemyData data)
    {
        for (int I = 0; I < count; I++)
        {
            var e = Enemy.FromData(data);
            e.transform.position = new Vector3(area.xMin + Random.value * area.width, area.yMin + Random.value * area.height) ;
        }
    }
    #endregion
    public void ClearEnemies()
    {
        foreach (var enemy in Level.main.bodies.ToArray())
        {
            if (!enemy.CompareTag("Player"))
            {
                enemypool.DeactivateObject(enemy.gameObject);
            }
        }
    }
}
