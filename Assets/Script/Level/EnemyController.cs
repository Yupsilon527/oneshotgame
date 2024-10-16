using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController main;
    private void Awake()
    {
        main = this;
    }
    public float lastSpawn = 3f;
    public ObjectPool enemypool;

    public EnemyData[] enemiesSpawned;
    private void Update()
    {

        if (Level.main.state == Level.GameState.running && lastSpawn < Time.time)
        {
            lastSpawn += .33f;
            SpawnWave(enemiesSpawned[Mathf.FloorToInt(Random.value * enemiesSpawned.Length)], 10);
            //Bullet.FireBullet(new Vector2(UnityEngine.Random.Range(CameraBounds.xMin, CameraBounds.xMax),CameraBounds.yMax), Vector2.down, 10, 4, .25f, 0);
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
        if (enemypool.activeObjs.childCount < 50)
            Enemy.FromData(data);
    }
    public void SpawnEnemiesInArea(Rect area, int count, EnemyData data)
    {
        for (int I = 0; I < count; I++)
        {
            var e = Enemy.FromData(data);
            e.transform.position = new Vector3(area.xMin + Random.value * area.width, area.yMin + Random.value * area.height);
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
